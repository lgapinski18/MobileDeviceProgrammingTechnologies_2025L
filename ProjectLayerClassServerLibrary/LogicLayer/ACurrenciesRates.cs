using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectLayerClassServerLibrary.LogicLayer
{
    public abstract class ACurrenciesRates
    {
        public abstract float EuroPurchaseRate { get; set; }
        public abstract float EuroSellRate { get; set; }
        public abstract float UsdPurchaseRate { get; set; }
        public abstract float UsdSellRate { get; set; }
        public abstract float GbpPurchaseRate { get; set; }
        public abstract float GbpSellRate { get; set; }
        public abstract float ChfPurchaseRate { get; set; }
        public abstract float ChfSellRate { get; set; }

    }
}
