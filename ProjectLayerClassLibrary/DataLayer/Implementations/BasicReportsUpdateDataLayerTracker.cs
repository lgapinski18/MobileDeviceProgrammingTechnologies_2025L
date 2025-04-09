using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectLayerClassLibrary.DataLayer.Implementations
{
    public class BasicReportsUpdateDataLayerTracker : AReportsUpdateDataLayerTracker
    {
        private List<IObserver<List<ABankAccountReport>>> observers;

        public BasicReportsUpdateDataLayerTracker()
        {
            observers = new List<IObserver<List<ABankAccountReport>>>();
        }

        public override IDisposable Subscribe(IObserver<List<ABankAccountReport>> observer)
        {
            if (!observers.Contains(observer))
                observers.Add(observer);
            return new Unsubscriber(observers, observer);
        }

        private class Unsubscriber : IDisposable
        {
            private List<IObserver<List<ABankAccountReport>>> _observers;
            private IObserver<List<ABankAccountReport>> _observer;

            public Unsubscriber(List<IObserver<List<ABankAccountReport>>> observers, IObserver<List<ABankAccountReport>> observer)
            {
                this._observers = observers;
                this._observer = observer;
            }

            public void Dispose()
            {
                if (_observer != null && _observers.Contains(_observer))
                    _observers.Remove(_observer);
            }
        }

        public override void TrackWhetherReportsUpdatesChanged(List<ABankAccountReport> doesChangesAppeared)
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
