namespace Server.Operations
{
  public class Del : Operation
  {
    public readonly string Key;

    public Del(string key)
    {
      Key = key;
    }
  }
}