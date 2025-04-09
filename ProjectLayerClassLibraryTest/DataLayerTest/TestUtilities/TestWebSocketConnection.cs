using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectLayerClassLibraryTest.DataLayerTest.TestUtilities
{
    internal abstract class TestWebSocketConnection
    {
        public virtual Action<byte[], int> onMessage { set; protected get; } = (bytes, count) => { };
        public virtual Action onClose { set; protected get; } = () => { };
        public virtual Action onError { set; protected get; } = () => { };

        public async Task SendAsync(byte[] message)
        {
            await SendTask(message);
        }

        public abstract Task DisconnectAsync();

        protected abstract Task SendTask(byte[] message);
    }
}
