using ProjectLayerClassLibrary.DataLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectLayerClassLibrary.LogicLayer.Implementations
{
    internal class BasicLogicLayer : ALogicLayer
    {
        public BasicLogicLayer(ADataLayer? dataLayer = default)
        {
            this.dataLayer = dataLayer == null ? ADataLayer.CreateDataLayerInstance() : dataLayer;
        }
    }
}
