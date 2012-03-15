using System;
using System.Collections.Generic;
using System.Xml;

namespace AMEEClient.MaterialMapper
{
    public class MaterialMapper
    {
        private readonly string _mapFilePath;
        private readonly Client _ameeClient;
        private Dictionary<string, MaterialDataItem> _materialMap;

        public MaterialMapper(string mapFilePath, Client ameeClient)
        {
            _mapFilePath = mapFilePath;
            _ameeClient = ameeClient;
        }

        public MaterialDataItem GetMaterialDataItem(string materialName)
        {
            EnsureMaterialMapLoaded();
            if (!_materialMap.ContainsKey(materialName))
            {
                throw new UnknownMaterialException("Unknown material: " + materialName);
            }
            return _materialMap[materialName];
        }

     

        private void EnsureMaterialMapLoaded()
        {
            if (_materialMap == null)
            {
                _materialMap = new Dictionary<string, MaterialDataItem>();
                
                var doc = new XmlDocument();
                var reader = new XmlTextReader(_mapFilePath);
                doc.Load(reader);
                var materials = doc.SelectNodes("//Material");
                foreach (XmlNode material in materials)
                {
                    var materialName = material.SelectNodes("MaterialName").Item(0).InnerText;
                    var densityKgPerM3 = Convert.ToDouble(material.SelectNodes("AMEE/Density").Item(0).InnerText);
                    var materialDrillDown = new List<List<string>>();
                    var drills = material.SelectNodes("AMEE/Drills/Drill");
                    foreach (XmlNode drill in drills)
                    {
                        materialDrillDown.Add(new List<string> {drill.Attributes["name"].Value, drill.InnerText});
                    }
                    var path = material.SelectNodes("AMEE/Path").Item(0).InnerText;
                    var materialDataItem = new MaterialDataItem(_ameeClient, materialName, densityKgPerM3, path, materialDrillDown);

                    _materialMap.Add(materialName, materialDataItem);
                }
                   
            }
        }
    }
}
