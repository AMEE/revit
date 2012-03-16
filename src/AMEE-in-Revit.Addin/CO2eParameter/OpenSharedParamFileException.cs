using System;

namespace AMEE_in_Revit.Addin.CO2eParameter
{
    public class OpenSharedParamFileException : Exception
    {
        public OpenSharedParamFileException(string message): base(message) { }
    }
}