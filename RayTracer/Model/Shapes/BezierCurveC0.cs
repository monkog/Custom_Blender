using System.Collections.Generic;

namespace RayTracer.Model.Shapes
{
    public class BezierCurveC0 : BezierCurve
    {
        #region Constructors
        public BezierCurveC0(double x, double y, double z, string name, IEnumerable<PointEx> points)
            : base(x, y, z, name, points, Continuity.C0)
        { }
        #endregion Constructors
    }
}

