# Game Event Tracker - Quick Start Guide

## 🎯 What is GameEventTracker?

**GameEventTracker** is a pre-built system that automatically tracks common game events and sends them to your Voice Agent via WebSocket. No need to write JSON serialization or event handling code!

---

## 🚀 Setup (2 Steps)

### Step 1: Setup Voice Agent Bridge
```
Tools > Voice Agent > Setup Bridge
```

### Step 2: Setup Event Tracker
```
Tools > Voice Agent > Setup Event Tracker
```

**Done!** The system is now ready to track events.

---

## 📖 Usage Examples

### Example 1: Track Level Progress

```csharp
using VoiceAgent;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    void Start()
    {
        // Track when level starts
        GameEventTracker.Instance.TrackLevelStart("Tutorial Level", 1);
    }

    void OnLevelComplete()
    {
        // Track when level completes
        GameEventTracker.Instance.TrackLevelComplete(
            levelName: "Tutorial Level",
            timeTaken: 45.5f,
            score: 1000,
            perfectClear: true
        );
    }
}
```

**JSON Sent:**
```json
{
  "type": "LevelCompleteEvent",
  "payload": {
    "levelName": "Tutorial Level",
    "timeTaken": 45.5,
    "score": 1000,
    "perfectClear": true,
    "timestamp": "2026-02-09 22:35:00"
  }
}
```

---

### Example 2: Track Player Actions

```csharp
public class PlayerController : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // Track jump action
            GameEventTracker.Instance.TrackPlayerAction(
                actionName: "jump",
                position: transform.position
            );
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // Track object interaction
        GameEventTracker.Instance.TrackInteraction(
            objectName: other.gameObject.name,
            interactionType: "collision",
            duration: 0f
        );
    }
}
```

---

### Example 3: Track Score/Points

```csharp
public class ScoreManager : MonoBehaviour
{
    void AddPoints(int points, string reason)
    {
        // Track score change
        GameEventTracker.Instance.TrackScore(
            score: points,
            playerName: "Player1",
            reason: reason
        );
    }
}
```

---

### Example 4: Track Emotions (ASD Therapy Games)

```csharp
public class EmotionDetector : MonoBehaviour
{
    void OnEmotionDetected(string emotion, float confidence)
    {
        // Track emotional state
        GameEventTracker.Instance.TrackEmotion(
            emotion: emotion,
            intensity: confidence,
            context: "facial_recognition"
        );
    }
}
```

**JSON Sent:**
```json
{
  "type": "EmotionEvent",
  "payload": {
    "emotion": "happy",
    "intensity": 0.85,
    "context": "facial_recognition",
    "timestamp": "2026-02-09 22:35:00"
  }
}
```

---

### Example 5: Track Learning Progress

```csharp
public class LearningModule : MonoBehaviour
{
    void OnTaskComplete(string skill, float accuracy, int attempts)
    {
        // Track learning progress
        GameEventTracker.Instance.TrackLearningProgress(
            skill: skill,
            accuracy: accuracy,
            attemptCount: attempts
        );
    }
}
```

---

### Example 6: Track Custom Game Phases

```csharp
public class GamePhaseManager : MonoBehaviour
{
    void StartMinigame(string minigameName)
    {
        GameEventTracker.Instance.TrackGamePhase(
            phaseName: minigameName,
            status: "started",
            metadata: "difficulty:easy"
        );
    }

    void EndMinigame(string minigameName)
    {
        GameEventTracker.Instance.TrackGamePhase(
            phaseName: minigameName,
            status: "completed",
            metadata: "difficulty:easy"
        );
    }
}
```

---

### Example 8: Track Your Own Custom Events (Developer's Own Classes)

**For game-specific events that only YOU know about:**

```csharp
using System;
using VoiceAgent;

// Create your own custom event class
[Serializable]
public class SecretPhaseEvent
{
    public string phaseName;
    public int secretCode;
    public bool isHidden;
}

[Serializable]
public class BossEncounterEvent
{
    public string bossName;
    public int bossLevel;
    public float playerHealth;
}

public class MyGameLogic : MonoBehaviour
{
    void OnSecretPhaseStart()
    {
        // Just pass your custom class - that's it!
        GameEventTracker.Instance.TrackEvent(new SecretPhaseEvent
        {
            phaseName = "HiddenLevel_X",
            secretCode = 12345,
            isHidden = true
        });
    }

    void OnBossEncounter(string boss, int level)
    {
        // Track any custom event
        GameEventTracker.Instance.TrackEvent(new BossEncounterEvent
        {
            bossName = boss,
            bossLevel = level,
            playerHealth = 75.5f
        });
    }
}
```

**JSON Sent:**
```json
{
  "type": "SecretPhaseEvent",
  "payload": {
    "phaseName": "HiddenLevel_X",
    "secretCode": 12345,
    "isHidden": true
  }
}
```

