using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Media.Media3D;
using RayTracer.ViewModel;

namespace RayTracer.Model.Shapes
{
    public class BezierPatchC0 : ModelBase
    {
        #region Private Members
        private bool _isCylinder;
        private ObservableCollection<BezierCurveC0> _horizontalCurves;
        private ObservableCollection<BezierCurveC0> _verticalCurves;
        private const int BezierSegmentPoints = 3;
        #endregion Private Members
        #region Public Properties
        public System.Collections.Generic.IEnumerable<object> SelectedItems { get { return Vertices.Where(p => p.IsSelected); } }
        public IEnumerable<BezierCurve> Curves { get { return _horizontalCurves; } }
        #endregion Public Properties
        #region Constructors
        public BezierPatchC0(double x, double y, double z, string name)
            : base(x, y, z, name)
        {
            _isCylinder = PatchManager.Instance.IsCylinder;
            SetVertices();
            PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == "DisplayEdges")
                {
                    foreach (var curve in _verticalCurves)
                        curve.DisplayEdges = DisplayEdges;
                    foreach (var curve in _horizontalCurves)
                        curve.DisplayEdges = DisplayEdges;
                }
            };
        }
        #endregion Constructors
        #region Private Methods
        /// <summary>
        /// Sets the vertices.
        /// </summary>
        private void SetVertices()
        {
            _horizontalCurves = new ObservableCollection<BezierCurveC0>();
            _verticalCurves = new ObservableCollection<BezierCurveC0>();
            if (_isCylinder) SetCylinderVertices();
            else SetPlaneVertices();
        }
        private void SetPlaneVertices()
        {
            var manager = PatchManager.Instance;
            PointEx[,] points = new PointEx[manager.VerticalPatches * BezierSegmentPoints + 1, manager.HorizontalPatches * BezierSegmentPoints + 1];

            Vector3D topLeft = new Vector3D(X - (manager.PatchWidth / 2), Y - (manager.PatchHeight / 2), 0);
            double dx = manager.PatchWidth / (manager.HorizontalPatches * BezierSegmentPoints);
            double dy = manager.PatchHeight / (manager.VerticalPatches * BezierSegmentPoints);

            for (int i = 0; i < points.GetLength(0); i++)
                for (int j = 0; j < points.GetLength(1); j++)
                {
                    var point = new PointEx(topLeft.X + (j * dx), topLeft.Y + (i * dy), topLeft.Z);
                    points[i, j] = point;
                    Vertices.Add(point);
                }

            for (int i = 0; i < points.GetLength(0); i++)
                for (int j = 0; j < points.GetLength(1) - 1; j += BezierSegmentPoints)
                {
                    var curve = new BezierCurveC0(0, 0, 0, "Bezier curve C0(" + 0 + ", " + 0 + ", " + 0 + ")"
                        , new ObservableCollection<PointEx>() { points[i, j], points[i, j + 1], points[i, j + 2], points[i, j + 3] });
                    _horizontalCurves.Add(curve);
                }

            for (int j = 0; j < points.GetLength(1); j++)
                for (int i = 0; i < points.GetLength(0) - 1; i += BezierSegmentPoints)
                {
                    var curve = new BezierCurveC0(0, 0, 0, "Bezier curve C0(" + 0 + ", " + 0 + ", " + 0 + ")"
                        , new ObservableCollection<PointEx>() { points[i, j], points[i + 1, j], points[i + 2, j], points[i + 3, j] });
                    _verticalCurves.Add(curve);
                }
        }
        private void SetCylinderVertices()
        {
            var manager = PatchManager.Instance;
            PointEx[,] points = new PointEx[manager.VerticalPatches * BezierSegmentPoints + 1, manager.HorizontalPatches * BezierSegmentPoints];

            double topLeftY = Y - (manager.PatchHeight / 2);
            double radius = manager.PatchWidth;
            double alpha = (Math.PI * 2.0f) / (BezierSegmentPoints * manager.HorizontalPatches);
            double dy = manager.PatchHeight / (manager.VerticalPatches * BezierSegmentPoints);

            for (int i = 0; i < points.GetLength(0); i++)
                for (int j = 0; j < points.GetLength(1); j++)
                {
                    var point = new PointEx(radius * Math.Cos(alpha * j), topLeftY + (i * dy), radius * Math.Sin(alpha * j));
                    points[i, j] = point;
                    Vertices.Add(point);
                }

            for (int i = 0; i < points.GetLength(0); i++)
                for (int j = 0; j < points.GetLength(1) - 1; j += BezierSegmentPoints)
                {
                    var curve = new BezierCurveC0(0, 0, 0, "Bezier curve C0(" + 0 + ", " + 0 + ", " + 0 + ")"
                        , new ObservableCollection<PointEx>() { points[i, j], points[i, j + 1], points[i, j + 2], points[i, (j + 3) % points.GetLength(1)] });
                    _horizontalCurves.Add(curve);
                }

            for (int j = 0; j < points.GetLength(1); j++)
                for (int i = 0; i < points.GetLength(0) - 1; i += BezierSegmentPoints)
                {
                    var curve = new BezierCurveC0(0, 0, 0, "Bezier curve C0(" + 0 + ", " + 0 + ", " + 0 + ")"
                        , new ObservableCollection<PointEx>() { points[i, j], points[i + 1, j], points[i + 2, j], points[i + 3, j] });
                    _verticalCurves.Add(curve);
                }
        }
        #endregion Private Methods
        #region Public Methods
        /// <summary>
        /// Transforms the patch using the given matrix.
        /// </summary>
        /// <param name="matrix">The matrix.</param>
        public void TransformPatch(Matrix3D matrix)
        {
            foreach (var curve in _verticalCurves)
                curve.ModelTransform = matrix * curve.ModelTransform;
            foreach (var curve in _horizontalCurves)
                curve.ModelTransform = matrix * curve.ModelTransform;
        }
        public override void Draw()
        {
            var manager = PatchManager.Instance;
            foreach (var curve in _verticalCurves)
                curve.Draw(1.0 / manager.VerticalPatchDivisions);
            foreach (var curve in _horizontalCurves)
                curve.Draw(1.0 / manager.HorizontalPatchDivisions);
        }
        #endregion Public Methods
    }
}

