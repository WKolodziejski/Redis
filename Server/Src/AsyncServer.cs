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
    private readonly Dictionary<string, TcpClient> _clients;

    protected AsyncServer(string ipAddress, int port)
    {
      _listener = new TcpListener(IPAddress.Parse(ipAddress), port);
      _clients = new Dictionary<string, TcpClient>();
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
    }

    protected void Send(string id, string data)
    {
      lock (_clients)
      {
        var client = _clients[id];
        var stream = client.GetStream();
        var bytes = Encoding.ASCII.GetBytes(data ?? string.Empty);
        stream.Write(bytes, 0, bytes.Length);
      }
    }

    protected void Disconnect(string id)
    {
      lock (_clients)
      {
        var client = _clients[id];
        client.Close();
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

          lock (_clients)
          {
            _clients.Add(id, client);
          }

          var clientThread = new Thread(() => ListenClient(id, client));
          clientThread.Start();
        }
      }
      catch (Exception e)
      {
        OnError(e);
      }

      Stop();
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

      client.Close();
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