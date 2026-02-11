using System;
using UnityEngine;

namespace VoiceAgent
{
    /// <summary>
    /// Minimal game event tracker for sending custom events to Voice Agent
    /// Use TrackEvent() to send your custom event classes
    /// </summary>
    public class GameEventTracker : MonoBehaviour
    {
        private static GameEventTracker _instance;
        public static GameEventTracker Instance
        {
            get
            {
                if (_instance == null)
                {
                    GameObject go = new GameObject("GameEventTracker");
                    _instance = go.AddComponent<GameEventTracker>();
                    DontDestroyOnLoad(go);
                }
                return _instance;
            }
        }

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

        /// <summary>
        /// Track ANY custom event by passing your own event class
        /// The class name will be used as the event type automatically
        /// Example: TrackEvent(new MyCustomEvent { field1 = "value", field2 = 123 });
        /// </summary>
        public void TrackEvent(object eventData)
        {
            VoiceAgentBridge.Instance.SendGameEvent(eventData);
        }

        /// <summary>
        /// Track custom game phase
        /// </summary>
        public void TrackGamePhase(string phaseName, string status = "started", string metadata = "")
        {
            VoiceAgentBridge.Instance.SendGameEvent(new GamePhaseEvent
            {
                phaseName = phaseName,
                status = status,
                metadata = metadata,
                timestamp = GetTimestamp()
            });
        }

        /// <summary>
        /// Track errors or issues
        /// </summary>
        public void TrackError(string errorMessage, string errorType = "gameplay", string stackTrace = "")
        {
            VoiceAgentBridge.Instance.SendGameEvent(new ErrorEvent
            {
                errorMessage = errorMessage,
                errorType = errorType,
                stackTrace = stackTrace,
                timestamp = GetTimestamp()
            });
        }

        /// <summary>
        /// Send a completely custom event with any data
        /// </summary>
        public void TrackCustomEvent(string eventName, object eventData)
        {
            VoiceAgentBridge.Instance.SendGameEvent(new CustomEvent
            {
                eventName = eventName,
                eventData = JsonUtility.ToJson(eventData),
                timestamp = GetTimestamp()
            });
        }

        #region Private Helper Methods

        private void TrackSceneChange(string sceneName)
        {
            // Only send if connected
            if (!VoiceAgentBridge.Instance.IsConnected)
                return;

            VoiceAgentBridge.Instance.SendGameEvent(new SceneChangeEvent
            {
                sceneName = sceneName,
                timestamp = GetTimestamp()
            });
        }

        private void TrackAppLifecycle(string state)
        {
            // Only send if connected
            if (!VoiceAgentBridge.Instance.IsConnected)
                return;

            VoiceAgentBridge.Instance.SendGameEvent(new AppLifecycleEvent
            {
                state = state,
                timestamp = GetTimestamp()
            });
        }

        private string GetTimestamp()
        {
            return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }

        #endregion
    }

    #region Event Data Classes

    [Serializable]
    public class GamePhaseEvent
    {
        public string phaseName;
        public string status;
        public string metadata;
        public string timestamp;
    }

    [Serializable]
    public class ErrorEvent
    {
        public string errorMessage;
        public string errorType;
        public string stackTrace;
        public string timestamp;
    }

    [Serializable]
    public class CustomEvent
    {
        public string eventName;
        public string eventData;
        public string timestamp;
    }

    #endregion

    #region ⭐ ADD YOUR CUSTOM EVENT CLASSES HERE ⭐

    // ============================================================================
    // DEVELOPERS: Add your game-specific event classes below this line
    // ============================================================================
    // 
    // Example 1: Secret Phase Event
    // [Serializable]
    // public class SecretPhaseEvent
    // {
    //     public string phaseName;
    //     public int secretCode;
    //     public bool isUnlocked;
    // }
    //
    // Example 2: Boss Encounter Event
    // [Serializable]
    // public class BossEncounterEvent
    // {
    //     public string bossName;
    //     public int bossLevel;
    //     public float playerHealth;
    // }
    //
    // Example 3: Mini-Game Event
    // [Serializable]
    // public class MiniGameEvent
    // {
    //     public string gameName;
    //     public string status; // "started", "completed", "failed"
    //     public int score;
    // }
    //
    // HOW TO USE:
    // 1. Create your [Serializable] class with public fields
    // 2. Call: GameEventTracker.Instance.TrackEvent(new YourEventClass { ... });
    // 3. The class name will automatically become the event type in JSON
    //
    // JSON OUTPUT EXAMPLE:
    // {
    //   "type": "YourEventClassName",
    //   "payload": {
    //     "field1": "value1",
    //     "field2": 123
    //   }
    // }
    // ============================================================================

    // 👇 ADD YOUR CUSTOM EVENT CLASSES BELOW 👇




    // 👆 ADD YOUR CUSTOM EVENT CLASSES ABOVE 👆

    #endregion
}
