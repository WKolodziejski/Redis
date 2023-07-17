using System;
using System.Collections.Generic;
using Server.Operations;

namespace Server
{
  public class Server : AsyncServer
  {
    private readonly Dictionary<string, DataParser> parsers;
    private readonly Dictionary<string, DataHandler> handlers;
    private readonly Dictionary<string, Data> table;

    public Server(string ipAddress, int port) : base(ipAddress, port)
    {
      parsers = new Dictionary<string, DataParser>();
      handlers = new Dictionary<string, DataHandler>();
      table = new Dictionary<string, Data>();
    }

    private void OnDataParsed(string id, Operation operation)
    {
      var handler = handlers[id];
      handler.Handle(id, operation);
    }
    
    protected override void OnDataReceived(string id, string data)
    {
      Console.WriteLine($"{id}: {data.Trim()}");
      
      var parser = parsers[id];
      parser.Parse(data.Trim());
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

      Write(id, "Welcome");

      lock (parsers)
      {
        parsers.Add(id, new DataParser(id, OnDataParsed));
      }

      lock (handlers)
      {
        handlers.Add(id, new DataHandler(table, Write));
      }
    }

    protected override void OnClientDisconnected(string id)
    {
      Console.WriteLine($"{id}: Disconnected");

      lock (parsers)
      {
        parsers.Remove(id);
      }
      
      lock (handlers)
      {
        handlers.Remove(id);
      }
    }

    protected override void OnError(Exception e)
    {
      Console.Error.WriteLine(e);
    }
  }
}