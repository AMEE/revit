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

        public static LogicalOrFilter CreateFilterForAMEECompatibleElements()
        {
            IList<ElementFilter> filterList = new List<ElementFilter>
                                                  {
                                                      new ElementCategoryFilter(BuiltInCategory.OST_Walls),
                                                      new ElementCategoryFilter(BuiltInCategory.OST_Floors),
                                                      new ElementCategoryFilter(BuiltInCategory.OST_Roofs),
                                                      new ElementCategoryFilter(BuiltInCategory.OST_Doors),
                                                      new ElementCategoryFilter(BuiltInCategory.OST_Columns),
                                                      new ElementCategoryFilter(BuiltInCategory.OST_StructuralColumns),
                                                      new ElementCategoryFilter(BuiltInCategory.OST_Stairs),
                                                      new ElementCategoryFilter(BuiltInCategory.OST_Ramps),
                                                      new ElementCategoryFilter(BuiltInCategory.OST_Ceilings)
                                                  };
            return new LogicalOrFilter(filterList);
        }

        public void UpdateElementCO2eParameters(ICollection<Element> forElements)
        {
            var uniqueMaterials = new List<string>();
            foreach (var element in forElements)
            {
                if (!element.ParametersMap.Contains("CO2e")) continue;

                double totalElementCO2e = 0;
                var materialSetIterator = element.Materials.ForwardIterator();
                while (materialSetIterator.MoveNext())
                {
                    var material = (Material)materialSetIterator.Current;
                 
                    //FIXME:  How do we calculate the Mass of the material in an element?
                    var materialMassInKg = element.GetMaterialVolume(material);
                    var materialKey = material.Id + ":" + material.Name;
                    if (!uniqueMaterials.Contains(materialKey))
                    {
                        uniqueMaterials.Add(materialKey);
                    }

                    var ameeMaterial = _materialMapper.GetMaterialDataItem(materialKey);
                    totalElementCO2e += ameeMaterial.CalculateCO2eByMass(materialMassInKg);
                }

                element.ParametersMap.get_Item("CO2e").Set(totalElementCO2e);
            }

            Debug.WriteLine("============= Unique material list ====================");
            uniqueMaterials.ForEach(m => Debug.WriteLine(m));
            
        }
    }
}
