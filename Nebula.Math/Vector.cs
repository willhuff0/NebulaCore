using System.Numerics;

namespace Nebula.Math;

public class Vector2<T> where T : INumber<T>
{
    public T X;
    public T Y;

    public Vector2(T x, T y)
    {
        X = x;
        Y = y;
    }

    public T[] ToArray() => new[] { X, Y };
}

public class Vector3<T> where T : INumber<T>
{
    public T X;
    public T Y;
    public T Z;

    public Vector3(T x, T y, T z)
    {
        X = x;
        Y = y;
        Z = z;
    }

    public Vector3(Vector3<T> copy)
    {
        X = copy.X;
        Y = copy.Y;
        Z = copy.Z;
    }

    public T[] ToArray() => new[] { X, Y, Z };

    public static Vector3<T> operator /(Vector3<T> v1, T v2) => new(v1.X / v2, v1.Y / v2, v1.Z / v2);

    public override string ToString() => $"({X},{Y},{Z})";
}

public static class Vector3Extensions
{
    public static T Length<T>(this Vector3<T> v) where T : INumber<T>, IRootFunctions<T> => T.Sqrt(v.X * v.X + v.Y * v.Y + v.Z * v.Z);

    public static Vector3<T> Normalized<T>(this Vector3<T> v) where T : INumber<T>, IRootFunctions<T> => v / v.Length();
}

public class Vector4<T> where T : INumber<T>
{
    public T X;
    public T Y;
    public T Z;
    public T W;

    public Vector4(T x, T y, T z, T w)
    {
        X = x;
        Y = y;
        Z = z;
        W = w;
    }
    
    public T[] ToArray() => new[] { X, Y, Z, W };
}

public class Vector2 : Vector2<double>
{
    public Vector2(double x, double y) : base(x, y)
    {
        
    }
}

public class Vector3 : Vector3<double>
{
    public Vector3(double x, double y, double z) : base(x, y, z)
    {
        
    }
}

public class Vector4 : Vector4<double>
{
    public Vector4(double x, double y, double z, double w) : base(x, y, z, w)
    {
        
    }
}
