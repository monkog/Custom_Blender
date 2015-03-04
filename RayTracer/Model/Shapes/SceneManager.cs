using System.Windows.Media.Media3D;

namespace RayTracer.Model.Shapes
{
    public class SceneManager
    {
        #region Private Members
        /// <summary>
        /// Instance of the SceneManager
        /// </summary>
        private SceneManager _instance;
        /// <summary>
        /// Scale matrix
        /// </summary>
        private readonly Matrix3D _scaleMatrix = new Matrix3D(100, 0, 0, 0
                                                            , 0, 100, 0, 0
                                                            , 0, 0, 100, 0
                                                            , 0, 0, 0, 1);
        #endregion Private Members
        #region Public Properties
        /// <summary>
        /// Instance of the SceneManager
        /// </summary>
        public SceneManager Instance
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
        #endregion Public Properties
        #region .ctor

        public SceneManager()
        {

            Matrix3D _transformMatrix = new Matrix3D(100, 0, 0, 0
                                                   , 0, 100, 0, 0
                                                   , 0, 0, 100, 0
                                                   , 0, 0, 0, 1);
        }
        #endregion .ctor
    }
}
