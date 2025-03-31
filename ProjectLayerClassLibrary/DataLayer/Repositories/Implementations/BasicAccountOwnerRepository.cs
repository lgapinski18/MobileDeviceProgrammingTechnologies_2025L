using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("ProjectLayerClassLibrary")]

namespace ProjectLayerClassLibrary.DataLayer.Repositories.Implementations
{
    internal class BasicAccountOwnerRepository : ARepository<AAccountOwner>, IAccountOwnerRepository
    {
        public BasicAccountOwnerRepository()
        {
            entities = new List<AAccountOwner>();
        }

        public ICollection<AAccountOwner> GetAll()
        {
            return entities;
        }
    }
}
