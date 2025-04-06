using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectLayerClassLibrary.DataLayer.Repositories
{
    internal abstract class ARepository<T> : IRepository<T> where T : IIdentifiable
    {
        protected ICollection<T> entities;

        public T? Get(int id)
        {
            return entities.Where(e => e.GetId() == id).FirstOrDefault();
        }

        public ICollection<T> GetAll()
        {
            return entities;
        }

        public bool Remove(T entity)
        {
            if (entity != null)
            {
                return entities.Remove(entity);
            }
            return true;
        }

        public bool Remove(int id)
        {
            T? t = Get(id);
            if (t != null)
            {
                return entities.Remove(t);
            }
            return true;
        }

        public bool Save(T entity)
        {
            entities.Add(entity);
            return true;
        }
    }
}
