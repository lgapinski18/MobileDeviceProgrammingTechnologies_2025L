﻿using System;
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
            WebSocketServer server = new WebSocketServer(8080);
            Console.WriteLine("Server started on port 8080");
            while (server.IsRunning)
            {

            }
            Console.WriteLine("Closing serwer on port 8080");
        }
    }
}
