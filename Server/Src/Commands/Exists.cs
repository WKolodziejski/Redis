namespace Server.Commands
{
  /*
   * This command checks whether the key exists or not.
   */
  public class Exists : Command
  {
    public readonly string Key;

    public Exists(string key)
    {
      Key = key;
    }
  }
}