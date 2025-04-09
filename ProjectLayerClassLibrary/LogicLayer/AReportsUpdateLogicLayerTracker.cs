using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectLayerClassLibrary.LogicLayer
{
    public abstract class AReportsUpdateLogicLayerTracker : IObservable<bool>
    {
        public abstract IDisposable Subscribe(IObserver<bool> observer);

        public abstract void TrackWhetherReportsUpdatesChanged(bool doesChangesAppeared);

        public abstract void EndAllObservations();
    }
}
