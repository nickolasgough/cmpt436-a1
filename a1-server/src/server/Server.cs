using System;
using System.Net;
using System.Net.Sockets;
using System.IO;

using System.Threading;
using System.Collections.Generic;

using service;

namespace server
{
    public class Server
    {
        public void Run()
        {
            IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
            int port = 8080;
            TcpListener server = new TcpListener(ipAddress, port);
            server.Start();
            Console.WriteLine("Server listening on port " + port);

            Service service = new Service();

            while (true) {
                Console.WriteLine("Waiting for a socket connection...");
                TcpClient client = server.AcceptTcpClient();
                Console.WriteLine("Received a socket connection...");

                Thread thread = new Thread(() => {
                    try {
                        this.Connect(client, service);
                    } catch (Exception e) {
                        Console.WriteLine("Socket connection to client lost...");
                        Console.WriteLine(e.Message);
                    }
                });
                thread.Start();
            }
        }

        private void Connect(TcpClient client, Service service) {
            StreamReader reader = new StreamReader(client.GetStream());
            StreamWriter writer = new StreamWriter(client.GetStream());

            string user = null;
            string command = null;
            bool success = false;
            string argument1 = null;
            string argument2 = null;
            List<string> results = null;

            Console.WriteLine("Waiting for user registration...");
            command = reader.ReadLine();
            user = reader.ReadLine();
            Console.WriteLine("Server received user: " + user);
            success = service.CreateUser(user);
            Console.WriteLine("Server responding with: " + success);
            writer.WriteLine(success);
            writer.Flush();
            while (!success) {
                Console.WriteLine("Waiting for user registration...");
                command = reader.ReadLine();
                user = reader.ReadLine();
                Console.WriteLine("Server received user: " + user);
                success = service.CreateUser(user);
                Console.WriteLine("Server responding with: " + success);
                writer.WriteLine(success);
                writer.Flush();
            }

            try {
                while (client.Connected) {
                    Console.WriteLine("Waiting for a command...");
                    command = reader.ReadLine();
                    Console.WriteLine("Server received command: " + command);

                    switch (command) {
                        case "create":
                            argument1 = reader.ReadLine();
                            Console.WriteLine("Server recieved argument: " + argument1);

                            lock (service) {
                                success = service.CreateRoom(argument1);
                            }
                            Console.WriteLine("Server responding with: " + success);
                            writer.WriteLine(success);
                            writer.Flush();
                            break;
                        case "join":
                            argument1 = reader.ReadLine();
                            Console.WriteLine("Server received argument: " + argument1);
                            argument2 = reader.ReadLine();
                            Console.WriteLine("Server received argument: " + argument2);

                            lock (service) {
                                results = service.JoinRoom(argument1, argument2);
                            }
                            if (results == null) {
                                Console.WriteLine("Server responding with: " + false);
                                writer.WriteLine(false);
                                writer.Flush();
                                break;
                            }

                            Console.WriteLine("Server responding with: " + true);
                            writer.WriteLine(true);
                            Console.WriteLine("Server responding with: " + results.Count);
                            writer.WriteLine(results.Count);
                            Console.WriteLine("Server responding with: ");
                            for (int c = 0; c < results.Count; c += 1) {
                                Console.WriteLine(results[c]);
                                writer.WriteLine(results[c]);
                            }
                            writer.Flush();
                            break;
                        case "leave":
                            argument1 = reader.ReadLine();
                            Console.WriteLine("Server recieved argument: " + argument1);

                            lock(service) {
                                success = service.LeaveRoom(argument1);
                            }
                            Console.WriteLine("Server responding with: " + success);
                            writer.WriteLine(success);
                            writer.Flush();
                            break;
                        case "list":
                            lock (service) {
                                results = service.ListRooms();
                            }
                            if (results == null) {
                                Console.WriteLine("Server responding with: " + false);
                                writer.WriteLine(false);
                                writer.Flush();
                                break;
                            }

                            Console.WriteLine("Server responding with: " + true);
                            writer.WriteLine(true);
                            Console.WriteLine("Server responding with: " + results.Count);
                            writer.WriteLine(results.Count);
                            Console.WriteLine("Server responding with: ");
                            for (int c = 0; c < results.Count; c += 1) {
                                Console.WriteLine(results[c]);
                                writer.WriteLine(results[c]);
                            }
                            writer.Flush();
                            break;
                        case "send":
                            argument1 = reader.ReadLine();
                            Console.WriteLine("Server received argument: " + argument1);
                            argument2 = reader.ReadLine();
                            Console.WriteLine("Server received argument: " + argument2);

                            lock (service) {
                                success = service.SendMessage(argument1, argument2);
                            }
                            Console.WriteLine("Server responding with: " + success);
                            writer.WriteLine(success);
                            writer.Flush();
                            break;
                        case "refresh":
                            argument1 = reader.ReadLine();
                            Console.WriteLine("Server received argument: " + argument1);

                            lock (service) {
                                results = service.RefreshMessages(argument1);
                            }
                            if (results == null) {
                                Console.WriteLine("Server responding with: " + false);
                                writer.WriteLine(false);
                                writer.Flush();
                                break;
                            }

                            Console.WriteLine("Server responding with: " + true);
                            writer.WriteLine(true);
                            Console.WriteLine("Server responding with: " + results.Count);
                            writer.WriteLine(results.Count);
                            Console.WriteLine("Server responding with: ");
                            for (int c = 0; c < results.Count; c += 1) {
                                Console.WriteLine(results[c]);
                                writer.WriteLine(results[c]);
                            }
                            writer.Flush();
                            break;
                        default:
                            Console.WriteLine("Server responding with: " + false);
                            writer.WriteLine(false);
                            writer.Flush();
                            Console.WriteLine("error: server - given command is unkown");
                            break;
                    }
                }
            } catch (Exception e) {
                Console.WriteLine("Socket connection to client lost...");
                Console.WriteLine(e.Message);

                service.LeaveRoom(user);
                service.DeleteUser(user);
            }
        }
    }
}
