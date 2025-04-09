using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectLayerClassLibrary.LogicLayer
{
    public abstract class AReportsUpdateLogicLayerTracker : IObservable<List<ABankAccountReport>>
    {
        public abstract IDisposable Subscribe(IObserver<List<ABankAccountReport>> observer);

        public abstract void TrackWhetherReportsUpdatesChanged(List<ABankAccountReport> doesChangesAppeared);

        public abstract void EndAllObservations();
    }
}
