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

    protected void Write(string uuid, string data)
    {
      lock (clients)
      {
        var client = clients[uuid];
        var stream = client.GetStream();
        var bytes = Encoding.ASCII.GetBytes(data ?? string.Empty);
        stream.Write(bytes, 0, bytes.Length);
      }
    }

    protected void CloseConnection(string uuid)
    {
      lock (clients)
      {
        var client = clients[uuid];
        client.Close();
        clients.Remove(uuid);
      }
    }

    private void ListenConnections()
    {
      OnStartListening();

      try
      {
        while (true)
        {
          var uuid = Guid.NewGuid().ToString();
          var client = listener.AcceptTcpClient();

          lock (clients)
          {
            clients.Add(uuid, client);
          }

          var clientThread = new Thread(() => ListenClient(uuid, client));
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

    private void ListenClient(string uuid, TcpClient client)
    {
      OnClientConnected(uuid);

      try
      {
        while (true)
        {
          var stream = client.GetStream();
          var buffer = new byte[client.ReceiveBufferSize];

          var read = stream.Read(buffer, 0, client.ReceiveBufferSize);
          var data = Encoding.ASCII.GetString(buffer, 0, read);

          OnDataReceived(uuid, data);
        }
      }
      catch (Exception e)
      {
        OnError(e);
      }

      client.Close();

      OnClientDisconnected(uuid);
    }

    protected abstract void OnStartListening();
    protected abstract void OnStopListening();
    protected abstract void OnDataReceived(string uuid, string data);
    protected abstract void OnClientConnected(string uuid);
    protected abstract void OnClientDisconnected(string uuid);
    protected abstract void OnError(Exception e);
  }
}