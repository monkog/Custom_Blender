using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media.Media3D;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Win32;
using RayTracer.Model;
using RayTracer.Model.Shapes;
using Point = System.Windows.Point;

namespace RayTracer.ViewModel
{
    public class RayViewModel : ViewModelBase
    {
        #region Private Members
        private double _viewportWidth;
        private double _viewportHeight;
        private int _l;
        private int _v;
        private double _a;
        private double _b;
        private double _c;
        private int _xSlider;
        private int _ySlider;
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
                TransformScene(matrix);
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
                TransformScene(matrix);
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
                TransformScene(matrix);
                Render();
                _zSlider = value;
                OnPropertyChanged("ZSlider");
            }
        }
        /// <summary>
        /// Gets or sets the width of the viewport.
        /// </summary>
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
        public MouseEventManager MouseManager { get { return MouseEventManager.Instance; } }
        /// <summary>
        /// Gets the keyboard manager.
        /// </summary>
        public KeyboardEventManager KeyboardManager { get { return KeyboardEventManager.Instance; } }
        /// <summary>
        /// Gets the camera manager.
        /// </summary>
        public CameraManager CameraManager { get { return CameraManager.Instance; } }
        /// <summary>
        /// Gets the scene manager.
        /// </summary>
        public SceneManager SceneManager { get { return SceneManager.Instance; } }
        /// <summary>
        /// Gets the point manager.
        /// </summary>
        /// <value>
        /// The point manager.
        /// </value>
        public PointManager PointManager { get { return PointManager.Instance; } }
        /// <summary>
        /// Gets the curve manager.
        /// </summary>
        public CurveManager CurveManager { get { return CurveManager.Instance; } }
        /// <summary>
        /// Gets the Patch manager.
        /// </summary>
        public PatchManager PatchManager { get { return PatchManager.Instance; } }
        /// <summary>
        /// Gets the mesh manager.
        /// </summary>
        public MeshManager MeshManager { get { return MeshManager.Instance; } }
        /// <summary>
        /// Gets the cursor.
        /// </summary>
        public Cursor3D Cursor { get { return Cursor3D.Instance; } }
        #endregion Public Properties
        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="RayViewModel"/> class.
        /// </summary>
        public RayViewModel()
        {
            MouseManager.PropertyChanged += MouseManager_PropertyChanged;
            SceneManager.PropertyChanged += SceneManager_PropertyChanged;
            Cursor.PropertyChanged += Cursor_PropertyChanged;
            PointManager.Points.CollectionChanged += (sender, args) => { Render(); };
            CurveManager.Curves.CollectionChanged += (sender, args) => { Render(); };
            PatchManager.Patches.CollectionChanged += (sender, args) => { Render(); };
            PatchManager.PropertyChanged += (sender, args) => { if (args.PropertyName == "HorizontalPatchDivisions" || args.PropertyName == "VerticalPatchDivisions") Render(); };
            MeshManager.Meshes.CollectionChanged += (sender, args) => { Render(); };
            L = 20;
            V = 20;
            A = 5;
            B = 6;
            C = 8;
            SceneManager.M = 4;
            PatchManager.HorizontalPatches = 2;
            PatchManager.VerticalPatches = 1;
            PatchManager.PatchHeight = 3;
            PatchManager.PatchWidth = 3;
            PatchManager.HorizontalPatchDivisions = 15;
            PatchManager.VerticalPatchDivisions = 15;
            Render();
        }
        #endregion Constructor
        #region Private Methods
        private void Render()
        {
            using (Graphics g = Graphics.FromImage(SceneManager.Instance.SceneImage))
            {
                g.Clear(Color.Black);
            }

            foreach (var curve in CurveManager.Curves)
                curve.Draw();

            foreach (var point in PointManager.Points)
                point.Draw();

            foreach (var patch in PatchManager.Patches)
                patch.Draw();

            foreach (var mesh in MeshManager.Meshes)
                mesh.Draw();

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
                        Matrix3D matrix;

                        if (Keyboard.IsKeyDown(Key.LeftCtrl))
                            matrix = Transformations.TranslationMatrix(new Vector3D(0, 0, delta.X));
                        else
                            matrix = Transformations.TranslationMatrix(new Vector3D(delta.X, delta.Y, 0));

                        TransformScene(matrix);
                    }
                    break;
                case "MouseScale":
                    {
                        double delta = MouseManager.MouseScale;
                        Matrix3D matrix = Transformations.ScaleMatrix(delta);
                        TransformScene(matrix);
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
        private void curve_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "EquidistantPoints":
                    ((BezierCurveC2)sender).UpdateVertices();
                    Render();
                    break;
                case "IsBernsteinBasis":
                case "DisplayEdges":
                    Render();
                    break;
            }
        }
        void Curve_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "IsPolygonVisible":
                    Render();
                    break;
            }
        }
        private void TransformScene(Matrix3D matrix)
        {
            foreach (var mesh in MeshManager.Meshes)
                mesh.ModelTransform = matrix * mesh.ModelTransform;
            foreach (var point in PointManager.Points)
                point.ModelTransform = matrix * point.ModelTransform;
            foreach (var patch in PatchManager.Patches)
                patch.ModelTransform = matrix * patch.ModelTransform;
        }
        #endregion Private Methods
        #region Commands
        private ICommand _saveSceneCommand;
        public ICommand SaveSceneCommand { get { return _saveSceneCommand ?? (_saveSceneCommand = new DelegateCommand(SaveSceneExecuted)); } }
        /// <summary>
        /// Saves the scene.
        /// </summary>
        private void SaveSceneExecuted()
        {
            var dialog = new SaveFileDialog
            {
                Filter = @"Model files (*.mg1)|*.mg1"
            };
            dialog.ShowDialog();
            if (dialog.CheckPathExists)
                SceneManager.SaveScene(dialog.FileName);
        }

        private ICommand _loadSceneCommand;
        public ICommand LoadSceneCommand { get { return _loadSceneCommand ?? (_loadSceneCommand = new DelegateCommand(LoadSceneExecuted)); } }
        /// <summary>
        /// Loads the scene.
        /// </summary>
        private void LoadSceneExecuted()
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.ShowDialog();
            var x = dialog.CheckPathExists;
        }

        private ICommand _addTorusCommand;
        public ICommand AddTorusCommand { get { return _addTorusCommand ?? (_addTorusCommand = new DelegateCommand(AddTorusExecuted)); } }
        /// <summary>
        /// Adds the torus.
        /// </summary>
        private void AddTorusExecuted()
        {
            var torus = new Torus(0, 0, 0, "Torus(L:" + L + ", V:" + V + ")", L, V);
            MeshManager.Meshes.Add(torus);
        }

        private ICommand _addEllipsoideCommand;
        public ICommand AddEllipsoideCommand { get { return _addEllipsoideCommand ?? (_addEllipsoideCommand = new DelegateCommand(AddEllipsoideExecuted)); } }
        /// <summary>
        /// Adds the Ellipse.
        /// </summary>
        private void AddEllipsoideExecuted()
        {
            var ellipsoide = new Ellipsoide(0, 0, 0, "Ellipsoide(a:" + A + ", b:" + B + ", c:" + C + ")", 1 / A, 1 / B, 1 / C);
            MeshManager.Meshes.Clear();
            MeshManager.Meshes.Add(ellipsoide);
            Render();
        }

        private ICommand _addBezierPatchC0Command;
        public ICommand AddBezierPatchC0Command { get { return _addBezierPatchC0Command ?? (_addBezierPatchC0Command = new DelegateCommand(AddBezierPatchC0Executed)); } }
        /// <summary>
        /// Adds the Bezier Patch with C0 continuity.
        /// </summary>
        private void AddBezierPatchC0Executed()
        {
            var patch = new BezierPatchC0(0, 0, 0, "Bezier Patch C0(" + 0 + ", " + 0 + ", " + 0 + ")");
            patch.PropertyChanged += (sender, e) => { if (e.PropertyName == "DisplayEdges")Render(); };
            PatchManager.Patches.Add(patch);
        }

        private ICommand _createBezierCurveC0;
        public ICommand CreateBezierCurveC0 { get { return _createBezierCurveC0 ?? (_createBezierCurveC0 = new DelegateCommand(CreateBezierCurveC0Executed)); } }
        /// <summary>
        /// Creates the bezier curve c0.
        /// </summary>
        private void CreateBezierCurveC0Executed()
        {
            if (!PointManager.SelectedItems.Any()) return;
            var curve = new BezierCurveC0(0, 0, 0, "Bezier curve C0(" + 0 + ", " + 0 + ", " + 0 + ")", PointManager.SelectedItems);
            curve.PropertyChanged += Curve_PropertyChanged;
            curve.Vertices.CollectionChanged += (sender, e) => { Render(); };
            curve.PropertyChanged += (sender, args) => { if (args.PropertyName == "DisplayEdges") Render(); };
            CurveManager.Curves.Add(curve);
        }

        private ICommand _createBezierCurveC2;
        public ICommand CreateBezierCurveC2 { get { return _createBezierCurveC2 ?? (_createBezierCurveC2 = new DelegateCommand(CreateBezierCurveC2Executed)); } }
        /// <summary>
        /// Creates the bezier curve c2.
        /// </summary>
        private void CreateBezierCurveC2Executed()
        {
            if (!PointManager.SelectedItems.Any()) return;
            var curve = new BezierCurveC2(0, 0, 0, "Bezier curve C2(" + 0 + ", " + 0 + ", " + 0 + ")", PointManager.SelectedItems, isInterpolation: false);
            curve.PropertyChanged += Curve_PropertyChanged;
            curve.PropertyChanged += (sender, e) => { if (e.PropertyName == "IsBernsteinBasis" || e.PropertyName == "DisplayEdges") Render(); };
            curve.Vertices.CollectionChanged += (sender, e) => { Render(); };
            curve.DeBooreVertices.CollectionChanged += (sender, e) => { Render(); };
            CurveManager.Curves.Add(curve);
        }

        private ICommand _createBezierCurveC2WithPoints;
        public ICommand CreateBezierCurveC2WithPoints { get { return _createBezierCurveC2WithPoints ?? (_createBezierCurveC2WithPoints = new DelegateCommand(CreateBezierCurveC2WithPointsExecuted)); } }
        /// <summary>
        /// Creates the bezier curve c2 that contains certain points.
        /// </summary>
        private void CreateBezierCurveC2WithPointsExecuted()
        {
            if (!PointManager.SelectedItems.Any()) return;
            var curve = new BezierCurveC2(0, 0, 0, "Bezier curve C2(" + 0 + ", " + 0 + ", " + 0 + ")", PointManager.SelectedItems, isInterpolation: true);
            curve.PropertyChanged += Curve_PropertyChanged;
            curve.PropertyChanged += curve_PropertyChanged;
            curve.Vertices.CollectionChanged += (sender, e) => { Render(); };
            curve.DeBooreVertices.CollectionChanged += (sender, e) => { Render(); };
            CurveManager.Curves.Add(curve);
        }

        private ICommand _addPointToBezierCurve;
        public ICommand AddPointToBezierCurve { get { return _addPointToBezierCurve ?? (_addPointToBezierCurve = new DelegateCommand(AddPointToBezierCurveExecuted)); } }
        /// <summary>
        /// Adds the point to bezier curve.
        /// </summary>
        private void AddPointToBezierCurveExecuted()
        {
            foreach (var point in PointManager.SelectedItems)
                foreach (var curve in CurveManager.SelectedItems)
                    if (curve.Continuity == Continuity.C0)
                    {
                        if (!curve.Vertices.Contains(point))
                            curve.Vertices.Add(point);
                    }
                    else if (!((BezierCurveC2)curve).DeBooreVertices.Contains(point))
                        ((BezierCurveC2)curve).DeBooreVertices.Add(point);
        }

        private ICommand _deselectAllCommand;
        public ICommand DeselectAllCommand { get { return _deselectAllCommand ?? (_deselectAllCommand = new DelegateCommand(DeselectAllExecuted)); } }
        /// <summary>
        /// Deselects all selections.
        /// </summary>
        private void DeselectAllExecuted()
        {
            foreach (var model in SceneManager.Models)
                model.IsSelected = false;

            foreach (var point in PointManager.Points)
                point.IsSelected = false;
        }

        private ICommand _selectAllCommand;
        public ICommand SelectAllCommand { get { return _selectAllCommand ?? (_selectAllCommand = new DelegateCommand(SelectAllExecuted)); } }
        /// <summary>
        /// Selects all items.
        /// </summary>
        private void SelectAllExecuted()
        {
            foreach (var point in PointManager.Points)
                point.IsSelected = true;

            foreach (var model in SceneManager.Models)
                model.IsSelected = false;
        }
        #endregion Commands
    }
}
