using System;

namespace AMEEClient.MaterialMapper
{
    public class UnknownMaterialException : Exception
    {
        public UnknownMaterialException(string message): base(message)
        {
        }
    }
}