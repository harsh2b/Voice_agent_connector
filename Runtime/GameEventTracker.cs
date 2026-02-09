using System;
using UnityEngine;

namespace VoiceAgent
{
    /// <summary>
    /// Automatic game event tracker that sends common game events to Voice Agent
    /// Developers just need to call simple methods like TrackLevelStart(), TrackPlayerAction(), etc.
    /// </summary>
    public class GameEventTracker : MonoBehaviour
    {
        [Header("Auto-Tracking Settings")]
        [Tooltip("Automatically track scene changes")]
        public bool autoTrackScenes = true;

        [Tooltip("Automatically track application lifecycle (pause, quit)")]
        public bool autoTrackLifecycle = true;

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

        private void Start()
        {
            if (autoTrackScenes)
            {
                UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
            }
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            if (autoTrackLifecycle)
            {
                TrackAppLifecycle(pauseStatus ? "paused" : "resumed");
            }
        }

        private void OnApplicationQuit()
        {
            if (autoTrackLifecycle)
            {
                TrackAppLifecycle("quit");
            }
        }

        private void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode)
        {
            TrackSceneChange(scene.name);
        }

        #region Public Tracking Methods

        /// <summary>
        /// Track when a level/scene starts
        /// </summary>
        public void TrackLevelStart(string levelName, int levelNumber = 0)
        {
            VoiceAgentBridge.Instance.SendGameEvent(new LevelStartEvent
            {
                levelName = levelName,
                levelNumber = levelNumber,
                timestamp = GetTimestamp()
            });
        }

        /// <summary>
        /// Track when a level is completed
        /// </summary>
        public void TrackLevelComplete(string levelName, float timeTaken, int score = 0, bool perfectClear = false)
        {
            VoiceAgentBridge.Instance.SendGameEvent(new LevelCompleteEvent
            {
                levelName = levelName,
                timeTaken = timeTaken,
                score = score,
                perfectClear = perfectClear,
                timestamp = GetTimestamp()
            });
        }

        /// <summary>
        /// Track player actions (jump, shoot, interact, etc.)
        /// </summary>
        public void TrackPlayerAction(string actionName, Vector3 position = default, string targetObject = "")
        {
            VoiceAgentBridge.Instance.SendGameEvent(new PlayerActionEvent
            {
                action = actionName,
                position = position,
                targetObject = targetObject,
                timestamp = GetTimestamp()
            });
        }

        /// <summary>
        /// Track player score/points
        /// </summary>
        public void TrackScore(int score, string playerName = "Player", string reason = "")
        {
            VoiceAgentBridge.Instance.SendGameEvent(new PlayerScoreEvent
            {
                score = score,
                playerName = playerName,
                reason = reason,
                timestamp = GetTimestamp()
            });
        }

        /// <summary>
        /// Track player achievements/milestones
        /// </summary>
        public void TrackAchievement(string achievementName, string description = "")
        {
            VoiceAgentBridge.Instance.SendGameEvent(new AchievementEvent
            {
                achievementName = achievementName,
                description = description,
                timestamp = GetTimestamp()
            });
        }

        /// <summary>
        /// Track player interactions with objects
        /// </summary>
        public void TrackInteraction(string objectName, string interactionType = "click", float duration = 0f)
        {
            VoiceAgentBridge.Instance.SendGameEvent(new InteractionEvent
            {
                objectName = objectName,
                interactionType = interactionType,
                duration = duration,
                timestamp = GetTimestamp()
            });
        }

        /// <summary>
        /// Track player emotional state (for ASD therapy games)
        /// </summary>
        public void TrackEmotion(string emotion, float intensity = 1.0f, string context = "")
        {
            VoiceAgentBridge.Instance.SendGameEvent(new EmotionEvent
            {
                emotion = emotion,
                intensity = intensity,
                context = context,
                timestamp = GetTimestamp()
            });
        }

        /// <summary>
        /// Track learning progress (for educational games)
        /// </summary>
        public void TrackLearningProgress(string skill, float accuracy, int attemptCount = 1)
        {
            VoiceAgentBridge.Instance.SendGameEvent(new LearningProgressEvent
            {
                skill = skill,
                accuracy = accuracy,
                attemptCount = attemptCount,
                timestamp = GetTimestamp()
            });
        }

        /// <summary>
        /// Track ANY custom event by passing your own event class
        /// The class name will be used as the event type automatically
        /// Example: TrackEvent(new MyHiddenPhaseEvent { phase = "secret_level", data = 123 });
        /// </summary>
        public void TrackEvent(object eventData)
        {
            // Directly send the event using VoiceAgentBridge
            // This allows developers to create ANY custom event class
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

        #endregion

        #region Private Helper Methods

        private void TrackSceneChange(string sceneName)
        {
            VoiceAgentBridge.Instance.SendGameEvent(new SceneChangeEvent
            {
                sceneName = sceneName,
                timestamp = GetTimestamp()
            });
        }

        private void TrackAppLifecycle(string state)
        {
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
    public class LevelStartEvent
    {
        public string levelName;
        public int levelNumber;
        public string timestamp;
    }

    [Serializable]
    public class LevelCompleteEvent
    {
        public string levelName;
        public float timeTaken;
        public int score;
        public bool perfectClear;
        public string timestamp;
    }

    [Serializable]
    public class PlayerActionEvent
    {
        public string action;
        public Vector3 position;
        public string targetObject;
        public string timestamp;
    }

    [Serializable]
    public class PlayerScoreEvent
    {
        public int score;
        public string playerName;
        public string reason;
        public string timestamp;
    }

    [Serializable]
    public class AchievementEvent
    {
        public string achievementName;
        public string description;
        public string timestamp;
    }

    [Serializable]
    public class InteractionEvent
    {
        public string objectName;
        public string interactionType;
        public float duration;
        public string timestamp;
    }

    [Serializable]
    public class EmotionEvent
    {
        public string emotion;
        public float intensity;
        public string context;
        public string timestamp;
    }

    [Serializable]
    public class LearningProgressEvent
    {
        public string skill;
        public float accuracy;
        public int attemptCount;
        public string timestamp;
    }

    [Serializable]
    public class GamePhaseEvent
    {
        public string phaseName;
        public string status;
        public string metadata;
        public string timestamp;
    }

    [Serializable]
    public class SceneChangeEvent
    {
        public string sceneName;
        public string timestamp;
    }

    [Serializable]
    public class AppLifecycleEvent
    {
        public string state;
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
