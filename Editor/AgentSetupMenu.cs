using UnityEngine;
using UnityEditor;

namespace VoiceAgent.Editor
{
    /// <summary>
    /// Editor utility for setting up Voice Agent Bridge in the scene
    /// </summary>
    public class AgentSetupMenu : MonoBehaviour
    {
        private const string CONNECTOR_OBJECT_NAME = "VoiceAgent_Connector";

        [MenuItem("Tools/Voice Agent/Setup Bridge")]
        public static void SetupBridge()
        {
            // Check if VoiceAgentBridge already exists in the scene
            VoiceAgentBridge existingBridge = FindObjectOfType<VoiceAgentBridge>();

            if (existingBridge != null)
            {
                // Bridge already exists, just select it
                Selection.activeGameObject = existingBridge.gameObject;
                EditorGUIUtility.PingObject(existingBridge.gameObject);
                
                Debug.Log($"[Voice Agent] Bridge already exists on '{existingBridge.gameObject.name}'. Selected for editing.");
                
                EditorUtility.DisplayDialog(
                    "Voice Agent Bridge", 
                    $"Voice Agent Bridge is already set up on GameObject '{existingBridge.gameObject.name}'.\n\nThe object has been selected in the hierarchy.", 
                    "OK"
                );
            }
            else
            {
                // Create new GameObject with VoiceAgentBridge component
                GameObject connectorObject = new GameObject(CONNECTOR_OBJECT_NAME);
                VoiceAgentBridge bridge = connectorObject.AddComponent<VoiceAgentBridge>();
                
                // Select the newly created object
                Selection.activeGameObject = connectorObject;
                EditorGUIUtility.PingObject(connectorObject);
                
                // Mark scene as dirty to ensure changes are saved
                UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
                    UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene()
                );
                
                Debug.Log($"[Voice Agent] ✓ Bridge setup complete! GameObject '{CONNECTOR_OBJECT_NAME}' created and selected.");
                
                EditorUtility.DisplayDialog(
                    "Voice Agent Bridge Setup Complete", 
                    $"Voice Agent Bridge has been successfully set up!\n\n" +
                    $"• GameObject '{CONNECTOR_OBJECT_NAME}' created\n" +
                    $"• VoiceAgentBridge component attached\n" +
                    $"• Default settings applied\n\n" +
                    $"Configure the connection settings in the Inspector.", 
                    "OK"
                );
            }
        }

        [MenuItem("Tools/Voice Agent/Setup Event Tracker")]
        public static void SetupEventTracker()
        {
            // Check if GameEventTracker already exists
            GameEventTracker existingTracker = FindObjectOfType<GameEventTracker>();

            if (existingTracker != null)
            {
                Selection.activeGameObject = existingTracker.gameObject;
                EditorGUIUtility.PingObject(existingTracker.gameObject);
                
                Debug.Log($"[Voice Agent] Event Tracker already exists on '{existingTracker.gameObject.name}'. Selected for editing.");
                
                EditorUtility.DisplayDialog(
                    "Voice Agent Event Tracker", 
                    $"Event Tracker is already set up on GameObject '{existingTracker.gameObject.name}'.\n\nThe object has been selected in the hierarchy.", 
                    "OK"
                );
            }
            else
            {
                // Create new GameObject with GameEventTracker component
                GameObject trackerObject = new GameObject("GameEventTracker");
                GameEventTracker tracker = trackerObject.AddComponent<GameEventTracker>();
                
                Selection.activeGameObject = trackerObject;
                EditorGUIUtility.PingObject(trackerObject);
                
                UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
                    UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene()
                );
                
                Debug.Log($"[Voice Agent] ✓ Event Tracker setup complete! GameObject 'GameEventTracker' created and selected.");
                
                EditorUtility.DisplayDialog(
                    "Voice Agent Event Tracker Setup Complete", 
                    $"Game Event Tracker has been successfully set up!\n\n" +
                    $"• GameObject 'GameEventTracker' created\n" +
                    $"• Auto-tracking enabled for scenes and lifecycle\n" +
                    $"• Use GameEventTracker.Instance.TrackXXX() in your code\n\n" +
                    $"Check the documentation for available tracking methods.", 
                    "OK"
                );
            }
        }

        [MenuItem("Tools/Voice Agent/About")]
        public static void ShowAbout()
        {
            EditorUtility.DisplayDialog(
                "Voice Agent Bridge v1.0.0",
                "A zero-friction WebSocket bridge for connecting Unity games to Voice Agents.\n\n" +
                "Features:\n" +
                "• Automatic event serialization\n" +
                "• Singleton pattern for easy access\n" +
                "• Cross-platform WebSocket support\n" +
                "• Debug logging\n\n" +
                "Usage:\n" +
                "VoiceAgentBridge.Instance.SendGameEvent(new MyEvent { data = value });",
                "OK"
            );
        }

        [MenuItem("Tools/Voice Agent/Documentation")]
        public static void OpenDocumentation()
        {
            Application.OpenURL("https://github.com/yourusername/voice-agent-bridge#readme");
        }
    }
}
