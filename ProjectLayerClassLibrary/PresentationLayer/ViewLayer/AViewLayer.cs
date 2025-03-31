using ProjectLayerClassLibrary.LogicLayer;
using ProjectLayerClassLibrary.PresentationLayer.ViewModelLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectLayerClassLibrary.PresentationLayer.ViewLayer
{
    public abstract class AViewLayer
    {
        protected AViewModelLayer viewModelLayer;

        public static AViewLayer createViewLayerInstance(AViewModelLayer? viewModelLayer = default(AViewModelLayer))
        {
            return new BasicViewModelLayer(viewModelLayer);
        }
    }
}
