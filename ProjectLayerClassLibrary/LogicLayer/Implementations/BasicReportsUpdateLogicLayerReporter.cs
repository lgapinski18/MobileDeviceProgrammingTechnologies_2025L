using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectLayerClassLibrary.LogicLayer.Implementations
{
    internal class BasicReportsUpdateLogicLayerReporter : AReportsUpdateLogicLayerReporter
    {
        private IDisposable? unsubscriber = null;
        private AReportsUpdateLogicLayerTracker tracker;

        public BasicReportsUpdateLogicLayerReporter(AReportsUpdateLogicLayerTracker tracker)
        {
            this.tracker = tracker;
        }

        public override void Subscribe(IObservable<List<DataLayer.ABankAccountReport>>? provider)
        {
            if (provider != null)
            {
                Unsubscribe();
                unsubscriber = provider.Subscribe(this);
            }
        }

        public override void OnCompleted()
        {
            tracker.EndAllObservations();
        }

        public override void OnError(Exception error)
        {
            return;
        }

        public override void OnNext(List<DataLayer.ABankAccountReport> value)
        {
            tracker.TrackWhetherReportsUpdatesChanged(value.Select(report => LogicLayer.ABankAccountReport.CreateBankAccountReport(report)).ToList());
        }

        public override void Unsubscribe()
        {
            if (unsubscriber != null)
            {
                unsubscriber.Dispose();
            }
        }
    }
}
