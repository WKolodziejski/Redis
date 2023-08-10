using System.Collections.Generic;
using System.Linq;
using System.Threading;
using NUnit.Framework;
using Server.Commands;

namespace Server.Tests.HandlerTests
{
  public class HandlerExistsTests
  {
    [Test]
    public void ExistsShouldNotReturn()
    {
      var table = new Dictionary<string, Data>();
      var exists = new Exists("KEY");
      var actions = Handler.Handle("id", exists, table);

      HandlerUtils.ExpectsWrite(actions.ToList(), "-1");
    }
    
    [Test]
    public void ExistsShouldReturn()
    {
      var table = new Dictionary<string, Data> { { "KEY", new Data("10") } };
      var exists = new Exists("KEY");
      var actions = Handler.Handle("id", exists, table);

      HandlerUtils.ExpectsWrite(actions.ToList(), "1");
    }
    
    [Test]
    public void ExistsShouldTimeout()
    {
      var table = new Dictionary<string, Data> { { "KEY", new Data("10", 1000) } };
      var exists = new Exists("KEY");
      var actionsBefore = Handler.Handle("id", exists, table);

      HandlerUtils.ExpectsWrite(actionsBefore.ToList(), "1");
      
      Thread.Sleep(1000);
      
      var actionsAfter = Handler.Handle("id", exists, table);

      HandlerUtils.ExpectsWrite(actionsAfter.ToList(), "-1");
    }
  }
}