namespace Orc.CommandLine
{
    using System;
    using System.Reflection;

    internal static class PropertyHelper
    {

        internal static void SetPrivatePropertyValue(this object obj, string propName, object value)
        {
            var objType = obj.GetType();
            var property = objType.GetProperty(propName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (property is null)
            {
                throw new InvalidOperationException($"Property {propName} was not found in Type {objType}");
            }
            var setter = property.GetSetMethod(nonPublic: true);
            if (setter is null)
            {
                var backingField = property.DeclaringType.GetField($"<{property.Name}>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance);
                if (backingField is null)
                {
                    throw new InvalidOperationException(
                        $"Could not find a way to set {property.DeclaringType.FullName}.{property.Name}. Try adding a private setter.");
                }
                backingField.SetValue(obj, value);
            }
            else
            {
                setter.Invoke(obj, new object[]
                {
                    value
                });
            }
        }
    }
}
