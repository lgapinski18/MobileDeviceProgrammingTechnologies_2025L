using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectLayerClassLibrary.LogicLayer
{
    public abstract class ACurrencyRateOfPurchaseAndSell
    {
        public abstract float CurrencyRateOfPurchase { get; } 
        public abstract float CurrencyRateOfSell { get; } 
    }
}
