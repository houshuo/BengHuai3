namespace MoleMole
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    public class ReflectionUtil
    {
        private static Dictionary<System.Type, FieldInfo[]> _reflectionFieldCache = new Dictionary<System.Type, FieldInfo[]>();
        private static Dictionary<System.Type, PropertyInfo[]> _reflectionPropertyCache = new Dictionary<System.Type, PropertyInfo[]>();

        private static FieldInfo[] GetFieldsWithCache(System.Type type)
        {
            if (!_reflectionFieldCache.ContainsKey(type))
            {
                _reflectionFieldCache.Add(type, type.GetFields());
            }
            return _reflectionFieldCache[type];
        }

        private static PropertyInfo[] GetPropertysWithCache(System.Type type)
        {
            if (!_reflectionPropertyCache.ContainsKey(type))
            {
                _reflectionPropertyCache.Add(type, type.GetProperties());
            }
            return _reflectionPropertyCache[type];
        }

        public static object GetValue(object instance, string propertyName)
        {
            FieldInfo info = TryGetFieldValue(instance, propertyName);
            if (info != null)
            {
                return info.GetValue(instance);
            }
            PropertyInfo info2 = TryGetPropertyValue(instance, propertyName);
            if (info2 != null)
            {
                return info2.GetValue(instance, null);
            }
            return null;
        }

        public static System.Type GetValueType(object instance, string propertyName)
        {
            FieldInfo info = TryGetFieldValue(instance, propertyName);
            if (info != null)
            {
                return info.FieldType;
            }
            PropertyInfo info2 = TryGetPropertyValue(instance, propertyName);
            if (info2 != null)
            {
                return info2.PropertyType;
            }
            return null;
        }

        public static void SetValue(object instance, string propertyName, object value)
        {
            FieldInfo info = TryGetFieldValue(instance, propertyName);
            if (info != null)
            {
                info.SetValue(instance, value);
            }
            else
            {
                PropertyInfo info2 = TryGetPropertyValue(instance, propertyName);
                if (info2 != null)
                {
                    info2.SetValue(instance, value, null);
                }
            }
        }

        private static FieldInfo TryGetFieldValue(object instance, string propertyName)
        {
            FieldInfo[] fieldsWithCache = GetFieldsWithCache(instance.GetType());
            for (int i = 0; i < fieldsWithCache.Length; i++)
            {
                if (propertyName == fieldsWithCache[i].Name)
                {
                    return fieldsWithCache[i];
                }
            }
            return null;
        }

        private static PropertyInfo TryGetPropertyValue(object instance, string propertyName)
        {
            PropertyInfo[] propertysWithCache = GetPropertysWithCache(instance.GetType());
            for (int i = 0; i < propertysWithCache.Length; i++)
            {
                if (propertyName == propertysWithCache[i].Name)
                {
                    return propertysWithCache[i];
                }
            }
            return null;
        }
    }
}

