using Microsoft.Extensions.Configuration;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace WorldWord.Config
{
    public static class Configuration
    {
        public static T Create<T>(IConfiguration config) where T : class, new()
        {
            T cfg = new T();
            setSection(cfg, config);
            return cfg;
        }

        private static void setSection(object cfgObject, IConfiguration section)
        {
            var properties = cfgObject.GetType().GetProperties().Where(p => !p.CustomAttributes.Any(x => x.AttributeType == typeof(NotMappedAttribute)) && (p.SetMethod != null || p.DeclaringType?.GetProperty(p.Name)?.SetMethod != null));

            foreach (var property in properties)
            {
                IConfigurationSection element = section.GetSection(property.Name);

                if (property.PropertyType == typeof(string) || property.PropertyType.GetTypeInfo().IsPrimitive)
                {
                    var value = Convert.ChangeType(element.Value, property.PropertyType);
                    if(value != null)
                    SetField(property, cfgObject, value);
                }
                else if (property.PropertyType == typeof(TimeSpan) || property.PropertyType == typeof(Nullable<TimeSpan>))
                {
                    var milliSeconds = Convert.ToInt64(element.Value);
                    var timespan = TimeSpan.FromMilliseconds(milliSeconds);
                    SetField(property, cfgObject, timespan);
                }
                else if (property.PropertyType.IsEnum)
                {   if (element.Value == null)
                        continue;

                    var value = Enum.Parse(property.PropertyType, element.Value);
                    if (value != null)
                        SetField(property, cfgObject, value);
                }
                else
                {
                    if (property.GetValue(cfgObject) == null)
                        SetField(property, cfgObject, Activator.CreateInstance(property.PropertyType)!);

                    setSection(property.GetValue(cfgObject)!, section.GetSection(property.Name));
                }

                if (property.GetValue(cfgObject) == null)
                    throw new ArgumentException($"{property.Name} is not provided");
            }
        }

        private static void SetField(PropertyInfo prop, object configuredObject, object value)
        {
            if (prop.SetMethod != null)
                prop.SetMethod.Invoke(configuredObject, new object[] { value });
            else
                prop.DeclaringType?.GetProperty(prop.Name)?.SetMethod?.Invoke(configuredObject, new object[] { value });
        }
    }
}
