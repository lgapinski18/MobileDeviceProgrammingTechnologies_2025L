using ProjectLayerClassServerLibrary.LogicLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectLayerClassServerLibrary.Presentation.Factory
{
    public static class WebSocketServerFactory
    {
        public static IWebSocketServer CreateWebSocketServer(int portNo, ALogicLayer logicLayer)
        {
            return new WebSocketServer(portNo, logicLayer);
        }
    }
}
