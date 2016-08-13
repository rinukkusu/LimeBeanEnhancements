using System;

namespace LimeBeanEnhancements.Attributes
{
	[AttributeUsage(AttributeTargets.Class)]
	public class BeanTableAttribute : Attribute
	{
		public string Table { get; private set; }

		public BeanTableAttribute(string table)
		{
			Table = table;
		}
	}
}