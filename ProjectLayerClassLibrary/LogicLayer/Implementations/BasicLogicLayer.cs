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
<<<<<<< HEAD:ProjectLayerClassLibrary/LogicLayer/Implementations/BasicLogicLayer.cs
            this.dataLayer = dataLayer == null ? ADataLayer.CreateDataLayerInstance() : dataLayer;
=======
            //this.dataLayer = dataLayer == null ? ADataLayer.createDataLayerInstance() : dataLayer;
>>>>>>> 0db1557180464e1cc4e643bade5af6cf7731cd6d:ProjectLayerClassLibrary/LogicLayer/BasicLogicLayer.cs
        }
    }
}
