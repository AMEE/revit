using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AMEE_in_Revit.Addin.SharedParameters;
using Autodesk.Revit.DB;

namespace AMEE_in_Revit.Addin
{
    public class Settings
    {
        public const string AmeeUrl = "https://stage.amee.com";
        public const string AmeeUserName = "AMEE_in_Revit";
        public const string AmeePassword = "ghmuasqx";
        public static SharedParameterDefinition CO2eParam 
        { 
            get
            {
                return new SharedParameterDefinition
                           {
                                SharedParametersFilename = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),"AMEE_shared_parameters.txt"),
                                GroupName = "AMEE",
                                ParameterName = "CO2e",
                                ParameterType = ParameterType.Number
                            };
            } 
        }
        public static List<BuiltInCategory> BuiltInCategoriesWithCO2eParameter
        {
            get
             {
                return new List<BuiltInCategory>
                           {
                            BuiltInCategory.OST_Walls,
                            BuiltInCategory.OST_Floors,
                            BuiltInCategory.OST_Roofs,
                            BuiltInCategory.OST_Doors,
                            BuiltInCategory.OST_Columns,
                            BuiltInCategory.OST_StructuralColumns,
                            BuiltInCategory.OST_Stairs,
                            BuiltInCategory.OST_Ramps,
                            BuiltInCategory.OST_Ceilings
                            };
            }
        }

        public static LogicalOrFilter CreateFilterForElementsWithCO2eParameter()
        {
            IList<ElementFilter> filterList = BuiltInCategoriesWithCO2eParameter
                .Select(builtInCategory => new ElementCategoryFilter(builtInCategory))
                .Cast<ElementFilter>()
                .ToList();
            return new LogicalOrFilter(filterList);
        }
    }
}
