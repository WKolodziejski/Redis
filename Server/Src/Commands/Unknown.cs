namespace Server.Commands
{
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