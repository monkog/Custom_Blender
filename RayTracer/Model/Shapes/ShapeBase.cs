using System.Collections.ObjectModel;
using System.Windows.Shapes;
using RayTracer.Helpers;

namespace RayTracer.Model.Shapes
{
    /// <summary>
    /// Base class for shape objects
    /// </summary>
    public abstract class ShapeBase
    {
        #region .ctor
        /// <summary>
        /// Initializes a new instance of the <see cref="Cube"/> class.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="z">The z.</param>
        protected ShapeBase(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;

            Vertices = new ObservableCollection<Vector4>();
        }
        #endregion .ctor
        #region Public Properties
        /// <summary>
        /// Gets or sets the x position.
        /// </summary>
        /// <value>
        /// The x position.
        /// </value>
        public double X { get; set; }
        /// <summary>
        /// Gets or sets the y position.
        /// </summary>
        /// <value>
        /// The y position.
        /// </value>
        public double Y { get; set; }
        /// <summary>
        /// Gets or sets the z position.
        /// </summary>
        /// <value>
        /// The z position.
        /// </value>
        public double Z { get; set; }
        /// <summary>
        /// Gets or sets the vertices representing the mesh.
        /// </summary>
        /// <value>
        /// The vertices representing the mesh.
        /// </value>
        public ObservableCollection<Vector4> Vertices { get; set; }
        /// <summary>
        /// Gets or sets the edges representing the mesh.
        /// </summary>
        /// <value>
        /// The edges representing the mesh.
        /// </value>
        public Polyline Edges { get; set; }
        #endregion Public Properties
    }
}
