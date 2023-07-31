using System;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace Client
{
  public abstract class AsyncClient
  {
    private static readonly Regex SpacesRegex = new Regex(@"\s+");

    private readonly TcpClient _client;
    private readonly Thread _listenThread;

    protected AsyncClient(string ipAddress, int port)
    {
      _client = new TcpClient(ipAddress, port);
      _listenThread = new Thread(ListenServer);
    }

    public void Start()
    {
      _listenThread.Start();
      Running = true;
    }

    public void Stop()
    {
      Running = false;
      _client.Close();
    }

    public void Write(string data)
    {
      SpacesRegex.Replace(data, data);

      var bytes = Encoding.ASCII.GetBytes(data);
      var stream = _client.GetStream();
      stream.Write(bytes, 0, bytes.Length);

      OnDataWritten(data);
    }

    private void ListenServer()
    {
      OnStartListening();

      Running = true;

      try
      {
        while (_client.Connected)
        {
          var stream = _client.GetStream();
          var response = new byte[255];
          Array.Resize(ref response, stream.Read(response, 0, response.Length));
          var data = Encoding.Default.GetString(response);

          OnDataReceived(data);
        }
      }
      catch (Exception e)
      {
        OnError(e);
      }

      Stop();
      OnStopListening();
    }

    public bool Running { get; private set; }

    protected abstract void OnStartListening();
    protected abstract void OnStopListening();
    protected abstract void OnDataReceived(string data);
    protected abstract void OnDataWritten(string data);
    protected abstract void OnError(Exception e);
  }
}