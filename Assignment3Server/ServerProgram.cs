﻿using Assignment3Server;
using System.Net;
using System.Net.Sockets;
using System.Text.Json;

var port = 5000;

var server = new TcpListener(IPAddress.Loopback, port);
server.Start();

Console.WriteLine("Server started");

while (true)
{
    var client = server.AcceptTcpClient();
    Console.WriteLine("Client connected");

    try
    {
        var t = Task.Run(() => HandleClient(client));
    }
    catch (Exception)
    {
        Console.WriteLine("Unable to communicate with client ...");
    }

}

static void HandleClient(TcpClient client)
{
        string request = client.MyRead();

        var requestJson = JsonSerializer.Deserialize<Request>(request);

        if (string.IsNullOrEmpty(requestJson?.Method))
        {
            var response = new Response
            {
                Status = "4 Missing method"
            };

            var responseText = JsonSerializer.Serialize<Response>(response);
            client.MyWrite(responseText);
        }
    
}

