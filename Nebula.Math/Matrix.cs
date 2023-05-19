namespace Nebula.Math;

// Column major order
public class Matrix4<T>
{
    // Column 0
    public T V00;
    public T V10;
    public T V20;
    public T V30;
    
    // Column 1
    public T V01;
    public T V11;
    public T V21;
    public T V31;
    
    // Column 2
    public T V02;
    public T V12;
    public T V22;
    public T V32;
    
    // Column 3
    public T V03;
    public T V13;
    public T V23;
    public T V33;

    public Matrix4(T v00, T v10, T v20, T v30, T v01, T v11, T v21, T v31, T v02, T v12, T v22, T v32, T v03, T v13, T v23, T v33)
    {
        V00 = v00;
        V10 = v10;
        V20 = v20;
        V30 = v30;
        V01 = v01;
        V11 = v11;
        V21 = v21;
        V31 = v31;
        V02 = v02;
        V12 = v12;
        V22 = v22;
        V32 = v32;
        V03 = v03;
        V13 = v13;
        V23 = v23;
        V33 = v33;
    }

    public T[] ToArray() => new[] { V00, V10, V20, V30, V01, V11, V21, V31, V02, V12, V22, V32, V03, V13, V23, V33 };
}

public class Matrix4 : Matrix4<double>
{
    public Matrix4(double v00, double v10, double v20, double v30, double v01, double v11, double v21, double v31, double v02, double v12, double v22, double v32, double v03, double v13, double v23, double v33) : base(v00, v10, v20, v30, v01, v11, v21, v31, v02, v12, v22, v32, v03, v13, v23, v33)
    {
        
    }
}