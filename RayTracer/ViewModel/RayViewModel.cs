using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows.Media.Media3D;
using Microsoft.Practices.Prism.Commands;
using RayTracer.Helpers;
using RayTracer.Model.Shapes;
using PerspectiveCamera = RayTracer.Model.Camera.PerspectiveCamera;

namespace RayTracer.ViewModel
{
    public class RayViewModel : ViewModelBase
    {
        #region Private Members
        /// <summary>
        /// The collection of viewed meshes
        /// </summary>
        private ObservableCollection<ShapeBase> _meshes;
        /// <summary>
        /// The up vector
        /// </summary>
        private readonly Vector3D _upVector = new Vector3D(0, 0, 1);
        /// <summary>
        /// The camera target
        /// </summary>
        private readonly Vector3D _cameraTarget = new Vector3D(0, 0.5, 0.5);
        /// <summary>
        /// The camera position
        /// </summary>
        private readonly Vector3D _cameraPosition = new Vector3D(3, 0.5, 0.5);
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
        /// <summary>
        /// The viewport width
        /// </summary>
        private double _viewportWidth;
        /// <summary>
        /// The viewport height
        /// </summary>
        private double _viewportHeight;
        #endregion Private Members
        #region Public Properties
        /// <summary>
        /// Gets or sets the collection of viewed meshes.
        /// </summary>
        /// <value>
        /// The collection of viewed meshes.
        /// </value>
        public ObservableCollection<ShapeBase> Meshes
        {
            get { return _meshes; }
            set
            {
                if (_meshes == value)
                    return;
                _meshes = value;
                OnPropertyChanged("Meshes");
            }
        }
        /// <summary>
        /// Gets the camera.
        /// </summary>
        /// <value>
        /// The camera.
        /// </value>
        public PerspectiveCamera Camera { get; private set; }
        /// <summary>
        /// Gets or sets the width of the viewport.
        /// </summary>
        /// <value>
        /// The width of the viewport.
        /// </value>
        public double ViewportWidth
        {
            get { return _viewportWidth; }
            set
            {
                if (_viewportWidth == value)
                    return;
                _viewportWidth = value;
                OnPropertyChanged("ViewportWidth");
            }
        }
        /// <summary>
        /// Gets or sets the height of the viewport.
        /// </summary>
        /// <value>
        /// The height of the viewport.
        /// </value>
        public double ViewportHeight
        {
            get { return _viewportHeight; }
            set
            {
                if (_viewportHeight == value)
                    return;
                _viewportHeight = value;
                OnPropertyChanged("ViewportHeight");
            }
        }
        #endregion Public Properties
        #region .ctor
        /// <summary>
        /// Initializes a new instance of the <see cref="RayViewModel"/> class.
        /// </summary>
        public RayViewModel()
        {
            Meshes = new ObservableCollection<ShapeBase>();
            Camera = new PerspectiveCamera(_upVector, _cameraTarget, _cameraPosition, _near, _far, _fov, _ratio);
        }
        #endregion .ctor
        #region Private Methods
        private void Render()
        {
            Matrix3D viewMatrix = Transformations.ViewMatrix(Camera);
            Matrix3D projectionMatrix =  Transformations.ProjectionMatrix(_fov, _near, _far, _ratio);

            foreach (ShapeBase mesh in Meshes)
                mesh.Transform = projectionMatrix * viewMatrix ;
        }
        #endregion Private Methods
        #region Commands
        private ICommand _addTorusCommand;
        /// <summary>
        /// Gets the add torus command.
        /// </summary>
        /// <value>
        /// The add torus command.
        /// </value>
        public ICommand AddTorusCommand { get { return _addTorusCommand ?? (_addTorusCommand = new DelegateCommand(AddTorusExecuted)); } }
        /// <summary>
        /// Adds the torus.
        /// </summary>
        private void AddTorusExecuted()
        {
            var cube = new Cube(0, 0, 0, 1);
            Meshes.Add(cube);
            Render();
        }
        #endregion Commands
    }
}
