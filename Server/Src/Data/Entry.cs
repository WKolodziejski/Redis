using System;

namespace Server.Data
{
  public class Entry<T>
  {
    public readonly T Value;
    private readonly DateTime _expiration;

    public bool Expires;

    public Entry(T value)
    {
      Value = value;
      Expires = false;
    }
    
    public Entry(T value, long duration)
    {
      Value = value;
      Expires = true;
      _expiration = DateTime.Now.AddMilliseconds(duration);
    }
    
    public DateTime Expiration => Expires ? _expiration : DateTime.MaxValue;

    public int Length => Type == typeof(string) ? ((string)Value).Length : 1;
  }
}