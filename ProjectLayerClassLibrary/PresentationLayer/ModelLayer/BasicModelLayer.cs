using ProjectLayerClassLibrary.DataLayer;
using ProjectLayerClassLibrary.LogicLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectLayerClassLibrary.PresentationLayer.ModelLayer
{
    internal class BasicModelLayer : AModelLayer
    {
        public BasicModelLayer(ALogicLayer? logicLayer = default(ALogicLayer))
        {
            this.logicLayer = logicLayer == null ? ALogicLayer.CreateLogicLayerInstance() : logicLayer;
        }
    }
}
