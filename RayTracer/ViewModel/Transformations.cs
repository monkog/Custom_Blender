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
                              , 0.0, 0.0, -(far + near) / (far - near), 2 * (-(far * near) / (far - near))
                              , 0.0, 0.0, -1.0, 0.0);
        }

        public static Matrix3D MatrixPerspective(double fieldOfView, double aspectRatio, double nearPlaneDistance,
            double farPlaneDistance)
        {
            double e = 1.0f / Math.Tan((fieldOfView / 2.0f));
            double fn = -(farPlaneDistance + nearPlaneDistance) / (farPlaneDistance - nearPlaneDistance);
            double fn2 = -(2 * farPlaneDistance * nearPlaneDistance) / (farPlaneDistance - nearPlaneDistance);
            return new Matrix3D(e, 0, 0, 0,
                0, e / aspectRatio, 0, 0,
                0, 0, fn, fn2,
                0, 0, -1, 0);
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
            Vector3D yAxis = Vector3D.CrossProduct(zAxisVersor, xAxisVersor);
            Vector3D yAxisVersor = Versor(yAxis);
            Matrix3D matrix = new Matrix3D(xAxisVersor.X, yAxisVersor.X, zAxisVersor.X, camera.CameraPosition.X
                                          , xAxisVersor.Y, yAxisVersor.Y, zAxisVersor.Y, camera.CameraPosition.Y
                                          , xAxisVersor.Z, yAxisVersor.Z, zAxisVersor.Z, camera.CameraPosition.Z
                                          , 0, 0, 0, 1);
            matrix.Invert();
            return matrix;
        }

        public static Matrix3D CreateLookAt(PerspectiveCamera camera)
        {
            Vector3D zaxis = camera.CameraTarget - camera.CameraPosition;
            zaxis.Normalize();
            Vector3D xaxisCross = Vector3D.CrossProduct(camera.UpVector, zaxis);
            Vector3D xaxis = xaxisCross;
            xaxis.Normalize();
            Vector3D yaxis = Vector3D.CrossProduct(zaxis, xaxis);

            return new Matrix3D(xaxis.X, yaxis.X, zaxis.X, 0,
                xaxis.Y, yaxis.Y, zaxis.Y, 0,
                xaxis.Z, yaxis.Z, zaxis.Z, 0,
                -Vector3D.DotProduct(xaxis, camera.CameraPosition), -Vector3D.DotProduct(yaxis, camera.CameraPosition), -Vector3D.DotProduct(zaxis, camera.CameraPosition), 1);
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
