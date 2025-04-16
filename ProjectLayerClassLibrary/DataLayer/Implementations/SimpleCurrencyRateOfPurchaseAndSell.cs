using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo("ProjectLayerClassLibraryTest")]

namespace ProjectLayerClassLibrary.DataLayer.Implementations
{
    internal class SimpleCurrencyRateOfPurchaseAndSell : ACurrencyRateOfPurchaseAndSell
    {
        private float currencyRateOfPurchase;
        private float currencyRateOfSell;

        public SimpleCurrencyRateOfPurchaseAndSell(float currencyRateOfPurchase, float currencyRateOfSell)
        {
            this.currencyRateOfPurchase = currencyRateOfPurchase;
            this.currencyRateOfSell = currencyRateOfSell;
        }

        public override float CurrencyRateOfPurchase => currencyRateOfPurchase;

        public override float CurrencyRateOfSell => currencyRateOfSell;
    }
}
