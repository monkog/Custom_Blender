using System;
using System.Drawing;
using System.Threading;
using System.Windows;
using System.Windows.Media.Media3D;
using RayTracer.Helpers;

namespace RayTracer.Model.Shapes
{
    public class Ellipsoide : ShapeBase
    {
        #region Private Members
        /// <summary>
        /// Thread working on updating the Ellipsoide
        /// </summary>
        private Thread _workerThread;
        #endregion Private Members
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
            _workerThread = new Thread(() => { Draw(64); });
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
            _workerThread.Abort();
            _workerThread = new Thread(() => { Draw(64); });
            _workerThread.Start();
        }
        #endregion Public Methods
        #region Private Methods
        /// <summary>
        /// Draws the Ellipsoide in the specified resolution
        /// </summary>
        /// <param name="pixelSize">Number of pixels representing one pixel</param>
        private void Draw(int pixelSize)
        {
            Bitmap bmp = SceneManager.Instance.SceneImage;
            var transformInvert = SceneManager.Instance.TransformMatrix * SceneManager.Instance.ScaleMatrix * ModelTransform;
            transformInvert.Invert();
            var totalMatrix = transformInvert.Transpose() * D * transformInvert;

            for (int i = 0; i < 800; i += pixelSize)
                for (int j = 0; j < 600; j += pixelSize)
                {
                    int x = (i + (pixelSize / 2)) % 800;
                    int y = (j + (pixelSize / 2)) % 600;
                    double b = totalMatrix.M31 * x + totalMatrix.M32 * y + totalMatrix.M13 * x + totalMatrix.M23 * y + totalMatrix.OffsetZ + totalMatrix.M34;
                    double c = x * (totalMatrix.M11 * x + y * totalMatrix.M21 + totalMatrix.OffsetX)
                        + y * (x * totalMatrix.M12 + y * totalMatrix.M22 + totalMatrix.OffsetY)
                        + x * totalMatrix.M14 + y * totalMatrix.M24 + totalMatrix.M44;
                    double fourAc = 4 * totalMatrix.M33 * c;
                    double delta = b * b - fourAc;
                    if (delta >= 0)
                    {
                        double z = Math.Min((-b + Math.Sqrt(delta)) / (2 * totalMatrix.M33), (-b - Math.Sqrt(delta)) / (2 * totalMatrix.M33));
                        Vector4 v = new Vector4(x, y, z, 1);
                        Vector4 n = v * totalMatrix * 2;
                        double light = Math.Pow((new Vector4(400, 300, -100, 1)).Dot(n), SceneManager.Instance.M);

                        for (int k = 0; k < pixelSize; k++)
                            for (int l = 0; l < pixelSize; l++)
                                bmp.SetPixel((i + k) % 800, (j + l) % 600, GetNormalizedColor(Color, light));
                    }
                    else
                        for (int k = 0; k < pixelSize && i + k < 800; k++)
                            for (int l = 0; l < pixelSize && j + l < 600; l++)
                                bmp.SetPixel(i + k, j + l, DefaultColor);
                }
            Application.Current.Dispatcher.Invoke(() => { SceneManager.Instance.SceneImage = bmp; });
            if (pixelSize > 1)
                Draw(pixelSize / 2);
        }
        /// <summary>
        /// Calculates the value of the color increased by the certain value and normalized to 255
        /// </summary>
        /// <param name="color">The color</param>
        /// <param name="value">The value</param>
        /// <returns>Normalized increased color</returns>
        private Color GetNormalizedColor(Color color, double value)
        {
            if (value < 0)
                return Color;
            Vector3D v = new Vector3D(color.R + value, color.G + value, color.B + value);
            v.Normalize();
            return Color.FromArgb((int)(v.X * 255), (int)(v.Y * 255), (int)(v.Z * 255));
        }
        #endregion Private Methods
    }
}
