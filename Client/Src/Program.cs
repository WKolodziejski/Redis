using System;
using System.Threading;
using System.Threading.Tasks;

namespace Client
{
  internal abstract class Program
  {
    private const int Port = 6379;
    private const string ServerIp = "127.0.0.1";

    public static async Task Main(string[] args)
    {
      var tcs = new TaskCompletionSource<bool>();
      var sigintReceived = false;
      
      var client = new Client(ServerIp, Port);
      client.Start();

      var thread = new Thread(() =>
      {
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
            sigintReceived = true;
            client.Stop();
            tcs.SetResult(true);
            break;
          }
        }
      });
      
      thread.Start();

      Console.CancelKeyPress += (_, ea) =>
      {
        Console.WriteLine("Initiating graceful shutdown...");
        
        sigintReceived = true;
        
        client.Stop();
        ea.Cancel = true;
        tcs.SetResult(true);
      };
      
      AppDomain.CurrentDomain.ProcessExit += (sender, eventArgs) =>
      {
        if (sigintReceived) return;
        
        client.Stop();
        tcs.SetResult(true);
      };
      
      await tcs.Task;
      
      thread.Abort();
      
      await Task.Delay(1000);

      Console.WriteLine("Finishing graceful shutdown...");

      Environment.Exit(0);
    }
  }
}