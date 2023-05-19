using System.Linq.Expressions;
using System.Reflection;
using System.Text.Json.Nodes;

namespace Nebula.JsonRpc;

public static class ConversionGenerator
{
    private static void RegisterApiClass(JsonRpcPeer peer, Type type)
    {
        var prefix = type.Name;
        var methods = type.GetMethods();
        foreach (var method in methods)
        {
            if (!method.IsStatic) continue;
            var func = CreateJsonConversionFunc(method);
            peer.RegisterMethod($"{prefix}_{method.Name}", func);
        }
    }

    public static Func<JsonNode?, JsonNode?> CreateJsonConversionFunc(MethodInfo methodInfo)
    {
        var jsonInputVariable = Expression.Parameter(typeof(JsonNode), "input");
        
        var outputs = new Dictionary<string, ParameterExpression>();

        var byNameParameters = new List<Expression>();
        var byPositionParameters = new List<Expression>();

        foreach (var parameterInfo in methodInfo.GetParameters())
        {
            if (parameterInfo.IsOut)
            {
                var variable = Expression.Variable(parameterInfo.ParameterType.GetElementType(), parameterInfo.Name);
                outputs.Add(parameterInfo.Name, variable);
                byNameParameters.Add(variable);
                byPositionParameters.Add(variable);
            }
            else
            {
                var indexByName = Expression.Property(jsonInputVariable, "Item", Expression.Constant(parameterInfo.Name));
                var indexByPosition = Expression.Property(jsonInputVariable, "Item", Expression.Constant(parameterInfo.Position));

                Expression getValueByName;
                Expression getValueByPosition;
                
                if (parameterInfo.ParameterType.IsPointer && parameterInfo.ParameterType.GetElementType() == typeof(void))
                {
                    
                    getValueByName = Expression.Call(indexByName, "GetValue", new[] { typeof(long) });
                    getValueByPosition = Expression.Call(indexByPosition, "GetValue", new []{ typeof(long) });

                    getValueByName = Expression.New(typeof(IntPtr).GetConstructor(new[] { typeof(long) })!, getValueByName);
                    getValueByPosition = Expression.New(typeof(IntPtr).GetConstructor(new[] { typeof(long) })!, getValueByPosition);

                    getValueByName = Expression.Call(getValueByName, "ToPointer", null);
                    getValueByPosition = Expression.Call(getValueByPosition, "ToPointer", null);
                }
                else
                {
                    getValueByName = Expression.Call(indexByName, "GetValue", new []{ parameterInfo.ParameterType });
                    getValueByPosition = Expression.Call(indexByPosition, "GetValue", new []{ parameterInfo.ParameterType });
                }
                
                byNameParameters.Add(getValueByName);
                byPositionParameters.Add(getValueByPosition);
            }
        }

        Expression byNameMethod;
        Expression byPositionMethod;
        byNameMethod = Expression.Call(methodInfo, byNameParameters);
        byPositionMethod = Expression.Call(methodInfo, byPositionParameters);
        
        if (methodInfo.ReturnType != typeof(void))
        {
            if (methodInfo.ReturnType.IsPointer)
            {
                var resultVariable = Expression.Variable(typeof(long), "methodCallOutputVariable");
                outputs.Add("return", resultVariable);
                byNameMethod = Expression.Assign(resultVariable, Expression.Call(Expression.New(typeof(IntPtr).GetConstructor(new[] { typeof(void*) })!, byNameMethod), "ToInt64", null));
                byPositionMethod = Expression.Assign(resultVariable, Expression.Call(Expression.New(typeof(IntPtr).GetConstructor(new[] { typeof(void*) })!, byPositionMethod), "ToInt64", null));
            }
            else
            {
                var resultVariable = Expression.Variable(methodInfo.ReturnType, "methodCallOutputVariable");
                outputs.Add("return", resultVariable);
                byNameMethod = Expression.Assign(resultVariable, byNameMethod);
                byPositionMethod = Expression.Assign(resultVariable, byPositionMethod);
            }
        }

        Expression byNameBlock = byNameMethod;
        Expression byPositionBlock = byPositionMethod;
        
        var returnTarget = Expression.Label(typeof(JsonNode));
        var returnLabel = Expression.Label(returnTarget, Expression.Constant(null, typeof(JsonNode)));

        if (outputs.Count == 1)
        {
            var (outputName, outputVariable) = outputs.First();
            
            Expression jsonValueWrappedVariable;

            if (typeof(JsonValue).GetMethods().Any(jsonValueMethod => jsonValueMethod.Name == "Create" && jsonValueMethod.GetParameters().First().ParameterType == outputVariable.Type))
            {
                jsonValueWrappedVariable = Expression.Call(typeof(JsonValue), "Create", null, outputVariable, Expression.Constant(null, typeof(JsonNodeOptions?)));
            }
            else
            {
                jsonValueWrappedVariable = Expression.Call(typeof(JsonValue), "Create", null, Expression.Constant(null, typeof(int?)), Expression.Constant(null, typeof(JsonNodeOptions?)));
            }

            var eitherReturn = Expression.Return(returnTarget, jsonValueWrappedVariable);

            byNameBlock = Expression.Block(new List<Expression> { byNameMethod, eitherReturn });
            byPositionBlock = Expression.Block(new List<Expression> { byPositionMethod, eitherReturn });
        }
        else if (outputs.Count > 1)
        {
            var byNameJsonOutputVariable = Expression.Variable(typeof(JsonObject), "jsonOutput");
            var byPositionJsonOutputVariable = Expression.Variable(typeof(JsonArray), "jsonOutput");

            var byNameJsonOutputAssign = Expression.Assign(byNameJsonOutputVariable, Expression.New(typeof(JsonObject).GetConstructor(new [] {typeof(JsonNodeOptions?)})!, Expression.Constant(null, typeof(JsonNodeOptions?))));
            var byPositionJsonOutputAssign = Expression.Assign(byPositionJsonOutputVariable, Expression.New(typeof(JsonArray).GetConstructor(new[] { typeof(JsonNodeOptions?) })!, Expression.Constant(null, typeof(JsonNodeOptions?))));

            var byNameAddToJsonOutputExpressions = new List<Expression>();
            var byPositionAddToJsonOutputExpressions = new List<Expression>();

            foreach (var (outputName, outputVariable) in outputs)
            {
                Expression jsonValueWrappedVariable;

                if (typeof(JsonValue).GetMethods().Any(jsonValueMethod => jsonValueMethod.Name == "Create" && jsonValueMethod.GetParameters().First().ParameterType == outputVariable.Type))
                {
                    jsonValueWrappedVariable = Expression.Call(typeof(JsonValue), "Create", null, outputVariable, Expression.Constant(null, typeof(JsonNodeOptions?)));
                }
                else
                {
                    jsonValueWrappedVariable = Expression.Call(typeof(JsonValue), "Create", null, Expression.Constant(null, typeof(int?)), Expression.Constant(null, typeof(JsonNodeOptions?)));
                }

                byNameAddToJsonOutputExpressions.Add(Expression.Call(byNameJsonOutputVariable, "Add", null, Expression.Constant(outputName, typeof(string)), jsonValueWrappedVariable));
                byPositionAddToJsonOutputExpressions.Add(Expression.Call(byPositionJsonOutputVariable, "Add", null, jsonValueWrappedVariable));
            }

            var byNameReturn = Expression.Return(returnTarget, byNameJsonOutputVariable);
            var byPositionReturn = Expression.Return(returnTarget, byPositionJsonOutputVariable);

            byNameBlock = Expression.Block(new [] {byNameJsonOutputVariable}, new List<Expression> { byNameMethod, byNameJsonOutputAssign }.Concat(byNameAddToJsonOutputExpressions).Concat(new List<Expression> { byNameReturn }));
            byPositionBlock = Expression.Block(new [] {byPositionJsonOutputVariable}, new List<Expression> { byPositionMethod, byPositionJsonOutputAssign }.Concat(byPositionAddToJsonOutputExpressions).Concat(new List<Expression> { byPositionReturn }));
        }

        var isJsonObjectOrArray = Expression.TypeIs(jsonInputVariable, typeof(JsonObject));
        var byNameOrPositionBranch = Expression.IfThenElse(isJsonObjectOrArray, byNameBlock, byPositionBlock);
        
        var body = Expression.Block(outputs.Values, byNameOrPositionBranch, returnLabel);
        var lambda = Expression.Lambda<Func<JsonNode?, JsonNode?>>(body, new []{jsonInputVariable});
        
        //Debugger.Log(0, "", $"{lambda.ToReadableString()}\n");
        var func = lambda.Compile();
        return func;
    }
}