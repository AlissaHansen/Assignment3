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

    string statusCode = "";
    string statusBody = "";
    
    var request = client.MyRead();

    var requestJson = JsonSerializer.Deserialize<Request>(request);

    if (string.IsNullOrEmpty(requestJson?.Method))
    {
        statusCode = "4";
        statusBody += " Missing method";
    }

    if (requestJson?.Method != "create" && requestJson?.Method != "read" &&  requestJson?.Method != "update" 
        && requestJson?.Method != "delete" && requestJson?.Method != "echo")
    {
        statusCode = "4";
        statusBody += " Illegal method";
    }

    if (requestJson?.Method == "create" || requestJson?.Method == "read" || requestJson?.Method == "update" 
        || requestJson?.Method == "delete")
    {
        if (string.IsNullOrEmpty(requestJson?.Path))
        {
            statusCode = "4";
            statusBody += " Missing resource";
        }
    }
    
    if (request == "create" || request == "update" || request == "delete")
    {
        if (string.IsNullOrEmpty(requestJson?.Body))
        {
            statusCode = "4";
            statusBody += " Missing resource";
        }
    }
    
    //Test om request bliver lavet til json
    Console.WriteLine(request);
    Console.WriteLine(requestJson?.Method);
    
    //send response
    var response = new Response
        {
            Status = statusCode + " " + statusBody
        };

        var responseText = JsonSerializer.Serialize<Response>(response);
        client.MyWrite(responseText);
    }


