using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using AMEEClient.Model;
using Newtonsoft.Json;

namespace AMEEClient.MaterialMapper
{
    public class MaterialMapper
    {
        private readonly string _mapFilePath;
        private Dictionary<string, DataItem> _materialMap;

        public MaterialMapper(string mapFilePath)
        {
            _mapFilePath = mapFilePath;
        }

        public DataItem GetItemForMaterial(string materialName)
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
                _materialMap = new Dictionary<string, DataItem>();
                
                var doc = new XmlDocument();
                var reader = new XmlTextReader(_mapFilePath);
                doc.Load(reader);
                var materials = doc.SelectNodes("//Material");
                foreach (XmlNode material in materials)
                {
                    var materialName = material.SelectNodes("MaterialName").Item(0).InnerText;
                    var item = new DataItem(); //material.SelectNodes("AMEE/Path").Item(0).InnerText
                    XmlNodeList drills = material.SelectNodes("AMEE/Drills/Drill");
                    foreach (XmlNode drill in drills)
                    {
                        // item.AddDrill(drill.Attributes["name"].Value, drill.InnerText);
                    }
                    _materialMap.Add(materialName, item);
                }
                   
            }
        }
    }
}
