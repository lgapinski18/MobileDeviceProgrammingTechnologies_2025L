using ProjectLayerClassServerLibrary.LogicLayer;
using ProjectLayerClassServerLibrary.Presentation;
using ProjectLayerClassServerLibrary.Presentation.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ProjectLayerClassLibrary.PresentationLayer.ModelLayer.Implementations
{
    internal class BasicReportsUpdateModelLayerReporter : AReportsUpdateModelLayerReporter
    {
        public delegate void OnObservationComplitionDelegate();
        public delegate void OnReportsUpdateObservationDelegate(bool value);

        private IDisposable? unsubscriber = null;
        private OnObservationComplitionDelegate onObservationComplitionDelegate;
        private WebSocketConnection connection;
        private ALogicLayer logicLayer;
        private Task? sendTask = null;

        public BasicReportsUpdateModelLayerReporter(WebSocketConnection connection, ALogicLayer logicLayer, OnObservationComplitionDelegate onObservationComplitionDelegate)
        {
            this.connection = connection;
            this.logicLayer = logicLayer;
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
                XmlSerializer serializer = new XmlSerializer(typeof(List<BankAccountReportDto>));
                List<BankAccountReportDto> reports = new();
                foreach (ABankAccount bankAccount in logicLayer.GetAccountOwnerBankAccounts(connection.LoggedOwnerId.Value))
                {
                    ABankAccountReport report = bankAccount.GetBankAccountReports().Last();
                    BankAccountReportDto reportDto = new BankAccountReportDto()
                    {
                        TimeOfReportCreation = report.TimeOfReportCreation,
                        CurrentAccountBalance = report.CurrentAccountBalance,
                        PreviousAccountBalance = report.PreviousAccountBalance,
                        OwnerName = report.OwnerName,
                        OwnerSurname = report.OwnerSurname,
                        OwnerEmail = report.OwnerEmail,
                    };
                    reports.Add(reportDto);
                }
                StringWriter writer = new StringWriter();
                serializer.Serialize(writer, reports);
                sendTask = connection.SendAsync(WebSocketConnection.ComunicationCodeFromServer.REACTIVE_REPORTS_UPDATE_CODE, 0, 0, writer.ToString());
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
