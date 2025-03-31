using ProjectLayerClassLibrary.LogicLayer;
using ProjectLayerClassLibrary.PresentationLayer.ViewModelLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectLayerClassLibrary.PresentationLayer.ViewLayer
{
    internal class BasicViewLayer : AViewLayer
    {
        public BasicViewLayer(AViewModelLayer? viewModelLayer = default(AViewModelLayer))
        {
            this.viewModelLayer = viewModelLayer == null ? AViewModelLayer.createViewModelLayerInstance() : viewModelLayer;
        }
    }
}
