using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectLayerClassLibrary.LogicLayer.Implementations
{
    internal class BasicLogicLayerAccountOwner : AAccountOwner
    {
        public BasicLogicLayerAccountOwner(DataLayer.AAccountOwner dataLayerAccountOwner) : base(dataLayerAccountOwner)
        {
        }
    }
}
