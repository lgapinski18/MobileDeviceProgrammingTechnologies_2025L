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
        private static AModelLayer? instance;
        public static AModelLayer Instance { get
            {
                instance ??= new BasicModelLayer();
                return instance;
            } 
        }

        internal abstract ALogicLayer LogicLayer { get; }
    }
}
