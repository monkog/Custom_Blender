using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Media.Media3D;
using RayTracer.Helpers;
using RayTracer.ViewModel;

namespace RayTracer.Model.Shapes
{
    public sealed class BezierCurveC2 : BezierCurve
    {
        #region Private Members
        private bool _isBernsteinBasis;
        private double[] _knots;
        private const int N = 3;
        private bool _isInterpolation;
        private bool _equidistantPoints;
        /// <summary>
        /// Value for updating the selection after re-drawing the curve
        /// </summary>
        private int _selectedPointIndex;
        /// <summary>
        /// Value for updating the selected point to be middle point after the change
        /// </summary>
        private bool _isMiddlePointSelected;
        #endregion Private Members
        #region Public Properties
        public override string Type
        {
            get
            {
                if (IsInterpolation)
                    return "InterpolationCurve";

                return "BezierCurveC2";
            }
        }
        /// <summary>
        /// Vertices for the B-Spline basis.
        /// </summary>
        public ObservableCollection<PointEx> DeBooreVertices { get; private set; }
        /// <summary>
        /// Vertices for the Bernstein basis.
        /// </summary>
        public ObservableCollection<PointEx> BezierVertices { get; private set; }
        /// <summary>
        /// Points to interpolate
        /// </summary>
        public ObservableCollection<PointEx> InterpolationPoints { get; private set; }
        /// <summary>
        /// Determines whether the Bezier curve is being displayed using Bernstein basis.
        /// If not, it's in B-Spline basis.
        /// </summary>
        public bool IsBernsteinBasis
        {
            get { return _isBernsteinBasis; }
            set
            {
                if (_isBernsteinBasis == value) return;
                _isBernsteinBasis = value;
                UpdateVertices();
                OnPropertyChanged("IsBernsteinBasis");
            }
        }
        /// <summary>
        /// Determines whether the points are equidistant
        /// </summary>
        public bool EquidistantPoints
        {
            get { return _equidistantPoints; }
            set
            {
                if (_equidistantPoints == value) return;
                _equidistantPoints = value;
                OnPropertyChanged("EquidistantPoints");
            }
        }
        /// <summary>
        /// Determines whether this is an interpolation curve
        /// </summary>
        public bool IsInterpolation
        {
            get { return _isInterpolation; }
            set
            {
                if (_isInterpolation == value) return;
                _isInterpolation = value;
                OnPropertyChanged("IsInterpolation");
            }
        }
        #endregion Public Properties
        #region Constructors
        public BezierCurveC2(double x, double y, double z, string name, IEnumerable<PointEx> points, bool isInterpolation)
            : base(x, y, z, name, points, Continuity.C2)
        {
            if ((IsInterpolation = isInterpolation))
                InterpolationPoints = new ObservableCollection<PointEx>(points);
            else
            {
                DeBooreVertices = new ObservableCollection<PointEx>(points);
                InterpolationPoints = new ObservableCollection<PointEx>();
            }
            EquidistantPoints = true;
            IsBernsteinBasis = true;
            UpdateVertices();
        }
        #endregion Constructors
        #region Private Methods
        private void AddPointInHalf(ObservableCollection<PointEx> oldPoints, ObservableCollection<PointEx> newPoints, int startIndex, int endIndex, bool isEndSegment = false)
        {
            var start = oldPoints.ElementAt(startIndex);
            var end = oldPoints.ElementAt(endIndex);
            var bezierPoint = start.TransformedPosition + (end.TransformedPosition - start.TransformedPosition) / 2;

            newPoints.Add(new BezierPoint(bezierPoint.X, bezierPoint.Y, bezierPoint.Z, start, end, BezierPointType.Second));
            if (isEndSegment)
                BezierVertices.Add(end);
        }
        private void AddHalfPointsOnNewEdges()
        {
            ObservableCollection<PointEx> bezierVertices = new ObservableCollection<PointEx>();
            if (BezierVertices.Count <= 4) return;
            bezierVertices.Add(BezierVertices[0]);
            bezierVertices.Add(BezierVertices[1]);

            for (int i = 2; i < BezierVertices.Count - 2; i += 2)
            {
                bezierVertices.Add(BezierVertices[i]);
                AddPointInHalf(BezierVertices, bezierVertices, i, i + 1);
                bezierVertices.Add(BezierVertices[i + 1]);
            }
            bezierVertices.Add(BezierVertices[BezierVertices.Count - 2]);
            bezierVertices.Add(BezierVertices[BezierVertices.Count - 1]);
            BezierVertices = bezierVertices;
        }
        /// <summary>
        /// Updates the position of Bezier vertices depending on deBoore nodes.
        /// </summary>
        private void UpdateBezierVertices()
        {
            BezierVertices = new ObservableCollection<PointEx>();
            if (!DeBooreVertices.Any()) return;

            BezierVertices.Add(DeBooreVertices.ElementAt(0));
            for (int i = 1; i < DeBooreVertices.Count(); i++)
            {
                var start = DeBooreVertices.ElementAt(i - 1);
                var end = DeBooreVertices.ElementAt(i);
                var bezierPointStep = (end.TransformedPosition - start.TransformedPosition) / 3;

                for (int j = 1; j < 3; j++)
                {
                    var bezierPoint = start.TransformedPosition + bezierPointStep * j;
                    BezierVertices.Add(new BezierPoint(bezierPoint.X, bezierPoint.Y, bezierPoint.Z, start, end, j == 1 ? BezierPointType.First : BezierPointType.Third));
                }
            }

            if (DeBooreVertices.Count() > 1)
                BezierVertices.Add(DeBooreVertices.ElementAt(DeBooreVertices.Count() - 1));

            AddHalfPointsOnNewEdges();
        }
        private void UpdateDeBooreVertices()
        {
            var pt = Vertices.FirstOrDefault(p => p.IsSelected);
            if (!(pt is BezierPoint)) return;

            var point = pt as BezierPoint;
            _selectedPointIndex = Vertices.IndexOf(point);

            if (point.BezierPointType == BezierPointType.Second)
            {
                var leftParent = point.LeftParent.TransformedPosition;

                var leftParentNew = point.TransformedPosition * 2 - point.RightParent.TransformedPosition;
                var transformMatrix = Transformations.TranslationMatrix(new Vector3D(leftParentNew.X - leftParent.X, leftParentNew.Y - leftParent.Y, leftParentNew.Z - leftParent.Z));
                point.LeftParent.IsSelected = true;
                point.IsSelected = false;
                _isMiddlePointSelected = true;
                point.LeftParent.ModelTransform = transformMatrix * point.LeftParent.ModelTransform;
                UpdateDeBooreVertices();
            }
            else
            {
                var leftParent = point.LeftParent.TransformedPosition;
                Vector4 leftParentNew;
                Matrix3D transformMatrix;

                if (point.BezierPointType == BezierPointType.First)
                {
                    leftParentNew = (point.TransformedPosition * 3 - point.RightParent.TransformedPosition) * 0.5;
                    transformMatrix = Transformations.TranslationMatrix(new Vector3D(leftParentNew.X - leftParent.X, leftParentNew.Y - leftParent.Y, leftParentNew.Z - leftParent.Z));
                }
                else
                {
                    leftParentNew = point.TransformedPosition * 3 - (point.RightParent.TransformedPosition * 2);
                    transformMatrix = Transformations.TranslationMatrix(new Vector3D(leftParentNew.X - leftParent.X, leftParentNew.Y - leftParent.Y, leftParentNew.Z - leftParent.Z));
                }
                if (_isMiddlePointSelected)
                {
                    Vertices[_selectedPointIndex + 1].IsSelected = true;
                    point.IsSelected = false;
                    _isMiddlePointSelected = false;
                }
                point.LeftParent.ModelTransform = transformMatrix * point.LeftParent.ModelTransform;
            }
        }
        private Vector4 BSplinePoint(List<Vector4> curve, double t)
        {
            Vector4 sum = new Vector4(0, 0, 0, 1);

            for (int i = 0; i < curve.Count; i++)
                sum += curve[i] * _knots.GetNFunctionValue(i, N, t);

            return new Vector4(sum.X, sum.Y, sum.Z, 1);
        }
        private void GetBezierCurveInBernsteinBasis(List<Tuple<List<Vector4>, double>> curves, ref List<Vector4> curve, ref double divisions)
        {
            int index = 0;
            for (int i = 3; i < Vertices.Count() - 3; i++)
            {
                curve.Add(Transformations.TransformPoint(Vertices.ElementAt(i).Vector4, Vertices.ElementAt(i).ModelTransform).Normalized);
                index = (index + 1) % 4;

                if (i < Vertices.Count - 1)
                    divisions += (Vertices.ElementAt(i).PointOnScreen - Vertices.ElementAt(i + 1).PointOnScreen).Length;

                if (index == 0 && i < Vertices.Count - 1)
                {
                    i--;
                    curves.Add(new Tuple<List<Vector4>, double>(curve, 1 / divisions));
                    curve = new List<Vector4>();
                }
            }
        }
        private void SetSplineKnots(int n)
        {
            _knots = new double[n + N + 4];

            double interval = 1 / (double)(n + N + 3);

            for (int i = 0; i < n + N + 4; i++)
                _knots[i] = i * interval;

            if (!_equidistantPoints)
            {
                double totalDistance = 0;
                double[] distances = new double[n - 1];

                for (int i = 0; i < n - 1; i++)
                {
                    distances[i] = (InterpolationPoints.ElementAt(Math.Max(0, Math.Min(i + 1, InterpolationPoints.Count - 1))).TransformedPosition
                        - InterpolationPoints.ElementAt(Math.Max(0, Math.Min(i, InterpolationPoints.Count - 1))).TransformedPosition).Length;
                    totalDistance += distances[i];
                }

                double inter = (n - 1) * interval;
                for (int i = N; i < N + distances.Length; i++)
                    _knots[i] = _knots[Math.Max(0, Math.Min(i - 1, InterpolationPoints.Count - 1))] + ((distances[Math.Max(0, Math.Min(i - n, InterpolationPoints.Count - 1))] * inter) / totalDistance);
            }
        }
        private ObservableCollection<PointEx> CalculateInterpolationDeBoore()
        {
            var mtx = CalculateSegments(InterpolationPoints.Count);
            double[][] s = new double[3][];
            for (int i = 0; i < 3; i++)
                s[i] = new double[InterpolationPoints.Count() + 2];
            for (int i = -1; i <= InterpolationPoints.Count(); i++)
            {
                s[0][i + 1] = InterpolationPoints.ElementAt(Math.Min(Math.Max(i, 0), InterpolationPoints.Count() - 1)).TransformedPosition.X;
                s[1][i + 1] = InterpolationPoints.ElementAt(Math.Min(Math.Max(i, 0), InterpolationPoints.Count() - 1)).TransformedPosition.Y;
                s[2][i + 1] = InterpolationPoints.ElementAt(Math.Min(Math.Max(i, 0), InterpolationPoints.Count() - 1)).TransformedPosition.Z;
            }
            double[][] result = { mtx.GaussElimination(s[0]), mtx.GaussElimination(s[1]), mtx.GaussElimination(s[2]) };
            ObservableCollection<PointEx> vertices = new ObservableCollection<PointEx>();
            for (int i = 0; i < InterpolationPoints.Count() + 2; i++)
                vertices.Add(new PointEx(result[0][i], result[1][i], result[2][i]));
            return vertices;
        }
        private double[,] CalculateSegments(int knotsCount)
        {
            SetSplineKnots(knotsCount);
            return _knots.CalculateNMatrix(N, knotsCount);
        }
        #endregion Private Methods
        #region Protected Methods
        /// <summary>
        /// Gets the list of points creating curves
        /// </summary>
        /// <returns>The list of points creating curves</returns>
        protected override List<Tuple<List<Vector4>, double>> GetBezierCurves()
        {
            var curves = new List<Tuple<List<Vector4>, double>>();
            var curve = new List<Vector4>();
            double divisions = 0;

            if (IsBernsteinBasis)
                GetBezierCurveInBernsteinBasis(curves, ref curve, ref divisions);
            else if (Vertices.Count > 0)
            {
                curve.Add(Transformations.TransformPoint(Vertices.ElementAt(0).Vector4, Vertices.ElementAt(0).ModelTransform).Normalized);

                for (int i = 1; i < Vertices.Count(); i++)
                {
                    curve.Add(Transformations.TransformPoint(Vertices.ElementAt(i).Vector4, Vertices.ElementAt(i).ModelTransform).Normalized);
                    divisions += (Vertices.ElementAt(i - 1).PointOnScreen - Vertices.ElementAt(i).PointOnScreen).Length;
                }
                SetSplineKnots(curve.Count);
            }

            if (curve.Count > 0)
                curves.Add(new Tuple<List<Vector4>, double>(curve, Math.Max(1 / divisions, 0)));

            return curves;
        }
        protected override Vector4 GetCurvePoint(List<Vector4> curve, double t)
        {
            if (IsBernsteinBasis)
                return base.GetCurvePoint(curve, t);

            return (t < _knots[3] || t > _knots[_knots.Length - N - 4]) ? null : BSplinePoint(curve, t);
        }
        protected override void SetEdges()
        {
            if (IsBernsteinBasis)
                SetEdgesInRange(from: N, to: Vertices.Count - N - 1);
            else
                base.SetEdges();
        }
        #endregion Protected Methods
        #region Public Methods
        /// <summary>
        /// Updates the position of vertices depending on current base.
        /// </summary>
        public void UpdateVertices()
        {
            if (_isInterpolation)
                DeBooreVertices = CalculateInterpolationDeBoore();

            if (IsBernsteinBasis)
            {
                UpdateBezierVertices();
                Vertices = BezierVertices;
            }
            else
            {
                UpdateDeBooreVertices();
                Vertices = DeBooreVertices;
            }
            SetEdges();
            CalculateShape();
        }
        public override void Draw()
        {
            if (IsBernsteinBasis && Vertices.Any(p => p.IsSelected))
            {
                UpdateDeBooreVertices();
                Vertices[_selectedPointIndex].IsSelected = true;
                SetEdges();
            }
            base.Draw();
        }
        public override void SaveControlPointsReference(StringBuilder stringBuilder)
        {
            if (IsInterpolation)
                foreach (var point in InterpolationPoints)
                    stringBuilder.AppendLine("CP=" + point.Id);
            else
                foreach (var point in DeBooreVertices)
                    stringBuilder.AppendLine("CP=" + point.Id);
        }
        #endregion Public Methods
    }
}

