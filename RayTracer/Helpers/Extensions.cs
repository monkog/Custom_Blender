using System.Windows.Media.Media3D;

namespace RayTracer.Helpers
{
    public static class Extensions
    {
        public static Matrix3D Transpose(this Matrix3D matrix)
        {
            return new Matrix3D(matrix.M11, matrix.M21, matrix.M31, matrix.OffsetX
                              , matrix.M12, matrix.M22, matrix.M32, matrix.OffsetY
                              , matrix.M13, matrix.M23, matrix.M33, matrix.OffsetZ
                              , matrix.M14, matrix.M24, matrix.M34, matrix.M44);
        }
    }
}
