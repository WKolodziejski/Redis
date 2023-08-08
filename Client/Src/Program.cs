using System;

namespace Client
{
  internal abstract class Program
  {
    private const int Port = 6379;
    private const string ServerIp = "127.0.0.1";

    public static void Main(string[] args)
    {
      var client = new Client(ServerIp, Port);
      client.Start();

      while (client.Running)
      {
        var data = Console.ReadLine();
        var resp = client.Send(data);

        if (resp != null)
        {
          Console.WriteLine(resp);
        }

        if (data != null && data.ToUpper() == "QUIT" && resp == "+OK")
        {
          client.Stop();
          break;
        }
      }
    }
  }
}