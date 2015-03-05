using System.Collections.Generic;
using System.Windows;

namespace RayTracer.Model.Shapes
{
    public class CustomLine
    {
        /// <summary>
        /// Gets or sets the line.
        /// </summary>
        /// <value>
        /// The line.
        /// </value>
        //public Line Line { get; set; }
        public List<Point> Points { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this line is visible.
        /// </summary>
        /// <value>
        /// <c>true</c> if this line is visible; otherwise, <c>false</c>.
        /// </value>
        public bool IsLineVisible { get; set; }
    }
}
