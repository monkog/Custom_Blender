using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Input;
using System.Windows.Media.Media3D;
using Microsoft.Practices.Prism.Commands;
using RayTracer.Model;
using RayTracer.Model.Shapes;
using Point = System.Windows.Point;

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
        private double _a;
        private double _b;
        private double _c;
        /// <summary>
        /// The x slider value
        /// </summary>
        private int _xSlider;
        /// <summary>
        /// The y slider value
        /// </summary>
        private int _ySlider;
        /// <summary>
        /// The z slider value
        /// </summary>
        private int _zSlider;
        #endregion Private Members
        #region Public Properties
        /// <summary>
        /// Gets or sets the number of torus donut divisions.
        /// </summary>
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
        /// <summary>
        /// Gets or sets the torus circle divisions.
        /// </summary>
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
        /// Gets or sets the first radius of Ellipsoide.
        /// </summary>
        public double A
        {
            get { return _a; }
            set
            {
                if (_a == value) return;
                _a = value;
                OnPropertyChanged("A");
            }
        }
        /// <summary>
        /// Gets or sets the second radius of Ellipsoide.
        /// </summary>
        public double B
        {
            get { return _b; }
            set
            {
                if (_b == value) return;
                _b = value;
                OnPropertyChanged("B");
            }
        }
        /// <summary>
        /// Gets or sets the third radius of Ellipsoide.
        /// </summary>
        public double C
        {
            get { return _c; }
            set
            {
                if (_c == value) return;
                _c = value;
                OnPropertyChanged("C");
            }
        }
        /// <summary>
        /// Gets or sets the x slider value.
        /// </summary>
        /// <value>
        /// The x slider value.
        /// </value>
        public int XSlider
        {
            get { return _xSlider; }
            set
            {
                if (_xSlider == value) return;
                Matrix3D matrix = Transformations.RotationMatrixX((Math.PI * 2.0f) * (value - _xSlider) / 360);
                foreach (var mesh in Meshes)
                    mesh.ModelTransform = matrix * mesh.ModelTransform;
                Render();
                _xSlider = value;
                OnPropertyChanged("XSlider");
            }
        }
        /// <summary>
        /// Gets or sets the y slider value.
        /// </summary>
        /// <value>
        /// The y slider value.
        /// </value>
        public int YSlider
        {
            get { return _ySlider; }
            set
            {
                if (_ySlider == value) return;
                Matrix3D matrix = Transformations.RotationMatrixY((Math.PI * 2.0f) * (value - _ySlider) / 360);
                foreach (var mesh in Meshes)
                    mesh.ModelTransform = matrix * mesh.ModelTransform;
                Render();
                _ySlider = value;
                OnPropertyChanged("YSlider");
            }
        }
        /// <summary>
        /// Gets or sets the z slider value.
        /// </summary>
        /// <value>
        /// The z slider value.
        /// </value>
        public int ZSlider
        {
            get { return _zSlider; }
            set
            {
                if (_zSlider == value) return;
                Matrix3D matrix = Transformations.RotationMatrixZ((Math.PI * 2.0f) * (value - _zSlider) / 360);
                foreach (var mesh in Meshes)
                    mesh.ModelTransform = matrix * mesh.ModelTransform;
                Render();
                _zSlider = value;
                OnPropertyChanged("ZSlider");
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
        /// Gets the keyboard manager.
        /// </summary>
        /// <value>
        /// The keyboard manager.
        /// </value>
        public KeyboardEventManager KeyboardManager { get { return KeyboardEventManager.Instance; } }
        /// <summary>
        /// Gets the camera manager.
        /// </summary>
        /// <value>
        /// The camera manager.
        /// </value>
        public CameraManager CameraManager { get { return CameraManager.Instance; } }
        /// <summary>
        /// Gets the scene manager.
        /// </summary>
        /// <value>
        /// The scene manager.
        /// </value>
        public SceneManager SceneManager { get { return SceneManager.Instance; } }
        /// <summary>
        /// Gets the point manager.
        /// </summary>
        /// <value>
        /// The point manager.
        /// </value>
        public PointManager PointManager { get { return PointManager.Instance; } }
        /// <summary>
        /// Gets the cursor.
        /// </summary>
        /// <value>
        /// The cursor.
        /// </value>
        public Cursor3D Cursor { get { return Cursor3D.Instance; } }
        #endregion Public Properties
        #region .ctor
        /// <summary>
        /// Initializes a new instance of the <see cref="RayViewModel"/> class.
        /// </summary>
        public RayViewModel()
        {
            Meshes = new ObservableCollection<ShapeBase>();
            MouseManager.PropertyChanged += MouseManager_PropertyChanged;
            SceneManager.PropertyChanged += SceneManager_PropertyChanged;
            Cursor.PropertyChanged += Cursor_PropertyChanged;
            PointManager.Points.CollectionChanged += (sender, args) => { Render(); };
            L = 20;
            V = 20;
            A = 5;
            B = 6;
            C = 8;
            SceneManager.M = 4;
            Render();
        }
        #endregion .ctor
        #region Private Methods
        private void Render()
        {
            using (Graphics g = Graphics.FromImage(SceneManager.Instance.SceneImage))
            {
                g.Clear(Color.Black);
            }

            foreach (ShapeBase mesh in Meshes)
                mesh.Draw();

            foreach (var point in PointManager.Instance.Points)
                point.Draw();

            Cursor3D.Instance.Draw();
        }
        /// <summary>
        /// Handles the PropertyChanged event of the CameraManager control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.ComponentModel.PropertyChangedEventArgs"/> instance containing the event data.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        private void MouseManager_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "MouseDelta":
                    {
                        Point delta = MouseManager.MouseDelta;

                        if (Keyboard.IsKeyDown(Key.LeftCtrl))
                        {
                            Matrix3D matrix = Transformations.TranslationMatrix(new Vector3D(0, 0, Math.Max(delta.X, delta.Y)));
                            foreach (var mesh in Meshes)
                                mesh.ModelTransform = matrix * mesh.ModelTransform;
                        }
                        else
                        {
                            Matrix3D matrix = Transformations.TranslationMatrix(new Vector3D(delta.X, delta.Y, 0));
                            foreach (var mesh in Meshes)
                                mesh.ModelTransform = matrix * mesh.ModelTransform;
                        }
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
        /// <summary>
        /// Handles the PropertyChanged event of the SceneManager control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="PropertyChangedEventArgs"/> instance containing the event data.</param>
        private void SceneManager_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "IsStereoscopic":
                    Render();
                    break;
            }
        }
        /// <summary>
        /// Handles the PropertyChanged event of the Cursor control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="PropertyChangedEventArgs"/> instance containing the event data.</param>
        private void Cursor_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "ModelTransform":
                    Render();
                    break;
            }
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
            var torus = new Torus(0, 0, 0, L, V);
            Meshes.Add(torus);
            Render();
        }

        private ICommand _addEllipsoideCommand;
        /// <summary>
        /// Gets the add Ellipse command.
        /// </summary>
        /// <value>
        /// The add Ellipse command.
        /// </value>
        public ICommand AddEllipsoideCommand { get { return _addEllipsoideCommand ?? (_addEllipsoideCommand = new DelegateCommand(AddEllipsoideExecuted)); } }
        /// <summary>
        /// Adds the Ellipse.
        /// </summary>
        private void AddEllipsoideExecuted()
        {
            var ellipsoide = new Ellipsoide(0, 0, 0, 1 / A, 1 / B, 1 / C);
            Meshes.Clear();
            Meshes.Add(ellipsoide);
            Render();
        }
        #endregion Commands
    }
}
