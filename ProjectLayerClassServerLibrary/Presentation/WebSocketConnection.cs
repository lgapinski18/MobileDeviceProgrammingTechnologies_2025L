﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectLayerClassServerLibrary.Presentation
{
    public abstract class WebSocketConnection
    {
        public virtual Action<byte[]> onMessage { set; protected get; } = x => { };
        public virtual Action onClose { set; protected get; } = () => { };
        public virtual Action onError { set; protected get; } = () => { };

        public async Task SendAsync(string messageType, int messageSequenceNo, int responseCode, string message)
        {
            Console.WriteLine($"Sending:\nMessageType: {messageType}, messageSequenceNo {messageSequenceNo}, responseCode: {responseCode}\n{message}");
            byte[] header = Encoding.ASCII.GetBytes(messageType)
                                            .Concat(BitConverter.GetBytes(messageSequenceNo))
                                            .Concat(BitConverter.GetBytes(responseCode))
                                            .Concat(BitConverter.GetBytes(message.Length))
                                            .ToArray();
            await SendTask(header, message);
        }

        public abstract Task DisconnectAsync();

        protected abstract Task SendTask(byte[] header, string message);

        public abstract int? LoggedOwnerId { get; set; }
    }
}
