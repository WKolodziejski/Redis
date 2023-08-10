namespace Server.Commands
{
  /*
   * Class used when a command is not known by the server.
   */
  public class Unknown : Command
  {
    private readonly string _message;

    public Unknown(string message)
    {
      _message = message;
    }

    public string Message => $"-ERR {_message}";
  }
}