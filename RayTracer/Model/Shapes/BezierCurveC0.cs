﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Media3D;
using RayTracer.Helpers;
using RayTracer.ViewModel;

namespace RayTracer.Model.Shapes
{
    public sealed class BezierCurveC0 : BezierCurve
    {
        #region Public Properties
        public override string Type { get { return "BezierCurveC0"; } }
        #endregion Public Properties
        #region Constructors
        public BezierCurveC0(double x, double y, double z, string name, IEnumerable<PointEx> points)
            : base(x, y, z, name, points, Continuity.C0)
        {
            SetEdges();
            TransformVertices(Matrix3D.Identity);
        }
        #endregion Constructors
        #region Protected Methods
        /// <summary>
        /// Gets the list of points creating curves
        /// </summary>
        /// <returns>The list of points creating curves</returns>
        protected override List<Tuple<List<Vector4>, double>> GetBezierCurves()
        {
            var curves = new List<Tuple<List<Vector4>, double>>();
            var curve = new List<Vector4>();
            double divisions = 0;
            int index = 0;
            for (int i = 0; i < Vertices.Count(); i++)
            {
                curve.Add(Transformations.TransformPoint(Vertices.ElementAt(i).Vector4, Vertices.ElementAt(i).MeshTransform * Vertices.ElementAt(i).ModelTransform).Normalized);
                index = (index + 1) % 4;

                if (i < Vertices.Count - 1)
                    divisions += (Vertices.ElementAt(i).PointOnScreen - Vertices.ElementAt(i + 1).PointOnScreen).Length;

                if (index == 0 && i < Vertices.Count - 1)
                {
                    i--;
                    curves.Add(new Tuple<List<Vector4>, double>(curve, 1.0 / divisions));
                    curve = new List<Vector4>();
                }
            }

            if (curve.Count > 0)
                curves.Add(new Tuple<List<Vector4>, double>(curve, 1.0 / divisions));

            return curves;
        }
        #endregion Protected Methods
        #region Public Methods
        public override void SaveControlPointsReference(StringBuilder stringBuilder)
        {
            foreach (var point in Vertices)
                stringBuilder.AppendLine("CP=" + point.Id);
        }
        #endregion Public Methods
    }
}

