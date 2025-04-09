using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectLayerClassLibrary.LogicLayer
{
    internal abstract class AReportsUpdateLogicLayerReporter : IObserver<List<DataLayer.ABankAccountReport>>
    {
        public abstract void Subscribe(IObservable<List<DataLayer.ABankAccountReport>>? provider);

        public abstract void OnCompleted();

        public abstract void OnError(Exception error);

        public abstract void OnNext(List<DataLayer.ABankAccountReport> value);

        public abstract void Unsubscribe();
    }
}
