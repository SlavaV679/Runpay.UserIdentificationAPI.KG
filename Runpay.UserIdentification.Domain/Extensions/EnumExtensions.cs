using System.ComponentModel;
using System.Reflection;

namespace Runpay.UserIdentification.Domain.Extensions
{
    public static class EnumExtensions
    {
        public static string GetDescriptionByEnumValue<TEnum>(this TEnum enumValue)
            where TEnum : Enum
        {
            if (enumValue == null)
                return string.Empty;

            var type = typeof(TEnum);
            var attribute = type.GetField(enumValue.ToString()).GetCustomAttribute(typeof(DescriptionAttribute));

            if (attribute == null)
            {
                throw new InvalidOperationException();
            }

            return ((DescriptionAttribute)attribute).Description;
        }
    }
}
