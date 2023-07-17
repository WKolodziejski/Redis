namespace Server.Operations
{
  public class Get : Operation
  {
    public readonly string Key;

    public Get(string key)
    {
      Key = key;
    }
  }
}