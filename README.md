# Voice Agent Bridge

A **zero-friction WebSocket bridge** for connecting Unity games to Voice Agents. Send game events to your AI agent with a single line of code.

[![Unity Version](https://img.shields.io/badge/Unity-2020.3%2B-blue.svg)](https://unity.com/)
[![License](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)

---

## ✨ Features

- 🔌 **Plug & Play**: One-line setup via Unity Editor menu
- 🚀 **Zero Friction**: Send any game event with `SendGameEvent(myEvent)`
- 🎯 **Minimal Event Tracker**: Simple methods for tracking game phases, errors, and custom events
- 🔄 **Auto-Serialization**: Automatically wraps events in `{ "type": "ClassName", "payload": {...} }` format
- 🌐 **Cross-Platform**: Built-in WebSocket support for Standalone, Mobile platforms
- 🎯 **Singleton Pattern**: Access from anywhere via `VoiceAgentBridge.Instance`
- 🐛 **Debug Mode**: Built-in logging for development

---

## 📦 Installation

### One-Step Installation ✨

1. Open Unity Package Manager (`Window > Package Manager`)
2. Click the **`+`** button → **"Add package from git URL..."**
3. Paste: **`https://github.com/harsh2b/Voice_agent_connector.git`**
4. Click **Add**

**That's it!** 🎉 No dependencies, no extra steps. The package includes its own WebSocket implementation.

### Alternative: Manual Installation

1. Download or clone this repository
2. Copy the folder into your project's `Packages/` directory
3. Unity will automatically import the package

### 🚀 Zero Dependencies

This package includes a built-in WebSocket client (`SimpleWebSocket`) using .NET's `ClientWebSocket`. No external dependencies required!

---

## 🚀 Quick Start

### 1. Setup the Bridge

**Option A: Via Menu (Recommended)**
- Go to `Tools > Voice Agent > Setup Bridge`
- This creates a `VoiceAgent_Connector` GameObject with the bridge component

**Option B: Manual Setup**
- Create an empty GameObject
- Add the `VoiceAgentBridge` component
- Configure the `Agent URL` (default: `ws://localhost:8080`)

### 1b. Setup Event Tracker (Optional but Recommended)

**Quick Setup:**
- Go to `Tools > Voice Agent > Setup Event Tracker`
- This creates a `GameEventTracker` GameObject

**What you get:**
- ✅ `TrackEvent()` - Send any custom event
- ✅ `TrackGamePhase()` - Track game phases (started, completed, etc.)
- ✅ `TrackError()` - Track errors and issues
- ✅ Zero boilerplate code

### 2. Send Your First Event

Create a serializable event class:

```csharp
using System;

[Serializable]
public class PlayerScoreEvent
{
    public int score;
    public string playerName;
}
```

Send it to the Voice Agent:

```csharp
using VoiceAgent;

public class GameManager : MonoBehaviour
{
    void OnPlayerScored(int points)
    {
        // That's it! The bridge handles everything else.
        VoiceAgentBridge.Instance.SendGameEvent(new PlayerScoreEvent 
        { 
            score = points, 
            playerName = "Alice" 
        });
    }
}
```

### 3. What Gets Sent

The bridge automatically wraps your event:

```json
{
  "type": "PlayerScoreEvent",
  "payload": {
    "score": 100,
    "playerName": "Alice"
  }
}
```

---

## 🎯 Using GameEventTracker (Easier Way)

Use the **GameEventTracker** for common tracking needs:

```csharp
using VoiceAgent;

public class GameManager : MonoBehaviour
{
    void Start()
    {
        // Track game phase - one line!
        GameEventTracker.Instance.TrackGamePhase("ball grab", "started");
    }

    void OnPhaseComplete()
    {
        // Track phase completion
        GameEventTracker.Instance.TrackGamePhase("ball grab", "completed");
    }

    void OnError()
    {
        // Track errors
        GameEventTracker.Instance.TrackError("Player fell off map", "gameplay");
    }
    
    void OnCustomEvent()
    {
        // Track any custom event
        GameEventTracker.Instance.TrackEvent(new MyCustomEvent 
        { 
            data = "value" 
        });
    }
}
```

---

## 🎨 Creating Custom Events

### Method 1: Separate Events File (Recommended)

Create your own events file in your project to keep them separate from the package:

**Step 1:** Create `Assets/Scripts/MyCustomEvents.cs`:

```csharp
using System;
using UnityEngine;

namespace MyGame.Events
{
    [Serializable]
    public class TherapySessionEvent
    {
        public string sessionId;
        public int childAge;
        public string taskType;
        public float duration;
    }

    [Serializable]
    public class TaskCompletionEvent
    {
        public string taskName;
        public bool success;
        public int attempts;
        public float accuracy;
    }
}
```

**Step 2:** Use them in your game:

```csharp
using VoiceAgent;
using MyGame.Events;

public class GameManager : MonoBehaviour
{
    void OnTaskComplete()
    {
        GameEventTracker.Instance.TrackEvent(new TaskCompletionEvent
        {
            taskName = "Color Matching",
            success = true,
            attempts = 3,
            accuracy = 0.85f
        });
    }
}
```

**✅ Advantages:**
- Survives package updates
- Easy to version control
- Clean project organization

---

### Method 2: Edit Package File Directly

**⚠️ Not Recommended** - Changes will be lost on package updates!

1. In Unity, go to **Packages > Voice Agent Bridge > Runtime > GameEventTracker.cs**
2. Scroll to the bottom and find the marked section:
   ```csharp
   // 👇 ADD YOUR CUSTOM EVENT CLASSES BELOW 👇
   ```
3. Add your event classes between the arrows

---

### What Gets Sent

Any custom event you create will be automatically wrapped in this format:

```json
{
  "type": "YourEventClassName",
  "payload": {
    "field1": "value1",
    "field2": 123
  }
}
```

The **class name** becomes the event **type** automatically!

---

## 📖 Usage Examples

### Example 1: Level Completion

```csharp
[Serializable]
public class LevelCompleteEvent
{
    public int levelId;
    public float timeTaken;
    public bool perfectClear;
}

// In your game code:
VoiceAgentBridge.Instance.SendGameEvent(new LevelCompleteEvent
{
    levelId = 5,
    timeTaken = 120.5f,
    perfectClear = true
});
```

### Example 2: Player Actions

```csharp
[Serializable]
public class PlayerActionEvent
{
    public string action;
    public Vector3 position;
}

// Send player jump event
VoiceAgentBridge.Instance.SendGameEvent(new PlayerActionEvent
{
    action = "jump",
    position = transform.position
});
```

### Example 3: Custom Connection Events

```csharp
using VoiceAgent;

public class MyGameController : MonoBehaviour
{
    void Start()
    {
        // Subscribe to connection events
        VoiceAgentBridge.Instance.OnConnected += HandleConnected;
        VoiceAgentBridge.Instance.OnDisconnected += HandleDisconnected;
        VoiceAgentBridge.Instance.OnMessageReceived += HandleMessage;
    }

    void HandleConnected()
    {
        Debug.Log("Voice Agent connected!");
    }

    void HandleDisconnected()
    {
        Debug.Log("Voice Agent disconnected!");
    }

    void HandleMessage(string message)
    {
        Debug.Log($"Received from agent: {message}");
    }
}
```

---

## ⚙️ Configuration

Select the `VoiceAgent_Connector` GameObject to configure:

| Property | Description | Default |
|----------|-------------|---------|
| **Agent URL** | WebSocket server address | `ws://localhost:8080` |
| **Auto Connect** | Connect automatically on Start | `true` |
| **Debug Mode** | Enable console logging | `true` |

---

## 🔌 Server-Side Integration

Your Voice Agent server should expect this JSON format:

```json
{
  "type": "EventClassName",
  "payload": {
    // Your event data here
  }
}
```

### Example Node.js Server

```javascript
const WebSocket = require('ws');
const wss = new WebSocket.Server({ port: 8080 });

wss.on('connection', (ws) => {
  console.log('Unity game connected!');

  ws.on('message', (data) => {
    const event = JSON.parse(data);
    console.log(`Received ${event.type}:`, event.payload);

    // Handle different event types
    switch(event.type) {
      case 'PlayerScoreEvent':
        console.log(`Player scored: ${event.payload.score}`);
        break;
      case 'LevelCompleteEvent':
        console.log(`Level ${event.payload.levelId} completed!`);
        break;
    }
  });
});
```

---

## 🛠️ API Reference

### VoiceAgentBridge

#### Properties
- `Instance` - Singleton instance
- `IsConnected` - Connection status (bool)

#### Methods
- `Connect()` - Establish WebSocket connection
- `Disconnect()` - Close WebSocket connection
- `SendGameEvent(object eventData)` - Send typed game event
- `SendRawMessage(string message)` - Send raw text message

#### Events
- `OnConnected` - Fired when connection established
- `OnDisconnected` - Fired when connection closed
- `OnMessageReceived(string message)` - Fired when message received
- `OnErrorOccurred(string error)` - Fired on error

---

## 🐛 Troubleshooting

### "WebSocket connection failed"
- Ensure your Voice Agent server is running
- Check the `Agent URL` is correct
- Verify firewall settings

### Events not sending
- Ensure your event class is `[Serializable]`
- Check `Debug Mode` is enabled to see logs
- Verify connection status with `IsConnected`

---

## 📄 License

MIT License - See [LICENSE](LICENSE) file for details

---

## 🤝 Contributing

Contributions are welcome! Please open an issue or submit a pull request.

---

## 📞 Support

- **Issues**: [GitHub Issues](https://github.com/harsh2b/Voice_agent_connector/issues)
- **Discussions**: [GitHub Discussions](https://github.com/harsh2b/Voice_agent_connector/discussions)

---

**Made with ❤️ for Unity Developers**
