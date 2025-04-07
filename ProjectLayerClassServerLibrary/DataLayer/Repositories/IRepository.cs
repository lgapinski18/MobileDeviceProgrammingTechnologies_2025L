using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectLayerClassServerLibrary.DataLayer.Repositories
{
    public interface IRepository<T> where T : IIdentifiable
    {
        public T? Get(int id);
        public bool Save(T entity);
        public bool Remove(T entity);
        public bool Remove(int id);
        public ICollection<T> GetAll();
    }
}
