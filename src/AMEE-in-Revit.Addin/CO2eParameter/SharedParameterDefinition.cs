using Autodesk.Revit.DB;

namespace AMEE_in_Revit.Addin.CO2eParameter
{
    public class SharedParameterDefinition
    {
        public string SharedParametersFilename { get; set; }
        public string GroupName { get; set; }
        public string ParameterName { get; set; }
        public ParameterType ParameterType { get; set; }
    }
}