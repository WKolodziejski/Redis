using System.Collections.Generic;
using System.Linq;
using System.Threading;
using NUnit.Framework;
using Server.Commands;
using Server.Data;

namespace Server.Tests.HandlerTests
{
  public class HandlerSetTests
  {
    [Test]
    public void SetShouldInsert()
    {
      var table = new Dictionary<string, Entry>();
      var set = new Set("KEY", "10");
      var actionsSet = Handler.Handle("id", set, table);

      HandlerUtils.ExpectsWrite(actionsSet.ToList(), "+OK");

      var get = new Get("KEY");
      var actionsGet = Handler.Handle("id", get, table);

      HandlerUtils.ExpectsWrite(actionsGet.ToList(), "$2", "10");
    }
    
    [Test]
    public void SetShouldReplace()
    {
      var table = new Dictionary<string, Entry> { { "KEY", new Entry("10") } };
      var set = new Set("KEY", "20");
      var actionsSet = Handler.Handle("id", set, table);

      HandlerUtils.ExpectsWrite(actionsSet.ToList(), "+OK");

      var get = new Get("KEY");
      var actionsGet = Handler.Handle("id", get, table);

      HandlerUtils.ExpectsWrite(actionsGet.ToList(), "$2", "20");
    }
    
    [Test]
    public void SetXxShouldReplace()
    {
      var table = new Dictionary<string, Entry> { { "KEY", new Entry("10") } };
      var set = new Set("KEY", "20", "XX");
      var actionsSet = Handler.Handle("id", set, table);

      HandlerUtils.ExpectsWrite(actionsSet.ToList(), "+OK");

      var get = new Get("KEY");
      var actionsGet = Handler.Handle("id", get, table);

      HandlerUtils.ExpectsWrite(actionsGet.ToList(), "$2", "20");
    }
    
    [Test]
    public void SetXxShouldNotReplace()
    {
      var table = new Dictionary<string, Entry>();
      var set = new Set("KEY", "10", "XX");
      var actionsSet = Handler.Handle("id", set, table);

      HandlerUtils.ExpectsWrite(actionsSet.ToList(), "-1");

      var get = new Get("KEY");
      var actionsGet = Handler.Handle("id", get, table);

      HandlerUtils.ExpectsWrite(actionsGet.ToList(), "-1");
    }
    
    [Test]
    public void SetXxShouldReplaceTimeout()
    {
      var table = new Dictionary<string, Entry> { { "KEY", new Entry("10", 1000) } };
      var set = new Set("KEY", "20", "XX");
      var actionsSet = Handler.Handle("id", set, table);

      HandlerUtils.ExpectsWrite(actionsSet.ToList(), "+OK");

      var get = new Get("KEY");
      var actionsGet = Handler.Handle("id", get, table);

      HandlerUtils.ExpectsWrite(actionsGet.ToList(), "$2", "20");
    }
    
    [Test]
    public void SetXxShouldNotReplaceTimeout()
    {
      var table = new Dictionary<string, Entry> { { "KEY", new Entry("10", 1000) } };
      var set = new Set("KEY", "20", "XX");
      
      Thread.Sleep(1000);
      
      var actionsSet = Handler.Handle("id", set, table);

      HandlerUtils.ExpectsWrite(actionsSet.ToList(), "-1");

      var get = new Get("KEY");
      var actionsGet = Handler.Handle("id", get, table);

      HandlerUtils.ExpectsWrite(actionsGet.ToList(), "-1");
    }
    
    [Test]
    public void SetNxShouldInsert()
    {
      var table = new Dictionary<string, Entry>();
      var set = new Set("KEY", "10", "NX");
      var actionsSet = Handler.Handle("id", set, table);

      HandlerUtils.ExpectsWrite(actionsSet.ToList(), "+OK");

      var get = new Get("KEY");
      var actionsGet = Handler.Handle("id", get, table);

      HandlerUtils.ExpectsWrite(actionsGet.ToList(), "$2", "10");
    }
    
    [Test]
    public void SetNxShouldNotInsert()
    {
      var table = new Dictionary<string, Entry> { { "KEY", new Entry("10") } };
      var set = new Set("KEY", "20", "NX");
      var actionsSet = Handler.Handle("id", set, table);

      HandlerUtils.ExpectsWrite(actionsSet.ToList(), "-1");

      var get = new Get("KEY");
      var actionsGet = Handler.Handle("id", get, table);

      HandlerUtils.ExpectsWrite(actionsGet.ToList(), "$2", "10");
    }
    
    [Test]
    public void SetNxShouldInsertTimeout()
    {
      var table = new Dictionary<string, Entry> { { "KEY", new Entry("10", 1000) } };
      var set = new Set("KEY", "10", "NX");
      
      Thread.Sleep(1000);
      
      var actionsSet = Handler.Handle("id", set, table);

      HandlerUtils.ExpectsWrite(actionsSet.ToList(), "+OK");

      var get = new Get("KEY");
      var actionsGet = Handler.Handle("id", get, table);

      HandlerUtils.ExpectsWrite(actionsGet.ToList(), "$2", "10");
    }
    
    [Test]
    public void SetNxShouldNotInsertTimeout()
    {
      var table = new Dictionary<string, Entry> { { "KEY", new Entry("10", 1000) } };
      var set = new Set("KEY", "20", "NX");
      var actionsSet = Handler.Handle("id", set, table);

      HandlerUtils.ExpectsWrite(actionsSet.ToList(), "-1");

      var get = new Get("KEY");
      var actionsGet = Handler.Handle("id", get, table);

      HandlerUtils.ExpectsWrite(actionsGet.ToList(), "$2", "10");
    }
  }
}