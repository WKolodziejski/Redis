using System;

namespace Client
{
  public class Client : AsyncClient
  {
    public Client(string ipAddress, int port) : base(ipAddress, port)
    {
    }

    protected override void OnStartListening()
    {
      Console.WriteLine("Started listening...");
    }

    protected override void OnStopListening()
    {
      Console.WriteLine("Stopped listening...");
    }

    protected override void OnDataReceived(string data)
    {
      Console.WriteLine($"Received: {data}");
    }

    protected override void OnError(Exception e)
    {
      // Console.Error.WriteLine(e);
    }
  }
}