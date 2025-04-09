using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectLayerClassLibrary.DataLayer
{
    public abstract class AReportsUpdateDataLayerTracker : IObservable<List<ABankAccountReport>>
    {
        public abstract IDisposable Subscribe(IObserver<List<ABankAccountReport>> observer);

        public abstract void TrackWhetherReportsUpdatesChanged(List<ABankAccountReport> doesChangesAppeared);

        public abstract void EndAllObservations();
    }
}
