using LimeBean.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LimeBeanEnhancements
{
    public abstract class BeanRepository<T> where T : EnhancedBean<T>, new()
    {
        protected IBeanAPI _beanApi { get; private set; }

        protected BeanRepository(IBeanAPI beanApi)
        {
            _beanApi = beanApi;
        }

        public T Create()
        {
            return _beanApi.Dispense<T>();
        }

        public T Get(long id)
        {
            return _beanApi.Load<T>(id);
        }

        public IList<T> GetAll()
        {
            return _beanApi.Find<T>(false).ToList();
        }

        public void Delete(T bean)
        {
            _beanApi.Trash(bean);
        }

        public void Delete(long id)
        {
            T bean = Get(id);
            Delete(bean);
        }

        public long Save(T bean)
        {
            return (long)_beanApi.Store(bean);
        }
    }
}
