using System;
using System.Reflection;
using LimeBean;
using LimeBeanEnhancements.Attributes;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace LimeBeanEnhancements
{
	[DataContract]
	public abstract class EnhancedBean<T> : Bean
	{
		[BeanProperty("id", 0)]
		[JsonProperty(Order = -2)]
		[DataMember]
		public long Id { get; set; }

		protected EnhancedBean() : base(GetKindStatic())
		{
		}

		private static string GetKindStatic()
		{
			BeanTableAttribute attribute = typeof(T).GetBeanTableAttribute();
			if (attribute != null)
			{
				return attribute.Table;
			}

			throw new ArgumentException("No BeanTableAttribute on class.");
		}

		protected override void AfterLoad()
		{
			var properties = GetType().GetSortedBeanProperties();

			foreach (var property in properties)
			{
				BeanPropertyAttribute propertyAttribute = property.GetCustomAttribute<BeanPropertyAttribute>();
				BeanRelationAttribute relationAttribute = property.GetCustomAttribute<BeanRelationAttribute>();

				if (relationAttribute != null)
				{
                    long id = this[propertyAttribute.Column] != null ? long.Parse(this[propertyAttribute.Column].ToString()) : 0;
					if (id <= 0) continue;

					BeanTableAttribute beanTableAttribute = property.PropertyType.GetBeanTableAttribute();

					MethodInfo loadMethod = GetApi().GetType().GetRuntimeMethod("Load", new [] { typeof(object) });
					MethodInfo loadMethodGeneric = loadMethod.MakeGenericMethod(relationAttribute.RelatedBeanType);

					var relatedInstance = loadMethodGeneric.Invoke(GetApi(), new object [] { id });

					property.SetValue(this, relatedInstance);
				}
				else
				{
                    object value = this[propertyAttribute.Column];

                    if (property.PropertyType == typeof(string))
                    {
                        value = value.ToString();
                    }

					property.SetValue(this, value);
				}
			}
		}

		protected override void BeforeStore()
		{
			var properties = GetType().GetSortedBeanProperties();

			foreach (var property in properties)
			{
				var attribute = property.GetCustomAttribute<BeanPropertyAttribute>();
				if (!(attribute.Column == "id" && Id == 0))
				{
					BeanRelationAttribute relationAttribute = property.GetCustomAttribute<BeanRelationAttribute>();

					if (relationAttribute != null)
					{
						Bean relatedInstance = (Bean)property.GetValue(this);

						if (relatedInstance != null)
						{
                            long id = (long)GetApi().Store(relatedInstance);
							this[attribute.Column] = id;
						}
						else
						{
							this[attribute.Column] = 0;
						}
					}
					else
					{
						this[attribute.Column] = property.GetValue(this);
					}
				}
			}
		}
	}
}
