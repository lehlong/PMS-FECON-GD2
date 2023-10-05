using SMO;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Web;

namespace SMO
{
    public static class ObjectExtension
    {
        public static object CloneObject(this object objSource)
        {
            //Get the type of source object and create a new instance of that type
            Type typeSource = objSource.GetType();
            object objTarget = Activator.CreateInstance(typeSource);
            //Get all the properties of source object type
            PropertyInfo[] propertyInfo = typeSource.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            //Assign all source property to taget object 's properties
            foreach (PropertyInfo property in propertyInfo)
            {
                //Check whether property can be written to
                if (property.CanWrite)
                {
                    //check whether property type is value type, enum or string type
                    if (property.PropertyType.IsValueType || property.PropertyType.IsEnum || property.PropertyType.Equals(typeof(System.String)))
                    {
                        property.SetValue(objTarget, property.GetValue(objSource, null), null);
                    }
                    //else property type is object/complex types, so need to recursively call this method until the end of the tree is reached
                    //else
                    //{
                    //    object objPropertyValue = property.GetValue(objSource, null);
                    //    if (objPropertyValue == null)
                    //    {
                    //        property.SetValue(objTarget, null, null);
                    //    }
                    //    else
                    //    {
                    //        property.SetValue(objTarget, objPropertyValue.CloneObject(), null);
                    //    }
                    //}
                }
            }
            return objTarget;
        }
    }

    public static class DecimalExtension
    {
        public static string ToStringVN(this decimal value)
        {
            try
            {
                CultureInfo cul = CultureInfo.GetCultureInfo("vi-VN");   // try with "en-US"
                var result = "";
                result = value.ToString(Global.NumberFormat, cul);
                return result;
            }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            catch (Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            {
                return "";
            }
        }

        public static string ToStringVN_6(this decimal value)
        {
            try
            {
                CultureInfo cul = CultureInfo.GetCultureInfo("vi-VN");   // try with "en-US"
                var result = "";
                result = value.ToString(Global.NumberFormat6, cul);
                return result;
            }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            catch (Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            {
                return "";
            }
        }

        public static string GetName(this Enum enumVal) 
        {
            try
            {
                var type = enumVal.GetType();
                var memInfo = type.GetMember(enumVal.ToString());
                var attributes = memInfo[0].GetCustomAttributes(typeof(EnumNameAttribute), false);
                return (attributes.Length > 0) ? ((EnumNameAttribute)attributes[0]).Name : string.Empty;
            }
            catch
            {
                return "";
            }
        }
        
        public static string GetValue(this Enum enumVal) 
        {
            try
            {
                var type = enumVal.GetType();
                var memInfo = type.GetMember(enumVal.ToString());
                var attributes = memInfo[0].GetCustomAttributes(typeof(EnumValueAttribute), false);
                return (attributes.Length > 0) ? ((EnumValueAttribute)attributes[0]).Value : string.Empty;
            }
            catch
            {
                return "";
            }
        }

        public static T GetEnum<T>(this string enumValue)
        {
            try
            {
                var type = typeof(T);
                foreach (T t in Enum.GetValues(typeof(T)))
                {
                    if ((t as Enum).GetValue().Equals(enumValue))
                    {
                        return t;
                    }
                }
                return default(T);
            }
            catch
            {
                return default(T);
            }
        }
        public static T GetEnumByName<T>(this string enumValue)
        {
            try
            {
                var type = typeof(T);
                foreach (T t in Enum.GetValues(typeof(T)))
                {
                    if (t.ToString().Equals(enumValue))
                    {
                        return t;
                    }
                }
                return default(T);
            }
            catch
            {
                return default(T);
            }
        }
    }

    public static class StringExtension
    {
        public static string RemoveZeroWidthSpaces(this string input)
        {
            if (String.IsNullOrEmpty(input)) return input;
            char ZeroWidthSpace = (char)8203;
            if (input.Contains(ZeroWidthSpace.ToString()))
                return input.Replace(ZeroWidthSpace.ToString(), "");
            return input;
        }
    }
}