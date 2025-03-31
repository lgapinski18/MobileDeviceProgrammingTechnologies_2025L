using ProjectLayerClassLibrary.PresentationLayer.ModelLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectLayerClassLibrary.PresentationLayer.ViewModelLayer
{
    public abstract class AViewModelLayer
    {
        protected AModelLayer modelLayer;

        public static AViewModelLayer createViewModelLayerInstance(AModelLayer? modelLayer = default(AModelLayer))
        {
            return new BasicViewModelLayer(modelLayer);
        }
    }
}
