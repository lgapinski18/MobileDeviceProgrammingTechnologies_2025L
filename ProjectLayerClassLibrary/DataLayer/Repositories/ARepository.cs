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

        public bool Save(T entity)
        {
            entities.Add(entity);
            return true;
        }
    }
}