**The class name becomes the event type automatically!**

---

### Example 9: Track Achievements

```csharp
public class AchievementSystem : MonoBehaviour
{
    void UnlockAchievement(string name)
    {
        GameEventTracker.Instance.TrackAchievement(
            achievementName: name,
            description: "First time completing tutorial"
        );
    }
}
```

---

### Example 10: Track Custom Events (Alternative Method)

```csharp
public class CustomGameLogic : MonoBehaviour
{
    void OnSpecialEvent()
    {
        // Create custom data
        var customData = new {
            eventId = 123,
            customField = "value",
            nestedData = new { x = 10, y = 20 }
        };

        // Track completely custom event
        GameEventTracker.Instance.TrackCustomEvent(
            eventName: "SpecialGameEvent",
            eventData: customData
        );
    }
}
```

---

## 🔧 Auto-Tracking Features

The GameEventTracker automatically tracks:

### ✅ Scene Changes
Automatically sends an event when scenes load:
```json
{
  "type": "SceneChangeEvent",
  "payload": {
    "sceneName": "MainMenu",
    "timestamp": "2026-02-09 22:35:00"
  }
}
```

### ✅ App Lifecycle
Automatically tracks when the app pauses, resumes, or quits:
```json
{
  "type": "AppLifecycleEvent",
  "payload": {
    "state": "paused",
    "timestamp": "2026-02-09 22:35:00"
  }
}
```

**To disable auto-tracking:**
1. Select the `GameEventTracker` GameObject
2. Uncheck `Auto Track Scenes` or `Auto Track Lifecycle`

---

## 📋 Available Tracking Methods

| Method | Description | Use Case |
|--------|-------------|----------|
| `TrackEvent()` | **Track ANY custom event class** | **Your game-specific events** |
| `TrackLevelStart()` | Track level/scene start | Level progression |
| `TrackLevelComplete()` | Track level completion | Level progression |
| `TrackPlayerAction()` | Track player actions | Gameplay analytics |
| `TrackScore()` | Track score/points | Scoring systems |
| `TrackAchievement()` | Track achievements | Achievement systems |
| `TrackInteraction()` | Track object interactions | Interaction analytics |
| `TrackEmotion()` | Track emotional states | ASD therapy, emotion detection |
| `TrackLearningProgress()` | Track learning metrics | Educational games |
| `TrackGamePhase()` | Track custom game phases | Game state management |
| `TrackError()` | Track errors/issues | Error logging |
| `TrackCustomEvent()` | Track with JSON string | Flexible tracking |

---

## 🎯 Complete Integration Example

```csharp
using UnityEngine;
using VoiceAgent;

public class MyGameManager : MonoBehaviour
{
    void Start()
    {
        // Track game start
        GameEventTracker.Instance.TrackGamePhase("MainGame", "started");
        
        // Track level start
        GameEventTracker.Instance.TrackLevelStart("Level 1", 1);
    }

    void OnPlayerJump()
    {
        GameEventTracker.Instance.TrackPlayerAction("jump", transform.position);
    }

    void OnCollectCoin()
    {
        GameEventTracker.Instance.TrackScore(10, "Player1", "coin_collected");
    }

    void OnLevelComplete(float time)
    {
        GameEventTracker.Instance.TrackLevelComplete("Level 1", time, 1000, true);
    }

    void OnEmotionDetected(string emotion)
    {
        GameEventTracker.Instance.TrackEmotion(emotion, 0.9f, "gameplay");
    }
}
```

---

## 🔌 Server-Side Handling

Your Voice Agent will receive events in this format:

```json
{
  "type": "EventClassName",
  "payload": {
    // Event-specific data
    "timestamp": "2026-02-09 22:35:00"
  }
}
```

**Python Example:**
```python
import json

async def handle_unity_event(message):
    event = json.loads(message)
    event_type = event["type"]
    payload = json.loads(event["payload"])
    
    if event_type == "EmotionEvent":
        emotion = payload["emotion"]
        intensity = payload["intensity"]
        print(f"Player emotion: {emotion} (intensity: {intensity})")
        
    elif event_type == "LevelCompleteEvent":
        level = payload["levelName"]
        time = payload["timeTaken"]
        print(f"Level {level} completed in {time} seconds")
```

---

## ✨ Benefits

- ✅ **Zero Boilerplate** - No JSON serialization code needed
- ✅ **Pre-built Events** - 12+ common game event types included
- ✅ **Auto-Tracking** - Scenes and lifecycle tracked automatically
- ✅ **Type-Safe** - IntelliSense support for all methods
- ✅ **Timestamps** - Automatic timestamp on every event
- ✅ **Flexible** - Custom events for anything not covered

---

**Made with ❤️ for Game Developers**
