using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectLayerClassLibrary.DataLayer.Implementations
{
    public class BasicReportsUpdateDataLayerTracker : AReportsUpdateDataLayerTracker
    {
        private List<IObserver<bool>> observers;

        public BasicReportsUpdateDataLayerTracker()
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
                this._observers = observers;
                this._observer = observer;
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
