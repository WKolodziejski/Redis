using System.Collections.Generic;
using NUnit.Framework;
using Server.Actions;

namespace Server.Tests.HandlerTests
{
  public static class HandlerUtils
  {
    public static void ExpectsWrite(List<Action> actual, params string[] expected)
    {
      Assert.AreEqual(expected.Length, actual.Count);

      for (var i = 0; i < expected.Length; i++)
      {
        Assert.IsInstanceOf<Write>(actual[i]);
        Assert.AreEqual(expected[i], (actual[i] as Write)?.Message);
      }
    }
  }
}