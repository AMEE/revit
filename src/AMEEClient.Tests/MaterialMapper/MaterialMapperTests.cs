using System;
using System.Linq;
using AMEEClient.Model;
using NUnit.Framework;

namespace AMEEClient.Tests
{
    [TestFixture]
    public class MaterialMapperTests
    {
        [Test]
        public void LoadsMaterialsFromJsonMappingFile()
        {
            var mapper = new MaterialMapper.MaterialMapper(@"MaterialMapper\SampleMaterialMap.xml");

            Assert.AreEqual("AMEEDataItemForSampleMaterial", mapper.GetItemForMaterial("1:SampleMaterial").Name);
        }

    }
}
