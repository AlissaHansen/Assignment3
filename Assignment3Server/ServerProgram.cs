﻿using Assignment3Server;
using System.Net;
using System.Net.Sockets;
using System.Text.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using JsonSerializer = System.Text.Json.JsonSerializer;

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
    string body = "";

    var requestJson = client.MyRead();
    var request = JsonSerializer.Deserialize<Request>(requestJson,
        new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

    Console.WriteLine(requestJson);

    if (string.IsNullOrEmpty(request?.Method))
    {
        statusCode = "4";
        statusBody += " Missing method";
    }

    if (request?.Method != "create" && request?.Method != "read" && request?.Method != "update"
        && request?.Method != "delete" && request?.Method != "echo")
    {
        statusCode = "4";
        statusBody += " Illegal method";
    }

    if (request?.Method == "create" || request?.Method == "read" || request?.Method == "update"
        || request?.Method == "delete")
    {
        if (string.IsNullOrEmpty(request?.Path))
        {
            statusCode = "4";
            statusBody += " Missing resource";
        }
    }

    if (request?.Method == "create" || request?.Method == "update" || request?.Method == "delete" ||
        request?.Method == "echo")
    {
        if (string.IsNullOrEmpty(request?.Body))
        {
            statusCode = "4";
            statusBody += " Missing body";
        }
    }

    if (string.IsNullOrEmpty(request?.Date))
    {
        statusCode = "4";
        statusBody += " Missing date";
    }

    if (!IsValidUnixTime(request?.Date))
    {
        statusCode = "4";
        statusBody += " Illegal date";
    }

    if (!string.IsNullOrEmpty(request?.Body))
    {
        if (!IsValidJson(request?.Body))
        {
            statusCode = "4";
            statusBody += " Illegal body";
        }
    }

    if (request?.Method == "echo" && IsValidUnixTime(request.Date))
    {
        if (!string.IsNullOrEmpty(request?.Body))
        {
            statusCode = "1";
            body = request.Body;
        }
    }


//send response
    var response = new Response
        {
            Status = statusCode + " " + statusBody,
            Body = body
        };

        var responseText = JsonSerializer.Serialize<Response>(response);
        client.MyWrite(responseText);
    }

static bool IsValidUnixTime(string timeInput)
{
    return int.TryParse(timeInput, out _);
}

static bool IsValidJson(string strInput)
{
    if (string.IsNullOrWhiteSpace(strInput)) { return false;}
    strInput = strInput.Trim();
    if ((strInput.StartsWith("{") && strInput.EndsWith("}")) || //For object
        (strInput.StartsWith("[") && strInput.EndsWith("]"))) //For array
    {
        try
        {
            var obj = JToken.Parse(strInput);
            return true;
        }
        catch (JsonReaderException jex)
        {
            //Exception in parsing json
            Console.WriteLine(jex.Message);
            return false;
        }
        catch (Exception ex) //some other exception
        {
            Console.WriteLine(ex.ToString());
            return false;
        }
    }
    else
    {
        return false;
    }
}
