using System;

namespace Client
{
  public class Client : AsyncClient
  {
    private string lastData = "";
    
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
      Console.WriteLine($"{data}");

      if (lastData.ToUpper() == "QUIT" && data == "+OK")
      {
        Stop();
      }
    }

    protected override void OnDataWritten(string data)
    {
      lastData = data;
    }

    protected override void OnError(Exception e)
    {
      Console.Error.WriteLine(e);
    }
  }
}