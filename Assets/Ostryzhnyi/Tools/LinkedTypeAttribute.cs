using System;

namespace Ostryzhnyi.Tools
{
    public class LinkedTypeAttribute : Attribute
    {
        public readonly Type Type;

        public LinkedTypeAttribute(Type type)
        {
            Type = type;
        }
    }
}