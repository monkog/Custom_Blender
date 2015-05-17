using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Windows;
using System.Windows.Media.Media3D;
using RayTracer.Helpers;
using RayTracer.ViewModel;

namespace RayTracer.Model.Shapes
{
    public class Ellipsoide : ModelBase
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
        /// Gets the color of the back.
        /// </summary>
        /// <value>
        /// The color of the back.
        /// </value>
        public static Color DefaultColor { get { return Color.Black; } }
        public override IEnumerable<ShapeBase> SelectedItems { get { return new List<ShapeBase>(); } }
        public override string Type { get { return "Ellipsoid"; } }
        #endregion Public Properties
        #region .ctor
        public Ellipsoide(double x, double y, double z, string name, double a, double b, double c)
            : base(x, y, z, name)
        {
            _workerThread = new Thread(() => { Draw(64); });
            D = new Matrix3D(a, 0, 0, 0
                           , 0, b, 0, 0
                           , 0, 0, c, 0
                           , 0, 0, 0, -1);
            Color = Color.FromArgb(255, 255, 1);
        }
        #endregion .ctor
        #region Protected Methods
        protected override void CalculateShape() { }
        #endregion Protected Methods
        #region Public Methods
        public override void Draw()
        {
            Transform = Transformations.ViewMatrix(400);
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
            lock (BitmapLock)
            {
                Bitmap bmp = SceneManager.Instance.SceneImage;
                var transformInvert = SceneManager.Instance.TransformMatrix * SceneManager.Instance.ScaleMatrix *
                                      ModelTransform;
                transformInvert.Invert();
                var totalMatrix = transformInvert.Transpose() * D * transformInvert;

                for (int i = 0; i < 800; i += pixelSize)
                    for (int j = 0; j < 600; j += pixelSize)
                    {
                        int x, y;
                        double b;
                        var delta = CalculateDelta(out x, pixelSize, i, j, totalMatrix, out y, out b);

                        if (delta >= 0)
                        {
                            double z = Math.Max((-b + Math.Sqrt(delta)) / (2 * totalMatrix.M33),
                                (-b - Math.Sqrt(delta)) / (2 * totalMatrix.M33));
                            var light = CalculateLightIntensity(x, y, z, D);

                            SetPixelColor(pixelSize, i, j, bmp
                                , Color.FromArgb((int)Math.Max(Math.Min(Color.R * light, 255), 0)
                                                            , (int)Math.Max(Math.Min(Color.G * light, 255), 0)
                                                            , (int)Math.Max(Math.Min(Color.B * light, 255), 0)));
                        }
                        else
                            SetPixelColor(pixelSize, i, j, bmp, DefaultColor);
                    }
                if (Application.Current != null)
                    Application.Current.Dispatcher.Invoke(() => { SceneManager.Instance.SceneImage = bmp; });
            }
            if (pixelSize > 1)
                Draw(pixelSize / 2);
        }
        /// <summary>
        /// Sets the specfied color on the pixel
        /// </summary>
        private static void SetPixelColor(int pixelSize, int i, int j, Bitmap bmp, Color color)
        {
            for (int k = 0; k < pixelSize && i + k < 800; k++)
                for (int l = 0; l < pixelSize && j + l < 600; l++)
                    bmp.SetPixel(i + k, j + l, color);
        }
        /// <summary>
        /// Calculates the light intensity.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="z">The z.</param>
        /// <param name="totalMatrix">The total transform matrix.</param>
        /// <returns>Light intensity for the given pixel</returns>
        private static double CalculateLightIntensity(int x, int y, double z, Matrix3D totalMatrix)
        {
            Vector4 v = new Vector4(x, y, z, 0);
            Vector4 n = (totalMatrix * v) * 2;
            Vector4 light = new Vector4(-10, 0, 20, 0);
            double dot = light.Dot(n.Normalized);
            if (dot < 0)
                return 0;

            return Math.Pow(dot, SceneManager.Instance.M);
        }
        /// <summary>
        /// Calculates the delta.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="pixelSize">Size of the pixel.</param>
        /// <param name="i">The i.</param>
        /// <param name="j">The j.</param>
        /// <param name="totalMatrix">The total transform matrix.</param>
        /// <param name="y">The y.</param>
        /// <param name="b">The b.</param>
        /// <returns></returns>
        private static double CalculateDelta(out int x, int pixelSize, int i, int j, Matrix3D totalMatrix, out int y
            , out double b)
        {
            x = Math.Min(i + (pixelSize / 2), 799);
            y = Math.Min(j + (pixelSize / 2), 599);
            b = totalMatrix.M31 * x + totalMatrix.M32 * y + totalMatrix.M13 * x + totalMatrix.M23 * y + totalMatrix.OffsetZ +
                totalMatrix.M34;
            double c = x * (totalMatrix.M11 * x + y * totalMatrix.M21 + totalMatrix.OffsetX)
                       + y * (x * totalMatrix.M12 + y * totalMatrix.M22 + totalMatrix.OffsetY)
                       + x * totalMatrix.M14 + y * totalMatrix.M24 + totalMatrix.M44;
            double fourAc = 4 * totalMatrix.M33 * c;
            double delta = b * b - fourAc;
            return delta;
        }
        #endregion Private Methods
    }
}
