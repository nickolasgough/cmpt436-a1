using System;
using System.Net;
using System.Net.Sockets;
using System.IO;

using server;

namespace program
{
    class Program
    {
        static void Main(string[] args)
        {
            Server server = new Server();
            server.Run();
        }
    }
}
