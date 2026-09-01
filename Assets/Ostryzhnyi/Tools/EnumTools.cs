using System;

namespace Ostryzhnyi.Tools
{
    public static class EnumTools
    {
        public static TEnumElement[] GetElements<TEnumElement>() where TEnumElement : Enum
        {
            return (TEnumElement[])Enum.GetValues(typeof(TEnumElement));
        }
    }
}