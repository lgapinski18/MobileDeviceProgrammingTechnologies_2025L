using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectLayerClassServerLibrary.Presentation
{
    public abstract class WebSocketConnection
    {
        public virtual Action<string> onMessage { set; protected get; } = x => { };
        public virtual Action onClose { set; protected get; } = () => { };
        public virtual Action onError { set; protected get; } = () => { };

        public async Task SendAsync(string messageType, int messageSequenceNo, int responseCode, string message)
        {
            byte[] header = Encoding.ASCII.GetBytes("_CAO")
                                            .Concat(BitConverter.GetBytes(messageSequenceNo))
                                            .Concat(BitConverter.GetBytes(responseCode))
                                            .Concat(BitConverter.GetBytes(message.Length))
                                            .ToArray();
            await SendTask(header, message);
        }

        public abstract Task DisconnectAsync();

        protected abstract Task SendTask(byte[] header, string message);
    }
}
