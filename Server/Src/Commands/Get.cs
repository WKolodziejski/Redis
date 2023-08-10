﻿namespace Server.Commands
{
  /*
   * Gets the value of a key.
   */
  public class Get : Command
  {
    public readonly string Key;

    public Get(string key)
    {
      Key = key;
    }
  }
}