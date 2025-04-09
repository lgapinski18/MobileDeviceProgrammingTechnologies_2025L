using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectLayerClassLibrary.DataLayer
{
    public abstract class AReportsUpdateDataLayerTracker : IObservable<bool>
    {
        public abstract IDisposable Subscribe(IObserver<bool> observer);

        public abstract void TrackWhetherReportsUpdatesChanged(bool doesChangesAppeared);

        public abstract void EndAllObservations();
    }
}
