using System;
using System.Collections.ObjectModel;
using RayTracer.Helpers;

namespace RayTracer.Model.Shapes
{
    public class Torus : ShapeBase
    {
        /// <summary>
        /// The donut divisions (beta)
        /// </summary>
        private int _donutDivision;
        /// <summary>
        /// The circle divisions (alpha)
        /// </summary>
        private int _circle_division;
        private double _r;
        private double _R;

        public Torus(double x, double y, double z, int l, int v)
            : base(x, y, z)
        {
            _donutDivision = l;
            _circle_division = v;
            _r = 0.5;
            _R = 1;
            SetVertices();
            SetEdges();
            TransformVertices();
        }
        #region Private Methods

        /// <summary>
        /// Sets the vertices.
        /// </summary>
        private void SetVertices()
        {
            double circleStride = (Math.PI * 2.0f) / _circle_division;
            double donutStride = (Math.PI * 2.0f) / _donutDivision;
            double alpha;
            double beta;

            for (int i = 0; i < _donutDivision; i++)
            {
                beta = i * donutStride;
                for (int j = 0; j < _circle_division; j++)
                {
                    alpha = j * circleStride;
                    Vertices.Add(new Vector4((_r * Math.Cos(alpha) + _R) * Math.Cos(beta)
                       , (_r * Math.Cos(alpha) + _R) * Math.Sin(beta), _r * Math.Sin(alpha), 1));
                }
            }
        }

        /// <summary>
        /// Sets the edges.
        /// </summary>
        private void SetEdges()
        {
            EdgesIndices = new ObservableCollection<Tuple<int, int>>();

            for (int i = 1; i < _donutDivision; i++)
                for (int j = 0; j < _circle_division; j++)
                    EdgesIndices.Add(new Tuple<int, int>(i * _circle_division + j, i * _circle_division + ((j + 1) % _circle_division)));

            for (int j = 0; j < _circle_division; j++)
                for (int i = 0; i < _donutDivision; i++)
                    EdgesIndices.Add(new Tuple<int, int>(i * _circle_division + j, ((i + 1) % _donutDivision) * _circle_division + j));
            EdgesIndices.Add(new Tuple<int, int>(_circle_division - 1, 0));
        }
        #endregion Private Methods
    }
}
