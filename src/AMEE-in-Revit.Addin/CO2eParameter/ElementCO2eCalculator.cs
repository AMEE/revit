using System;
using System.Collections.Generic;
using System.Diagnostics;
using AMEEClient.MaterialMapper;
using Autodesk.Revit.DB;

namespace AMEE_in_Revit.Addin.CO2eParameter
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
                var materialKey = "undefined";
                try
                {
                    if (!element.ParametersMap.Contains("CO2e")) continue;

                    double totalElementCO2e = 0;
                    var materialSetIterator = element.Materials.ForwardIterator();
                    while (materialSetIterator.MoveNext())
                    {
                        var material = (Material)materialSetIterator.Current;
                 
                        var volumeInM3 = element.GetMaterialVolume(material);
                        materialKey = material.Id + ":" + material.Name;

                        var ameeMaterial = _materialMapper.GetMaterialDataItem(materialKey);
                        totalElementCO2e += ameeMaterial.CalculateCO2eByVolume(volumeInM3);
                    }
                    element.ParametersMap.get_Item("CO2e").Set(totalElementCO2e);
                }
                catch (Exception e)
                {
                    Debug.WriteLine(string.Format("Unable to add CO2e value for element {0} -> material: {1} because {2}", 
                        element.Name, materialKey, e));
                    element.ParametersMap.get_Item("CO2e").Set(0);
                }
            }
        }
    }
}
