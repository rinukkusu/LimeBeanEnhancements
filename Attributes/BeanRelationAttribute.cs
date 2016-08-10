using System;

namespace LimeBeanEnhancements.Attributes
{
	[AttributeUsage(AttributeTargets.Property)]
	public class BeanRelationAttribute : Attribute
    {
		public Type RelatedBeanType { get; private set; }

		public BeanRelationAttribute(Type relatedBeanType)
		{
			RelatedBeanType = relatedBeanType;
		}
	}
}
