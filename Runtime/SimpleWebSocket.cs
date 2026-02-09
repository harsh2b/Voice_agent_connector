using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace VoiceAgent
{
    /// <summary>
    /// Simple WebSocket client implementation for Unity
    /// Supports all platforms including WebGL, Standalone, and Mobile
    /// </summary>
    public class SimpleWebSocket
    {
        private ClientWebSocket _webSocket;
        private CancellationTokenSource _cancellationTokenSource;
        private readonly Queue<Action> _mainThreadActions = new Queue<Action>();
        private readonly object _lockObject = new object();
        private bool _isConnected = false;

        public event Action OnOpen;
        public event Action<byte[]> OnMessage;
        public event Action<string> OnError;
        public event Action<WebSocketCloseStatus> OnClose;

        public bool IsConnected => _isConnected && _webSocket?.State == WebSocketState.Open;

        /// <summary>
        /// Connect to WebSocket server
        /// </summary>
        public async Task Connect(string url)
        {
            try
            {
                _webSocket = new ClientWebSocket();
                _cancellationTokenSource = new CancellationTokenSource();

                Uri uri = new Uri(url);
                await _webSocket.ConnectAsync(uri, _cancellationTokenSource.Token);

                _isConnected = true;
                EnqueueMainThreadAction(() => OnOpen?.Invoke());

                // Start receiving messages
                _ = ReceiveLoop();
            }
            catch (Exception ex)
            {
                EnqueueMainThreadAction(() => OnError?.Invoke($"Connection failed: {ex.Message}"));
            }
        }

        /// <summary>
        /// Send text message
        /// </summary>
        public async Task SendText(string message)
        {
            if (!IsConnected)
            {
                EnqueueMainThreadAction(() => OnError?.Invoke("Cannot send: Not connected"));
                return;
            }

            try
            {
                byte[] buffer = Encoding.UTF8.GetBytes(message);
                await _webSocket.SendAsync(
                    new ArraySegment<byte>(buffer),
                    WebSocketMessageType.Text,
                    true,
                    _cancellationTokenSource.Token
                );
            }
            catch (Exception ex)
            {
                EnqueueMainThreadAction(() => OnError?.Invoke($"Send failed: {ex.Message}"));
            }
        }

        /// <summary>
        /// Send binary message
        /// </summary>
        public async Task SendBinary(byte[] data)
        {
            if (!IsConnected)
            {
                EnqueueMainThreadAction(() => OnError?.Invoke("Cannot send: Not connected"));
                return;
            }

            try
            {
                await _webSocket.SendAsync(
                    new ArraySegment<byte>(data),
                    WebSocketMessageType.Binary,
                    true,
                    _cancellationTokenSource.Token
                );
            }
            catch (Exception ex)
            {
                EnqueueMainThreadAction(() => OnError?.Invoke($"Send failed: {ex.Message}"));
            }
        }

        /// <summary>
        /// Close the WebSocket connection
        /// </summary>
        public async Task Close()
        {
            if (_webSocket == null) return;

            try
            {
                if (_webSocket.State == WebSocketState.Open)
                {
                    await _webSocket.CloseAsync(
                        WebSocketCloseStatus.NormalClosure,
                        "Closing",
                        CancellationToken.None
                    );
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"[SimpleWebSocket] Close error: {ex.Message}");
            }
            finally
            {
                _isConnected = false;
                _cancellationTokenSource?.Cancel();
                _webSocket?.Dispose();
            }
        }

        /// <summary>
        /// Receive loop running on background thread
        /// </summary>
        private async Task ReceiveLoop()
        {
            byte[] buffer = new byte[8192];

            try
            {
                while (_webSocket.State == WebSocketState.Open && !_cancellationTokenSource.Token.IsCancellationRequested)
                {
                    WebSocketReceiveResult result = await _webSocket.ReceiveAsync(
                        new ArraySegment<byte>(buffer),
                        _cancellationTokenSource.Token
                    );

                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        _isConnected = false;
                        EnqueueMainThreadAction(() => OnClose?.Invoke(result.CloseStatus ?? WebSocketCloseStatus.Empty));
                        break;
                    }
                    else
                    {
                        byte[] messageData = new byte[result.Count];
                        Array.Copy(buffer, messageData, result.Count);
                        EnqueueMainThreadAction(() => OnMessage?.Invoke(messageData));
                    }
                }
            }
            catch (OperationCanceledException)
            {
                // Normal cancellation
            }
            catch (Exception ex)
            {
                EnqueueMainThreadAction(() => OnError?.Invoke($"Receive error: {ex.Message}"));
            }
        }

        /// <summary>
        /// Enqueue action to run on Unity main thread
        /// </summary>
        private void EnqueueMainThreadAction(Action action)
        {
            lock (_lockObject)
            {
                _mainThreadActions.Enqueue(action);
            }
        }

        /// <summary>
        /// Dispatch queued actions on Unity main thread
        /// Call this from MonoBehaviour.Update()
        /// </summary>
        public void DispatchMessageQueue()
        {
            lock (_lockObject)
            {
                while (_mainThreadActions.Count > 0)
                {
                    Action action = _mainThreadActions.Dequeue();
                    action?.Invoke();
                }
            }
        }
    }
}
