using ProjectLayerClassLibrary.DataLayer;
using ProjectLayerClassLibrary.LogicLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectLayerClassLibrary.PresentationLayer.ModelLayer
{
    public abstract class AModelLayer
    {
        protected ALogicLayer logicLayer;

        public static AModelLayer createModelLayerInstance(ALogicLayer? logicLayer = default(ALogicLayer))
        {
            return new BasicModelLayer(logicLayer);
        }
    }
}
