﻿using System;
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
        /// Multiplies the specified matrix by the specified value.
        /// </summary>
        /// <param name="matrix">The matrix.</param>
        /// <param name="value">Value to multiply by</param>
        /// <returns>Multiplied matrix</returns>
        public static Matrix3D Multiply(this Matrix3D matrix, double value)
        {
            return new Matrix3D(matrix.M11 * value, matrix.M21 * value, matrix.M31 * value, matrix.OffsetX * value
                              , matrix.M12 * value, matrix.M22 * value, matrix.M32 * value, matrix.OffsetY * value
                              , matrix.M13 * value, matrix.M23 * value, matrix.M33 * value, matrix.OffsetZ * value
                              , matrix.M14 * value, matrix.M24 * value, matrix.M34 * value, matrix.M44 * value);
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
        /// <summary>
        /// Gets the bezier point.
        /// </summary>
        /// <param name="v">The v.</param>
        /// <returns>Bezier point: [B_3_0, B_3_1, B_3_2, B_3_3]</returns>
        public static Vector4 GetBezierPoint(this double v)
        {
            return new Vector4(Math.Pow((1.0 - v), 3), 3 * v * Math.Pow((1.0 - v), 2), 3 * v * v * (1.0 - v), Math.Pow(v, 3));
        }
        /// <summary>
        /// Gets the bezier derivative point.
        /// </summary>
        /// <param name="v">The v.</param>
        /// <returns>Bezier point: [B_3_0', B_3_1', B_3_2', B_3_3']</returns>
        public static Vector4 GetBezierDerivativePoint(this double v)
        {
            return new Vector4(-3 * Math.Pow((1.0 - v), 2), (9 * v * v) - (12 * v) + 3, 3 * v * (2.0 - 3 * v), 3 * Math.Pow(v, 2));
        }
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

            double a = (knots[i + n] - knots[i] != 0) ? (ti - knots[i]) / (knots[i + n] - knots[i]) : 0;
            double b = (knots[i + n + 1] - knots[i + 1] != 0) ? (knots[i + n + 1] - ti) / (knots[i + n + 1] - knots[i + 1]) : 0;
            return (a * knots.GetNFunctionValue(i, n - 1, ti)) + (b * knots.GetNFunctionValue(i + 1, n - 1, ti));
        }
        /// <summary>
        /// Gets the value of the N function derivative.<para/>
        /// |\  | n<para/>
        /// | \ |   (ti)<para/>
        /// |  \| i
        /// </summary>
        /// <param name="knots">The knots array for evaluating N.</param>
        /// <param name="i">The i.</param>
        /// <param name="n">The n.</param>
        /// <param name="ti">The ti.</param>
        /// <returns></returns>
        public static double GetNFunctionDerivativeValue(this double[] knots, int i, int n, double ti)
        {
            if (n < 0)
                return 0;
            if (n == 0)
            {
                if (ti >= knots[i] && ti < knots[i + 1])
                    return 1;
                return 0;
            }

            double a = (knots[i + n] - knots[i] != 0) ? n / (knots[i + n] - knots[i]) : 0;
            double b = (knots[i + n + 1] - knots[i + 1] != 0) ? n / (knots[i + n + 1] - knots[i + 1]) : 0;
            return (a * knots.GetNFunctionValue(i, n - 1, ti)) + (b * knots.GetNFunctionValue(i + 1, n - 1, ti));
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
        /// <summary>
        /// Finds the Coordinateses of an item in two dimmensional array
        /// </summary>
        /// <typeparam name="T">Type of the item</typeparam>
        /// <param name="matrix">The matrix.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static Tuple<int, int> CoordinatesOf<T>(this T[,] matrix, T value)
        {
            int w = matrix.GetLength(0); // width
            int h = matrix.GetLength(1); // height

            for (int x = 0; x < w; ++x)
            {
                for (int y = 0; y < h; ++y)
                {
                    if (matrix[x, y].Equals(value))
                        return Tuple.Create(x, y);
                }
            }

            return Tuple.Create(-1, -1);
        }
    }
}
