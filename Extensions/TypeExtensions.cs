using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using LimeBeanEnhancements.Attributes;

namespace LimeBeanEnhancements
{
	public static class TypeExtensions
	{
		public static List<PropertyInfo> GetSortedBeanProperties(this Type objType)
		{
			return objType.GetTypeInfo().GetProperties().ToList().SortByBeanProperty();
		}

		public static BeanTableAttribute GetBeanTableAttribute(this Type objType)
		{
			return objType.GetTypeInfo().GetCustomAttribute<BeanTableAttribute>();
		}
	}
}