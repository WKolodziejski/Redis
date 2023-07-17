using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Server
{
  public abstract class AsyncServer
  {
    private readonly TcpListener listener;
    private readonly Thread connectionsThread;
    private readonly Dictionary<string, TcpClient> clients;

    protected AsyncServer(string ipAddress, int port)
    {
      listener = new TcpListener(IPAddress.Parse(ipAddress), port);
      clients = new Dictionary<string, TcpClient>();
      connectionsThread = new Thread(ListenConnections);
    }

    public void Start()
    {
      listener.Start();
      connectionsThread.Start();
    }

    public void Stop()
    {
      connectionsThread.Abort();
      listener.Stop();
    }

    protected void Write(string id, string data)
    {
      lock (clients)
      {
        var client = clients[id];
        var stream = client.GetStream();
        var bytes = Encoding.ASCII.GetBytes(data ?? string.Empty);
        stream.Write(bytes, 0, bytes.Length);
      }
    }

    protected void CloseConnection(string id)
    {
      lock (clients)
      {
        var client = clients[id];
        client.Close();
        clients.Remove(id);
      }
    }

    private void ListenConnections()
    {
      OnStartListening();

      try
      {
        while (true)
        {
          var id = Guid.NewGuid().ToString();
          var client = listener.AcceptTcpClient();

          lock (clients)
          {
            clients.Add(id, client);
          }

          var clientThread = new Thread(() => ListenClient(id, client));
          clientThread.Start();
        }
      }
      catch (Exception e)
      {
        OnError(e);
      }

      listener.Stop();

      OnStopListening();
    }

    private void ListenClient(string id, TcpClient client)
    {
      OnClientConnected(id);

      try
      {
        while (true)
        {
          var stream = client.GetStream();
          var buffer = new byte[client.ReceiveBufferSize];

          var read = stream.Read(buffer, 0, client.ReceiveBufferSize);
          var data = Encoding.ASCII.GetString(buffer, 0, read);

          OnDataReceived(id, data);
        }
      }
      catch (Exception e)
      {
        OnError(e);
      }

      client.Close();

      OnClientDisconnected(id);
    }

    protected abstract void OnStartListening();
    protected abstract void OnStopListening();
    protected abstract void OnDataReceived(string id, string data);
    protected abstract void OnClientConnected(string id);
    protected abstract void OnClientDisconnected(string id);
    protected abstract void OnError(Exception e);
  }
}