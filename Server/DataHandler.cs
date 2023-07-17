using System;
using System.Collections.Generic;
using Server.Operations;

namespace Server
{
  public class DataHandler
  {
    public delegate void WriteCallback(string id, string data);

    private readonly Dictionary<string, Data> table;
    private readonly WriteCallback writeCallback;

    public DataHandler(Dictionary<string, Data> table, WriteCallback writeCallback)
    {
      this.table = table;
      this.writeCallback = writeCallback;
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
            writeCallback(id, "+OK");
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
          writeCallback(id, "null");
          return;
        }

        writeCallback(id, IsExpired(o.Key, data) ? "null" : data.Value);
      }
    }

    // TODO: aceitar várias keys e retornar quantidade de removidas
    private void Del(string id, Del o)
    {
      lock (table)
      {
        if (!table.ContainsKey(o.Key))
        {
          writeCallback(id, ":0");
          return;
        }

        table.Remove(o.Key);

        writeCallback(id, ":1");
      }
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