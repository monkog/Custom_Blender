using System;
using System.Windows.Media.Media3D;

namespace RayTracer.Helpers
{
    public class Vector4
    {
        private double _x, _y, _z, _a;

        public Vector4(double x, double y, double z, double a)
        {
            _x = x;
            _y = y;
            _z = z;
            _a = a;
        }

        public double X { get { return _x; } set { _x = value; } }
        public double Y { get { return _y; } set { _y = value; } }
        public double Z { get { return _z; } set { _z = value; } }
        public double A { get { return _a; } set { _a = value; } }

        public double Length { get { return Math.Sqrt(_x * _x + _y * _y + _z * _z + _a * _a); } }

        public double LengthSquared { get { return _x * _x + _y * _y + _z * _z + _a * _a; } }

        public Vector4 Normalized
        {
            get
            {
                if (_a == 0)
                    return this;
                return this / _a;
            }
        }

        public double Dot(Vector4 vector)
        {
            return _x * vector.X + _y * vector.Y + _z * vector.Z + _a * vector.A;
        }

        public Vector4 Cross(Vector4 v)
        {
            return new Vector4(Y * v.Z - (Z * v.Y), Z * v.X - (X * v.Z), X * v.Y - (Y * v.Z), 1);
        }

        public static Vector4 operator *(Vector4 vector, double value)
        {
            return new Vector4(vector.X * value, vector.Y * value, vector.Z * value, vector.A * value);
        }

        public static double operator *(Vector4 vector, Vector4 value)
        {
            return vector.X * value.X + vector.Y * value.Y + vector.Z * value.Z + vector.A * value.A;
        }

        public static Vector4 operator /(Vector4 vector, double value)
        {
            return new Vector4(vector.X / value, vector.Y / value, vector.Z / value, vector.A / value);
        }

        public static Vector4 operator +(Vector4 vector, Vector4 vector1)
        {
            return new Vector4(vector.X + vector1.X, vector.Y + vector1.Y, vector.Z + vector1.Z, vector.A + vector1.A);
        }

        public static Vector4 operator -(Vector4 vector, Vector4 vector1)
        {
            return new Vector4(vector.X - vector1.X, vector.Y - vector1.Y, vector.Z - vector1.Z, vector.A - vector1.A);
        }

        public static Vector4 operator *(Matrix3D matrix, Vector4 vector)
        {
            return new Vector4(vector.X * matrix.M11 + vector.Y * matrix.M12 + vector.Z * matrix.M13 + vector.A * matrix.M14
                                , vector.X * matrix.M21 + vector.Y * matrix.M22 + vector.Z * matrix.M23 + vector.A * matrix.M24
                                , vector.X * matrix.M31 + vector.Y * matrix.M32 + vector.Z * matrix.M33 + vector.A * matrix.M34
                                , vector.X * matrix.OffsetX + vector.Y * matrix.OffsetY + vector.Z * matrix.OffsetZ + vector.A * matrix.M44);
        }

        public static Vector4 operator *(Vector4 vector, Matrix3D matrix)
        {
            return new Vector4(vector.X * matrix.M11 + vector.Y * matrix.M21 + vector.Z * matrix.M31 + vector.A * matrix.OffsetX
                                , vector.X * matrix.M12 + vector.Y * matrix.M22 + vector.Z * matrix.M32 + vector.A * matrix.OffsetY
                                , vector.X * matrix.M13 + vector.Y * matrix.M23 + vector.Z * matrix.M33 + vector.A * matrix.OffsetZ
                                , vector.X * matrix.M14 + vector.Y * matrix.M24 + vector.Z * matrix.M34 + vector.A * matrix.M44);
        }

        public static Vector4 operator *(Vector4 vector, double[,] matrix)
        {
            if (matrix.GetLength(0) != 4 || matrix.GetLength(1) != 4)
                throw new Exception("Matrix dimensions should be 4x4");

            return new Vector4(vector.X * matrix[0, 0] + vector.Y * matrix[0, 1] + vector.Z * matrix[0, 2] + vector.A * matrix[0, 3]
                                , vector.X * matrix[1, 0] + vector.Y * matrix[1, 1] + vector.Z * matrix[1, 2] + vector.A * matrix[1, 3]
                                , vector.X * matrix[2, 0] + vector.Y * matrix[2, 1] + vector.Z * matrix[2, 2] + vector.A * matrix[2, 3]
                                , vector.X * matrix[3, 0] + vector.Y * matrix[3, 1] + vector.Z * matrix[3, 2] + vector.A * matrix[3, 3]);
        }
    }
}
