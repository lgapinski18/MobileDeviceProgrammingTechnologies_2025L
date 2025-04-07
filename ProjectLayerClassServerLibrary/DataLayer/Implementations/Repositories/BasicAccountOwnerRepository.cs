using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using ProjectLayerClassServerLibrary.DataLayer.Repositories;

[assembly: InternalsVisibleTo("ProjectLayerClassServerLibraryTest")]

namespace ProjectLayerClassServerLibrary.DataLayer.Implementations.Repositories
{
    internal class BasicAccountOwnerRepository : ARepository<AAccountOwner>, IAccountOwnerRepository
    {
        public BasicAccountOwnerRepository()
        {
            entities = new List<AAccountOwner>();
        }

        public AAccountOwner? GetByOwnerLogin(string ownerLogin)
        {
            return entities.Where(accountOwner => accountOwner.OwnerLogin == ownerLogin).FirstOrDefault();
        }
    }
}
