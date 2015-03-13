using System;
using System.Windows.Media.Media3D;
using RayTracer.Helpers;

namespace RayTracer.ViewModel
{
    public static class Transformations
    {
        #region Public Properties
        /// <summary>
        /// Gets the identity matrix.
        /// </summary>
        /// <value>
        /// The identity matrix.
        /// </value>
        public static Matrix3D Identity
        {
            get
            {
                return new Matrix3D(1, 0, 0, 0
                                  , 0, 1, 0, 0
                                  , 0, 0, 1, 0
                                  , 0, 0, 0, 1);
            }
        }
        #endregion Public Properties
        #region Public Methods
        /// <summary>
        /// Calculates the versor of the given vector.
        /// </summary>
        /// <param name="vector">Given vector.</param>
        /// <returns>Returns the calculated versor.</returns>
        public static Vector3D Versor(Vector3D vector)
        {
            double sqrt = Math.Sqrt(vector.X * vector.X + vector.Y * vector.Y + vector.Z * vector.Z);
            return sqrt == 0 ? vector : new Vector3D(vector.X / sqrt, vector.Y / sqrt, vector.Z / sqrt);
        }
        /// <summary>
        /// Calculates the view matrix.
        /// </summary>
        /// <param name="r">The distance between the object and the projection surface</param>
        /// <returns>Returns the view matrix.</returns>
        public static Matrix3D ViewMatrix(double r)
        {
            return new Matrix3D(1, 0, 0, 0
                              , 0, 1, 0, 0
                              , 0, 0, 0, 0
                              , 0, 0, 1 / r, 1);
        }
        /// <summary>
        /// Creates the dot product of two 3D vectors.
        /// </summary>
        /// <param name="v">The first vector.</param>
        /// <param name="w">The second vector.</param>
        /// <returns>The dot product of two 3D vectors.</returns>
        public static double DotProduct(Vector3D v, Vector3D w)
        {
            return v.X * w.X + v.Y * w.Y + v.Z * w.Z;
        }
        /// <summary>
        /// Transforms the point.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <param name="matrix">The matrix.</param>
        /// <returns>Vector multiplied by the matrix</returns>
        public static Vector4 TransformPoint(Vector4 point, Matrix3D matrix)
        {
            return matrix * point;
        }
        /// <summary>
        /// Creates rotation matrix for rotation on x axis.
        /// </summary>
        /// <param name="alpha">The alpha.</param>
        /// <returns>Rotation matrix for rotation on x axis.</returns>
        public static Matrix3D RotationMatrixX(double alpha)
        {
            return new Matrix3D(1, 0, 0, 0
                              , 0, Math.Cos(alpha), -Math.Sin(alpha), 0
                              , 0, Math.Sin(alpha), Math.Cos(alpha), 0
                              , 0, 0, 0, 1);
        }
        /// <summary>
        /// Creates rotation matrix for rotation on y axis.
        /// </summary>
        /// <param name="alpha">The alpha.</param>
        /// <returns>Rotation matrix for rotation on y axis.</returns>
        public static Matrix3D RotationMatrixY(double alpha)
        {
            return new Matrix3D(Math.Cos(alpha), 0, Math.Sin(alpha), 0
                              , 0, 1, 0, 0
                              , -Math.Sin(alpha), 0, Math.Cos(alpha), 0
                              , 0, 0, 0, 1);
        }
        /// <summary>
        /// Creates rotation matrix for rotation on z axis.
        /// </summary>
        /// <param name="alpha">The alpha.</param>
        /// <returns>Rotation matrix for rotation on z axis.</returns>
        public static Matrix3D RotationMatrixZ(double alpha)
        {
            return new Matrix3D(Math.Cos(alpha), -Math.Sin(alpha), 0, 0
                              , Math.Sin(alpha), Math.Cos(alpha), 0, 0
                              , 0, 0, 1, 0
                              , 0, 0, 0, 1);
        }
        /// <summary>
        /// Returns the translation matrix.
        /// </summary>
        /// <param name="vector">The vector.</param>
        /// <returns>The translation matrix</returns>
        public static Matrix3D TranslationMatrix(Vector3D vector)
        {
            return new Matrix3D(1, 0, 0, vector.X
                              , 0, 1, 0, vector.Y
                              , 0, 0, 1, vector.Z
                              , 0, 0, 0, 1);
        }
        /// <summary>
        /// Returns the scale matrix.
        /// </summary>
        /// <param name="scale">The scale.</param>
        /// <returns>The scale matrix</returns>
        public static Matrix3D ScaleMatrix(double scale)
        {
            return new Matrix3D(scale, 0, 0, 0
                              , 0, scale, 0, 0
                              , 0, 0, scale, 0
                              , 0, 0, 0, 1);
        }
        /// <summary>
        /// Stereographics the left matrix.
        /// </summary>
        /// <param name="e">The e.</param>
        /// <param name="r">The distance between the object and the projection surface</param>
        /// <returns>Returns the left stereographic view matrix.</returns>
        public static Matrix3D StereographicLeftMatrix(double e, double r)
        {
            return new Matrix3D(1, 0, -e / (2 * r), 0
                              , 0, 1, 0, 0
                              , 0, 0, 0, 0
                              , 0, 0, 1 / r, 1);
        }
        /// <summary>
        /// Stereographics the right matrix.
        /// </summary>
        /// <param name="e">The e.</param>
        /// <param name="r">The distance between the object and the projection surface</param>
        /// <returns>Returns the right stereographic view matrix.</returns>
        public static Matrix3D StereographicRightMatrix(double e, double r)
        {
            return new Matrix3D(1, 0, e / (2 * r), 0
                              , 0, 1, 0, 0
                              , 0, 0, 0, 0
                              , 0, 0, 1 / r, 1);
        }
        #endregion Public Methods
    }
}
