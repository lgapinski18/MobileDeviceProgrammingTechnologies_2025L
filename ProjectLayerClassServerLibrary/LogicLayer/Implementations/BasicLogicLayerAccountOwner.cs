using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo("ProjectLayerClassServerLibraryTest")]

namespace ProjectLayerClassServerLibrary.LogicLayer.Implementations
{
    internal class BasicLogicLayerAccountOwner : AAccountOwner
    {
        public BasicLogicLayerAccountOwner(DataLayer.AAccountOwner dataLayerAccountOwner) : base(dataLayerAccountOwner)
        {
        }
    }
}
