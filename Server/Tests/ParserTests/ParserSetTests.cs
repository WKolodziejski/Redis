using NUnit.Framework;
using Server.Commands;

namespace Server.Tests.ParserTests
{
  public class ParserSetTests
  {
    [Test]
    public void ShouldCreateSet()
    {
      const string data = "SET A 1";

      var cmd = Parser.Parse(data);
      
      Assert.IsInstanceOf<Set>(cmd);

      var get = cmd as Set;
      
      Assert.AreEqual("A", get!.Key);
      Assert.AreEqual("1", get!.Value);
    }
    
    [Test]
    public void ShouldNotCreateSetFewArgs()
    {
      const string data = "SET A";

      var cmd = Parser.Parse(data);
      
      Assert.IsInstanceOf<Error>(cmd);
    }
    
    [Test]
    public void ShouldNotCreateSetExtraArgs()
    {
      const string data = "SET A 1 A";

      var cmd = Parser.Parse(data);
      
      Assert.IsInstanceOf<Error>(cmd);
    }
    
    [Test]
    public void ShouldCreateSetXx()
    {
      const string data = "SET A 1 XX";

      var cmd = Parser.Parse(data);
      
      Assert.IsInstanceOf<Set>(cmd);

      var get = cmd as Set;
      
      Assert.AreEqual("A", get!.Key);
      Assert.AreEqual("1", get!.Value);
      Assert.AreEqual("XX", get!.Replacement);
    }

    [Test]
    public void ShouldNotCreateSetXxExtraArgs()
    {
      const string data = "SET A XX A";

      var cmd = Parser.Parse(data);
      
      Assert.IsInstanceOf<Error>(cmd);
    }
    
    [Test]
    public void ShouldCreateSetNx()
    {
      const string data = "SET A 1 NX";

      var cmd = Parser.Parse(data);
      
      Assert.IsInstanceOf<Set>(cmd);

      var get = cmd as Set;
      
      Assert.AreEqual("A", get!.Key);
      Assert.AreEqual("1", get!.Value);
      Assert.AreEqual("NX", get!.Replacement);
    }

    [Test]
    public void ShouldNotCreateSetNxExtraArgs()
    {
      const string data = "SET A NX A";

      var cmd = Parser.Parse(data);
      
      Assert.IsInstanceOf<Error>(cmd);
    }
    
    [Test]
    public void ShouldCreateSetEx()
    {
      const string data = "SET A 1 EX 100";

      var cmd = Parser.Parse(data);
      
      Assert.IsInstanceOf<Set>(cmd);

      var get = cmd as Set;
      
      Assert.AreEqual("A", get!.Key);
      Assert.AreEqual("1", get!.Value);
      Assert.AreEqual("EX", get!.DurationUnity);
      Assert.AreEqual(100, get!.Duration);
    }
    
    [Test]
    public void ShouldNotCreateSetExFewArgs()
    {
      const string data = "SET A 1 EX";

      var cmd = Parser.Parse(data);
      
      Assert.IsInstanceOf<Error>(cmd);
    }
    
    [Test]
    public void ShouldNotCreateSetExExtraArgs()
    {
      const string data = "SET A 1 EX 100 A";

      var cmd = Parser.Parse(data);
      
      Assert.IsInstanceOf<Error>(cmd);
    }
    
    [Test]
    public void ShouldCreateSetPx()
    {
      const string data = "SET A 1 PX 100";

      var cmd = Parser.Parse(data);
      
      Assert.IsInstanceOf<Set>(cmd);

      var get = cmd as Set;
      
      Assert.AreEqual("A", get!.Key);
      Assert.AreEqual("1", get!.Value);
      Assert.AreEqual("PX", get!.DurationUnity);
      Assert.AreEqual(100, get!.Duration);
    }
    
    [Test]
    public void ShouldNotCreateSetPxFewArgs()
    {
      const string data = "SET A 1 PX";

      var cmd = Parser.Parse(data);
      
      Assert.IsInstanceOf<Error>(cmd);
    }
    
    [Test]
    public void ShouldNotCreateSetPxExtraArgs()
    {
      const string data = "SET A 1 PX 100 A";

      var cmd = Parser.Parse(data);
      
      Assert.IsInstanceOf<Error>(cmd);
    }
    
    [Test]
    public void ShouldCreateSetAllArgs()
    {
      const string data = "SET A 1 XX PX 100";

      var cmd = Parser.Parse(data);
      
      Assert.IsInstanceOf<Set>(cmd);

      var get = cmd as Set;
      
      Assert.AreEqual("A", get!.Key);
      Assert.AreEqual("1", get!.Value);
      Assert.AreEqual("XX", get!.Replacement);
      Assert.AreEqual("PX", get!.DurationUnity);
      Assert.AreEqual(100, get!.Duration);
    }
    
    [Test]
    public void ShouldNotCreateSetAllArgsWrongOrder()
    {
      const string data = "SET A 1 PX 100 XX";

      var cmd = Parser.Parse(data);
      
      Assert.IsInstanceOf<Error>(cmd);
    }
  }
}