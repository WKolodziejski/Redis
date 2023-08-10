using System;
using System.Collections.Generic;
using Server.Actions;
using Server.Commands;
using Action = Server.Actions.Action;

namespace Server
{
  public abstract class Handler
  {
    public static List<Action> Handle(string id, Command command, Dictionary<string, Data> table)
    {
      return new List<Action>(command switch
      {
        Get o => Get(id, o, table),
        Set o => Set(id, o, table),
        Del o => Del(id, o, table),
        Exists o => Exists(id, o, table),
        Persist o => Persist(id, o, table),
        Quit _ => Quit(id),
        Ping _ => Ping(id),
        Unknown o => new Action[] { new Write(id, o.Message) },
        _ => new Action[] { new Error(id, command) }
      });
    }

    private static Action[] Set(string id, Set o, Dictionary<string, Data> table)
    {
      lock (table)
      {
        var nx = o.Replacement == "NX"; // Only set the key if it does not already exist.
        var xx = o.Replacement == "XX"; // Only set the key if it already exist.

        var exists = TryGetValue(o.Key, out _, table);

        switch (exists)
        {
          case true when nx:
            return new Action[] { new Write(id, "-1") };

          case false when xx:
            return new Action[] { new Write(id, "-1") };
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

        return new Action[] { new Write(id, "+OK") };
      }
    }

    private static Action[] Get(string id, Get o, Dictionary<string, Data> table)
    {
      lock (table)
      {
        return TryGetValue(o.Key, out var data, table)
          ? new Action[]
          {
            new Write(id, $"${data.Value.Length}"),
            new Write(id, data.Value),
          }
          : new Action[] { new Write(id, "-1") };
      }
    }

    private static Action[] Del(string id, Del o, Dictionary<string, Data> table)
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

        return new Action[] { new Write(id, $":{count}") };
      }
    }

    private static Action[] Exists(string id, Exists o, Dictionary<string, Data> table)
    {
      lock (table)
      {
        return TryGetValue(o.Key, out _, table)
          ? new Action[] { new Write(id, "1") }
          : new Action[] { new Write(id, "-1") };
      }
    }
    
    private static Action[] Persist(string id, Persist o, Dictionary<string, Data> table)
    {
      lock (table)
      {
        if (!TryGetValue(o.Key, out var data, table) || !data.Expires)
        {
          return new Action[] { new Write(id, ":0") };
        }

        data.Expires = false;
            
        return new Action[] { new Write(id, ":1") };
      }
    }

    private static Action[] Quit(string id)
    {
      return new Action[]
      {
        new Write(id, "+OK"),
        new Disconnect(id),
      };
    }

    private static Action[] Ping(string id)
    {
      return new Action[]
      {
        new Write(id, "+PONG")
      };
    }

    private static bool TryGetValue(string key, out Data value, Dictionary<string, Data> table)
    {
      lock (table)
      {
        var exists = table.TryGetValue(key, out var data);

        if (!exists)
        {
          value = null;
          return false;
        }

        if (data.Expiration > DateTime.Now)
        {
          value = data;
          return true;
        }

        table.Remove(key);

        value = null;
        return false;
      }
    }
  }
}