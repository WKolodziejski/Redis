namespace Server.Operations
{
  public class Error : Operation
  {
    public readonly string Message;

    public Error(string message)
    {
      Message = message;
    }
  }
}