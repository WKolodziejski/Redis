using System;

namespace Client
{
  internal class Program
  {
    private const int Port = 6379;
    private const string ServerIp = "127.0.0.1";

    public static void Main(string[] args)
    {
      var client = new Client(ServerIp, Port);
      client.Start();

      while (true)
      {
        var data = Console.ReadLine();
        client.Write(data);

        if (data.Equals("EXIT"))
        {
          client.Stop();
          break;
        }
      }
    }
  }
}