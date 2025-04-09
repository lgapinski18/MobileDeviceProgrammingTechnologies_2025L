using ProjectLayerClassServerLibrary.LogicLayer;
using ProjectLayerClassServerLibrary.Presentation.Factory;
using ProjectLayerClassServerLibrary.Presentation.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectLayerClassServerLibrary.Presentation
{
    public class SerwerApp
    {
        private static List<WebSocketConnection> connections = new();

        public static void Main(string[] args)
        {
            Console.WriteLine("Starting server on port 8080");
            IWebSocketServer server = WebSocketServerFactory.CreateWebSocketServer(8080, ALogicLayer.CreateLogicLayerInstance());
            Console.WriteLine("Server started on port 8080");
            while (true)
            {

            }
            Console.WriteLine("Closing serwer on port 8080");
        }
    }
}
