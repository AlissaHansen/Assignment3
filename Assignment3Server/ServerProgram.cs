using Assignment3Server;
using System.Net;
using System.Net.Sockets;

var port = 5000;

var server = new TcpListener(IPAddress.Loopback, port);
server.Start();

Console.WriteLine("Server started");

while(true)
{
    var client = server.AcceptTcpClient();
    Console.WriteLine("Client connected");
    HandleClient(client);
}

static void HandleClient(TcpClient tcpClient)
{
    string request = tcpClient.MyRead();

    Console.WriteLine($"Request: {request}");

    var response = request.ToUpper();

    tcpClient.MyWrite(response);
}

