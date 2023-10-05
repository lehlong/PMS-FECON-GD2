using System;

namespace SMO
{
    public class EnumNameAttribute : Attribute
    {
        public string Name { get; set; }
        public EnumNameAttribute(string name)
        {
            Name = name;
        }
    }
    
    public class EnumValueAttribute : Attribute
    {
        public string Value { get; set; }
        public EnumValueAttribute(string value)
        {
            Value = value;
        }
    }
}