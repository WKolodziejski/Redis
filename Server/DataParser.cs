using System;
using System.Collections.Generic;
using System.Linq;
using Server.Operations;

namespace Server
{
  public class DataParser
  {
    public delegate void OnDataParsed(string id, Operation o);

    private readonly string id;
    private readonly OnDataParsed onDataParsed;

    public DataParser(string id, OnDataParsed onDataParsed)
    {
      this.id = id;
      this.onDataParsed = onDataParsed;
    }

    public void Parse(string data)
    {
      var tokens = data.Split(' ');

      if (tokens.Length == 0)
        return;

      var o = tokens[0].ToUpper() switch
      {
        "GET" => Get(tokens),
        "SET" => Set(tokens),
        "DEL" => Del(tokens),
        "QUIT" => Quit(tokens),
        _ => new Error($"-ERR unknown command '{tokens[0]}'")
      };

      onDataParsed(id, o);
    }

    private static Operation Get(IList<string> args)
    {
      if (args.Count != 2)
        return new Error("-ERR wrong number of arguments");

      return new Get(args[1]);
    }

    private static Operation Del(IList<string> args)
    {
      if (args.Count < 2)
        return new Error("-ERR wrong number of arguments");

      return new Del(args.Skip(1).ToArray());
    }

    private static Operation Set(IList<string> args)
    {
      if (args.Count < 3 || args.Count > 6)
        return new Error("-ERR wrong number of arguments");

      if (args.Count == 3)
      {
        return new Set(args[1], args[2]);
      }

      if (args.Count == 4)
      {
        args[3] = args[3].ToUpper();

        if (args[3] == "NX" || args[3] == "XX")
        {
          return new Set(args[1], args[2], args[3]);
        }
      }

      if (args.Count == 5)
      {
        args[3] = args[3].ToUpper();
        args[4] = args[4].ToUpper();

        if (args[3] == "EX" || args[3] == "PX")
        {
          return new Set(args[1], args[2], args[3], Convert.ToInt64(args[4]));
        }
      }

      if (args.Count == 6)
      {
        args[3] = args[3].ToUpper();
        args[4] = args[4].ToUpper();
        args[5] = args[5].ToUpper();

        if ((args[3] == "NX" || args[3] == "XX") && (args[4] == "EX" || args[4] == "PX"))
        {
          return new Set(args[1], args[2], args[3], args[4], Convert.ToInt64(args[5]));
        }
      }

      return new Error("-ERR wrong arguments");
    }

    private static Operation Quit(IList<string> args)
    {
      if (args.Count != 1)
        return new Error("-ERR wrong number of arguments");

      return new Quit();
    }
  }
}