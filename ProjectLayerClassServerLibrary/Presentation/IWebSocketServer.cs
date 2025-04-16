using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectLayerClassServerLibrary.Presentation
{
    public interface IWebSocketServer
    {
        public bool IsRunning { get; }
        public Task Finish();
    }
}
