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
            WebSocketServer task = new WebSocketServer(8080);
            Console.WriteLine("Server started on port 8080");
            while (!task.IsCompleted)
            {

            }
            Console.WriteLine("Closing serwer on port 8080");
        }

        private static void OnConnection(WebSocketConnection connection)
        {
            Console.WriteLine($"New Connection: {connection}");
            connections.Add(connection);
            connection.onClose = () => connections.Remove(connection);
            connection.onError = () =>
            {
                Console.WriteLine("Error happened");
            };

            connection.onMessage = message =>
            {
                Console.WriteLine(message);
            };
        }
    }
}
