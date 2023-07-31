namespace Server.Actions
{
  public abstract class Action
  {
    public readonly string Id;

    protected Action(string id)
    {
      Id = id;
    }
  }
}