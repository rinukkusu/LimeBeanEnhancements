using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using LimeBean;
using LimeBean.Interfaces;
using LimeBeanEnhancements.Attributes;
using Newtonsoft.Json;

namespace LimeBeanEnhancements
{
    public class BaseBean<T> : IBaseBean
    {
		[BeanProperty("id", 0)]
		[JsonProperty(Order = -2)]
		public long Id { get; set; }

		private static IBeanAPI _beanApi;

	    protected BaseBean(IBeanAPI beanApi)
		{
		    if (_beanApi == null)
		    {
			    _beanApi = beanApi;
		    }
		}

		public BaseBean()
		{
		}

		private static string GetKind()
	    {
			var attribute = typeof(T).GetTypeInfo().GetCustomAttribute<BeanTableAttribute>();
		    if (attribute != null)
		    {
			    return attribute.Table;
		    }

			throw new ArgumentException("No BeanTableAttribute on class.");
	    }

		private static string GetRelatedKind(Type type)
		{
			var attribute = type.GetTypeInfo().GetCustomAttribute<BeanTableAttribute>();
			if (attribute != null)
			{
				return attribute.Table;
			}

			throw new ArgumentException("No BeanTableAttribute on class.");
		}

		private static List<PropertyInfo> SortProperties(PropertyInfo[] properties)
	    {
		    var propertyList = properties.ToList();

			propertyList.Sort((a, b) =>
		    {
				var attributeA = a.GetCustomAttribute<BeanPropertyAttribute>();
				var attributeB = b.GetCustomAttribute<BeanPropertyAttribute>();

				return attributeA.Position - attributeB.Position;
		    });

			return propertyList;
	    }

	    private static T BeanToClass(Bean bean)
	    {
		    if (bean == null)
		    {
			    return default(T);
		    }

		    T instance =  (T)Activator.CreateInstance(typeof(T), _beanApi);

			foreach (var property in SortProperties(typeof(T).GetTypeInfo().GetProperties()))
			{
				var propertyAttribute = property.GetCustomAttribute<BeanPropertyAttribute>();
				var relationAttribute = property.GetCustomAttribute<BeanRelationAttribute>();

				if (relationAttribute != null)
				{
					ulong id = bean[propertyAttribute.Column] != null ? UInt64.Parse(bean[propertyAttribute.Column].ToString()) : 0;
					if (id > 0)
					{
						MethodInfo methodInfo = relationAttribute.RelatedBeanType.GetTypeInfo().BaseType.GetTypeInfo().GetMethod("Load", BindingFlags.Public | BindingFlags.Static);
						var relatedInstance = methodInfo.Invoke(null, new object[] { id });
						property.SetValue(instance, relatedInstance);
					}
				}
				else
				{
					property.SetValue(instance, bean[propertyAttribute.Column]);
				}
			}

		    return instance;
	    }

	    private static Bean ClassToBean(IBaseBean instance)
	    {
			if (instance == null)
			{
				return default(Bean);
			}

			Bean bean = _beanApi.Load(GetKind(), instance.Id) ?? _beanApi.Dispense(GetKind());

			foreach (var property in SortProperties(typeof(T).GetTypeInfo().GetProperties()))
			{
				var attribute = property.GetCustomAttribute<BeanPropertyAttribute>();
				if (attribute.Column == "id" && instance.Id == 0)
				{
				}
				else
				{
					var relationAttribute = property.GetCustomAttribute<BeanRelationAttribute>();

					if (relationAttribute != null)
					{
						var relatedInstance = property.GetValue(instance);

						if (relatedInstance != null)
						{
							MethodInfo methodInfo = relationAttribute.RelatedBeanType.GetTypeInfo()
								.BaseType.GetTypeInfo().GetMethod("Save", BindingFlags.Public | BindingFlags.Static);
							var id = methodInfo.Invoke(null, new object[] {relatedInstance});
							bean[attribute.Column] = (ulong)id;
						}
						else
						{
							bean[attribute.Column] = (ulong)0;
						}
					}
					else
					{
						bean[attribute.Column] = property.GetValue(instance);
					}
				}
			}

			_beanApi.Store(bean);
			return bean;
	    }

	    public static T Load(ulong id)
	    {
		    Bean bean = _beanApi.Load(GetKind(), id);
		    return BeanToClass(bean);
	    }

	    public static ulong Save(IBaseBean instance)
	    {
		    Bean bean = ClassToBean(instance);
		    return (ulong)_beanApi.Store(bean);
	    }

	    public static IEnumerable<T> GetAll()
	    {
		    Bean[] beans = _beanApi.Find(false, GetKind());
		    IList<T> returnList = new List<T>();
		    foreach (var bean in beans)
		    {
			    returnList.Add(BeanToClass(bean));
		    }

		    return returnList;
	    }
    }
}
