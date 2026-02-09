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

    // NOTE: Event classes have been moved to GameEventTracker.cs
    // Use GameEventTracker.Instance.TrackXXX() methods or create your own custom events
}
