using System.Collections.Generic;
using System.Reflection;
using LimeBeanEnhancements.Attributes;

namespace LimeBeanEnhancements
{
	public static class PropertyInfoListExtensions
	{
		public static List<PropertyInfo> SortByBeanProperty(this List<PropertyInfo> propertyList)
		{
			propertyList = propertyList.FindAll(a =>
			{
				if (a.GetCustomAttribute<BeanPropertyAttribute>() != null) return true;
				else return false;
			});

			propertyList.Sort((a, b) =>
			{
				var attributeA = a.GetCustomAttribute<BeanPropertyAttribute>();
				var attributeB = b.GetCustomAttribute<BeanPropertyAttribute>();

				return attributeA.Position - attributeB.Position;
			});

			return propertyList;
		}
	}
}