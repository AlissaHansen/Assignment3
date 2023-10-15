using Assignment3Server;
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

    string statusCode;
    string statusBody;
    
    string request = client.MyRead();

    var requestJson = JsonSerializer.Deserialize<Request>(request);

    if (string.IsNullOrEmpty(requestJson?.Method))
    {
        statusCode = "4";
        statusBody = "Missing method";
        var response = new Response
        {
            Status = statusCode + " " + statusBody
        };

        var responseText = JsonSerializer.Serialize<Response>(response);
        client.MyWrite(responseText);
    }
}

