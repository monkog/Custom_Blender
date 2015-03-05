using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Media3D;
using Microsoft.Practices.Prism.Commands;
using RayTracer.Model.Shapes;

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
        /// The viewport width
        /// </summary>
        private double _viewportWidth;
        /// <summary>
        /// The viewport height
        /// </summary>
        private double _viewportHeight;
        private int _l;
        private int _v;
        #endregion Private Members
        #region Public Properties

        public int L
        {
            get { return _l; }
            set
            {
                if (_l == value) return;
                _l = value;
                OnPropertyChanged("L");
            }
        }

        public int V
        {
            get { return _v; }
            set
            {
                if (_v == value) return;
                _v = value;
                OnPropertyChanged("V");
            }
        }
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
        /// <summary>
        /// Gets the mouse manager.
        /// </summary>
        /// <value>
        /// The mouse manager.
        /// </value>
        public MouseEventManager MouseManager { get { return MouseEventManager.Instance; } }
        /// <summary>
        /// Gets the camera manager.
        /// </summary>
        /// <value>
        /// The camera manager.
        /// </value>
        public CameraManager CameraManager { get { return CameraManager.Instance; } }
        #endregion Public Properties
        #region .ctor
        /// <summary>
        /// Initializes a new instance of the <see cref="RayViewModel"/> class.
        /// </summary>
        public RayViewModel()
        {
            Meshes = new ObservableCollection<ShapeBase>();
            MouseManager.PropertyChanged += MouseManager_PropertyChanged;
        }
        #endregion .ctor
        #region Private Methods
        private void Render()
        {
            Matrix3D viewMatrix = Transformations.ViewMatrix(CameraManager.Instance.Camera);
            //Matrix3D viewMatrix = Transformations.ViewMatrix(200);
            Matrix3D projectionMatrix = Transformations.ProjectionMatrix(CameraManager.Instance.Fov, CameraManager.Instance.Near
                , CameraManager.Instance.Far, CameraManager.Instance.Ratio);

            foreach (ShapeBase mesh in Meshes)
                mesh.Transform = projectionMatrix * viewMatrix;
        }
        /// <summary>
        /// Handles the PropertyChanged event of the CameraManager control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.ComponentModel.PropertyChangedEventArgs"/> instance containing the event data.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        private void MouseManager_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "MouseDelta":
                    {
                        Point delta = MouseManager.MouseDelta;
                        Matrix3D matrix = Transformations.TranslationMatrix(new Vector3D(0, delta.X, delta.Y));
                        foreach (var mesh in Meshes)
                            mesh.ModelTransform = matrix * mesh.ModelTransform;
                    }
                    break;
                case "MouseScale":
                    {
                        double delta = MouseManager.MouseScale;
                        Matrix3D matrix = Transformations.ScaleMatrix(delta);
                        foreach (var mesh in Meshes)
                            mesh.ModelTransform = matrix * mesh.ModelTransform;
                    }
                    break;
                default:
                    return;
            }
            Render();
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
            //var cube = new Cube(0, 0, 0, 1);
            //Meshes.Add(cube);
            var torus = new Torus(0, 0, 0, 30, 25);
            Meshes.Clear();
            Meshes.Add(torus);
            Render();
        }
        #endregion Commands
    }
}
