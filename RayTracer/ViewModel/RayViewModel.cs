using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Media3D;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Win32;
using RayTracer.Helpers;
using RayTracer.Helpers.EventCommand;
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
        private double _a;
        private double _b;
        private double _c;
        private int _xSlider;
        private int _ySlider;
        private int _zSlider;
        #endregion Private Members
        #region Public Properties
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
            MeshManager.SmallR = 0.1;
            MeshManager.BigR = 0.2;
            MeshManager.L = 20;
            MeshManager.V = 20;
            A = 5;
            B = 6;
            C = 8;
            SceneManager.M = 4;
            PatchManager.HorizontalPatches = 1;
            PatchManager.VerticalPatches = 1;
            PatchManager.PatchHeight = 3;
            PatchManager.PatchWidth = 3;
            PatchManager.HorizontalPatchDivisions = 4;
            PatchManager.VerticalPatchDivisions = 4;
            PatchManager.PatchContinuity = Continuity.C0;

            MouseManager.PropertyChanged += MouseManager_PropertyChanged;
            SceneManager.PropertyChanged += SceneManager_PropertyChanged;
            Cursor.PropertyChanged += Cursor_PropertyChanged;
            PatchManager.PropertyChanged += (sender, args) => { if (args.PropertyName == "HorizontalPatchDivisions" || args.PropertyName == "VerticalPatchDivisions") Render(); };

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

            foreach (var patch in PatchManager.GregoryPatches)
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
                        Matrix3D matrix;
                        if (Keyboard.IsKeyDown(Key.X) || Keyboard.IsKeyDown(Key.Y) || Keyboard.IsKeyDown(Key.Z))
                        {
                            Vector3D dimensions = new Vector3D(Keyboard.IsKeyDown(Key.X) ? 1 : 0,
                                Keyboard.IsKeyDown(Key.Y) ? 1 : 0, Keyboard.IsKeyDown(Key.Z) ? 1 : 0);
                            matrix = Transformations.ScaleMatrix(delta, dimensions);
                        }
                        else
                            matrix = Transformations.ScaleMatrix(delta);
                        TransformScene(matrix);
                    }
                    break;
                default:
                    return;
            }
            Render();
        }
        private void LoadFile(string fileName)
        {
            SceneManager.LoadScene(fileName);
            foreach (var curve in CurveManager.Curves)
            {
                if (curve.Continuity == Continuity.C0)
                    curve.PropertyChanged +=
                        (sender, args) =>
                        {
                            if (args.PropertyName == "DisplayEdges" || args.PropertyName == "IsPolygonVisible") Render();
                        };
                else
                {
                    curve.PropertyChanged +=
                        (sender, e) =>
                        {
                            if (e.PropertyName == "IsBernsteinBasis" || e.PropertyName == "DisplayEdges" ||
                                e.PropertyName == "IsPolygonVisible") Render();
                        };
                    if (((BezierCurveC2)curve).IsInterpolation)
                        curve.PropertyChanged += curve_PropertyChanged;
                }
            }
            foreach (var patch in PatchManager.Patches)
                patch.PropertyChanged += (sender, e) => { if (e.PropertyName == "DisplayEdges") Render(); };
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
        private void TransformScene(Matrix3D matrix)
        {
            bool onlySelected = Keyboard.IsKeyDown(Key.S);
            foreach (var mesh in onlySelected ? MeshManager.Meshes.Where(m => m.IsSelected) : MeshManager.Meshes)
                mesh.ModelTransform = matrix * mesh.ModelTransform;
            foreach (var point in onlySelected ? PointManager.Points.Where(p => p.IsSelected) : PointManager.Points)
                point.ModelTransform = matrix * point.ModelTransform;
            foreach (var patch in onlySelected ? PatchManager.Patches.Where(p => p.IsSelected) : PatchManager.Patches)
                patch.ModelTransform = matrix * patch.ModelTransform;

            if (onlySelected)
                foreach (var patch in PatchManager.Patches)
                    foreach (var vertex in patch.Vertices.Where(v => v.IsSelected))
                        vertex.ModelTransform = matrix * vertex.ModelTransform;
        }
        private PointEx FindInterpolationPoint(List<PointEx> points)
        {
            double x = 0, y = 0, z = 0;
            foreach (var point in points)
            {
                x += point.TransformedPosition.X;
                y += point.TransformedPosition.Y;
                z += point.TransformedPosition.Z;
            }

            return new PointEx(x / points.Count, y / points.Count, z / points.Count);
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

            if (dialog.ShowDialog() == true && dialog.CheckPathExists)
                SceneManager.SaveScene(dialog.FileName);
        }

        private ICommand _loadSceneCommand;
        public ICommand LoadSceneCommand { get { return _loadSceneCommand ?? (_loadSceneCommand = new DelegateCommand(LoadSceneExecuted)); } }
        /// <summary>
        /// Loads the scene.
        /// </summary>
        private void LoadSceneExecuted()
        {
            OpenFileDialog dialog = new OpenFileDialog();
            if (dialog.ShowDialog() == true && dialog.CheckFileExists)
            {
                SceneManager.RemoveAllObjects();
                LoadFile(dialog.FileName);
            }
            Render();
        }

        private ICommand _addMeshFromFileCommand;
        public ICommand AddMeshFromFileCommand { get { return _addMeshFromFileCommand ?? (_addMeshFromFileCommand = new DelegateCommand(AddMeshFromFileExecuted)); } }
        /// <summary>
        /// Loads the specified mesh from file.
        /// </summary>
        private void AddMeshFromFileExecuted()
        {
            OpenFileDialog dialog = new OpenFileDialog();
            if (dialog.ShowDialog() == true && dialog.CheckFileExists)
                LoadFile(dialog.FileName);
            Render();
        }

        private ICommand _clearSceneCommand;
        public ICommand ClearSceneCommand { get { return _clearSceneCommand ?? (_clearSceneCommand = new DelegateCommand(ClearSceneExecuted)); } }
        /// <summary>
        /// Clears the scene.
        /// </summary>
        private void ClearSceneExecuted()
        {
            SceneManager.RemoveAllObjects();
            Render();
        }

        private ICommand _addTorusCommand;
        public ICommand AddTorusCommand { get { return _addTorusCommand ?? (_addTorusCommand = new DelegateCommand(AddTorusExecuted)); } }
        /// <summary>
        /// Adds the torus.
        /// </summary>
        private void AddTorusExecuted()
        {
            var torus = new Torus(0, 0, 0, "Torus(L:" + MeshManager.L + ", V:" + MeshManager.V + ")", MeshManager.SmallR, MeshManager.BigR, MeshManager.L, MeshManager.V);
            MeshManager.Meshes.Add(torus);
            Render();
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

        private ICommand _addBezierPatchCommand;
        public ICommand AddBezierPatchCommand { get { return _addBezierPatchCommand ?? (_addBezierPatchCommand = new DelegateCommand(AddBezierPatchExecuted)); } }
        /// <summary>
        /// Adds the Bezier Patch with the Chosen continuity.
        /// </summary>
        private void AddBezierPatchExecuted()
        {
            BezierPatch patch;

            if (PatchManager.PatchContinuity == Continuity.C0)
                patch = new BezierPatchC0(0, 0, 0, "Bezier Patch C0(" + 0 + ", " + 0 + ", " + 0 + ")", PatchManager.IsCylinder
                    , PatchManager.PatchWidth, PatchManager.PatchHeight, PatchManager.VerticalPatches, PatchManager.HorizontalPatches);
            else
                patch = new BezierPatchC2(0, 0, 0, "Bezier Patch C2(" + 0 + ", " + 0 + ", " + 0 + ")", PatchManager.IsCylinder
                    , PatchManager.PatchWidth, PatchManager.PatchHeight, PatchManager.VerticalPatches, PatchManager.HorizontalPatches);
            patch.PropertyChanged += (sender, e) => { if (e.PropertyName == "DisplayEdges")Render(); };
            PatchManager.Patches.Add(patch);
            Render();
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
            curve.Vertices.CollectionChanged += (sender, e) => { Render(); };
            curve.PropertyChanged += (sender, args) => { if (args.PropertyName == "DisplayEdges" || args.PropertyName == "IsPolygonVisible") Render(); };
            CurveManager.Curves.Add(curve);
            Render();
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
            curve.PropertyChanged += (sender, e) => { if (e.PropertyName == "IsBernsteinBasis" || e.PropertyName == "DisplayEdges" || e.PropertyName == "IsPolygonVisible") Render(); };
            CurveManager.Curves.Add(curve);
            Render();
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
            curve.PropertyChanged += curve_PropertyChanged;
            CurveManager.Curves.Add(curve);
            Render();
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
            Render();
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
            Render();
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
            Render();
        }

        private ICommand _mergePointsCommand;
        public ICommand MergePointsCommand { get { return _mergePointsCommand ?? (_mergePointsCommand = new DelegateCommand(MergePointsExecuted)); } }
        /// <summary>
        /// Merges selected points.
        /// </summary>
        private void MergePointsExecuted()
        {
            var points = new List<PointEx>();
            foreach (var patch in PatchManager.Patches.Where(p => p.Vertices.Any(v => v.IsSelected)))
            {
                var point = patch.Vertices.First(p => p.IsSelected);
                var coordinates = patch.Points.CoordinatesOf(point);
                if (!((coordinates.Item1 == 0 || coordinates.Item1 == 3) && (coordinates.Item2 == 0 || coordinates.Item2 == 3)))
                {
                    MessageBox.Show("Cannot collapse points");
                    return;
                }
                points.Add(point);
            }

            var interpolationPoint = FindInterpolationPoint(points);
            foreach (var patch in PatchManager.Patches.Where(p => p.Vertices.Any(v => v.IsSelected)))
                patch.ReplaceVertex(patch.Vertices.First(p => p.IsSelected), interpolationPoint);

            Render();
        }

        private ICommand _fillInSurfaceCommand;
        public ICommand FillInSurfaceCommand { get { return _fillInSurfaceCommand ?? (_fillInSurfaceCommand = new DelegateCommand(FillInSurfaceExecuted)); } }
        /// <summary>
        /// Merges selected points.
        /// </summary>
        private void FillInSurfaceExecuted()
        {
            var patches = new List<BezierPatch>();
            foreach (var p in PatchManager.Patches.Where(x => x.IsSelected))
            {
                if (p.CommonPoints.Count != 2)
                {
                    MessageBox.Show("No surface to fill");
                    return;
                }
                patches.Add(p);
            }

            if (patches.Count != 3)
            {
                MessageBox.Show("No surface to fill");
                return;
            }
            var patch = new GregoryPatch(0, 0, 0, "Gregory Patch(" + 0 + ", " + 0 + ", " + 0 + ")", patches);
            patch.PropertyChanged += (sender, e) => { if (e.PropertyName == "DisplayEdges")Render(); };
            PatchManager.GregoryPatches.Add(patch);

            Render();
        }

        private ActionCommand<KeyEventArgs> _keyDeleteCommand;
        public ActionCommand<KeyEventArgs> KeyDeleteCommand
        {
            get
            {
                return _keyDeleteCommand ??
                       (_keyDeleteCommand = new ActionCommand<KeyEventArgs>(KeyDeleteExecuted));
            }
        }
        /// <summary>
        /// Removes the selected point
        /// </summary>
        private void KeyDeleteExecuted(KeyEventArgs args)
        {
            KeyboardManager.KeyDeleteExecuted();
            Render();
        }

        private ActionCommand<KeyEventArgs> _keySelectCommand;
        public ActionCommand<KeyEventArgs> KeySelectCommand
        {
            get
            {
                return _keySelectCommand ??
                       (_keySelectCommand = new ActionCommand<KeyEventArgs>(KeySelectExecuted));
            }
        }
        /// <summary>
        /// Selects / deselects the point
        /// </summary>
        private void KeySelectExecuted(KeyEventArgs args)
        {
            KeyboardManager.KeySelectExecuted();
            Render();
        }

        private ActionCommand<KeyEventArgs> _keyInsertCommand;
        public ActionCommand<KeyEventArgs> KeyInsertCommand
        {
            get
            {
                return _keyInsertCommand ??
                       (_keyInsertCommand = new ActionCommand<KeyEventArgs>(KeyInsertExecuted));
            }
        }
        /// <summary>
        /// Adds a new point in the position of the 3D cursor
        /// </summary>
        private void KeyInsertExecuted(KeyEventArgs args)
        {
            KeyboardManager.KeyInsertExecuted();
            Render();
        }
        #endregion Commands
    }
}
