using NUnit.Framework;
using Server.Commands;

namespace Server.Tests.ParserTests
{
  public class ParserPersistTests
  {
    [Test]
    public void ShouldCreatePersist()
    {
      const string data = "PERSIST A";

      var cmd = Parser.Parse(data);
      
      Assert.IsInstanceOf<Persist>(cmd);

      var persist = cmd as Persist;
      
      Assert.AreEqual("A", persist!.Key);
    }
    
    [Test]
    public void ShouldNotCreatePersistFewArgs()
    {
      const string data = "PERSIST";

      var cmd = Parser.Parse(data);
      
      Assert.IsInstanceOf<Unknown>(cmd);
    }
    
    [Test]
    public void ShouldNotCreatePersistExtraArgs()
    {
      const string data = "PERSIST A XX";

      var cmd = Parser.Parse(data);
      
      Assert.IsInstanceOf<Unknown>(cmd);
    }
  }
}