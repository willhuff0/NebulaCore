namespace Nebula.Math;

public class Matrix4<T>
{
    public T V00; // Column 0, Row 0
    public T V10; // Column 1, Row 0
    public T V20; // Column 2, Row 0
    public T V30; // Column 3, Row 0
    public T V01; // Column 0, Row 1
    public T V11; // Column 1, Row 1
    public T V21; // Column 2, Row 1
    public T V31; // Column 3, Row 1
    public T V02; // Column 0, Row 2
    public T V12; // Column 1, Row 2
    public T V22; // Column 2, Row 2
    public T V32; // Column 3, Row 2
    public T V03; // Column 0, Row 3
    public T V13; // Column 1, Row 3
    public T V23; // Column 2, Row 3
    public T V33; // Column 3, Row 3

    // Row major constructor
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
    
    public T[] ToArrayColumnMajor() => new[] { V00, V01, V02, V03, V10, V11, V12, V13, V20, V21, V22, V23, V30, V31, V32, V33 };
    public T[] ToArrayRowMajor() => new[] { V00, V10, V20, V30, V01, V11, V21, V31, V02, V12, V22, V32, V03, V13, V23, V33 };
    
    public static Matrix4<T> operator * (Matrix4<T> m1, Matrix4<T> m2)
    {
        
    }
}

public class Matrix4 : Matrix4<double>
{
    public Matrix4(double v00, double v10, double v20, double v30, double v01, double v11, double v21, double v31, double v02, double v12, double v22, double v32, double v03, double v13, double v23, double v33) : base(v00, v10, v20, v30, v01, v11, v21, v31, v02, v12, v22, v32, v03, v13, v23, v33)
    {
        
    }

    public static Matrix4 MakePerspective(double aspect, double fov, double near, double far) => new(
        1.0 / aspect * double.Tan(fov / 2.0), 0.0,                         0.0,                          0.0,
        0.0,                                  1.0 / double.Tan(fov / 2.0), 0.0,                          0.0,
        0.0,                                  0.0,                         -((far + near)/(far - near)), -((2.0 * far * near)/(far - near)),
        0.0,                                  0.0,                         -1.0,                         0.0
    );

    public static Matrix4 Identity() => new(
        1.0, 0.0, 0.0, 0.0,
        0.0, 1.0, 0.0, 0.0,
        0.0, 0.0, 1.0, 0.0,
        0.0, 0.0, 0.0, 1.0
        );
}