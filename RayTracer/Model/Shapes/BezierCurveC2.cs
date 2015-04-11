using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Media.Media3D;

namespace RayTracer.Model.Shapes
{
    public class BezierCurveC2 : BezierCurve
    {
        #region Private Methods
        #endregion Private Methods
        #region Public Properties
        #endregion Public Properties
        #region Constructors
        public BezierCurveC2(double x, double y, double z, string name, IEnumerable<PointEx> points)
            : base(x, y, z, name, points)
        {
            //_bezierPolygonIndices = new ObservableCollection<Tuple<int, int>>();
            //_isPolygonVisible = true;

            //SetVertices(points);
            //SetEdges();
            //TransformVertices(Matrix3D.Identity);
            //DisplayVertices = false;
        }
        #endregion Constructors
        #region Private Methods
        #endregion Private Methods
        #region Public Methods
        #endregion Public Methods
    }
}

