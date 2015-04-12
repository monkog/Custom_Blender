using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace RayTracer.Model.Shapes
{
    public class BezierCurveC2 : BezierCurve
    {
        #region Private Members
        private bool _isBernsteinBasis;
        #endregion Private Members
        #region Public Properties
        /// <summary>
        /// Vertices for the B-Spline basis.
        /// </summary>
        public ObservableCollection<PointEx> DeBooreVertices { get; private set; }
        /// <summary>
        /// Vertices for the Bernstein basis.
        /// </summary>
        public ObservableCollection<PointEx> BezierVertices { get; private set; }
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
                OnPropertyChanged("IsBernsteinBasis");
            }
        }
        #endregion Public Properties
        #region Constructors
        public BezierCurveC2(double x, double y, double z, string name, IEnumerable<PointEx> points)
            : base(x, y, z, name, points, Continuity.C2)
        {
            DeBooreVertices = new ObservableCollection<PointEx>(points);
            DisplayVertices = true;
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

            newPoints.Add(new PointEx(bezierPoint.X, bezierPoint.Y, bezierPoint.Z));
            if (isEndSegment)
                BezierVertices.Add(end);
        }
        private void AddHalfPointsOnNewEdges()
        {
            ObservableCollection<PointEx> bezierVertices = new ObservableCollection<PointEx>();
            bezierVertices.Add(BezierVertices[0]);
            if (BezierVertices.Count == 3)
                bezierVertices.Add(BezierVertices[1]);

            for (int i = 1; i < BezierVertices.Count - 2; i += 2)
            {
                bezierVertices.Add(BezierVertices[i]);
                AddPointInHalf(BezierVertices, bezierVertices, i, i + 1);
                bezierVertices.Add(BezierVertices[i + 1]);
            }
            if (BezierVertices.Count > 1)
                bezierVertices.Add(BezierVertices[BezierVertices.Count - 1]);
            BezierVertices = bezierVertices;
        }
        /// <summary>
        /// Updates the position of Bezier vertices depending on deBoore nodes.
        /// </summary>
        private void UpdateBezierVertices()
        {
            BezierVertices = new ObservableCollection<PointEx>();
            if (DeBooreVertices.Count() == 0) return;

            BezierVertices.Add(DeBooreVertices.ElementAt(0));
            if (DeBooreVertices.Count() > 1)
                AddPointInHalf(DeBooreVertices, BezierVertices, startIndex: 0, endIndex: 1, isEndSegment: DeBooreVertices.Count() == 2);

            for (int i = 1; i < DeBooreVertices.Count() - 2; i++)
            {
                var start = DeBooreVertices.ElementAt(i);
                var end = DeBooreVertices.ElementAt(i + 1);
                var bezierPointStep = (end.TransformedPosition - start.TransformedPosition) / 3;

                for (int j = 1; j < 3; j++)
                {
                    var bezierPoint = start.TransformedPosition + bezierPointStep * j;
                    BezierVertices.Add(new PointEx(bezierPoint.X, bezierPoint.Y, bezierPoint.Z));
                }
            }

            if (DeBooreVertices.Count() > 2)
                AddPointInHalf(DeBooreVertices, BezierVertices, startIndex: DeBooreVertices.Count() - 2, endIndex: DeBooreVertices.Count() - 1, isEndSegment: true);

            AddHalfPointsOnNewEdges();
        }
        #endregion Private Methods
        #region Protected Methods
        #endregion Protected Methods
        #region Public Methods
        /// <summary>
        /// Updates the position of vertices depending on current base.
        /// </summary>
        public void UpdateVertices()
        {
            if (IsBernsteinBasis)
            {
                UpdateBezierVertices();
                Vertices = BezierVertices;
            }
            else
            {
                Vertices = DeBooreVertices;
            }
        }
        #endregion Public Methods
    }
}

