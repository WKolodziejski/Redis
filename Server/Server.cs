using System;

namespace Server
{
  public class Server : AsyncServer
  {
    public Server(string ipAddress, int port) : base(ipAddress, port)
    {
    }

    private void ParseData(string uuid, string data)
    {
      if (data.Equals("EXIT"))
      {
        CloseConnection(uuid);
      }
    }

    protected override void OnDataReceived(string uuid, string data)
    {
      Console.WriteLine($"{uuid}: {data}");
      ParseData(uuid, data);
      Write(uuid, data);
    }

    protected override void OnStartListening()
    {
      Console.WriteLine("Started listening...");
    }

    protected override void OnStopListening()
    {
      Console.WriteLine("Stopped listening...");
    }

    protected override void OnClientConnected(string uuid)
    {
      Console.WriteLine($"{uuid}: Connected");

      Write(uuid, "Welcome");
    }

    protected override void OnClientDisconnected(string uuid)
    {
      Console.WriteLine($"{uuid}: Disconnected");
    }

    protected override void OnError(Exception e)
    {
      // Console.Error.WriteLine(e);
    }
  }
}