using System;
using NUnit.Framework;

namespace AMEEClient.Tests.MaterialMapper
{
    [TestFixture]
    public class MaterialMapperTests
    {
        private Client _ameeClient;
        public const string AmeeUrl = "https://stage.amee.com";
        public const string AmeeUserName = "AMEE_in_Revit";
        public const string AmeePassword = "ghmuasqx";

        [SetUp]
        public void SetUp()
        {
            _ameeClient = new Client(new Uri(AmeeUrl), AmeeUserName, AmeePassword);
        }

        [Test]
        public void LoadsMaterialsFromJsonMappingFile()
        {
            var mapper = new AMEEClient.MaterialMapper.MaterialMapper(@"MaterialMapper\SampleMaterialMap.xml", _ameeClient);

            Assert.AreEqual("1:Copper", mapper.GetMaterialDataItem("1:Copper").MaterialName);
        }

        [Test, Category("CallsAMEEConnect")]
        public void LoadsMaterialDetailsFromAmeeConnect()
        {
            var mapper = new AMEEClient.MaterialMapper.MaterialMapper(@"MaterialMapper\SampleMaterialMap.xml", _ameeClient);

            Assert.AreEqual("7C397Y3KCPFW", mapper.GetMaterialDataItem("1:Copper").UID);
        }

        [Test, Category("CallsAMEEConnect")]
        public void CanCalculateMaterialCO2eByMass()
        {
            var mapper = new AMEEClient.MaterialMapper.MaterialMapper(@"MaterialMapper\SampleMaterialMap.xml", _ameeClient);

            Assert.AreEqual(271, mapper.GetMaterialDataItem("1:Copper").CalculateCO2eByMass(100));
        }

        [Test, Category("CallsAMEEConnect")]
        public void CanCalculateMaterialCO2eByVolume()
        {
            var mapper = new AMEEClient.MaterialMapper.MaterialMapper(@"MaterialMapper\SampleMaterialMap.xml", _ameeClient);

            var material = mapper.GetMaterialDataItem("1:Copper");
            var co2eFor1Kg = material.CalculateCO2eByMass(1);
            Assert.AreEqual(co2eFor1Kg*material.Density, material.CalculateCO2eByVolume(1));
        }

        [Test, Explicit("Should only run this manually"), Category("CallsAMEEConnect")]
        public void TimeMultipleCalculationCalls()
        {
            var mapper = new AMEEClient.MaterialMapper.MaterialMapper(@"MaterialMapper\SampleMaterialMap.xml", _ameeClient);

            for (var i = 0; i < 2600; i++)
            {
                Console.WriteLine("1:Copper x {0}kg = {1}CO2e", i, 
                                  mapper.GetMaterialDataItem("1:Copper").CalculateCO2eByMass(i));
            }

            Assert.True(true);
        }
    }
}
