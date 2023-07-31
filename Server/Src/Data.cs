using System;

namespace Server
{
  public class Data
  {
    public readonly string Value;
    private readonly bool _expires;
    private readonly DateTime _expiration;

    public Data(string value)
    {
      Value = value;
      _expires = false;
    }
    
    public Data(string value, long duration)
    {
      Value = value;
      _expires = true;
      _expiration = DateTime.Now.AddMilliseconds(duration);
    }
    
    public DateTime Expiration => _expires ? _expiration : DateTime.MaxValue;
  }
}