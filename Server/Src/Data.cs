using System;

namespace Server
{
  public class Data
  {
    public readonly string Value;
    private readonly DateTime _expiration;

    public bool Expires;

    public Data(string value)
    {
      Value = value;
      Expires = false;
    }
    
    public Data(string value, long duration)
    {
      Value = value;
      Expires = true;
      _expiration = DateTime.Now.AddMilliseconds(duration);
    }
    
    public DateTime Expiration => Expires ? _expiration : DateTime.MaxValue;
  }
}