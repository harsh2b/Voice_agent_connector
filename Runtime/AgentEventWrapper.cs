using System;
using UnityEngine;

namespace VoiceAgent
{
    /// <summary>
    /// Wrapper class for formatting game events into standardized JSON structure
    /// Format: { "type": "EventClassName", "payload": { ...eventData... } }
    /// </summary>
    [Serializable]
    public class AgentEventWrapper
    {
        /// <summary>
        /// The type/name of the event (automatically set to the class name)
        /// </summary>
        public string type;

        /// <summary>
        /// The actual event data payload
        /// </summary>
        [SerializeField]
        private string payloadJson;

        /// <summary>
        /// Gets or sets the payload object. Automatically serializes/deserializes to JSON.
        /// </summary>
        public object payload
        {
            get
            {
                // Return the JSON string representation
                return payloadJson;
            }
            set
            {
                // Serialize the object to JSON
                if (value != null)
                {
                    payloadJson = JsonUtility.ToJson(value);
                }
                else
                {
                    payloadJson = "{}";
                }
            }
        }

        /// <summary>
        /// Creates a new event wrapper
        /// </summary>
        public AgentEventWrapper()
        {
            type = "";
            payloadJson = "{}";
        }

        /// <summary>
        /// Creates a new event wrapper with specified type and payload
        /// </summary>
        public AgentEventWrapper(string eventType, object eventPayload)
        {
            type = eventType;
            payload = eventPayload;
        }
    }

    // Example event classes for reference
    #region Example Events
    
    /// <summary>
    /// Example: Player score event
    /// Usage: VoiceAgentBridge.Instance.SendGameEvent(new PlayerScoreEvent { score = 100, playerName = "Alice" });
    /// </summary>
    [Serializable]
    public class PlayerScoreEvent
    {
        public int score;
        public string playerName;
        public float timestamp;
    }

    /// <summary>
    /// Example: Level completion event
    /// Usage: VoiceAgentBridge.Instance.SendGameEvent(new LevelCompleteEvent { levelId = 5, timeTaken = 120.5f });
    /// </summary>
    [Serializable]
    public class LevelCompleteEvent
    {
        public int levelId;
        public float timeTaken;
        public bool perfectClear;
    }

    /// <summary>
    /// Example: Player action event
    /// Usage: VoiceAgentBridge.Instance.SendGameEvent(new PlayerActionEvent { action = "jump", position = transform.position });
    /// </summary>
    [Serializable]
    public class PlayerActionEvent
    {
        public string action;
        public Vector3 position;
        public float intensity;
    }

    #endregion
}
