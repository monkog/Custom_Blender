using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Media.Media3D;
using RayTracer.Helpers;
using RayTracer.ViewModel;

namespace RayTracer.Model.Shapes
{
    public class BezierCurveC2 : BezierCurve
    {
        #region Private Members
        private ObservableCollection<PointEx> _deBooreVertices;
        private ObservableCollection<PointEx> _bezierVertices;
        #endregion Private Members
        #region Public Properties
        #endregion Public Properties
        #region Constructors
        public BezierCurveC2(double x, double y, double z, string name, IEnumerable<PointEx> points)
            : base(x, y, z, name, points)
        {
            _deBooreVertices = new ObservableCollection<PointEx>(points);
            SetBezierVertices(points);
            Vertices = _bezierVertices;

            SetEdges();
            DisplayVertices = true;
        }
        #endregion Constructors
        #region Private Methods
        private void AddPointInHalf(IEnumerable<PointEx> points, int startIndex, int endIndex)
        {
            var start = points.ElementAt(startIndex);
            var end = points.ElementAt(endIndex);
            var bezierPoint = start.TransformedPosition + (end.TransformedPosition - start.TransformedPosition) / 2;

            _bezierVertices.Add(start);
            _bezierVertices.Add(new PointEx(bezierPoint.X, bezierPoint.Y, bezierPoint.Z));
        }
        private void SetBezierVertices(IEnumerable<PointEx> points)
        {
            _bezierVertices = new ObservableCollection<PointEx>();
            if (points.Count() > 0)
                AddPointInHalf(points, startIndex: 0, endIndex: 1);

            for (int i = 1; i < points.Count() - 2; i++)
            {
                var start = points.ElementAt(i);
                var end = points.ElementAt(i + 1);
                var bezierPointStep = (end.TransformedPosition - start.TransformedPosition) / 3;

                for (int j = 1; j < 3; j++)
                {
                    var bezierPoint = start.TransformedPosition + bezierPointStep * j;
                    _bezierVertices.Add(new PointEx(bezierPoint.X, bezierPoint.Y, bezierPoint.Z));
                }
            }

            if (points.Count() > 2)
                AddPointInHalf(points, startIndex: points.Count() - 2, endIndex: points.Count() - 1);
        }
        #endregion Private Methods
        #region Protected Methods

        protected override List<Tuple<List<Vector4>, double>> GetBezierCurves()
        {
            var curves = new List<Tuple<List<Vector4>, double>>();
            var curve = new List<Vector4>();
            double divisions = 0;
            int index = 0;
            for (int i = 0; i < Vertices.Count(); i++)
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

            if (curve.Count > 0)
                curves.Add(new Tuple<List<Vector4>, double>(curve, 1 / divisions));

            return curves;
        }
        #endregion Protected Methods
        #region Public Methods
        #endregion Public Methods
    }
}

