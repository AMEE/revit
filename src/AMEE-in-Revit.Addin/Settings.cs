using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
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

        public static List<BuiltInCategory> BuiltInCategoriesWithCO2eParamater
        {
            get
             {
                return new List<BuiltInCategory>
                            {
                            BuiltInCategory.OST_Casework,
                            BuiltInCategory.OST_Areas,
                            BuiltInCategory.OST_Ceilings,
                            BuiltInCategory.OST_Columns,
                            BuiltInCategory.OST_CurtainWallPanels,
                            BuiltInCategory.OST_Walls,
                            BuiltInCategory.OST_Doors,
                            BuiltInCategory.OST_Windows,
                            BuiltInCategory.OST_Roofs,
                            BuiltInCategory.OST_Furniture,
                            BuiltInCategory.OST_FurnitureSystems,
                            BuiltInCategory.OST_PlumbingFixtures,
                            BuiltInCategory.OST_Stairs,
                            BuiltInCategory.OST_Ramps,
                            BuiltInCategory.OST_StructuralFoundation,
                            BuiltInCategory.OST_StructuralColumns,
                            BuiltInCategory.OST_StructuralFraming,
                            };
            }
        }
    }
}
