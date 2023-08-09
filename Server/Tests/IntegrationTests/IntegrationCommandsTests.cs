using System.Threading;
using NUnit.Framework;

namespace Server.Tests.IntegrationTests
{
  public class IntegrationCommandsTests
  {
    private const int Port = 6380;
    private const string ServerIp = "127.0.0.1";
    
    [Test]
    public void SetAndGetAndDel()
    {
      using var server = new Server(ServerIp, Port);
      server.Start();
      
      Thread.Sleep(100);
      
      using var client = new Client.Client(ServerIp, Port);
      client.Start();
      Thread.Sleep(100);

      Assert.AreEqual("+OK", client.Send("SET KEY ABC"));
      Assert.AreEqual("$3\nABC", client.Send("GET KEY"));
      Assert.AreEqual(":1", client.Send("DEL KEY"));
    }
    
    [Test]
    public void SetAndGetAndDelTimeout()
    {
      using var server = new Server(ServerIp, Port);
      server.Start();
      
      Thread.Sleep(100);
      
      using var client = new Client.Client(ServerIp, Port);
      client.Start();
      Thread.Sleep(100);

      Assert.AreEqual("+OK", client.Send("SET KEY ABC PX 2000"));
      Assert.AreEqual("$3\nABC", client.Send("GET KEY"));
      
      Thread.Sleep(2000);

      Assert.AreEqual("-1", client.Send("GET KEY"));
    }
  }
}