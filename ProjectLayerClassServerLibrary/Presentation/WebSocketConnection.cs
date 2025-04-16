using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectLayerClassServerLibrary.Presentation
{
    public abstract class WebSocketConnection : IDisposable
    {
        public enum ComunicationCodeFromServer
        {
            CODE_NOT_SELECTED,
            CREATE_ACCOUNT_OWNER_CODE,
            CREATE_BANK_ACCOUNT_CODE,
            GET_ACCOUNT_OWNER_CODE,
            GET_ACCOUNT_OWNER_LOGIN_CODE,
            GET_ALL_ACCOUNT_OWNERS_CODE,
            GET_BANK_ACCOUNT_CODE,
            GET_ALL_BANK_ACCOUNTS_CODE,
            GET_BANK_ACCOUNTS_CODE,
            AUTHENTICATE_ACCOUNT_OWNER_CODE,
            PERFORM_TRANSFER_CODE,
            REACTIVE_REPORTS_UPDATE_CODE,
            REACTIVE_BROADCAST_TO_FILTER_CURRENCY_UPDATE_CODE,
            CHECK_FOR_BANK_ACCOUNT_REPORTS_UPDATE_CODE,
            BANK_ACCOUNTS_UPDATES_CODE
        }

        public virtual Action<byte[]> onMessage { set; protected get; } = x => { };
        public virtual Action onClose { set; protected get; } = () => { };
        public virtual Action onError { set; protected get; } = () => { };

        public async Task SendAsync(ComunicationCodeFromServer messageType, int messageSequenceNo, int responseCode, string message)
        {
            byte[] messagesBytes = Encoding.UTF8.GetBytes(message);
            Console.WriteLine($"Sending:\nMessageType: {messageType}, messageSequenceNo {messageSequenceNo}, responseCode: {responseCode}\n{message}");
            byte[] header = BitConverter.GetBytes((int)messageType)
                                        .Concat(BitConverter.GetBytes(messageSequenceNo))
                                        .Concat(BitConverter.GetBytes(responseCode))
                                        .Concat(BitConverter.GetBytes(messagesBytes.Length))
                                        .ToArray();
            await SendTask(header, messagesBytes);
        }

        public abstract Task DisconnectAsync();

        protected abstract Task SendTask(byte[] header, byte[] message);

        public abstract void Dispose();

        public abstract int? LoggedOwnerId { get; set; }
    }
}
