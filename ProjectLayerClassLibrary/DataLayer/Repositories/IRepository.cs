using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectLayerClassLibrary.DataLayer.Repositories
{
    public interface IRepository<T> where T : IIdentifiable
    {
        T? Get(int id);

        bool Save(T entity);
    }
}
