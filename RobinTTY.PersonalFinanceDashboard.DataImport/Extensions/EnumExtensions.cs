using System.ComponentModel;

namespace RobinTTY.PersonalFinanceDashboard.DataImport.Extensions
{
    public static class EnumExtensions
    {
        /// <summary>
        /// Gets the description of an enum element.
        /// </summary>
        /// <param name="value">The enum element for which to get the description.</param>
        /// <returns>The description of the enum element if it exists, otherwise null.</returns>
        public static string? GetDescription(this Enum value)
        {
            var type = value.GetType();
            var name = Enum.GetName(type, value);
            if (name == null) return null;

            var field = type.GetField(name);
            if (field == null) return null;

            if (Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) is DescriptionAttribute attr)
            {
                return attr.Description;
            }

            return null;
        }
    }
}
