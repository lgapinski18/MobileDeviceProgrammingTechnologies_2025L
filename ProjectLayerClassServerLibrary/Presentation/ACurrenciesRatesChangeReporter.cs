using ProjectLayerClassServerLibrary.LogicLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectLayerClassServerLibrary.Presentation
{
    public abstract class ACurrenciesRatesChangeReporter : IObserver<ACurrenciesRates>
    {
        public abstract void Subscribe(IObservable<ACurrenciesRates>? provider);

        public abstract void OnCompleted();

        public abstract void OnError(Exception error);

        public abstract void OnNext(ACurrenciesRates value);

        public abstract void Unsubscribe();
    }
}
