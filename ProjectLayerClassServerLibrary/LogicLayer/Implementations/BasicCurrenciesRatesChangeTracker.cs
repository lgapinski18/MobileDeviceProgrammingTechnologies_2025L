using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectLayerClassServerLibrary.LogicLayer.Implementations
{
    internal class BasicCurrenciesRatesChangeTracker : ACurrenciesRatesChangeTracker
    {
        private List<IObserver<ACurrenciesRates>> observers;

        public BasicCurrenciesRatesChangeTracker()
        {
            observers = new List<IObserver<ACurrenciesRates>>();
        }

        public override IDisposable Subscribe(IObserver<ACurrenciesRates> observer)
        {
            if (!observers.Contains(observer))
                observers.Add(observer);
            return new Unsubscriber(observers, observer);
        }

        private class Unsubscriber : IDisposable
        {
            private List<IObserver<ACurrenciesRates>> _observers;
            private IObserver<ACurrenciesRates> _observer;

            public Unsubscriber(List<IObserver<ACurrenciesRates>> observers, IObserver<ACurrenciesRates> observer)
            {
                _observers = observers;
                _observer = observer;
            }

            public void Dispose()
            {
                if (_observer != null && _observers.Contains(_observer))
                    _observers.Remove(_observer);
            }
        }

        public override void EndAllObservations()
        {
            foreach (var observer in observers)
            {
                observer.OnCompleted();
            }

            observers.Clear();
        }
         
        public override void TrackCurrenciesRatesChanged(ACurrenciesRates currenciesRates)
        {
            foreach (var observer in observers)
            {
                observer.OnNext(currenciesRates);
            }
        }
    }
}
