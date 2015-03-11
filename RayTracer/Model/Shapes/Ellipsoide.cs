using System;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using RayTracer.Helpers;
using System.Drawing;

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
        public static Color Color { get { return Color.Yellow; } }
        /// <summary>
        /// Gets the color of the back.
        /// </summary>
        /// <value>
        /// The color of the back.
        /// </value>
        public static Color DefaultColor { get { return Color.Black; } }
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
            Bitmap bmp = SceneManager.Instance.SceneImage;
            var transformInvert = SceneManager.Instance.TransformMatrix * SceneManager.Instance.ScaleMatrix * ModelTransform;
            transformInvert.Invert();
            var totalMatrix = transformInvert.Transpose() * D * transformInvert;

            for (int i = 0; i < 800; i++)
                for (int j = 0; j < 600; j++)
                {
                    double b = totalMatrix.M31 * i + totalMatrix.M32 * j + totalMatrix.M13 * i + totalMatrix.M23 * j + totalMatrix.OffsetZ + totalMatrix.M34;
                    double c = i * (totalMatrix.M11 * i + j * totalMatrix.M21 + totalMatrix.OffsetX)
                        + j * (i * totalMatrix.M12 + j * totalMatrix.M22 + totalMatrix.OffsetY)
                        + i * totalMatrix.M14 + j * totalMatrix.M24 + totalMatrix.M44;
                    double fourAc = 4 * totalMatrix.M33 * c;
                    double delta = b * b - fourAc;
                    if (delta >= 0)
                    {
                        double z = Math.Min((-b + Math.Sqrt(delta)) / (2 * totalMatrix.M33), (-b - Math.Sqrt(delta)) / (2 * totalMatrix.M33));

                        bmp.SetPixel(i, j, Color);
                    }
                    else
                        bmp.SetPixel(i, j, DefaultColor);
                }
            SceneManager.Instance.SceneImage = bmp;
        }
        #endregion Public Methods
    }
}
