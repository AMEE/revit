using System;
using NUnit.Framework;

namespace AMEEClient.Tests
{
    [TestFixture]
    public class MaterialMapperTests
    {
        public const string AmeeUrl = "https://stage.amee.com";
        public const string AmeeUserName = "AMEE_in_Revit";
        public const string AmeePassword = "ghmuasqx";

        [Test]
        public void LoadsMaterialsFromJsonMappingFile()
        {
            var ameeClient = new Client(new Uri(AmeeUrl), AmeeUserName, AmeePassword);

            var mapper = new MaterialMapper.MaterialMapper(@"MaterialMapper\SampleMaterialMap.xml", ameeClient);

            Assert.AreEqual("1:Copper", mapper.GetMaterialDataItem("1:Copper").MaterialName);
        }

        [Test, Category("CallsAMEEConnect")]
        public void LoadsMaterialDetailsFromAmeeConnect()
        {
            var ameeClient = new Client(new Uri(AmeeUrl), AmeeUserName, AmeePassword);

            var mapper = new MaterialMapper.MaterialMapper(@"MaterialMapper\SampleMaterialMap.xml", ameeClient);

            Assert.AreEqual("7C397Y3KCPFW", mapper.GetMaterialDataItem("1:Copper").UID);
        }
    }
}
