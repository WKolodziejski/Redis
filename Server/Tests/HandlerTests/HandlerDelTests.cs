using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Server.Commands;

namespace Server.Tests.HandlerTests
{
  public class HandlerDelTests
  {
    [Test]
    public void ShouldDeleteZero()
    {
      var table = new Dictionary<string, Data> { { "KEY", new Data("10") } };
      var del = new Del("KEY_INVALID");
      var actions = Handler.Handle("id", del, table);

      HandlerUtils.ExpectsWrite(actions.ToList(), ":0");
    }

    [Test]
    public void ShouldDeleteOne()
    {
      var table = new Dictionary<string, Data> { { "KEY", new Data("10") } };
      var del = new Del("KEY");
      var actions = Handler.Handle("id", del, table);

      HandlerUtils.ExpectsWrite(actions.ToList(), ":1");
    }

    [Test]
    public void ShouldDeleteTwo()
    {
      var table = new Dictionary<string, Data>
      {
        { "KEY1", new Data("10") },
        { "KEY2", new Data("20") },
        { "KEY3", new Data("30") }
      };
      var del = new Del("KEY1", "KEY3");
      var actions = Handler.Handle("id", del, table);

      HandlerUtils.ExpectsWrite(actions.ToList(), ":2");
    }
  }
}