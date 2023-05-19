namespace Nebula.Math;

public class Vector2<T>
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

public class Vector3<T>
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
    
    public T[] ToArray() => new[] { X, Y, Z };
}

public class Vector4<T>
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
