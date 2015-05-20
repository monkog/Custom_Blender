using System;
using System.Drawing;
using System.Windows.Media.Media3D;
using RayTracer.Model.Shapes;
using RayTracer.ViewModel;

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
        /// <summary>
        /// Trims the vector to the screen.
        /// </summary>
        /// <param name="vector">The vector.</param>
        /// <returns>Vector with positions within the screen bounds</returns>
        public static Vector4 TrimToScreen(this Vector4 vector)
        {
            return new Vector4(Math.Min(SceneManager.Width, Math.Max(0, vector.X)), Math.Min(SceneManager.Height, Math.Max(0, vector.Y)), vector.Z, vector.A);
        }
        /// <summary>
        /// Performs the Gauss ellimination on the matrix.
        /// </summary>
        /// <param name="a">The matrix.</param>
        /// <param name="b">The result vector.</param>
        /// <returns>Vector of results of Gauss ellimination on Matrix a</returns>
        public static double[] GaussElimination(this double[,] a, double[] b)
        {
            int n = b.Length;
            double[] x = new double[n];

            double[,] tmpA = new double[n, n + 1];

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    tmpA[i, j] = a[i, j];
                }
                tmpA[i, n] = b[i];
            }

            double tmp = 0;

            for (int k = 0; k < n - 1; k++)
            {
                for (int i = k + 1; i < n; i++)
                {
                    tmp = tmpA[i, k] / tmpA[k, k];
                    for (int j = k; j < n + 1; j++)
                    {
                        tmpA[i, j] -= tmp * tmpA[k, j];
                    }
                }
            }

            for (int k = n - 1; k >= 0; k--)
            {
                tmp = 0;
                for (int j = k + 1; j < n; j++)
                {
                    tmp += tmpA[k, j] * x[j];
                }
                x[k] = (tmpA[k, n] - tmp) / tmpA[k, k];
            }

            return x;
        }
        //public static double[] LinearGaussElimination(this double[,] a, double[] b)
        //{
        //    int n = b.Length;
        //    double[] x = new double[n];

        //    double tmp = 0;

        //    for (int k = 2; k < n - 1; k++)
        //    {
        //        for (int i = k + 1; i < n; i++)
        //        {
        //            tmp = tmpA[i, k] / tmpA[k, k];
        //            for (int j = k; j < n + 1; j++)
        //            {
        //                tmpA[i, j] -= tmp * tmpA[k, j];
        //            }
        //        }
        //    }

        //    for (int k = n - 1; k >= 0; k--)
        //    {
        //        tmp = 0;
        //        for (int j = k + 1; j < n; j++)
        //        {
        //            tmp += tmpA[k, j] * x[j];
        //        }
        //        x[k] = (tmpA[k, n] - tmp) / tmpA[k, k];
        //    }

        //    return x;
        //}
        public static void FindMaxMinCoords(this PointEx[,] points, out double minX, out double minY, out double maxX, out double maxY)
        {
            maxX = double.MinValue;
            maxY = double.MinValue;
            minX = double.MaxValue;
            minY = double.MaxValue;
            for (int i = 0; i < points.GetLength(0); i++)
                for (int j = 0; j < points.GetLength(1); j++)
                {
                    var point = points[i, j].PointOnScreen;
                    if (maxX < point.X) maxX = point.X;
                    if (maxY < point.Y) maxY = point.Y;
                    if (minX > point.X) minX = point.X;
                    if (minY > point.Y) minY = point.Y;
                }
        }
        /// <summary>
        /// Gets the value of the N function.<para/>
        /// |\  | n<para/>
        /// | \ |   (ti)<para/>
        /// |  \| i
        /// </summary>
        /// <param name="knots">The knots array for evaluating N.</param>
        /// <param name="i">The i.</param>
        /// <param name="n">The n.</param>
        /// <param name="ti">The ti.</param>
        /// <returns></returns>
        public static double GetNFunctionValue(this double[] knots, int i, int n, double ti)
        {
            if (n < 0)
                return 0;
            if (n == 0)
            {
                if (ti >= knots[i] && ti < knots[i + 1])
                    return 1;
                return 0;
            }

            double a = knots[i + n] - knots[i] != 0 ? (ti - knots[i]) / (knots[i + n] - knots[i]) : 0;
            double b = knots[i + n + 1] - knots[i + 1] != 0 ? (knots[i + n + 1] - ti) / (knots[i + n + 1] - knots[i + 1]) : 0;
            return a * knots.GetNFunctionValue(i, n - 1, ti) + b * knots.GetNFunctionValue(i + 1, n - 1, ti);
        }
        /// <summary>
        /// Calculates the N matrix for n degree polynomials.
        /// </summary>
        /// <param name="knots">The knots.</param>
        /// <param name="n">The degree of the polynomials.</param>
        /// <param name="knotsCount">The knots count.</param>
        /// <returns>N matrix</returns>
        public static double[,] CalculateNMatrix(this double[] knots, int n, int knotsCount)
        {

            double[,] nMatrix = new double[knotsCount + 2, knotsCount + 2];

            for (int i = 1; i <= knotsCount + 2; i++)
                for (int j = 1; j <= knotsCount + 2; j++)
                {
                    double t = knots[i + n - 1];
                    nMatrix[j - 1, i - 1] = knots.GetNFunctionValue(j, n, t);
                }

            return nMatrix;
        }
    }
}
