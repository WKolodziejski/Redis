using System;
using System.Collections.Generic;
using Server.Operations;

namespace Server
{
  public class DataHandler
  {
    public delegate void WriteCallback(string id, string data);
    public delegate void DisconnectCallback(string id);

    private readonly Dictionary<string, Data> table;
    private readonly WriteCallback writeCallback;
    private readonly DisconnectCallback disconnectCallback;

    public DataHandler(Dictionary<string, Data> table, WriteCallback writeCallback, DisconnectCallback disconnectCallback)
    {
      this.table = table;
      this.writeCallback = writeCallback;
      this.disconnectCallback = disconnectCallback;
    }

    public void Handle(string id, Operation operation)
    {
      switch (operation)
      {
        case Get o:
          Get(id, o);
          break;

        case Set o:
          Set(id, o);
          break;

        case Del o:
          Del(id, o);
          break;
        
        case Quit _:
          Quit(id);
          break;

        case Error o:
          writeCallback(id, o.Message);
          break;
      }
    }

    private void Set(string id, Set o)
    {
      lock (table)
      {
        var exists = table.TryGetValue(o.Key, out var data);
        var nx = o.Replacement == "NX"; // Only set the key if it does not already exist.
        var xx = o.Replacement == "XX"; // Only set the key if it already exist.

        if (exists)
        {
          exists = !IsExpired(o.Key, data);
        }

        switch (exists)
        {
          case true when nx:
            writeCallback(id, "Value not inserted");
            return;
          
          case false when xx:
            writeCallback(id, "Value not inserted");
            return;
        }

        var duration = o.DurationUnity switch
        {
          "EX" => o.Duration * 1000,
          "PX" => o.Duration,
          _ => -1
        };

        var value = duration == -1 ? new Data(o.Value) : new Data(o.Value, duration);

        if (exists)
        {
          table[o.Key] = value;
        }
        else
        {
          table.Add(o.Key, value);
        }

        writeCallback(id, "+OK");
      }
    }

    private void Get(string id, Get o)
    {
      lock (table)
      {
        var exists = table.TryGetValue(o.Key, out var data);

        if (!exists)
        {
          writeCallback(id, "-1");
          return;
        }

        if (IsExpired(o.Key, data))
        {
          writeCallback(id, "-1");
        }
        else
        {
          writeCallback(id, $"${data.Value.Length}");
          writeCallback(id, data.Value);
        }
      }
    }
    
    private void Del(string id, Del o)
    {
      lock (table)
      {
        var count = 0;
        
        foreach (var key in o.Keys)
        {
          if (!table.ContainsKey(key))
          {
            continue;
          }

          table.Remove(key);
          count++;
        }
        
        writeCallback(id, $":{count}");
      }
    }
    
    private void Quit(string id)
    {
      writeCallback(id, "+OK");
      disconnectCallback(id);
    }

    private bool IsExpired(string key, Data data)
    {
      if (data.Expiration > DateTime.Now)
        return false;

      lock (table)
      {
        table.Remove(key);
      }

      return true;
    }
  }
}