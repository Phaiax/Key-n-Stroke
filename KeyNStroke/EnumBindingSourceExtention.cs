using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;

namespace KeyNStroke
{
    public class EnumBindingSourceExtention : MarkupExtension
    {
        public Type EnumType { get; private set; }

        public EnumBindingSourceExtention(Type enumType)
        {
            if (enumType == null || !enumType.IsEnum)
                throw new Exception("enumType must be of type enum");
            this.EnumType = enumType;
        }

        public static T GetAttributeOfType<T>(Enum enumVal) where T : System.Attribute
        {
            var type = enumVal.GetType();
            var memInfo = type.GetMember(Enum.GetName(type, enumVal)); // type.GetMember(enumVal.ToString());
            var attributes = memInfo[0].GetCustomAttributes(typeof(T), false);
            return (attributes.Length > 0) ? (T)attributes[0] : null;
        }

        public static string GetAttributeDescription(Enum enumValue)
        {
            var attribute = GetAttributeOfType<System.ComponentModel.DescriptionAttribute>(enumValue);
            return attribute == null ? String.Empty : attribute.Description;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            Array variants = Enum.GetValues(EnumType);
            List<ComboBoxItem> b = new List<ComboBoxItem>(variants.Length);
            foreach (var a in variants)
            {
                ComboBoxItem i = new ComboBoxItem();
                i.Content = GetAttributeDescription((Enum)a); // new TextBlock(new Run("Hi"));
                i.Tag = a;
                b.Add(i);
            }
            return b;
        }

    }
}
