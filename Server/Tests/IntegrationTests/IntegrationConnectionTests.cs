using System.Threading;
using NUnit.Framework;

namespace Server.Tests.IntegrationTests
{
  public class IntegrationConnectionTests
  {
    
    private const int Port = 6380;
    private const string ServerIp = "127.0.0.1";
    
    [Test]
    public void ShouldConnectOneClient()
    {
      using var server = new Server(ServerIp, Port);
      server.Start();
      
      Thread.Sleep(100);
      
      using var client = new Client.Client(ServerIp, Port);
      client.Start();
      Thread.Sleep(100);

      Assert.AreEqual("+OK", client.Send("PING"));
      Assert.AreEqual("+OK", client.Send("QUIT"));
    }
    
    [Test]
    public void ShouldConnectTwoClients()
    {
      using var server = new Server(ServerIp, Port);
      server.Start();
      
      Thread.Sleep(100);
      
      using var client1 = new Client.Client(ServerIp, Port);
      client1.Start();
      Thread.Sleep(100);
      
      using var client2 = new Client.Client(ServerIp, Port);
      client2.Start();
      Thread.Sleep(100);

      Assert.AreEqual("+OK", client1.Send("PING"));
      Assert.AreEqual("+OK", client2.Send("PING"));
      Assert.AreEqual("+OK", client1.Send("QUIT"));
      Assert.AreEqual("+OK", client2.Send("QUIT"));
    }
  }
}