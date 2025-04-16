using ComunicationApiXmlDto;
using ProjectLayerClassServerLibrary.LogicLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ProjectLayerClassServerLibrary.Presentation.Implementations
{
    internal class BasicCurrenciesRatesChangeReporter : ACurrenciesRatesChangeReporter
    {
        public delegate void OnObservationComplitionDelegate();
        public delegate void OnReportsUpdateObservationDelegate(ACurrenciesRates value);

        private IDisposable? unsubscriber = null;
        private OnObservationComplitionDelegate onObservationComplitionDelegate;
        private WebSocketConnection connection;
        private ALogicLayer logicLayer;
        private Task? sendTask = null;

        public BasicCurrenciesRatesChangeReporter(WebSocketConnection connection, ALogicLayer logicLayer, OnObservationComplitionDelegate onObservationComplitionDelegate)
        {
            this.connection = connection;
            this.logicLayer = logicLayer;
            this.onObservationComplitionDelegate = onObservationComplitionDelegate;
        }

        public override void Subscribe(IObservable<ACurrenciesRates>? provider)
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

        public override void OnNext(ACurrenciesRates value)
        {
            if (connection.LoggedOwnerId != null)
            {
                Console.WriteLine($"ReportsUpdate sending to: {connection}");
                XmlSerializer serializer = new XmlSerializer(typeof(CurrenciesPurchaseSellRateDto));
                CurrenciesPurchaseSellRateDto reports = new();
                reports.PurchaseEuro = value.EuroPurchaseRate;
                reports.SellEuro = value.EuroSellRate;
                reports.PurchaseUsd = value.UsdPurchaseRate;
                reports.SellUsd = value.UsdSellRate;
                reports.PurchaseGbp = value.GbpPurchaseRate;
                reports.SellGbp = value.GbpSellRate;
                reports.PurchaseChf = value.ChfPurchaseRate;
                reports.SellChf = value.ChfSellRate;


                StringWriter writer = new StringWriter();
                serializer.Serialize(writer, reports);
                sendTask = connection.SendAsync(WebSocketConnection.ComunicationCodeFromServer.REACTIVE_BROADCAST_TO_FILTER_CURRENCY_UPDATE_CODE, 0, 0, writer.ToString());
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
