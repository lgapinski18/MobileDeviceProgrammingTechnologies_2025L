﻿using ProjectLayerClassServerLibrary.Presentation;
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
        public delegate void OnReportsUpdateObservationDelegate(bool value);

        private IDisposable? unsubscriber = null;
        private OnObservationComplitionDelegate onObservationComplitionDelegate;
        private WebSocketConnection connection;
        private Task? sendTask = null;

        public BasicReportsUpdateModelLayerReporter(WebSocketConnection connection, OnObservationComplitionDelegate onObservationComplitionDelegate)
        {
            this.connection = connection;
            this.onObservationComplitionDelegate = onObservationComplitionDelegate;
        }

        public override void Subscribe(IObservable<bool>? provider)
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

        public override void OnNext(bool value)
        {
            if (value && connection.LoggedOwnerId != null)
            {
                Console.WriteLine($"ReportsUpdate sending to: {connection}");
                sendTask = connection.SendAsync("_RRU", 0, 0, "");
            }
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
