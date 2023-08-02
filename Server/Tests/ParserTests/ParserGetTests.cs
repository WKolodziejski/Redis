using NUnit.Framework;
using Server.Commands;

namespace Server.Tests.ParserTests
{
  public class ParserGetTests
  {
    [Test]
    public void ShouldCreateGet()
    {
      const string data = "GET A";

      var cmd = Parser.Parse(data);
      
      Assert.IsInstanceOf<Get>(cmd);

      var get = cmd as Get;
      
      Assert.AreEqual("A", get!.Key);
    }
    
    [Test]
    public void ShouldNotCreateGetFewArgs()
    {
      const string data = "GET";

      var cmd = Parser.Parse(data);
      
      Assert.IsInstanceOf<Error>(cmd);
    }
    
    [Test]
    public void ShouldNotCreateGetExtraArgs()
    {
      const string data = "GET A XX";

      var cmd = Parser.Parse(data);
      
      Assert.IsInstanceOf<Error>(cmd);
    }
  }
}