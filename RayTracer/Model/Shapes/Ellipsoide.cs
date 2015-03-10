using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using RayTracer.Helpers;

namespace RayTracer.Model.Shapes
{
    public class Ellipsoide : ShapeBase
    {
        #region Public Properties
        /// <summary>
        /// Matrix keeping a, b and c values - radius of Ellipsoide
        /// </summary>
        public Matrix3D D { get; private set; }
        /// <summary>
        /// Gets the color of the Ellipsoide.
        /// </summary>
        /// <value>
        /// The color of the Ellipsoide.
        /// </value>
        public static Color Color { get { return Colors.Yellow; } }
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
        #region Protected Methods
        protected override void CalculateShape() { }
        #endregion Protected Methods
        #region Public Methods
        public override void Draw()
        {
            WriteableBitmap bmp = SceneManager.Instance.SceneImage;
            var pixels = new byte[800 * 600];
            var transformInvert = ModelTransform;
            transformInvert.Invert();
            var totalMatrix = transformInvert.Transpose() * D * transformInvert;

            for (int i = 0; i < 800; i++)
                for (int j = 0; j < 600; j++)
                {
                    double zSquared = D.M33 * (1 - ((i * i / D.M11) + (j * j / D.M22)));
                    if (zSquared > 0)
                    {
                        double z = Math.Sqrt(zSquared);
                        pixels[i * 800 + j] = 1;
                    }
                }
            bmp.WritePixels(new System.Windows.Int32Rect(0, 0, 800, 600), pixels, 800, 0);
        }
        #endregion Public Methods
    }
}
