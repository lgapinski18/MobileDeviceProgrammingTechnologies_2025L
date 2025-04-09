using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectLayerClassLibrary.PresentationLayer.ModelLayer.Implementations
{
    internal class BasicReportsUpdateModelLayerReporter : AReportsUpdateModelLayerReporter
    {
        public delegate void OnObservationComplitionDelegate();
        public delegate void OnReportsUpdateObservationDelegate(List<LogicLayer.ABankAccountReport> value);

        private IDisposable? unsubscriber = null;
        private OnObservationComplitionDelegate onObservationComplitionDelegate;
        private OnReportsUpdateObservationDelegate onReportsUpdateObservationDelegate;

        public BasicReportsUpdateModelLayerReporter(OnObservationComplitionDelegate onObservationComplitionDelegate, OnReportsUpdateObservationDelegate onReportsUpdateObservationDelegate)
        {
            this.onObservationComplitionDelegate = onObservationComplitionDelegate;
            this.onReportsUpdateObservationDelegate = onReportsUpdateObservationDelegate;
        }

        public override void Subscribe(IObservable<List<LogicLayer.ABankAccountReport>>? provider)
        {
            if (provider != null)
            {
                Unsubscribe();
                unsubscriber = provider.Subscribe(this);
            }
        }

        public override void OnCompleted()
        {
            onObservationComplitionDelegate();
        }

        public override void OnError(Exception error)
        {
            return;
        }

        public override void OnNext(List<LogicLayer.ABankAccountReport> value)
        {
            onReportsUpdateObservationDelegate(value);
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
