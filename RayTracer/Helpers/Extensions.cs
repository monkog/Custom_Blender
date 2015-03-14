using System;
using System.Drawing;
using System.Windows.Media.Media3D;

namespace RayTracer.Helpers
{
    public static class Extensions
    {
        /// <summary>
        /// Transposes the specified matrix.
        /// </summary>
        /// <param name="matrix">The matrix.</param>
        /// <returns>Transposed matrix</returns>
        public static Matrix3D Transpose(this Matrix3D matrix)
        {
            return new Matrix3D(matrix.M11, matrix.M21, matrix.M31, matrix.OffsetX
                              , matrix.M12, matrix.M22, matrix.M32, matrix.OffsetY
                              , matrix.M13, matrix.M23, matrix.M33, matrix.OffsetZ
                              , matrix.M14, matrix.M24, matrix.M34, matrix.M44);
        }
        /// <summary>
        /// Combines the colors.
        /// </summary>
        /// <param name="color">The color.</param>
        /// <param name="value">The value.</param>
        /// <returns>Combined colors</returns>
        public static Color CombinedColor(this Color color, Color value)
        {
            return Color.FromArgb(Math.Min(Math.Max(color.R + value.R, 0), 255)
                                , Math.Min(Math.Max(color.G + value.G, 0), 255)
                                , Math.Min(Math.Max(color.B + value.B, 0), 255));
        }
    }
}
