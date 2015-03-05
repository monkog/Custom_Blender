using System.Windows.Media.Media3D;

namespace RayTracer.Model.Shapes
{
    public class SceneManager
    {
        #region Private Members
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
        /// <value>
        /// The transform matrix.
        /// </value>
        public Matrix3D TransformMatrix
        {
            get { return _transformMatrix; }
        }
        #endregion Public Properties
    }
}
