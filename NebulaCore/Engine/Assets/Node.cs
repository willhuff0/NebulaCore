using System.Numerics;
using System.Reflection;
using System.Text.Json.Nodes;
using Guid = System.Guid;

namespace NebulaCore.Engine.Assets;

[AssetDefinition("nodes")]
public class Node : Asset
{
    private Node[] _children;
    private Dictionary<Guid, Dictionary<string, object>> _behaviors;
    private float[] _position;
    private float[] _rotation;
    private float[] _scale;
    
    public Node(Project project, JsonNode json) : base(project, json)
    {
        _children = json["children"]?.AsArray().Select((value) => new Node(Project, value!)).ToArray() ?? Array.Empty<Node>();
        _behaviors = new Dictionary<Guid, Dictionary<string, object>>();
        var jsonBehaviors = json["textures"]?.AsObject();
        if (jsonBehaviors != null)
        {
            foreach (var (behaviorKey, jsonBehavior) in jsonBehaviors)
            {
                var variables = _behaviors[Guid.Parse(behaviorKey)] = new Dictionary<string, object>();
                if (jsonBehavior != null)
                {
                    foreach (var (key, value) in jsonBehavior.AsObject())
                    {
                        variables[key] = value?.GetValue<object>();
                    }
                }
            }
        }
        _position = json["position"]?.GetValue<float[]>() ?? new[] { 0.0f, 0.0f, 0.0f };
        _rotation = json["rotation"]?.GetValue<float[]>() ?? new[] { 0.0f, 0.0f, 0.0f, 1.0f };
        _scale = json["scale"]?.GetValue<float[]>() ?? new[] { 1.0f, 1.0f, 1.0f };
    }

    public override JsonObject Serialize()
    {
        var json = new JsonObject();
        if (_children.Length > 0)
        {
            var jsonChildren = new JsonArray();
            foreach (var child in _children) jsonChildren.Add(child.Serialize());
            json["children"] = jsonChildren;
        }
        if (_position != new [] { 0.0f, 0.0f, 0.0f }) json["position"] = JsonValue.Create(_position);
        if (_rotation != new [] { 0.0f, 0.0f, 0.0f, 1.0f }) json["rotation"] = JsonValue.Create(_rotation);
        if (_scale != new [] { 1.0f, 1.0f, 1.0f }) json["scale"] = JsonValue.Create(_scale);
        return json;
    }

    public override async Task<RuntimeAsset?> Load()
    {
        var runtimeBehaviors = new List<Behavior>();
        foreach (var (behaviorKey, behaviorVariables) in _behaviors)
        {
            var behavior = Project.Bundle.GetAsset<BehaviorAsset>("behaviors", behaviorKey);
            if (behavior == null) continue;
            var behaviorInstance = Assembly.GetCallingAssembly().CreateInstance(behavior.Class);
            if (behaviorInstance == null)
            {
                Console.WriteLine($"Skipping behavior {behavior.Name} ({behavior.Class}) on node {Name} because the class was not found in the current assembly");
                continue;
            }
            var behaviorType = behaviorInstance.GetType();
            foreach (var (key, value) in behaviorVariables)
            {
                behaviorType.GetField(key)?.SetValue(behaviorInstance, value);
            }
            runtimeBehaviors.Add((behaviorInstance as Behavior)!);
        }
        
        var runtimeNode = new RuntimeNode(Project, this, new List<RuntimeNode>(_children.Length), runtimeBehaviors);
        NodeWorldComponent worldComponent = new NodeWorldComponent(
            runtimeNode, 
            new Vector3(_position[0], _position[1], _position[2]), 
            new Quaternion(_rotation[0], _rotation[1], _rotation[2], _rotation[3]), 
            new Vector3(_scale[0], _scale[1], _scale[2])
            );
        runtimeNode.WorldComponent = worldComponent;

        foreach (var child in _children)
        {
            var runtimeChild = (RuntimeNode?)(await child.Load());
            if (runtimeChild == null) continue;
            runtimeChild.Parent = runtimeNode;
            runtimeNode.Children.Add(runtimeChild);
        }

        return runtimeNode;
    }
}

public class RuntimeNode : RuntimeAsset
{
    public NodeWorldComponent WorldComponent = null!;
    public RuntimeNode? Parent;
    public List<RuntimeNode> Children;
    public List<Behavior> Behaviors;
    
    public RuntimeNode(Project project, Asset from, List<RuntimeNode>? children = null, List<Behavior>? behaviors = null) : base(project, from)
    {
        Children = children ?? new List<RuntimeNode>();
        Behaviors = behaviors ?? new List<Behavior>();
    }

    public async Task Load()
    {
        foreach (var child in Children)
        {
            await child.Load();
        }
        
        foreach (var behavior in Behaviors)
        {
            await behavior.OnLoad();
        }
    }

    public void Frame(FrameEventArgs args)
    {
        foreach (var child in Children)
        {
            child.Frame(args);
        }
        
        foreach (var behavior in Behaviors)
        {
            behavior.OnFrame(args);
        }
    }

    public override async Task Unload()
    {
        foreach (var child in Children)
        {
            await child.Unload();
        }
        
        foreach (var behavior in Behaviors)
        {
            await behavior.OnUnload();
        }
    }
}

