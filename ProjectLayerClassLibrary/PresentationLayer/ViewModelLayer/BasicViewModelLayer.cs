using ProjectLayerClassLibrary.PresentationLayer.ModelLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectLayerClassLibrary.PresentationLayer.ViewModelLayer
{
    internal class BasicViewModelLayer : AViewModelLayer
    {
        public BasicViewModelLayer(AModelLayer? modelLayer = default(AModelLayer))
        {
            this.modelLayer = modelLayer == null ? AModelLayer.createModelLayerInstance() : modelLayer;
        }
    }
}
