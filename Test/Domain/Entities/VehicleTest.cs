using Microsoft.VisualStudio.TestTools.UnitTesting;
using MinimalApi.Domain.Entities;

namespace Test.Domain.Entities
{
    [TestClass]
    public class VehicleTest
    {
        [TestMethod]
        public void TestGetSetProperties()
        {
            var vehicle = new Vehicle();

            vehicle.Id = 1;
            vehicle.Name = "Civic";
            vehicle.Mark = "Honda";
            vehicle.Year = 1994;

            Assert.AreEqual(1, vehicle.Id);
            Assert.AreEqual("Civic", vehicle.Name);
            Assert.AreEqual("Honda", vehicle.Mark);
            Assert.AreEqual(1994, vehicle.Year);
        }
    }
}