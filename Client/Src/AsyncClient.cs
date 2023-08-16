using System;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace Client
{
  public abstract class AsyncClient : IDisposable
  {
    private static readonly Regex SpacesRegex = new Regex(@"\s+");

    private readonly TcpClient _client;
    private readonly Thread _listenThread;
    private readonly Semaphore _semaphore;

    private string _lastResponse;
    private bool _awaitingResponse;

    protected AsyncClient(string ipAddress, int port)
    {
      _client = new TcpClient(ipAddress, port);
      _listenThread = new Thread(ListenServer);
      _semaphore = new Semaphore(0, 1);
    }

    public void Start()
    {
      _listenThread.Start();
      Running = true;
    }

    public void Stop()
    {
      if (_client.Connected)
      {
        var bytes = Encoding.ASCII.GetBytes("QUIT");
        var stream = _client.GetStream();
        stream.Write(bytes, 0, bytes.Length);
      }

      Running = false;
      _client.Close();
    }

    public string Send(string data)
    {
      SpacesRegex.Replace(data, data);

      if (string.IsNullOrEmpty(data))
      {
        return null;
      }
      
      _awaitingResponse = true;
      
      var bytes = Encoding.ASCII.GetBytes(data);
      var stream = _client.GetStream();
      stream.Write(bytes, 0, bytes.Length);
      
      if (_semaphore.WaitOne())
      {
        _awaitingResponse = false;
      }
      
      var response = _lastResponse;

      _lastResponse = null;

      return response;
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

          _lastResponse = data;
          
          if (_awaitingResponse)
          {
            _semaphore.Release();
          }

          OnDataReceived(data);
        }
      }
      catch (Exception e)
      {
        Stop();
        OnError(e);
      }
      
      OnStopListening();
    }

    public bool Running { get; private set; }

    protected abstract void OnStartListening();
    protected abstract void OnStopListening();
    protected abstract void OnDataReceived(string data);
    protected abstract void OnError(Exception e);

    public void Dispose()
    {
      _client.Close();
    }
  }
}