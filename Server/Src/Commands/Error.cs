namespace Server.Commands
{
  public class Error : Command
  {
    private readonly string _message;

    public Error(string message)
    {
      _message = message;
    }

    public string Message => $"-ERR {_message}";
  }
}