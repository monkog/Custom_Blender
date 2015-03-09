using System.Windows.Media.Media3D;

namespace RayTracer.Model.Shapes
{
    public class Ellipsoide : ShapeBase
    {
        #region Public Properties
        /// <summary>
        /// Matrix keeping a, b and c values - radius of Ellipsoide
        /// </summary>
        public Matrix3D D { get; private set; }
        #endregion Public Properties
        #region .ctor
        public Ellipsoide(double x, double y, double z, double a, double b, double c)
            : base(x, y, z)
        {
            D = new Matrix3D(a, 0, 0, 0
                           , 0, b, 0, 0
                           , 0, 0, c, 0
                           , 0, 0, 0, -1);
        }
        #endregion .ctor
    }
}
