namespace Server.Commands
{
  public class Del : Command
  {
    public readonly string[] Keys;

    public Del(params string[] keys)
    {
      Keys = keys;
    }
  }
}