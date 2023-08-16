using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Server.Commands;
using Server.Data;

namespace Server.Tests.HandlerTests
{
  public class HandlerDelTests
  {
    [Test]
    public void ShouldDeleteZero()
    {
      var table = new Dictionary<string, Entry> { { "KEY", new Entry(10, typeof(int)) } };
      var del = new Del("KEY_INVALID");
      var actions = Handler.Handle("id", del, table);

      HandlerUtils.ExpectsWrite(actions.ToList(), ":0");
    }

    [Test]
    public void ShouldDeleteOne()
    {
      var table = new Dictionary<string, Entry> { { "KEY", new Entry(10, typeof(int)) } };
      var del = new Del("KEY");
      var actions = Handler.Handle("id", del, table);

      HandlerUtils.ExpectsWrite(actions.ToList(), ":1");
    }

    [Test]
    public void ShouldDeleteTwo()
    {
      var table = new Dictionary<string, Entry>
      {
        { "KEY1", new Entry(10, typeof(int)) },
        { "KEY2", new Entry(20, typeof(int)) },
        { "KEY3", new Entry(30, typeof(int)) }
      };
      var del = new Del("KEY1", "KEY3");
      var actions = Handler.Handle("id", del, table);

      HandlerUtils.ExpectsWrite(actions.ToList(), ":2");
    }
  }
}