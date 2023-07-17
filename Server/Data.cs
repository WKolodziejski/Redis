using System;

namespace Server
{
  public class Data
  {
    public readonly string Value;
    private readonly bool expires;
    private readonly DateTime expiration;

    public Data(string value)
    {
      Value = value;
      expires = false;
    }
    
    public Data(string value, long duration)
    {
      Value = value;
      expires = true;
      expiration = DateTime.Now.AddMilliseconds(duration);
    }
    
    public DateTime Expiration => expires ? expiration : DateTime.MaxValue;
  }
}