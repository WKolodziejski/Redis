using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace Server
{
  public abstract class AsyncServer : IDisposable
  {
    private static readonly Regex SpacesRegex = new Regex(@"\s+");
    
    private readonly TcpListener _listener;
    private readonly Thread _connectionsThread;
    private readonly Dictionary<string, (TcpClient, Thread)> _clients;

    protected AsyncServer(string ipAddress, int port)
    {
      _listener = new TcpListener(IPAddress.Parse(ipAddress), port);
      _clients = new Dictionary<string, (TcpClient, Thread)>();
      _connectionsThread = new Thread(ListenConnections);
    }

    public void Start()
    {
      _listener.Start();
      _connectionsThread.Start();
    }

    public void Stop()
    {
      _listener.Stop();

      lock (_clients)
      {
        foreach (var client in _clients.Values)
        {
          client.Item1.Close();
          client.Item2.Abort();
        }
      }
    }

    protected void Send(string id, string data)
    {
      lock (_clients)
      {
        var client = _clients[id];
        var stream = client.Item1.GetStream();
        var bytes = Encoding.ASCII.GetBytes(data ?? string.Empty);
        stream.Write(bytes, 0, bytes.Length);
      }
    }

    protected void Disconnect(string id)
    {
      lock (_clients)
      {
        var client = _clients[id];
        client.Item1.Close();
        _clients.Remove(id);
        OnClientDisconnected(id);
      }
    }

    private void ListenConnections()
    {
      OnStartListening();

      try
      {
        while (_listener.Server.IsBound)
        {
          var id = Guid.NewGuid().ToString();
          var client = _listener.AcceptTcpClient();
          var thread = new Thread(() => ListenClient(id, client));

          lock (_clients)
          {
            _clients.Add(id, (client, thread));
          }
          
          thread.Start();
        }
      }
      catch (Exception e)
      {
        Stop();
        OnError(e);
      }

      OnStopListening();
    }

    private void ListenClient(string id, TcpClient client)
    {
      OnClientConnected(id);

      try
      {
        while (client.Connected)
        {
          var stream = client.GetStream();
          var buffer = new byte[client.ReceiveBufferSize];

          var read = stream.Read(buffer, 0, client.ReceiveBufferSize);
          var data = Encoding.ASCII.GetString(buffer, 0, read);

          SpacesRegex.Replace(data, data);
          
          OnDataReceived(id, data);
        }
      }
      catch (Exception e)
      {
        OnError(e);
      }

      Disconnect(id);
    }

    protected abstract void OnStartListening();
    protected abstract void OnStopListening();
    protected abstract void OnDataReceived(string id, string data);
    protected abstract void OnClientConnected(string id);
    protected abstract void OnClientDisconnected(string id);
    protected abstract void OnError(Exception e);
    
    public void Dispose()
    {
      _listener.Stop();
    }
  }
}