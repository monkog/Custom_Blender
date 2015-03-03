using System;
using System.Windows.Media.Media3D;
using FakeItEasy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RayTracer.ViewModel;
using PerspectiveCamera = RayTracer.Model.Camera.PerspectiveCamera;

namespace UnitTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void CreateVersor()
        {
            Vector3D v0 = Transformations.Versor(new Vector3D(0, 0, 0));
            Vector3D v1 = Transformations.Versor(new Vector3D(1, 0, 0));
            Vector3D vCustom = Transformations.Versor(new Vector3D(4, -3, 0));

            Assert.AreEqual(new Vector3D(0, 0, 0), v0);
            Assert.AreEqual(new Vector3D(1, 0, 0), v1);
            Assert.AreEqual(new Vector3D(4 / 5f, -3 / 5f, 0), vCustom);
        }

        [TestMethod]
        public void CreateViewMatrix()
        {
            var camera = A.Fake<PerspectiveCamera>();
            camera.UpVector = new Vector3D(0, 0, 1);
            camera.CameraTarget = new Vector3D(8, 0.5, 0.5);
            camera.CameraPosition = new Vector3D(4, 0.5, 0.5);
            var viewMatrix = Transformations.ViewMatrix(camera);

            var matrix = new Matrix3D(0, 0, 1, -0.5
                                    , 0, 1, 0, -0.5
                                    , -1, 0, 0, 4
                                    , -0, 0, 0, 1);

            Assert.AreEqual(matrix, viewMatrix);
        }

        [TestMethod]
        public void CreateProjectionMatrix()
        {
            var projectionMatrix = Transformations.ProjectionMatrix(45, 1, 100, 1);

            var matrix = new Matrix3D(2.414213562, 0, 0, 0
                                    , 0, 2.414213562, 0, 0
                                    , 0, 0, -1.02020202, -2.02020202
                                    , -0, 0, -1, 0);

            Assert.AreEqual(Math.Round(matrix.M11, 2), Math.Round(projectionMatrix.M11, 2));
            Assert.AreEqual(Math.Round(matrix.M12, 2), Math.Round(projectionMatrix.M12, 2));
            Assert.AreEqual(Math.Round(matrix.M13, 2), Math.Round(projectionMatrix.M13, 2));
            Assert.AreEqual(Math.Round(matrix.M14, 2), Math.Round(projectionMatrix.M14, 2));
            Assert.AreEqual(Math.Round(matrix.M21, 2), Math.Round(projectionMatrix.M21, 2));
            Assert.AreEqual(Math.Round(matrix.M22, 2), Math.Round(projectionMatrix.M22, 2));
            Assert.AreEqual(Math.Round(matrix.M23, 2), Math.Round(projectionMatrix.M23, 2));
            Assert.AreEqual(Math.Round(matrix.M24, 2), Math.Round(projectionMatrix.M24, 2));
            Assert.AreEqual(Math.Round(matrix.M31, 2), Math.Round(projectionMatrix.M31, 2));
            Assert.AreEqual(Math.Round(matrix.M32, 2), Math.Round(projectionMatrix.M32, 2));
            Assert.AreEqual(Math.Round(matrix.M33, 2), Math.Round(projectionMatrix.M33, 2));
            Assert.AreEqual(Math.Round(matrix.M34, 2), Math.Round(projectionMatrix.M34, 2));
            Assert.AreEqual(Math.Round(matrix.OffsetX, 2), Math.Round(projectionMatrix.OffsetX, 2));
            Assert.AreEqual(Math.Round(matrix.OffsetY, 2), Math.Round(projectionMatrix.OffsetY, 2));
            Assert.AreEqual(Math.Round(matrix.OffsetZ, 2), Math.Round(projectionMatrix.OffsetZ, 2));
            Assert.AreEqual(Math.Round(matrix.M44, 2), Math.Round(projectionMatrix.M44, 2));
        }
    }
}
