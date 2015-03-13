using System.Drawing;
using System.Windows.Media.Media3D;
using RayTracer.ViewModel;

namespace RayTracer.Model.Shapes
{
    public class SceneManager : ViewModelBase
    {
        #region Private Members
        /// <summary>
        /// Is the view stereoscopic
        /// </summary>
        private bool _isStereoscopic;
        /// <summary>
        /// The scene image
        /// </summary>
        private Bitmap _sceneImage;
        /// <summary>
        /// The light intensity
        /// </summary>
        private double _m;
        /// <summary>
        /// Instance of the SceneManager
        /// </summary>
        private static SceneManager _instance;
        /// <summary>
        /// Scale matrix
        /// </summary>
        private readonly Matrix3D _scaleMatrix = new Matrix3D(50, 0, 0, 0
                                                            , 0, 50, 0, 0
                                                            , 0, 0, 50, 0
                                                            , 0, 0, 0, 1);
        /// <summary>
        /// The transform matrix
        /// </summary>
        private readonly Matrix3D _transformMatrix = new Matrix3D(1, 0, 0, 400
                                                                , 0, 1, 0, 300
                                                                , 0, 0, 1, 0
                                                                , 0, 0, 0, 1);
#warning change matrices to be dynamic
        #endregion Private Members
        #region Public Properties
        /// <summary>
        /// Instance of the SceneManager
        /// </summary>
        public static SceneManager Instance
        {
            get { return _instance ?? (_instance = new SceneManager()); }
        }
        /// <summary>
        /// Scale matrix
        /// </summary>
        public Matrix3D ScaleMatrix
        {
            get { return _scaleMatrix; }
        }
        /// <summary>
        /// Gets the transform matrix.
        /// </summary>
        /// <value>
        /// The transform matrix.
        /// </value>
        public Matrix3D TransformMatrix
        {
            get { return _transformMatrix; }
        }
        /// <summary>
        /// Gets or sets the scene image.
        /// </summary>
        /// <value>
        /// The scene image.
        /// </value>
        public Bitmap SceneImage
        {
            get { return _sceneImage; }
            set
            {
                _sceneImage = value;
                OnPropertyChanged("SceneImage");
            }
        }
        /// <summary>
        /// Gets or sets the intensity of the light.
        /// </summary>
        public double M
        {
            get { return _m; }
            set
            {
                if (_m == value) return;
                _m = value;
                OnPropertyChanged("M");
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether the view is stereoscopic.
        /// </summary>
        /// <value>
        /// <c>true</c> if the view is stereoscopic; otherwise, <c>false</c>.
        /// </value>
        public bool IsStereoscopic
        {
            get { return _isStereoscopic; }
            set
            {
                if (_isStereoscopic == value)
                    return;
                _isStereoscopic = value;
                OnPropertyChanged("IsStereoscopic");
            }
        }
        #endregion Public Properties
        #region .ctor
        private SceneManager()
        {
            SceneImage = new Bitmap(800, 600);
        }
        #endregion .ctor
    }
}
