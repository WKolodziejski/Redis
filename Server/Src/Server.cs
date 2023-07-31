using System;
using System.Collections.Generic;

namespace Server
{
  public class Server : AsyncServer
  {
    public Dictionary<string, Data> Table { get; }
    
    public Server(string ipAddress, int port) : base(ipAddress, port)
    {
      Table = new Dictionary<string, Data>();
    }

    protected override void OnDataReceived(string id, string data)
    {
      Console.WriteLine($"{data}");
      var actions = Handler.Handle(id, Parser.Parse(data), Table);

      foreach (var action in actions)
      {
        switch (action)
        {
          case Actions.Write a:
            Write(a.Id, a.Message);
            break;

          case Actions.Disconnect a:
            Disconnect(a.Id);
            break;
        }
      }
    }

    protected override void OnStartListening()
    {
      Console.WriteLine("Started listening...");
    }

    protected override void OnStopListening()
    {
      Console.WriteLine("Stopped listening...");
    }

    protected override void OnClientConnected(string id)
    {
      Console.WriteLine($"{id}: Connected");
    }

    protected override void OnClientDisconnected(string id)
    {
      Console.WriteLine($"{id}: Disconnected");
    }

    protected override void OnError(Exception e)
    {
      Console.Error.WriteLine(e);
    }
  }
}