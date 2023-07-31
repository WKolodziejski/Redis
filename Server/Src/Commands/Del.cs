namespace Server.Commands
{
  public class Del : Command
  {
    public readonly string[] Keys;

    public Del(string[] keys)
    {
      Keys = keys;
    }
  }
}