using System.Windows.Media.Media3D;

namespace RayTracer.Model.Camera
{
    public class PerspectiveCamera
    {
        #region Public Properties
        /// <summary>
        /// Gets up vector.
        /// </summary>
        /// <value>
        /// Up vector.
        /// </value>
        public Vector3D UpVector { get; set; }
        /// <summary>
        /// Gets the camera target.
        /// </summary>
        /// <value>
        /// The camera target.
        /// </value>
        public Vector3D CameraTarget { get; set; }
        /// <summary>
        /// Gets the camera position.
        /// </summary>
        /// <value>
        /// The camera position.
        /// </value>
        public Vector3D CameraPosition { get; set; }
        /// <summary>
        /// Gets the near plane.
        /// </summary>
        /// <value>
        /// The near plane.
        /// </value>
        public double Near { get; set; }
        /// <summary>
        /// Gets the far plane.
        /// </summary>
        /// <value>
        /// The far plane.
        /// </value>
        public double Far { get; set; }
        /// <summary>
        /// Gets the field of view.
        /// </summary>
        /// <value>
        /// The field of view.
        /// </value>
        public double Fov { get; set; }
        /// <summary>
        /// Gets the aspect ratio of the viewport.
        /// </summary>
        /// <value>
        /// The aspect ratio of the viewport.
        /// </value>
        public double Ratio { get; set; }
        #endregion Public Properties
        #region .ctor
        /// <summary>
        /// Initializes a new instance of the <see cref="PerspectiveCamera"/> class.
        /// </summary>
        /// <param name="upVector">Up vector.</param>
        /// <param name="cameraTarget">The camera target.</param>
        /// <param name="cameraPosition">The camera position.</param>
        /// <param name="near">The near plane.</param>
        /// <param name="far">The far plane.</param>
        /// <param name="fov">The field of view.</param>
        /// <param name="ratio">The aspect ratio of the viewport.</param>
        public PerspectiveCamera(Vector3D upVector, Vector3D cameraTarget, Vector3D cameraPosition, double near, double far, double fov, double ratio)
        {
            UpVector = upVector;
            CameraTarget = cameraTarget;
            CameraPosition = cameraPosition;
            Near = near;
            Far = far;
            Fov = fov;
            Ratio = ratio;
        }
        #endregion .ctor
    }
}
