using System.Numerics;

namespace Nebula.Math;

public class Matrix4<T> where T : INumber<T>
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

    public Matrix4(Matrix4<T> copy)
    {
        V00 = copy.V00;
        V10 = copy.V10;
        V20 = copy.V20;
        V30 = copy.V30;
        V01 = copy.V01;
        V11 = copy.V11;
        V21 = copy.V21;
        V31 = copy.V31;
        V02 = copy.V02;
        V12 = copy.V12;
        V22 = copy.V22;
        V32 = copy.V32;
        V03 = copy.V03;
        V13 = copy.V13;
        V23 = copy.V23;
        V33 = copy.V33;
    }

    public T[] ToColumnMajorArray() => new[] { V00, V01, V02, V03, V10, V11, V12, V13, V20, V21, V22, V23, V30, V31, V32, V33 };
    public T[] ToRowMajorArray() => new[] { V00, V10, V20, V30, V01, V11, V21, V31, V02, V12, V22, V32, V03, V13, V23, V33 };

    private Matrix4() {}
    
    public static Matrix4<double> Identity() => new(
        1.0, 0.0, 0.0, 0.0,
        0.0, 1.0, 0.0, 0.0,
        0.0, 0.0, 1.0, 0.0,
        0.0, 0.0, 0.0, 1.0
    );

    public static Matrix4<double> Perspective(double aspect, double fov, double near, double far)
    {
        var f = 1.0 / double.Tan(fov / 2.0);
        return new(
            f/ aspect, 0.0, 0.0, 0.0,
            0.0, f, 0.0, 0.0,
            0.0, 0.0, -((far + near) / (near - far)), -((2.0 * far * near) / (near - far)),
            0.0, 0.0, -1.0, 0.0
        );
    }

    public static Matrix4<double> Translation(double x, double y, double z) => new(
        1.0, 0.0, 0.0, x,
        0.0, 1.0, 0.0, y,
        0.0, 0.0, 1.0, z,
        0.0, 0.0, 0.0, 1.0
    );

    public static Matrix4<T> operator *(Matrix4<T> m1, Matrix4<T> m2) => new()
    {
        V00 = m1.V00 * m2.V00 + m1.V10 * m2.V01 + m1.V20 * m2.V02 + m1.V30 * m2.V03,
        V10 = m1.V00 * m2.V10 + m1.V10 * m2.V11 + m1.V20 * m2.V12 + m1.V30 * m2.V13,
        V20 = m1.V00 * m2.V20 + m1.V10 * m2.V21 + m1.V20 * m2.V22 + m1.V30 * m2.V23,
        V30 = m1.V00 * m2.V30 + m1.V10 * m2.V31 + m1.V20 * m2.V32 + m1.V30 * m2.V33,

        V01 = m1.V01 * m2.V00 + m1.V11 * m2.V01 + m1.V21 * m2.V02 + m1.V31 * m2.V03,
        V11 = m1.V01 * m2.V10 + m1.V11 * m2.V11 + m1.V21 * m2.V12 + m1.V31 * m2.V13,
        V21 = m1.V01 * m2.V20 + m1.V11 * m2.V21 + m1.V21 * m2.V22 + m1.V31 * m2.V23,
        V31 = m1.V01 * m2.V30 + m1.V11 * m2.V31 + m1.V21 * m2.V32 + m1.V31 * m2.V33,

        V02 = m1.V02 * m2.V00 + m1.V12 * m2.V01 + m1.V22 * m2.V02 + m1.V32 * m2.V03,
        V12 = m1.V02 * m2.V10 + m1.V12 * m2.V11 + m1.V22 * m2.V12 + m1.V32 * m2.V13,
        V22 = m1.V02 * m2.V20 + m1.V12 * m2.V21 + m1.V22 * m2.V22 + m1.V32 * m2.V23,
        V32 = m1.V02 * m2.V30 + m1.V12 * m2.V31 + m1.V22 * m2.V32 + m1.V32 * m2.V33,

        V03 = m1.V03 * m2.V00 + m1.V13 * m2.V01 + m1.V23 * m2.V02 + m1.V33 * m2.V03,
        V13 = m1.V03 * m2.V10 + m1.V13 * m2.V11 + m1.V23 * m2.V12 + m1.V33 * m2.V13,
        V23 = m1.V03 * m2.V20 + m1.V13 * m2.V21 + m1.V23 * m2.V22 + m1.V33 * m2.V23,
        V33 = m1.V03 * m2.V30 + m1.V13 * m2.V31 + m1.V23 * m2.V32 + m1.V33 * m2.V33,
    };
}

public class Matrix4 : Matrix4<double> {
    public Matrix4(double v00, double v10, double v20, double v30, double v01, double v11, double v21, double v31, double v02, double v12, double v22, double v32, double v03, double v13, double v23, double v33) : base(v00, v10, v20, v30, v01, v11, v21, v31, v02, v12, v22, v32, v03, v13, v23, v33)
    {
    }

    public Matrix4(Matrix4<double> copy) : base(copy)
    {
    }
}

public static class Matrix4DoubleExtensions
{
    public static float[] ToColumnMajorFloatArray(this Matrix4<double> m) => new[] { (float)m.V00, (float)m.V01, (float)m.V02, (float)m.V03, (float)m.V10, (float)m.V11, (float)m.V12, (float)m.V13, (float)m.V20, (float)m.V21, (float)m.V22, (float)m.V23, (float)m.V30, (float)m.V31, (float)m.V32, (float)m.V33 };
    public static float[] ToRowMajorFloatArray(this Matrix4<double> m) => new[] { (float)m.V00, (float)m.V10, (float)m.V20, (float)m.V30, (float)m.V01, (float)m.V11, (float)m.V21, (float)m.V31, (float)m.V02, (float)m.V12, (float)m.V22, (float)m.V32, (float)m.V03, (float)m.V13, (float)m.V23, (float)m.V33 };
}