using System.Windows.Media.Media3D;

namespace RayTracer.Helpers
{
    /// <summary>
    /// Helper class determining the begginning and end of a line
    /// </summary>
    public class SimpleLine
    {
        /// <summary>
        /// Gets or sets begginning point of the line.
        /// </summary>
        /// <value>
        /// Begginning point of the line.
        /// </value>
        public Point3D From { get; set; }
        /// <summary>
        /// Gets or sets destination point of the line.
        /// </summary>
        /// <value>
        /// Destination point of the line.
        /// </value>
        public Point3D To { get; set; }
    }
}
