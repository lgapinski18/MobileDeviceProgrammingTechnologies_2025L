using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectLayerClassServerLibrary.LogicLayer
{
    public abstract class ACurrenciesRatesChangeTracker : IObservable<ACurrenciesRates>
    {
        public abstract IDisposable Subscribe(IObserver<ACurrenciesRates> observer);

        public abstract void TrackCurrenciesRatesChanged(ACurrenciesRates currenciesRates);

        public abstract void EndAllObservations();
    }
}
