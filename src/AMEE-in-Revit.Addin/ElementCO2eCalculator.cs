using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using AMEEClient;
using AMEEClient.MaterialMapper;
using Autodesk.Revit.DB;

namespace AMEE_in_Revit.Addin
{
    public class ElementCO2eCalculator
    {
        private MaterialMapper _materialMapper;

        public ElementCO2eCalculator(MaterialMapper materialMapper)
        {
            _materialMapper = materialMapper;
        }

       

        public void UpdateElementCO2eParameters(ICollection<Element> forElements)
        {
            foreach (var element in forElements)
            {
                if (!element.ParametersMap.Contains("CO2e")) continue;

                double totalElementCO2e = 0;
                var materialSetIterator = element.Materials.ForwardIterator();
                while (materialSetIterator.MoveNext())
                {
                    var material = (Material)materialSetIterator.Current;
                 
                    var volumeInM3 = element.GetMaterialVolume(material);
                    var materialKey = material.Id + ":" + material.Name;

                    var ameeMaterial = _materialMapper.GetMaterialDataItem(materialKey);
                    totalElementCO2e += ameeMaterial.CalculateCO2eByVolume(volumeInM3);
                }
                element.ParametersMap.get_Item("CO2e").Set(totalElementCO2e);
            }
        }
    }
}
