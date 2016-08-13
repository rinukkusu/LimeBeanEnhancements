using System.Collections.Generic;
using System.Linq;
using LimeBean;
using LimeBean.Interfaces;

namespace LimeBeanEnhancements.Extensions
{
    public static class BeanApiExtensions
    {
		public static IList<T> GetAll<T>(this IBeanAPI api) where T : Bean, new()
		{
			return api.Find<T>(false).ToList();
		}
    }
}

