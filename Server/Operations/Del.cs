namespace Server.Operations
{
  public class Del : Operation
  {
    public readonly string[] Keys;

    public Del(string[] keys)
    {
      Keys = keys;
    }
  }
}