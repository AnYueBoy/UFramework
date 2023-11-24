using System;
using UnityEngine;

public class RotationTransformation : Transformation
{
    public Vector3 rotation;

    // public override Vector3 Apply(Vector3 point)
    // {
    //     float radX = rotation.x * Mathf.Deg2Rad;
    //     float radY = rotation.y * Mathf.Deg2Rad;
    //     float radZ = rotation.z * Mathf.Deg2Rad;
    //     float sinX = Mathf.Sin(radX);
    //     float cosX = Mathf.Cos(radX);
    //     float sinY = Mathf.Sin(radY);
    //     float cosY = Mathf.Cos(radY);
    //     float sinZ = Mathf.Sin(radZ);
    //     float cosZ = Mathf.Cos(radZ);
    //
    //     Vector3 xAxis = new Vector3(
    //         cosY * cosZ,
    //         cosX * sinZ + sinX * sinY * cosZ,
    //         sinX * sinZ - cosX * sinY * cosZ
    //     );
    //     Vector3 yAxis = new Vector3(
    //         -cosY * sinZ,
    //         cosX * cosZ - sinX * sinY * sinZ,
    //         sinX * cosZ + cosX * sinY * sinZ
    //     );
    //     Vector3 zAxis = new Vector3(
    //         sinY,
    //         -sinX * cosY,
    //         cosX * cosY
    //     );
    //
    //     // float a, b, c, d, e, f, g, h, i;
    //     // float x, y, z;
    //     // Vector3 xAsisTest = new Vector3(a, d, g);
    //     // Vector3 yAxisTest = new Vector3(b, e, h);
    //     // Vector3 zAxisTest = new Vector3(c, f, i);
    //     // Vector3 result = xAsisTest * x + yAxisTest * y + zAxisTest * point.z;
    //     // result = new Vector3(a * x, d * x, g * x) + new Vector3(b * y, e * y, h * y) + new Vector3(c * z, f * z, i * z);
    //     // result = new Vector3(a * x + b * y + c * z, d * x + e * y + f * z, g * x + h * y + i * z);
    //
    //     return xAxis * point.x + yAxis * point.y + zAxis * point.z;
    // }
    public override Matrix4x4 Matrix
    {
        get
        {
            float radX = rotation.x * Mathf.Deg2Rad;
            float radY = rotation.y * Mathf.Deg2Rad;
            float radZ = rotation.z * Mathf.Deg2Rad;
            float sinX = Mathf.Sin(radX);
            float cosX = Mathf.Cos(radX);
            float sinY = Mathf.Sin(radY);
            float cosY = Mathf.Cos(radY);
            float sinZ = Mathf.Sin(radZ);
            float cosZ = Mathf.Cos(radZ);

            Vector3 xAxis = new Vector3(
                cosY * cosZ,
                cosX * sinZ + sinX * sinY * cosZ,
                sinX * sinZ - cosX * sinY * cosZ
            );
            Vector3 yAxis = new Vector3(
                -cosY * sinZ,
                cosX * cosZ - sinX * sinY * sinZ,
                sinX * cosZ + cosX * sinY * sinZ
            );
            Vector3 zAxis = new Vector3(
                sinY,
                -sinX * cosY,
                cosX * cosY
            );
            Matrix4x4 matrix = new Matrix4x4();
            matrix.SetColumn(0, new Vector4(cosY * cosZ,
                cosX * sinZ + sinX * sinY * cosZ,
                sinX * sinZ - cosX * sinY * cosZ, 0f));
            matrix.SetColumn(1, new Vector4(
                -cosY * sinZ,
                cosX * cosZ - sinX * sinY * sinZ,
                sinX * cosZ + cosX * sinY * sinZ,
                0f
            ));
            matrix.SetColumn(2, new Vector4(
                sinY,
                -sinX * cosY,
                cosX * cosY,
                0f
            ));
            matrix.SetColumn(3, new Vector4(
                0f,
                0f,
                0f,
                1f
            ));

            return matrix;
        }
    }
}