using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using AMEE_in_Revit.Addin.CO2eParameter;
using AMEEClient;
using AMEEClient.MaterialMapper;
using Autodesk.Revit.DB;
using log4net;

namespace AMEE_in_Revit.Addin
{
    public class Settings
    {
        public const string AmeeUrl = "https://stage.amee.com";
        public const string AmeeUserName = "AMEE_in_Revit";
        public const string AmeePassword = "ghmuasqx";
        private static readonly ILog logger = LogManager.GetLogger(typeof(Settings));
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

        private static ElementCO2eCalculator _calculator;
        private static string _mapFilePath =  Path.Combine(Path.GetDirectoryName(typeof(Settings).Assembly.CodeBase), @"Revit2AMEEMaterialMap.xml");

        public static ElementCO2eCalculator GetCO2eCalculator()
        {
            if (_calculator==null)
            {
                var ameeClient = new Client(new Uri(AmeeUrl), AmeeUserName, AmeePassword);
                _calculator = new ElementCO2eCalculator(new MaterialMapper(_mapFilePath, ameeClient));
            }
            return _calculator;
        }
    }
}
