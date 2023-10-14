﻿using Assignment3Server;
using System.Net;
using System.Net.Sockets;

var port = 5000;

var server = new TcpListener(IPAddress.Loopback, port);
server.Start();

Console.WriteLine("Server started");

while (true)
{
    var client = server.AcceptTcpClient();
    Console.WriteLine("Client connected");

    var t = Task.Run(() => HandleClient(client));
    
}

static void HandleClient(TcpClient client)
{
    try
    {
        string request = client.MyRead();

        Console.WriteLine($"Request: {request}");

        var response = request.ToUpper();

        client.MyWrite(response);
    }
    catch (Exception)
    {
        Console.WriteLine("Unable to connect to client ...");
    }
}

