namespace Server.Actions
{
  public class Write : Action
  {
    public readonly string Message;

    public Write(string id, string message) : base(id)
    {
      Message = message;
    }
  }
}