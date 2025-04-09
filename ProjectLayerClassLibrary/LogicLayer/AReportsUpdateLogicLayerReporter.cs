using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectLayerClassLibrary.LogicLayer
{
    internal abstract class AReportsUpdateLogicLayerReporter : IObserver<bool>
    {
        public abstract void Subscribe(IObservable<bool>? provider);

        public abstract void OnCompleted();

        public abstract void OnError(Exception error);

        public abstract void OnNext(bool value);

        public abstract void Unsubscribe();
    }
}
