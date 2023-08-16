using System;
using System.Threading.Tasks;

namespace Server
{
  internal abstract class Program
  {
    private const int Port = 6379;
    private const string ServerIp = "127.0.0.1";

    public static async Task Main(string[] args)
    {
      var tcs = new TaskCompletionSource<bool>();
      var sigintReceived = false;
      
      var server = new Server(ServerIp, Port);
      server.Start();

      Console.CancelKeyPress += (_, ea) =>
      {
        Console.WriteLine("Initiating graceful shutdown...");
        
        server.Stop();
        ea.Cancel = true;
        tcs.SetResult(true);
        
        sigintReceived = true;
      };
      
      AppDomain.CurrentDomain.ProcessExit += (sender, eventArgs) =>
      {
        if (sigintReceived) return;
        
        server.Stop();
        tcs.SetResult(true);
      };
      
      await tcs.Task;

      await Task.Delay(1000);

      Console.WriteLine("Finishing graceful shutdown...");
    }
  }
}