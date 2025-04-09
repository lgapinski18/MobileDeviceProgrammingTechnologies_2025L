using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectLayerClassLibrary.PresentationLayer.ModelLayer
{
    internal abstract class AReportsUpdateModelLayerReporter : IObserver<List<LogicLayer.ABankAccountReport>>
    {
        public abstract void Subscribe(IObservable<List<LogicLayer.ABankAccountReport>>? provider);

        public abstract void OnCompleted();

        public abstract void OnError(Exception error);

        public abstract void OnNext(List<LogicLayer.ABankAccountReport> value);

        public abstract void Unsubscribe();
    }
}
