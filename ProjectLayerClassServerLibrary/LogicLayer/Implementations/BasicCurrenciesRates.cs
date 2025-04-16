using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectLayerClassServerLibrary.LogicLayer.Implementations
{
    internal class BasicCurrenciesRates : ACurrenciesRates
    {
        private float euroPurchaseRate;
        public override float EuroPurchaseRate { get => euroPurchaseRate; set => euroPurchaseRate = value; }

        private float euroSellRate;
        public override float EuroSellRate { get => euroSellRate; set => euroSellRate = value; }

        private float usdPurchaseRate;
        public override float UsdPurchaseRate { get => usdPurchaseRate; set => usdPurchaseRate = value; }

        private float usdSellRate;
        public override float UsdSellRate { get => usdSellRate; set => usdSellRate = value; }

        private float gbpPurchaseRate;
        public override float GbpPurchaseRate { get => gbpPurchaseRate; set => gbpPurchaseRate = value; }

        private float gbpSellRate;
        public override float GbpSellRate { get => gbpSellRate; set => gbpSellRate = value; }

        private float chfPurchaseRate;
        public override float ChfPurchaseRate { get => chfPurchaseRate; set => chfPurchaseRate = value; }

        private float chfSellRate;
        public override float ChfSellRate { get => chfSellRate; set => chfSellRate = value; }

        public BasicCurrenciesRates(float euroPurchaseRate, float euroSellRate, float usdPurchaseRate, float usdSellRate, float gbpPurchaseRate, float gbpSellRate, float chfPurchaseRate, float chfSellRate)
        {
            EuroPurchaseRate = euroPurchaseRate;
            EuroSellRate = euroSellRate;
            UsdPurchaseRate = usdPurchaseRate;
            UsdSellRate = usdSellRate;
            GbpPurchaseRate = gbpPurchaseRate;
            GbpSellRate = gbpSellRate;
            ChfPurchaseRate = chfPurchaseRate;
            ChfSellRate = chfSellRate;
        }
    }
}
