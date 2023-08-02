using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Server.Commands;

namespace Server.Tests.HandlerTests
{
  public class HandlerSetTests
  {
    [Test]
    public void SetShouldInsert()
    {
      var table = new Dictionary<string, Data>();
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
      var table = new Dictionary<string, Data> { { "KEY", new Data("10") } };
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
      var table = new Dictionary<string, Data> { { "KEY", new Data("10") } };
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
      var table = new Dictionary<string, Data>();
      var set = new Set("KEY", "10", "XX");
      var actionsSet = Handler.Handle("id", set, table);

      HandlerUtils.ExpectsWrite(actionsSet.ToList(), "-1");

      var get = new Get("KEY");
      var actionsGet = Handler.Handle("id", get, table);

      HandlerUtils.ExpectsWrite(actionsGet.ToList(), "-1");
    }
    
    [Test]
    public void SetNxShouldInsert()
    {
      var table = new Dictionary<string, Data>();
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
      var table = new Dictionary<string, Data> { { "KEY", new Data("10") } };
      var set = new Set("KEY", "20", "NX");
      var actionsSet = Handler.Handle("id", set, table);

      HandlerUtils.ExpectsWrite(actionsSet.ToList(), "-1");

      var get = new Get("KEY");
      var actionsGet = Handler.Handle("id", get, table);

      HandlerUtils.ExpectsWrite(actionsGet.ToList(), "$2", "10");
    }
  }
}