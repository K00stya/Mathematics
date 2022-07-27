using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace Vector
{
[Serializable]
public class MyVector3 : IFormattable
{
    public float x;
    public float y;
    public float z;

    public MyVector3(float x, float y, float z)
    {
        this.x = x; this.y = y; this.z = z;
    }

    public Vector3 ToUnityVector3()
    {
        return new Vector3(x,y,z);
    }

    public void SetUnityVector3(Vector3 vector3)
    {
        x = vector3.x; y = vector3.y; z = vector3.z;
    }

    public float Magnitude()
    {
        return (float)Math.Sqrt(Math.Pow(x, 2d) + Math.Pow(y, 2d) + Math.Pow(z, 2d));
        return Vector3.Magnitude(new Vector3(x, y, z));
    }

    public MyVector3 Normalize()
    {
        return this / Magnitude();
    }
    
    public static MyVector3 operator *(MyVector3 vector, float t) => new (vector.x * t,vector.y * t,vector.z * t);
    
    public static MyVector3 operator /(MyVector3 vector, float d)
    {
        d = 1f / d;
        return new MyVector3(vector.x * d, vector.y * d, vector.z * d);
    }
    public static MyVector3 operator +(MyVector3 a, MyVector3 b) => new(a.x + b.x, a.y + b.y, a.z + b.z);
    public static MyVector3 operator -(MyVector3 a, MyVector3 b) => new(a.x - b.x, a.y - b.y, a.z - b.z);

    public override string ToString() => ToString(null, null);
    
    public string ToString(string format, IFormatProvider formatProvider)
    {
        format ??= "F2";
        formatProvider ??= CultureInfo.InvariantCulture.NumberFormat;
        return string.Format("({0}, {1}, {2})",
            x.ToString(format, formatProvider),
            y.ToString(format, formatProvider),
            z.ToString(format, formatProvider));
    }
}
}

