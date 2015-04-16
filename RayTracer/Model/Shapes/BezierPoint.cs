namespace RayTracer.Model.Shapes
{
    public class BezierPoint : PointEx
    {
        #region Public Properties
        /// <summary>
        /// Gets the left parent.
        /// </summary>
        public PointEx LeftParent { get; private set; }
        /// <summary>
        /// Gets the right parent.
        /// </summary>
        public PointEx RightParent { get; private set; }
        /// <summary>
        /// Gets a value indicating whether this point was created in the second division.
        /// </summary>
        public BezierPointType BezierPointType { get; private set; }
        #endregion Public Properties
        #region Constructors
        public BezierPoint(double x, double y, double z, PointEx leftParent, PointEx rightParent, BezierPointType bezierPointType)
            : base(x, y, z)
        {
            LeftParent = leftParent;
            RightParent = rightParent;
            BezierPointType = bezierPointType;
        }
        #endregion Constructors
    }
    /// <summary>
    /// Determines whether the point is in 1/3, 1/2 or 2/3 position
    /// </summary>
    public enum BezierPointType
    {
        First, Second, Third
    }
}

