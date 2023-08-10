using NUnit.Framework;
using Server.Commands;

namespace Server.Tests.ParserTests
{
  public class ParserExistsTests
  {
    [Test]
    public void ShouldCreateExists()
    {
      const string data = "EXISTS A";

      var cmd = Parser.Parse(data);
      
      Assert.IsInstanceOf<Exists>(cmd);

      var exists = cmd as Exists;
      
      Assert.AreEqual("A", exists!.Key);
    }
    
    [Test]
    public void ShouldNotCreateExistsFewArgs()
    {
      const string data = "EXISTS";

      var cmd = Parser.Parse(data);
      
      Assert.IsInstanceOf<Unknown>(cmd);
    }
    
    [Test]
    public void ShouldNotCreateExistsExtraArgs()
    {
      const string data = "EXISTS A XX";

      var cmd = Parser.Parse(data);
      
      Assert.IsInstanceOf<Unknown>(cmd);
    }
  }
}