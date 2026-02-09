using UnityEngine;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using System.Linq;

namespace VoiceAgent.Editor
{
    /// <summary>
    /// Automatically installs required dependencies when Voice Agent Bridge is imported
    /// </summary>
    [InitializeOnLoad]
    public static class AutoInstallDependencies
    {
        private const string NATIVEWEBSOCKET_PACKAGE = "com.endel.nativewebsocket";
        private const string NATIVEWEBSOCKET_GIT_URL = "https://github.com/endel/NativeWebSocket.git#upm";
        private const string PREFS_KEY = "VoiceAgentBridge_DependencyCheckDone";

        private static ListRequest listRequest;
        private static AddRequest addRequest;

        static AutoInstallDependencies()
        {
            // Only run once per session to avoid repeated checks
            if (SessionState.GetBool(PREFS_KEY, false))
            {
                return;
            }

            // Check if NativeWebSocket is already installed
            listRequest = Client.List(true);
            EditorApplication.update += CheckPackageList;
        }

        private static void CheckPackageList()
        {
            if (listRequest == null || !listRequest.IsCompleted)
            {
                return;
            }

            EditorApplication.update -= CheckPackageList;

            if (listRequest.Status == StatusCode.Success)
            {
                // Check if NativeWebSocket is already installed
                bool isInstalled = listRequest.Result.Any(package => package.name == NATIVEWEBSOCKET_PACKAGE);

                if (!isInstalled)
                {
                    Debug.Log("[Voice Agent Bridge] NativeWebSocket not found. Installing automatically...");
                    InstallNativeWebSocket();
                }
                else
                {
                    Debug.Log("[Voice Agent Bridge] ✓ All dependencies are installed.");
                    SessionState.SetBool(PREFS_KEY, true);
                }
            }
            else if (listRequest.Status >= StatusCode.Failure)
            {
                Debug.LogError($"[Voice Agent Bridge] Failed to list packages: {listRequest.Error.message}");
            }

            listRequest = null;
        }

        private static void InstallNativeWebSocket()
        {
            addRequest = Client.Add(NATIVEWEBSOCKET_GIT_URL);
            EditorApplication.update += CheckInstallProgress;
        }

        private static void CheckInstallProgress()
        {
            if (addRequest == null || !addRequest.IsCompleted)
            {
                return;
            }

            EditorApplication.update -= CheckInstallProgress;

            if (addRequest.Status == StatusCode.Success)
            {
                Debug.Log($"[Voice Agent Bridge] ✓ Successfully installed NativeWebSocket ({addRequest.Result.displayName})");
                
                EditorUtility.DisplayDialog(
                    "Voice Agent Bridge - Dependencies Installed",
                    "NativeWebSocket has been automatically installed!\n\n" +
                    "Voice Agent Bridge is now ready to use.\n\n" +
                    "Go to Tools > Voice Agent > Setup Bridge to get started.",
                    "OK"
                );

                SessionState.SetBool(PREFS_KEY, true);
            }
            else if (addRequest.Status >= StatusCode.Failure)
            {
                Debug.LogError($"[Voice Agent Bridge] Failed to install NativeWebSocket: {addRequest.Error.message}");
                
                EditorUtility.DisplayDialog(
                    "Voice Agent Bridge - Manual Installation Required",
                    "Failed to automatically install NativeWebSocket.\n\n" +
                    "Please install it manually:\n" +
                    "1. Open Package Manager\n" +
                    "2. Click + > Add package from git URL\n" +
                    "3. Paste: https://github.com/endel/NativeWebSocket.git#upm\n\n" +
                    $"Error: {addRequest.Error.message}",
                    "OK"
                );
            }

            addRequest = null;
        }
    }
}
