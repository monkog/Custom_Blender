﻿using System.Drawing;
using System.Windows;
using System.Windows.Media.Media3D;
using RayTracer.Helpers;

namespace RayTracer.ViewModel
{
    public class SceneManager : ViewModelBase
    {
        #region Private Members
        /// <summary>
        /// Is the view stereoscopic
        /// </summary>
        private bool _isStereoscopic;
        /// <summary>
        /// The scene image
        /// </summary>
        private Bitmap _sceneImage;
        /// <summary>
        /// The light intensity
        /// </summary>
        private double _m;
        /// <summary>
        /// Instance of the SceneManager
        /// </summary>
        private static SceneManager _instance;
        /// <summary>
        /// Scale matrix
        /// </summary>
        private readonly Matrix3D _scaleMatrix = new Matrix3D(50, 0, 0, 0
                                                            , 0, 50, 0, 0
                                                            , 0, 0, 50, 0
                                                            , 0, 0, 0, 1);
        /// <summary>
        /// The transform matrix
        /// </summary>
        private readonly Matrix3D _transformMatrix = new Matrix3D(1, 0, 0, 400
                                                                , 0, 1, 0, 300
                                                                , 0, 0, 1, 0
                                                                , 0, 0, 0, 1);
#warning change matrices to be dynamic
        #endregion Private Members
        #region Public Properties
        /// <summary>
        /// Instance of the SceneManager
        /// </summary>
        public static SceneManager Instance
        {
            get { return _instance ?? (_instance = new SceneManager()); }
        }
        /// <summary>
        /// Scale matrix
        /// </summary>
        public Matrix3D ScaleMatrix
        {
            get { return _scaleMatrix; }
        }
        /// <summary>
        /// Gets the transform matrix.
        /// </summary>
        public Matrix3D TransformMatrix
        {
            get { return _transformMatrix; }
        }
        /// <summary>
        /// The total transform matrix (transform * scale)
        /// </summary>
        public Matrix3D TotalMatrix { get { return TransformMatrix * ScaleMatrix; } }
        /// <summary>
        /// Gets or sets the scene image.
        /// </summary>
        public Bitmap SceneImage
        {
            get { return _sceneImage; }
            set
            {
                _sceneImage = value;
                OnPropertyChanged("SceneImage");
            }
        }
        /// <summary>
        /// Gets or sets the intensity of the light.
        /// </summary>
        public double M
        {
            get { return _m; }
            set
            {
                if (_m == value) return;
                _m = value;
                OnPropertyChanged("M");
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether the view is stereoscopic.
        /// </summary>
        public bool IsStereoscopic
        {
            get { return _isStereoscopic; }
            set
            {
                if (_isStereoscopic == value)
                    return;
                _isStereoscopic = value;
                OnPropertyChanged("IsStereoscopic");
            }
        }
        public static int Width = 800;
        public static int Height = 600;
        #endregion Public Properties
        #region .ctor
        private SceneManager()
        {
            SceneImage = new Bitmap(Width, Height);
        }
        #endregion .ctor
        #region Public Methods        
        /// <summary>
        /// Draws the curve point on the bitmap.
        /// </summary>
        /// <param name="bmp">The bitmap.</param>
        /// <param name="g">The g.</param>
        /// <param name="point">The point.</param>
        /// <param name="thickness">The thickness of the point.</param>
        public static void DrawCurvePoint(Bitmap bmp, Graphics g, Vector4 point, int thickness)
        {
            if (Instance.IsStereoscopic)
            {
                Color color;
                var p = Instance.TransformMatrix * Instance.ScaleMatrix *
                        Transformations.StereographicLeftViewMatrix(20, 400) * point;
                if (double.IsNaN(p.X) || double.IsNaN(p.Y) || !(p.X < 0 || p.X >= bmp.Width || p.Y < 0 || p.Y >= bmp.Height))
                {
                    color = bmp.GetPixel((int)p.X, (int)p.Y);
                    g.FillRectangle(new SolidBrush(color.CombinedColor(Color.Red)), (int)p.X, (int)p.Y, thickness, thickness);
                }

                p = Instance.TransformMatrix * Instance.ScaleMatrix *
                    Transformations.StereographicRightViewMatrix(20, 400) * point;
                if (double.IsNaN(p.X) || double.IsNaN(p.Y) || p.X < 0 || p.X >= bmp.Width || p.Y < 0 || p.Y >= bmp.Height)
                    return;
                color = bmp.GetPixel((int)p.X, (int)p.Y);
                g.FillRectangle(new SolidBrush(color.CombinedColor(Color.Blue)), (int)p.X, (int)p.Y, thickness, thickness);
            }
            else
            {
                var p = Instance.TransformMatrix * Instance.ScaleMatrix * Transformations.ViewMatrix(400) *
                        point;
                if (double.IsNaN(p.X) || double.IsNaN(p.Y) || p.X < 0 || p.X >= bmp.Width || p.Y < 0 || p.Y >= bmp.Height)
                    return;
                Color color = bmp.GetPixel((int)p.X, (int)p.Y);
                g.FillRectangle(new SolidBrush(color.CombinedColor(Color.DarkCyan)), (int)p.X, (int)p.Y, thickness, thickness);
            }
        }
        #endregion Public Methods
    }
}
