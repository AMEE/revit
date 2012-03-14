using System;

namespace AMEE_in_Revit.Addin.SharedParameters
{
    public class OpenSharedParamFileException : Exception
    {
        public OpenSharedParamFileException(string message): base(message) { }
    }
}