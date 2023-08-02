using System;
using System.Collections.Generic;
using Server.Actions;
using Server.Commands;
using Action = Server.Actions.Action;

namespace Server
{
  public abstract class Handler
  {
    public static IEnumerable<Action> Handle(string id, Command command, Dictionary<string, Data> table)
    {
      return command switch
      {
        Get o => Get(id, o, table),
        Set o => Set(id, o, table),
        Del o => Del(id, o, table),
        Quit _ => Quit(id),
        Error o => new Action[] { new Write(id, o.Message) },
        _ => new Action[0]
      };
    }

    private static Action[] Set(string id, Set o, Dictionary<string, Data> table)
    {
      lock (table)
      {
        var exists = table.TryGetValue(o.Key, out var data);
        var nx = o.Replacement == "NX"; // Only set the key if it does not already exist.
        var xx = o.Replacement == "XX"; // Only set the key if it already exist.

        if (exists)
        {
          if (data.Expiration <= DateTime.Now)
          {
            table.Remove(o.Key);
            exists = false;
          }
        }

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
        var exists = table.TryGetValue(o.Key, out var data);

        if (!exists)
        {
          return new Action[] { new Write(id, "-1") };
        }

        if (data.Expiration > DateTime.Now)
        {
          return new Action[]
          {
            new Write(id, $"${data.Value.Length}"),
            new Write(id, data.Value),
          };
        }

        table.Remove(o.Key);

        return new Action[] { new Write(id, "-1") };
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

    private static Action[] Quit(string id)
    {
      return new Action[]
      {
        new Write(id, "+OK"),
        new Disconnect(id),
      };
    }
  }
}