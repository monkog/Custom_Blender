using System.Windows.Media.Media3D;
using PerspectiveCamera = RayTracer.Model.Camera.PerspectiveCamera;

namespace RayTracer.ViewModel
{
    public class CameraManager : ViewModelBase
    {
        #region Private Members
        /// <summary>
        /// The instance of the CameraManager
        /// </summary>
        private static CameraManager _instance;
        /// <summary>
        /// The up vector
        /// </summary>
        private Vector3D _upVector = new Vector3D(0, 0, 1);
        /// <summary>
        /// The camera target
        /// </summary>
        private Vector3D _cameraTarget = new Vector3D(0, 0.5, 0.5);
        /// <summary>
        /// The camera position
        /// </summary>
        private Vector3D _cameraPosition = new Vector3D(3, 0.5, 0.5);
        /// <summary>
        /// The near plane
        /// </summary>
        private readonly double _near = 1;
        /// <summary>
        /// The far plane
        /// </summary>
        private readonly double _far = 100;
        /// <summary>
        /// The field of view
        /// </summary>
        private readonly double _fov = 45;
        /// <summary>
        /// The aspect ratio of the viewport
        /// </summary>
        private readonly double _ratio = 1;//500 / 700;
        #endregion Private Members
        #region  Public Properties
        /// <summary>
        /// Gets the instance.
        /// </summary>
        public static CameraManager Instance { get { return _instance ?? (_instance = new CameraManager()); } }
        /// <summary>
        /// Gets the camera.
        /// </summary>
        /// <value>
        /// The camera.
        /// </value>
        public PerspectiveCamera Camera { get; private set; }
        /// <summary>
        /// Gets the near.
        /// </summary>
        /// <value>
        /// The near.
        /// </value>
        public double Near { get { return _near; } }
        /// <summary>
        /// Gets the far.
        /// </summary>
        /// <value>
        /// The far.
        /// </value>
        public double Far { get { return _far; } }
        /// <summary>
        /// Gets the fov.
        /// </summary>
        /// <value>
        /// The fov.
        /// </value>
        public double Fov { get { return _fov; } }
        /// <summary>
        /// Gets or sets the camera target.
        /// </summary>
        /// <value>
        /// The camera target.
        /// </value>
        public Vector3D CameraTarget
        {
            get { return _cameraTarget; }
            set
            {
                if (_cameraTarget == value)
                    return;
                _cameraTarget = value;
                OnPropertyChanged("CameraTarget");
            }
        }
        /// <summary>
        /// Gets or sets the camera position.
        /// </summary>
        /// <value>
        /// The camera position.
        /// </value>
        public Vector3D CameraPosition
        {
            get { return _cameraPosition; }
            set
            {
                if (_cameraPosition == value)
                    return;
                _cameraPosition = value;
                OnPropertyChanged("CameraPosition");
            }
        }
        /// <summary>
        /// Gets the ratio.
        /// </summary>
        /// <value>
        /// The ratio.
        /// </value>
        public double Ratio { get { return _ratio; } }
        #endregion  Public Properties
        #region .ctor
        /// <summary>
        /// Initializes a new instance of the <see cref="CameraManager"/> class.
        /// </summary>
        private CameraManager()
        {
            Camera = new PerspectiveCamera(_upVector, _cameraTarget, _cameraPosition, _near, _far, _fov, _ratio);
        }
        #endregion .ctor
        #region Commands
        #endregion Commands
    }
}
