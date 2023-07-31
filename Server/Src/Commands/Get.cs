namespace Server.Commands
{
  public class Get : Command
  {
    public readonly string Key;

    public Get(string key)
    {
      Key = key;
    }
  }
}