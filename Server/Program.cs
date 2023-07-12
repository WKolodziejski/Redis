using System;

namespace Server
{
  internal class Program
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

        if (data.Equals("EXIT"))
        {
          server.Stop();
          break;
        }
      }
    }
  }
}