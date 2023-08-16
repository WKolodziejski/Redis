using System.Collections.Generic;
using System.Linq;
using System.Threading;
using NUnit.Framework;
using Server.Commands;
using Server.Data;

namespace Server.Tests.HandlerTests
{
  public class HandlerPersisTests
  {
    [Test]
    public void PersistShouldNotRemoveTimeout()
    {
      var table = new Dictionary<string, Entry> { { "KEY", new Entry("10") } };

      var actionsBefore = Handler.Handle("id",  new Persist("KEY"), table);

      HandlerUtils.ExpectsWrite(actionsBefore.ToList(), ":0");
    }
    
    [Test]
    public void PersistShouldRemoveTimeout()
    {
      var table = new Dictionary<string, Entry> { { "KEY", new Entry("10", 1000) } };

      var actionsBefore = Handler.Handle("id",  new Persist("KEY"), table);

      HandlerUtils.ExpectsWrite(actionsBefore.ToList(), ":1");
      
      Thread.Sleep(1000);
      
      var actionsAfter = Handler.Handle("id", new Get("KEY"), table);

      HandlerUtils.ExpectsWrite(actionsAfter.ToList(), "$2", "10");
    }
  }
}