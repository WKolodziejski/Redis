using System.Collections.Generic;
using System.Linq;
using System.Threading;
using NUnit.Framework;
using Server.Commands;

namespace Server.Tests.HandlerTests
{
  public class HandlerGetTests
  {
    [Test]
    public void GetShouldNotReturn()
    {
      var table = new Dictionary<string, Data>();
      var get = new Get("KEY");
      var actions = Handler.Handle("id", get, table);

      HandlerUtils.ExpectsWrite(actions.ToList(), "-1");
    }
    
    [Test]
    public void GetShouldReturn()
    {
      var table = new Dictionary<string, Data> { { "KEY", new Data("10") } };
      var get = new Get("KEY");
      var actions = Handler.Handle("id", get, table);

      HandlerUtils.ExpectsWrite(actions.ToList(), "$2", "10");
    }
    
    [Test]
    public void GetShouldTimeout()
    {
      var table = new Dictionary<string, Data> { { "KEY", new Data("10", 1000) } };
      var get = new Get("KEY");
      var actionsBefore = Handler.Handle("id", get, table);

      HandlerUtils.ExpectsWrite(actionsBefore.ToList(), "$2", "10");
      
      Thread.Sleep(1000);
      
      var actionsAfter = Handler.Handle("id", get, table);

      HandlerUtils.ExpectsWrite(actionsAfter.ToList(), "-1");
    }
  }
}