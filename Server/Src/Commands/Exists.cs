namespace Server.Commands
{
  public class Exists : Command
  {
    public readonly string Key;

    public Exists(string key)
    {
      Key = key;
    }
  }
}