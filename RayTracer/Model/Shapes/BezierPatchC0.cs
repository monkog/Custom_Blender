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
        private ObservableCollection<BezierCurveC0> _curves;
        private const int BezierSegmentPoints = 3;
        #endregion Private Members
        #region Public Properties
        public System.Collections.Generic.IEnumerable<object> SelectedItems { get { return Vertices.Where(p => p.IsSelected); } }
        #endregion Public Properties
        #region Constructors
        public BezierPatchC0(double x, double y, double z, string name)
            : base(x, y, z, name)
        {
            _isCylinder = PatchManager.Instance.IsCylinder;
            SetVertices();
        }
        #endregion Constructors
        #region Private Methods
        /// <summary>
        /// Sets the vertices.
        /// </summary>
        private void SetVertices()
        {
            _curves = new ObservableCollection<BezierCurveC0>();
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
                    _curves.Add(curve);
                }

            for (int j = 0; j < points.GetLength(1); j++)
                for (int i = 0; i < points.GetLength(0) - 1; i += BezierSegmentPoints)
                {
                    var curve = new BezierCurveC0(0, 0, 0, "Bezier curve C0(" + 0 + ", " + 0 + ", " + 0 + ")"
                        , new ObservableCollection<PointEx>() { points[i, j], points[i + 1, j], points[i + 2, j], points[i + 3, j] });
                    _curves.Add(curve);
                }
        }
        private void SetCylinderVertices()
        {
            //double circleStride = (Math.PI * 2.0f) / _circle_division;
            //double donutStride = (Math.PI * 2.0f) / _donutDivision;
            //double alpha;
            //double beta;

            //for (int i = 0; i < _donutDivision; i++)
            //{
            //    beta = i * donutStride;
            //    for (int j = 0; j < _circle_division; j++)
            //    {
            //        alpha = j * circleStride;
            //        Vertices.Add(new PointEx((_r * Math.Cos(alpha) + _R) * Math.Cos(beta)
            //           , (_r * Math.Cos(alpha) + _R) * Math.Sin(beta), _r * Math.Sin(alpha)));
            //    }
            //}
        }
        #endregion Private Methods
        #region Public Methods
        /// <summary>
        /// Transforms the patch using the given matrix.
        /// </summary>
        /// <param name="matrix">The matrix.</param>
        public void TransformPatch(Matrix3D matrix)
        {
            foreach (var curve in _curves)
                curve.ModelTransform = matrix * curve.ModelTransform;
        }
        public override void Draw()
        {
            foreach (var curve in _curves)
                curve.Draw();
        }
        #endregion Public Methods
    }
}

