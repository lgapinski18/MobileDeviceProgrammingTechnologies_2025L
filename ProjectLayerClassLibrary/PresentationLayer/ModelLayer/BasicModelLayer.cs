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
        private ALogicLayer logicLayer;
        public BasicModelLayer()
        {
            logicLayer = ALogicLayer.CreateLogicLayerInstance();
        }

        internal override ALogicLayer LogicLayer => logicLayer;
    }
}
