﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Media.Media3D;
using RayTracer.Helpers;
using RayTracer.Model.Shapes;

namespace RayTracer.ViewModel
{
    public class SceneManager : ViewModelBase
    {
        #region Private Members
        private int _currentModelId = 1;
        private bool _isStereoscopic;
        private Bitmap _sceneImage;
        private Bitmap _firstSurfaceImage;
        private Bitmap _secondSurfaceImage;
        private double _m;
        private static SceneManager _instance;
        private readonly Matrix3D _scaleMatrix = new Matrix3D(50, 0, 0, 0
                                                            , 0, 50, 0, 0
                                                            , 0, 0, 50, 0
                                                            , 0, 0, 0, 1);
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
        /// Gets or sets the image of the scene from the first surface perspective.
        /// </summary>
        /// <remarks>Shows the trimming curves</remarks>
        public Bitmap FirstSurfaceImage
        {
            get { return _firstSurfaceImage; }
            set
            {
                _firstSurfaceImage = value;
                OnPropertyChanged("FirstSurfaceImage");
            }
        }
        /// <summary>
        /// Gets or sets the image of the scene from the second surface perspective.
        /// </summary>
        /// <remarks>Shows the trimming curves</remarks>
        public Bitmap SecondSurfaceImage
        {
            get { return _secondSurfaceImage; }
            set
            {
                _secondSurfaceImage = value;
                OnPropertyChanged("SecondSurfaceImage");
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
        /// Gets all the models in the scene.
        /// </summary>
        public IEnumerable<ModelBase> Models
        {
            get
            {
                IEnumerable<ModelBase> models = PatchManager.Instance.Patches;
                models = models.Union(CurveManager.Instance.Curves);
                models = models.Union(MeshManager.Instance.Meshes);
                return models;
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
        /// <summary>
        /// Degree of bezier segment
        /// </summary>
        public const int BezierSegmentPoints = 3;
        public static int Width = 800;
        public static int Height = 600;
        public static double Epsilon = 1.0e-7;
        #endregion Public Properties
        #region .ctor
        private SceneManager()
        {
            SceneImage = new Bitmap(Width, Height);
            FirstSurfaceImage = new Bitmap(Width, Height);
            SecondSurfaceImage = new Bitmap(Width, Height);
        }
        #endregion .ctor
        #region Public Methods
        /// <summary>
        /// Draws the point on the bitmap.
        /// </summary>
        /// <param name="bmp">The bitmap.</param>
        /// <param name="g">The g.</param>
        /// <param name="point">The point.</param>
        /// <param name="thickness">The thickness of the point.</param>
        public static void DrawPoint(Bitmap bmp, Graphics g, Vector4 point, int thickness, Color color)
        {
            if (Instance.IsStereoscopic)
            {
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
                Color c = bmp.GetPixel((int)p.X, (int)p.Y);
                g.FillRectangle(new SolidBrush(color), (int)p.X, (int)p.Y, thickness, thickness);
            }
        }
        /// <summary>
        /// Draws the line on the bitmap.
        /// </summary>
        /// <param name="bmp">The bitmap.</param>
        /// <param name="g">The g.</param>
        /// <param name="point1">The start point.</param>
        /// <param name="point2">The end point.</param>
        /// <param name="thickness">The thickness of the point.</param>
        public static void DrawLine(Bitmap bmp, Graphics g, Vector4 point1, Vector4 point2, int thickness, Color color)
        {
            if (Instance.IsStereoscopic)
            {
                var p1 = Instance.TransformMatrix * Instance.ScaleMatrix *
                        Transformations.StereographicLeftViewMatrix(20, 400) * point1;
                var p2 = Instance.TransformMatrix * Instance.ScaleMatrix *
                        Transformations.StereographicLeftViewMatrix(20, 400) * point2;
                if (double.IsNaN(p1.X) || double.IsNaN(p1.Y) || (p1.X < 0 || p1.X >= bmp.Width || p1.Y < 0 || p1.Y >= bmp.Height)
                    || double.IsNaN(p2.X) || double.IsNaN(p2.Y) || (p2.X < 0 || p2.X >= bmp.Width || p2.Y < 0 || p2.Y >= bmp.Height))
                    return;
                color = bmp.GetPixel((int)p1.X, (int)p1.Y);
                g.DrawLine(new Pen(new SolidBrush(color.CombinedColor(Color.Red))), (float)p1.X, (float)p1.Y, (float)p2.X, (float)p2.Y);

                p1 = Instance.TransformMatrix * Instance.ScaleMatrix *
                    Transformations.StereographicRightViewMatrix(20, 400) * point1;
                p2 = Instance.TransformMatrix * Instance.ScaleMatrix *
                        Transformations.StereographicRightViewMatrix(20, 400) * point2;
                if (double.IsNaN(p1.X) || double.IsNaN(p1.Y) || (p1.X < 0 || p1.X >= bmp.Width || p1.Y < 0 || p1.Y >= bmp.Height)
                    || double.IsNaN(p2.X) || double.IsNaN(p2.Y) || (p2.X < 0 || p2.X >= bmp.Width || p2.Y < 0 || p2.Y >= bmp.Height))
                    return;
                color = bmp.GetPixel((int)p1.X, (int)p1.Y);
                g.DrawLine(new Pen(new SolidBrush(color.CombinedColor(Color.Red))), (float)p1.X, (float)p1.Y, (float)p2.X, (float)p2.Y);
            }
            else
            {
                var p1 = Instance.TransformMatrix * Instance.ScaleMatrix *
                        Transformations.ViewMatrix(400) * point1;
                var p2 = Instance.TransformMatrix * Instance.ScaleMatrix *
                        Transformations.ViewMatrix(400) * point2;
                if (double.IsNaN(p1.X) || double.IsNaN(p1.Y) || (p1.X < 0 || p1.X >= bmp.Width || p1.Y < 0 || p1.Y >= bmp.Height)
                    || double.IsNaN(p2.X) || double.IsNaN(p2.Y) || (p2.X < 0 || p2.X >= bmp.Width || p2.Y < 0 || p2.Y >= bmp.Height))
                    return;
                var c = bmp.GetPixel((int)p1.X, (int)p1.Y);
                g.DrawLine(new Pen(new SolidBrush(color/*.CombinedColor(c)*/)), (float)p1.X, (float)p1.Y, (float)p2.X, (float)p2.Y);
            }
        }
        /// <summary>
        /// Gets the next identifier.
        /// </summary>
        /// <returns>Next Id</returns>
        public int GetNextId()
        {
            return _currentModelId++;
        }
        /// <summary>
        /// Saves the current scene.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        public void SaveScene(string fileName)
        {
            var stringBuilder = new StringBuilder();

            foreach (var point in PointManager.Instance.Points)
                point.Save(stringBuilder);
            foreach (var model in Models)
                model.Save(stringBuilder);

            stringBuilder.AppendLine("Selected");
            foreach (var model in Models)
            {
                if (model.IsSelected)
                    stringBuilder.AppendLine(model.Id.ToString(CultureInfo.InvariantCulture));
                foreach (var item in model.SelectedItems)
                    stringBuilder.AppendLine(item.Id.ToString(CultureInfo.InvariantCulture));
            }

            using (var streamWriter = new StreamWriter(fileName))
                streamWriter.Write(stringBuilder.ToString());
        }
        /// <summary>
        /// Loads the scene from file.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        public void LoadScene(string fileName)
        {
            using (var streamReader = new StreamReader(fileName))
            {
                string line;
                while ((line = streamReader.ReadLine()) != null)
                {
                    switch (line)
                    {
                        case "Torus":
                            LoadTorus(streamReader);
                            break;
                        case "Point":
                            PointManager.Instance.Points.Add(LoadPoint(streamReader));
                            break;
                        //case "Ellipsoid":
                        //    LoadElipsoide(streamReader);
                        //    break;
                        case "BezierCurveC0":
                            LoadCurve(streamReader, Continuity.C0);
                            break;
                        case "BezierCurveC2":
                            LoadCurve(streamReader, Continuity.C2);
                            break;
                        case "InterpolationCurve":
                            LoadCurve(streamReader, Continuity.C2, isInterpolation: true);
                            break;
                        case "BezierSurfaceC0":
                            LoadSurface(streamReader, Continuity.C0);
                            break;
                        case "BezierSurfaceC2":
                            LoadSurface(streamReader, Continuity.C2);
                            break;
                        case "Selected":
                            SelectObjects(streamReader);
                            break;

                    }
                }
            }
        }
        /// <summary>
        /// Removes all objects from the scene.
        /// </summary>
        public void RemoveAllObjects()
        {
            PointManager.Instance.Points.Clear();
            MeshManager.Instance.Meshes.Clear();
            PatchManager.Instance.Patches.Clear();
            PatchManager.Instance.GregoryPatches.Clear();
            CurveManager.Instance.Curves.Clear();
            CurveManager.Instance.TrimmingCurves.Clear();
        }
        /// <summary>
        /// Draws the normal vector of the surface in the given point.
        /// </summary>
        /// <param name="graphics">The graphics.</param>
        /// <param name="points">The points.</param>
        public static void DrawNormalVectors(Graphics graphics, Vector4[,] points)
        {
            // Point [1, 1]
            var du = (((points[1, 0] - points[0, 0]) * 3).Cross((points[0, 1] - points[0, 0]) * 3)).Normalized;
            DrawNormal(graphics, du, points[1, 1]);

            // Point [1, 2]
            du = (((points[0, 2] - points[0, 3]) * 3).Cross((points[1, 3] - points[0, 3]) * 3)).Normalized;
            DrawNormal(graphics, du, points[1, 2]);

            // Point [3, 3]
            du = (((points[2, 3] - points[3, 3]) * 3).Cross((points[3, 2] - points[3, 3]) * 3)).Normalized;
            DrawNormal(graphics, du, points[2, 2]);

            // Point [2, 1]
            du = (((points[3, 1] - points[3, 0]) * 3).Cross((points[2, 0] - points[3, 0]) * 3)).Normalized;
            DrawNormal(graphics, du, points[2, 1]);
        }
        #endregion Public Methods
        #region Private Methods
        private void LoadTorus(StreamReader streamReader)
        {
            var id = ReadInt(streamReader.ReadLine());
            var name = ReadString(streamReader.ReadLine());
            var smallR = ReadDouble(streamReader.ReadLine());
            var bigR = ReadDouble(streamReader.ReadLine());
            var x = ReadDouble(streamReader.ReadLine());
            var y = ReadDouble(streamReader.ReadLine());
            var z = ReadDouble(streamReader.ReadLine());
            var matrix = ReadMatrix(streamReader);
            var colorName = ReadString(streamReader.ReadLine());
            var color = Color.FromName(colorName);
            var torus = new Torus(x, y, z, name, smallR, bigR, MeshManager.Instance.L, MeshManager.Instance.V)
            {
                Id = id,
                Color = color,
                ModelTransform = matrix
            };
            MeshManager.Instance.Meshes.Add(torus);
            streamReader.ReadLine();
        }
        private PointEx LoadPoint(StreamReader streamReader)
        {
            var id = ReadInt(streamReader.ReadLine());
            var name = ReadString(streamReader.ReadLine());
            var x = ReadDouble(streamReader.ReadLine());
            var y = ReadDouble(streamReader.ReadLine());
            var z = ReadDouble(streamReader.ReadLine());
            var matrix = ReadMatrix(streamReader);
            var colorName = ReadString(streamReader.ReadLine());
            var color = Color.FromName(colorName);
            var point = new PointEx(x, y, z)
            {
                Id = id,
                Name = name,
                Color = color,
                ModelTransform = matrix
            };
            streamReader.ReadLine();
            return point;
        }
        private void LoadElipsoide(StreamReader streamReader)
        {
            var id = ReadInt(streamReader.ReadLine());
            var name = ReadString(streamReader.ReadLine());
            var x = ReadDouble(streamReader.ReadLine());
            var y = ReadDouble(streamReader.ReadLine());
            var z = ReadDouble(streamReader.ReadLine());
            var tMatrix = ReadMatrix(streamReader);
            var mMatrix = ReadMatrix(streamReader);
            var colorName = ReadString(streamReader.ReadLine());
            var color = Color.FromName(colorName);
            var ellipsoide = new Ellipsoide(x, y, z, name, 1, 1, 1)
            {
                Id = id,
                Color = color,
                Transform = tMatrix,
                ModelTransform = mMatrix
            };
            streamReader.ReadLine();
        }
        private void LoadCurve(StreamReader streamReader, Continuity continuity, bool isInterpolation = false)
        {
            var id = ReadInt(streamReader.ReadLine());
            var name = ReadString(streamReader.ReadLine());
            var x = ReadDouble(streamReader.ReadLine());
            var y = ReadDouble(streamReader.ReadLine());
            var z = ReadDouble(streamReader.ReadLine());
            var matrix = ReadMatrix(streamReader);
            var colorName = ReadString(streamReader.ReadLine());
            var color = Color.FromName(colorName);
            var points = ReadPoints(streamReader, PointManager.Instance.Points);
            BezierCurve curve = null;
            switch (continuity)
            {
                case Continuity.C0:
                    curve = new BezierCurveC0(x, y, z, name, points)
                    {
                        Id = id,
                        Color = color,
                        ModelTransform = matrix
                    };
                    break;
                case Continuity.C2:
                    curve = new BezierCurveC2(x, y, z, name, points, isInterpolation)
                    {
                        Id = id,
                        Color = color,
                        ModelTransform = matrix
                    };
                    break;
            }
            CurveManager.Instance.Curves.Add(curve);
        }
        private void LoadSurface(StreamReader streamReader, Continuity continuity)
        {
            var id = ReadInt(streamReader.ReadLine());
            var name = ReadString(streamReader.ReadLine());
            var width = ReadDouble(streamReader.ReadLine());
            var height = ReadDouble(streamReader.ReadLine());
            var horizontalPatches = ReadInt(streamReader.ReadLine());
            var verticalPatches = ReadInt(streamReader.ReadLine());
            var isCylindrical = ReadBool(streamReader.ReadLine());
            var x = ReadDouble(streamReader.ReadLine());
            var y = ReadDouble(streamReader.ReadLine());
            var z = ReadDouble(streamReader.ReadLine());
            var matrix = ReadMatrix(streamReader);
            var colorName = ReadString(streamReader.ReadLine());
            var color = Color.FromName(colorName);
            var points = ReadPoints(streamReader, PointManager.Instance.Points);
            BezierPatch patch = null;

            int verticalPoints = continuity == Continuity.C0 ? verticalPatches * BezierSegmentPoints + 1 : verticalPatches + BezierSegmentPoints;
            int horizontalPoints = isCylindrical ? (continuity == Continuity.C0 ? horizontalPatches * BezierSegmentPoints : horizontalPatches)
                : (continuity == Continuity.C0 ? horizontalPatches * BezierSegmentPoints + 1 : BezierSegmentPoints + horizontalPatches);
            var p = new PointEx[verticalPoints, horizontalPoints];
            int index = 0;
            for (int i = 0; i < p.GetLength(0); i++)
                for (int j = 0; j < p.GetLength(1); j++)
                {
                    var point = points.ElementAt(index++);
                    PointManager.Instance.Points.Remove(point);
                    p[i, j] = point;
                }

            switch (continuity)
            {
                case Continuity.C0:
                    patch = new BezierPatchC0(x, y, z, name, isCylindrical, width, height, verticalPatches, horizontalPatches, p, points)
                    {
                        Id = id,
                        Color = color,
                        ModelTransform = matrix
                    };
                    break;
                case Continuity.C2:
                    patch = new BezierPatchC2(x, y, z, name, isCylindrical, width, height, verticalPatches, horizontalPatches, p, points)
                    {
                        Id = id,
                        Color = color,
                        ModelTransform = matrix
                    };
                    break;
            }
            PatchManager.Instance.Patches.Add(patch);
        }
        private List<PointEx> ReadPoints(StreamReader streamReader, ObservableCollection<PointEx> pts)
        {
            var points = new List<PointEx>();
            while (true)
            {
                var line = streamReader.ReadLine();
                if (string.IsNullOrEmpty(line)) break;
                var id = ReadInt(line);
                points.Add(pts.First(p => p.Id == id));
            }
            return points;
        }
        private void SelectObjects(StreamReader streamReader)
        {
            while (true)
            {
                var line = streamReader.ReadLine();
                if (string.IsNullOrEmpty(line)) break;

                var id = ReadInt(line);

                foreach (var point in PointManager.Instance.Points.Where(p => p.Id == id)) point.IsSelected = true;
                foreach (var mesh in MeshManager.Instance.Meshes)
                {
                    if (mesh.Id == id)
                    {
                        mesh.IsSelected = true;
                        break;
                    }
                    foreach (var point in mesh.Vertices.Where(p => p.Id == id)) point.IsSelected = true;
                }
            }
        }
        private Matrix3D ReadMatrix(StreamReader streamReader)
        {
            streamReader.ReadLine();
            var data = streamReader.ReadLine() + "\r\n";
            data = data + streamReader.ReadLine() + "\r\n";
            data = data + streamReader.ReadLine() + "\r\n";
            data = data + streamReader.ReadLine() + "\r\n";
            data = data.Replace(",", ".");
            return Matrix3D.Parse(data);
        }
        private double ReadDouble(string line)
        {
            return double.Parse(line.Substring(line.IndexOf("=", StringComparison.Ordinal) + 1).Replace(',', '.'));
        }
        private string ReadString(string line)
        {
            return line.Substring(line.IndexOf("=", StringComparison.Ordinal) + 1);
        }
        private int ReadInt(string line)
        {
            return int.Parse(line.Substring(line.IndexOf("=", StringComparison.Ordinal) + 1));
        }
        private bool ReadBool(string line)
        {
            return bool.Parse(line.Substring(line.IndexOf("=", StringComparison.Ordinal) + 1));
        }
        private static void DrawNormal(Graphics graphics, Vector4 du, Vector4 point)
        {
            Vector4 endPoint = Instance.TransformMatrix * Instance.ScaleMatrix * Transformations.ViewMatrix(400) * new Vector4(du.X + point.X, du.Y + point.Y, du.Z + point.Z, 1);
            Vector4 startPoint = Instance.TransformMatrix * Instance.ScaleMatrix * Transformations.ViewMatrix(400) * point;

            graphics.DrawLine(new Pen(Color.White) { Width = 1 }, (int)startPoint.X, (int)startPoint.Y, (int)endPoint.X, (int)endPoint.Y);
        }
        #endregion Private Methods
    }
}
