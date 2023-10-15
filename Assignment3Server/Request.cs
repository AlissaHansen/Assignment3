namespace Assignment3Server;

public class Request
{
    public string? Method { get; set; }
    public string? Path { get; set; }
    public int Date { get; set; }
    public string? Body { get; set; }
}