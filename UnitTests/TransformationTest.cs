using System.Windows.Media.Media3D;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RayTracer.Helpers;
using RayTracer.ViewModel;

namespace UnitTests
{
    [TestClass]
    public class TransformationTest
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
        public void CreateDotProduct()
        {
            Vector4 v0 = new Vector4(0, 0, 0, 0);
            Vector4 v1 = new Vector4(1, 1, 1, 1);
            Vector4 vRand1 = new Vector4(2, 3, 4, 1);
            Vector4 vRand2 = new Vector4(5, 1, 7, 3);

            Assert.AreEqual(0, v0.Dot(v1));
            Assert.AreEqual(10, v1.Dot(vRand1));
            Assert.AreEqual(vRand1.Dot(vRand2), vRand2.Dot(vRand1));
        }
    }
}
