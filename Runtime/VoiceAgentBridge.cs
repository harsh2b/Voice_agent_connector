using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using UnityEngine;

namespace VoiceAgent
{
    /// <summary>
    /// Singleton MonoBehaviour that manages WebSocket connection to Voice Agent
    /// </summary>
    public class VoiceAgentBridge : MonoBehaviour
    {
        #region Singleton
        private static VoiceAgentBridge _instance;
        public static VoiceAgentBridge Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<VoiceAgentBridge>();
                    if (_instance == null)
                    {
                        GameObject go = new GameObject("VoiceAgent_Connector");
                        _instance = go.AddComponent<VoiceAgentBridge>();
                        DontDestroyOnLoad(go);
                    }
                }
                return _instance;
            }
        }
        #endregion

        #region Inspector Fields
        [Header("Connection Settings")]
        [Tooltip("WebSocket URL of the Voice Agent server")]
        public string agentUrl = "ws://localhost:8080";

        [Tooltip("Automatically connect on Start")]
        public bool autoConnect = true;

        [Header("Debug Settings")]
        [Tooltip("Enable debug logging")]
        public bool debugMode = true;
        #endregion

        #region Private Fields
        private SimpleWebSocket _websocket;
        private bool _isConnected = false;
        #endregion

        #region Events
        public event Action OnConnected;
        public event Action OnDisconnected;
        public event Action<string> OnMessageReceived;
        public event Action<string> OnErrorOccurred;
        #endregion

        #region Unity Lifecycle
        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            if (autoConnect)
            {
                Connect();
            }
        }

        private void Update()
        {
            #if !UNITY_WEBGL || UNITY_EDITOR
            // Dispatch WebSocket messages on main thread
            if (_websocket != null)
            {
                _websocket.DispatchMessageQueue();
            }
            #endif
        }

        private void OnApplicationQuit()
        {
            Disconnect();
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Establishes connection to the Voice Agent server
        /// </summary>
        public async void Connect()
        {
            if (_isConnected)
            {
                Log("Already connected to Voice Agent");
                return;
            }

            try
            {
                _websocket = new SimpleWebSocket();

                _websocket.OnOpen += OnOpen;
                _websocket.OnMessage += OnMessage;
                _websocket.OnError += OnError;
                _websocket.OnClose += OnClose;

                Log($"Connecting to Voice Agent at {agentUrl}...");
                await _websocket.Connect(agentUrl);
            }
            catch (Exception e)
            {
                LogError($"Connection failed: {e.Message}");
                OnErrorOccurred?.Invoke(e.Message);
            }
        }

        /// <summary>
        /// Disconnects from the Voice Agent server
        /// </summary>
        public async void Disconnect()
        {
            if (_websocket != null && _isConnected)
            {
                Log("Disconnecting from Voice Agent...");
                await _websocket.Close();
            }
        }

        /// <summary>
        /// Sends a game event to the Voice Agent. Automatically wraps the event in JSON format.
        /// </summary>
        /// <param name="eventData">Any serializable object representing the game event</param>
        public async void SendGameEvent(object eventData)
        {
            if (!_isConnected)
            {
                LogError("Cannot send event: Not connected to Voice Agent");
                return;
            }

            if (eventData == null)
            {
                LogError("Cannot send null event data");
                return;
            }

            try
            {
                // Get the class name of the event
                string eventType = eventData.GetType().Name;

                // Serialize the payload
                string payloadJson = JsonUtility.ToJson(eventData);

                // Manually create the wrapper JSON to avoid double serialization
                string json = $"{{\"type\":\"{eventType}\",\"payload\":{payloadJson}}}";

                Log($"Sending event: {eventType}\n{json}");

                // Send via WebSocket
                await _websocket.SendText(json);
            }
            catch (Exception e)
            {
                LogError($"Failed to send event: {e.Message}");
                OnErrorOccurred?.Invoke(e.Message);
            }
        }

        /// <summary>
        /// Sends raw text message to the Voice Agent
        /// </summary>
        public async void SendRawMessage(string message)
        {
            if (!_isConnected)
            {
                LogError("Cannot send message: Not connected to Voice Agent");
                return;
            }

            try
            {
                await _websocket.SendText(message);
                Log($"Sent raw message: {message}");
            }
            catch (Exception e)
            {
                LogError($"Failed to send message: {e.Message}");
            }
        }
        #endregion

        #region WebSocket Callbacks
        private void OnOpen()
        {
            _isConnected = true;
            Log("✓ Connected to Voice Agent successfully!");
            OnConnected?.Invoke();
        }

        private void OnMessage(byte[] data)
        {
            string message = System.Text.Encoding.UTF8.GetString(data);
            Log($"Received message: {message}");
            OnMessageReceived?.Invoke(message);
        }

        private void OnError(string errorMsg)
        {
            LogError($"WebSocket Error: {errorMsg}");
            OnErrorOccurred?.Invoke(errorMsg);
        }

        private void OnClose(WebSocketCloseStatus closeStatus)
        {
            _isConnected = false;
            Log($"Connection closed: {closeStatus}");
            OnDisconnected?.Invoke();
        }
        #endregion

        #region Logging
        private void Log(string message)
        {
            if (debugMode)
            {
                Debug.Log($"[VoiceAgentBridge] {message}");
            }
        }

        private void LogError(string message)
        {
            if (debugMode)
            {
                Debug.LogError($"[VoiceAgentBridge] {message}");
            }
        }
        #endregion

        #region Properties
        public bool IsConnected => _isConnected;
        #endregion
    }
}
