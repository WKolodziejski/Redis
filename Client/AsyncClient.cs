using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Client
{
  public abstract class AsyncClient
  {
    private readonly TcpClient client;

    protected AsyncClient(string ipAddress, int port)
    {
      client = new TcpClient(ipAddress, port);
    }

    public void Start()
    {
      var listenThread = new Thread(ListenServer);
      listenThread.Start();
    }

    public void Stop()
    {
      client.Close();
    }

    public void Write(string data)
    {
      var bytes = Encoding.ASCII.GetBytes(data ?? string.Empty);
      var stream = client.GetStream();
      stream.Write(bytes, 0, bytes.Length);
    }

    private void ListenServer()
    {
      OnStartListening();

      try
      {
        while (true)
        {
          var stream = client.GetStream();
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

      client.Close();

      OnStopListening();
    }

    protected abstract void OnStartListening();
    protected abstract void OnStopListening();
    protected abstract void OnDataReceived(string data);
    protected abstract void OnError(Exception e);
  }
}