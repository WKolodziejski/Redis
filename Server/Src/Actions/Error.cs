using Server.Commands;

namespace Server.Actions
{
  public class Error : Action
  {

    public readonly Command Command;
    public Error(string id, Command command) : base(id)
    {
      Command = command;
    }
  }
}