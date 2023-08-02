using System.Linq;
using NUnit.Framework;
using Server.Commands;

namespace Server.Tests.ParserTests
{
  public class ParserDelTests
  {
    [Test]
    public void ShouldCreateDel()
    {
      const string data = "DEL A";

      var cmd = Parser.Parse(data);
      
      Assert.IsInstanceOf<Del>(cmd);

      var del = cmd as Del;
      
      Assert.AreEqual(1, del!.Keys.Length);
      Assert.AreEqual(true, del!.Keys.Contains("A"));
    }
    
    [Test]
    public void ShouldNotCreateDelFewArgs()
    {
      const string data = "DEL";

      var cmd = Parser.Parse(data);
      
      Assert.IsInstanceOf<Error>(cmd);
    }
    
    [Test]
    public void ShouldCreateDelMultipleArgs()
    {
      const string data = "DEL A B C";

      var cmd = Parser.Parse(data);
      
      Assert.IsInstanceOf<Del>(cmd);

      var del = cmd as Del;
      
      Assert.AreEqual(3, del!.Keys.Length);
      Assert.AreEqual(true, del!.Keys.Contains("A"));
      Assert.AreEqual(true, del!.Keys.Contains("B"));
      Assert.AreEqual(true, del!.Keys.Contains("C"));
    }
  }
}