using System;
using System.Windows.Media.Media3D;
using RayTracer.Helpers;
using PerspectiveCamera = RayTracer.Model.Camera.PerspectiveCamera;

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
        #region Private Methods
        /// <summary>
        /// Calculates the versor of the given vector.
        /// </summary>
        /// <param name="vector">Given vector.</param>
        /// <returns>Returns the calculated versor.</returns>
        private static Vector3D Versor(Vector3D vector)
        {
            double sqrt = Math.Sqrt(vector.X * vector.X + vector.Y * vector.Y + vector.Z * vector.Z);
            return new Vector3D(vector.X / sqrt, vector.Y / sqrt, vector.Z / sqrt);
        }
        #endregion Private Methods
        #region Public Methods
        /// <summary>
        /// Calculates the projection matrix.
        /// </summary>
        /// <param name="fov">The field of view.</param>
        /// <param name="near">The near plane.</param>
        /// <param name="far">The far plane.</param>
        /// <param name="ratio">The aspect ratio of the viewport.</param>
        /// <returns>Returns the projection matrix.</returns>
        public static Matrix3D ProjectionMatrix(double fov, double near, double far, double ratio)
        {
            double f = 1.0f / Math.Tan((fov / 2.0f) * (Math.PI / 180.0f));

            return new Matrix3D(f, 0.0, 0.0, 0.0
                              , 0.0, f / ratio, 0.0, 0.0
                              , 0.0, 0.0, -(far + near) / (far - near), 2 * (-(far + near) / (far - near))
                              , 0.0, 0.0, -1.0, 0.0);
        }
        /// <summary>
        /// Calculates the view matrix.
        /// </summary>
        /// <returns>Returns the view matrix.</returns>
        public static Matrix3D ViewMatrix(PerspectiveCamera camera)
        {
            Vector3D zAxis = camera.CameraPosition - camera.CameraTarget;
            Vector3D zAxisVersor = Versor(zAxis);
            Vector3D xAxis = Vector3D.CrossProduct(camera.UpVector, zAxisVersor);
            Vector3D xAxisVersor = Versor(xAxis);
            Vector3D yAxis = Vector3D.CrossProduct(zAxis, xAxisVersor);
            Vector3D yAxisVersor = Versor(yAxis);
            Matrix3D matrix = new Matrix3D(xAxisVersor.X, yAxisVersor.X, zAxisVersor.X, camera.CameraPosition.X
                                          , xAxisVersor.Y, yAxisVersor.Y, zAxisVersor.Y, camera.CameraPosition.Y
                                          , xAxisVersor.Z, yAxisVersor.Z, zAxisVersor.Z, camera.CameraPosition.Z
                                          , 0, 0, 0, 1);
            matrix.Invert();
            return matrix;
        }
        /// <summary>
        /// Transforms the point.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <param name="matrix">The matrix.</param>
        /// <returns>Vector multiplied by the matrix</returns>
        public static Vector4 TransformPoint(Vector4 point, Matrix3D matrix)
        {
            return point * matrix;
        }
        /// <summary>
        /// Creates rotation matrix for rotation on x axis.
        /// </summary>
        /// <param name="alpha">The alpha.</param>
        /// <returns>Rotation matrix for rotation on x axis.</returns>
        public static Matrix3D RotationMatrixX(float alpha)
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
        public static Matrix3D RotationMatrixY(float alpha)
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
        public static Matrix3D RotationMatrixZ(float alpha)
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
        public static Matrix3D ScaleMatrix(float scale)
        {
            return new Matrix3D(scale, 0, 0, 0
                              , 0, scale, 0, 0
                              , 0, 0, scale, 0
                              , 0, 0, 0, 1);
        }
        #endregion Public Methods
    }
}
