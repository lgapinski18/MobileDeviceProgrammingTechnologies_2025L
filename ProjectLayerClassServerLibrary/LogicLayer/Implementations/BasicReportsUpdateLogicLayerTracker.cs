using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectLayerClassServerLibrary.LogicLayer.Implementations
{
    internal class BasicReportsUpdateLogicLayerTracker : AReportsUpdateLogicLayerTracker
    {
        private List<IObserver<bool>> observers;

        public BasicReportsUpdateLogicLayerTracker()
        {
            observers = new List<IObserver<bool>>();
        }

        public override IDisposable Subscribe(IObserver<bool> observer)
        {
            if (!observers.Contains(observer))
                observers.Add(observer);
            return new Unsubscriber(observers, observer);
        }

        private class Unsubscriber : IDisposable
        {
            private List<IObserver<bool>> _observers;
            private IObserver<bool> _observer;

            public Unsubscriber(List<IObserver<bool>> observers, IObserver<bool> observer)
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

        public override void TrackWhetherReportsUpdatesChanged(bool doesChangesAppeared)
        {
            foreach (var observer in observers)
            {
                observer.OnNext(doesChangesAppeared);
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
    }
}
