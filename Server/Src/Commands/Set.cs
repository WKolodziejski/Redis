namespace Server.Commands
{
  public class Set : Command
  {
    public readonly string Key;
    public readonly string Value;
    public readonly string Replacement;
    public readonly string DurationUnity;
    public readonly long Duration;
    
    public Set(string key, string value)
    {
      Key = key;
      Value = value;
    }

    public Set(string key, string value, string replacement)
    {
      Key = key;
      Value = value;
      Replacement = replacement;
    }
    
    public Set(string key, string value, string durationUnity, long duration)
    {
      Key = key;
      Value = value;
      DurationUnity = durationUnity;
      Duration = duration;
    }
    
    public Set(string key, string value, string replacement, string durationUnity, long duration)
    {
      Key = key;
      Value = value;
      Replacement = replacement;
      DurationUnity = durationUnity;
      Duration = duration;
    }
  }
}