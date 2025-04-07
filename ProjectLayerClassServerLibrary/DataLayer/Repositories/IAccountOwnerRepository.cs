using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectLayerClassServerLibrary.DataLayer.Repositories
{
    public interface IAccountOwnerRepository : IRepository<AAccountOwner>
    {
        public AAccountOwner? GetByOwnerLogin(string ownerLogin);
    }
}
