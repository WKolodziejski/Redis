using System;
using System.Collections.Generic;
using System.Linq;
using Server.Actions;

namespace Server
{
  public class Server : AsyncServer
  {
    private readonly Dictionary<string, Data> _table;

    public Server(string ipAddress, int port) : base(ipAddress, port)
    {
      _table = new Dictionary<string, Data>();
    }

    protected override void OnDataReceived(string id, string data)
    {
      Console.WriteLine($"{id}: {data}");

      var actions = Handler.Handle(id, Parser.Parse(data), _table);

      var responses = (from Write a in actions.OfType<Write>() select a.Message).ToList();

      if (responses.Count > 0)
      {
        var response = string.Join("\n", responses);
        Send(id, response);
      }

      foreach (var a in actions.OfType<Disconnect>())
      {
        Disconnect(a.Id);
      }

      foreach (var a in actions.OfType<Error>())
      {
        Send(id, $"-ERR {a.GetType()} notImplemented");
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
      // Console.Error.WriteLine(e);
    }
  }
}