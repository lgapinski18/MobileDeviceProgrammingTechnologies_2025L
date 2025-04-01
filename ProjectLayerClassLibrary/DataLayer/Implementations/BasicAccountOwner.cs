using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo("ProjectLayerClassLibraryTest")]

namespace ProjectLayerClassLibrary.DataLayer.Implementations
{
    internal class BasicAccountOwner : AAccountOwner
    {
        public BasicAccountOwner(int ownerId, string ownerLogin, string ownerName, string ownerSurname, string ownerEmail, string ownerPassword) 
            : base(ownerId, ownerLogin, ownerName, ownerSurname, ownerEmail, ownerPassword)
        {
        }
    }
}
