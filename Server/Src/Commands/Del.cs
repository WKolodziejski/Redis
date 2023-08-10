namespace Server.Commands
{
  /*
   * This command deletes the key, if it exists.
   */
  public class Del : Command
  {
    public readonly string[] Keys;

    public Del(params string[] keys)
    {
      Keys = keys;
    }
  }
}