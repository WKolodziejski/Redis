using System;

namespace Server
{
  internal abstract class Program
  {
    private const int Port = 6379;
    private const string ServerIp = "127.0.0.1";

    public static void Main(string[] args)
    {
      var server = new Server(ServerIp, Port);
      server.Start();

      while (true)
      {
        var data = Console.ReadLine();

        if (data.ToUpper().Equals("QUIT"))
        {
          server.Stop();
          break;
        }
      }
    }
  }
}