public class NodeWorldComponent
{
    private RuntimeNode? _node;

    private Vector3 _position;
    private Quaternion _rotation;
    private Vector3 _scale;

    private Matrix4x4 _matrix;

    public NodeWorldComponent(RuntimeNode? node, Vector3? position, Quaternion? rotation, Vector3? scale)
    {
        _node = node;
        _position = position ?? Vector3.Zero;
        _rotation = rotation ?? Quaternion.Identity;
        _scale = scale ?? Vector3.One;
        _updateMatrix();
    }

    public RuntimeNode? Node
    {
        get => _node;
        set
        {
            _node = value;
            _updateMatrix();
        }
    }
    
    public Vector3 WorldPosition
    {
        get
        {
            Vector3? parentPosition = _node.Parent?.WorldComponent.WorldPosition;
            if (parentPosition == null) return _position;
            return (Vector3)parentPosition + _position;
        }
        set
        {
            Vector3? parentPosition = _node.Parent?.WorldComponent.WorldPosition;
            if (parentPosition == null) _position = value;
            else _position = value - (Vector3)parentPosition;
            _updateMatrix();
        }
    }
    
    public Quaternion WorldQuaternionRotation
    {
        get
        {
            Quaternion? parentRotation = _node.Parent?.WorldComponent.WorldQuaternionRotation;
            if (parentRotation == null) return _rotation;
            return (Quaternion)parentRotation + _rotation;
        }
        set
        {
            Quaternion? parentRotation = _node.Parent?.WorldComponent.WorldQuaternionRotation;
            if (parentRotation == null) _rotation = value;
            else _rotation = value - (Quaternion)parentRotation;
            _updateMatrix();
        }
    }

    public Vector3 WorldEulerRotation
    {
        get => ToEulerAngles(WorldQuaternionRotation);
        set => WorldQuaternionRotation = ToQuaternion(value);
    }
    
    public Vector3 WorldScale
    {
        get
        {
            Vector3? parentScale = _node.Parent?.WorldComponent.WorldScale;
            if (parentScale == null) return _scale;
            return (Vector3)parentScale + _scale;
        }
        set
        {
            Vector3? parentScale = _node.Parent?.WorldComponent.WorldScale;
            if (parentScale == null) _scale = value;
            else _scale = value - (Vector3)parentScale;
            _updateMatrix();
        }
    }

    public Vector3 Position
    {
        get => _position;
        set
        {
            _position = value;
            _updateMatrix();
        }
    }

    public Quaternion QuaternionRotation
    {
        get => _rotation;
        set
        {
            _rotation = value;
            _updateMatrix();
        }
    }

    public Vector3 EulerRotation
    {
        get => ToEulerAngles(_rotation);
        set
        {
            _rotation = ToQuaternion(value);
            _updateMatrix();
        }
    }

    public Vector3 Scale
    {
        get => _scale;
        set
        {
            _scale = value;
            _updateMatrix();
        }
    }

    public Matrix4x4 Matrix
    {
        get => _matrix;
        set => _matrix = value;
    }

    private void _updateMatrix()
    {
        _matrix = Matrix4x4.CreateTranslation(WorldPosition) * Matrix4x4.CreateFromQuaternion(WorldQuaternionRotation) * Matrix4x4.CreateScale(WorldScale);
    }
    
    private static Quaternion ToQuaternion(Vector3 v)
    {
        float cy = (float)Math.Cos(v.Z * 0.5);
        float sy = (float)Math.Sin(v.Z * 0.5);
        float cp = (float)Math.Cos(v.Y * 0.5);
        float sp = (float)Math.Sin(v.Y * 0.5);
        float cr = (float)Math.Cos(v.X * 0.5);
        float sr = (float)Math.Sin(v.X * 0.5);

        return new Quaternion
        {
            W = (cr * cp * cy + sr * sp * sy),
            X = (sr * cp * cy - cr * sp * sy),
            Y = (cr * sp * cy + sr * cp * sy),
            Z = (cr * cp * sy - sr * sp * cy)
        };
    }
    
    private static Vector3 ToEulerAngles(Quaternion q)
    {
        Vector3 angles = new();

        // roll / x
        double sinr_cosp = 2 * (q.W * q.X + q.Y * q.Z);
        double cosr_cosp = 1 - 2 * (q.X * q.X + q.Y * q.Y);
        angles.X = (float)Math.Atan2(sinr_cosp, cosr_cosp);

        // pitch / y
        double sinp = 2 * (q.W * q.Y - q.Z * q.X);
        if (Math.Abs(sinp) >= 1)
        {
            angles.Y = (float)Math.CopySign(Math.PI / 2, sinp);
        }
        else
        {
            angles.Y = (float)Math.Asin(sinp);
        }

        // yaw / z
        double siny_cosp = 2 * (q.W * q.Z + q.X * q.Y);
        double cosy_cosp = 1 - 2 * (q.Y * q.Y + q.Z * q.Z);
        angles.Z = (float)Math.Atan2(siny_cosp, cosy_cosp);

        return angles;
    }
}