# Voice Agent Bridge

A **zero-friction WebSocket bridge** for connecting Unity games to Voice Agents. Send game events to your AI agent with a single line of code.

[![Unity Version](https://img.shields.io/badge/Unity-2020.3%2B-blue.svg)](https://unity.com/)
[![License](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)

---

## ✨ Features

- 🔌 **Plug & Play**: One-line setup via Unity Editor menu
- 🚀 **Zero Friction**: Send any game event with `SendGameEvent(myEvent)`
- 🔄 **Auto-Serialization**: Automatically wraps events in `{ "type": "ClassName", "payload": {...} }` format
- 🌐 **Cross-Platform**: Uses NativeWebSocket for WebGL, Standalone, Mobile support
- 🎯 **Singleton Pattern**: Access from anywhere via `VoiceAgentBridge.Instance`
- 🐛 **Debug Mode**: Built-in logging for development

---

## 📦 Installation

### Step 1: Install NativeWebSocket (Required Dependency)

1. Open Unity Package Manager (`Window > Package Manager`)
2. Click the **`+`** button in the top-left corner
3. Select **"Add package from git URL..."**
4. Paste: `https://github.com/endel/NativeWebSocket.git#upm`
5. Click **Add**
6. Wait for installation to complete

### Step 2: Install Voice Agent Bridge

#### Method A: Via Git URL (Recommended)

1. In Package Manager, click **`+`** again
2. Select **"Add package from git URL..."**
3. Paste: `https://github.com/harsh2b/Voice_agent_connector.git`
4. Click **Add**

#### Method B: Manual Installation

1. Download or clone this repository
2. Copy the entire folder into your project's `Packages/` directory
3. Unity will automatically detect and import the package

### ⚠️ Important Notes

- **NativeWebSocket must be installed FIRST** before installing Voice Agent Bridge
- Both packages use Git URLs and are managed via Unity Package Manager
- If you encounter errors, ensure both packages are properly installed

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

### "NativeWebSocket not found"
- Install the dependency manually: `https://github.com/endel/NativeWebSocket.git#upm`

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

- **Issues**: [GitHub Issues](https://github.com/yourusername/voice-agent-bridge/issues)
- **Discussions**: [GitHub Discussions](https://github.com/yourusername/voice-agent-bridge/discussions)

---

**Made with ❤️ for Unity Developers**
