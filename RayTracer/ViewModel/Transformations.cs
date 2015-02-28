using System;
using System.Windows.Media.Media3D;
using RayTracer.Helpers;
using PerspectiveCamera = RayTracer.Model.Camera.PerspectiveCamera;

namespace RayTracer.ViewModel
{
    public static class Transformations
    {
        /// <summary>
        /// Calculates the versor of the given vector.
        /// </summary>
        /// <param name="vector">Given vector.</param>
        /// <returns>Returns the calculated versor.</returns>
        private static Vector3D CreateVersor(Vector3D vector)
        {
            double sqrt = Math.Sqrt(vector.X * vector.X + vector.Y * vector.Y + vector.Z * vector.Z);
            return new Vector3D(vector.X / sqrt, vector.Y / sqrt, vector.Z / sqrt);
        }
        /// <summary>
        /// Calculates the projection matrix.
        /// </summary>
        /// <param name="fov">The field of view.</param>
        /// <param name="near">The near plane.</param>
        /// <param name="far">The far plane.</param>
        /// <param name="ratio">The aspect ratio of the viewport.</param>
        /// <returns>Returns the projection matrix.</returns>
        public static Matrix3D CalculateProjectionMatrix(double fov, double near, double far, double ratio)
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
        public static Matrix3D CreateViewMatrix(PerspectiveCamera camera)
        {
            Vector3D zAxis = camera.CameraPosition - camera.CameraTarget;
            Vector3D zAxisVersor = CreateVersor(zAxis);
            Vector3D xAxis = Vector3D.CrossProduct(camera.UpVector, zAxisVersor);
            Vector3D xAxisVersor = CreateVersor(xAxis);
            Vector3D yAxis = Vector3D.CrossProduct(zAxis, xAxisVersor);
            Vector3D yAxisVersor = CreateVersor(yAxis);
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
    }
}
