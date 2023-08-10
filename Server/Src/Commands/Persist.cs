namespace Server.Commands
{
  /*
   * Removes the expiration from the key.
   */
  public class Persist : Command
  {
    public readonly string Key;

    public Persist(string key)
    {
      Key = key;
    }
  }
}