#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;

namespace EinheitsKiste.Internal
{
    [InitializeOnLoad]
    public class KeyObjectReferenceAutoInitializer
    {
        static KeyObjectReferenceAutoInitializer()
        {
            EditorSceneManager.sceneSaved += OnSceneSaved;
            EditorApplication.update += OnEditorUpdate;
        }

        private static void OnSceneSaved(Scene scene)
        {
            // Initialize all KeyObjectReferences in the scene after save to catch any that changed
            InitializeAllInScene(scene);
        }

        private static void OnEditorUpdate()
        {
            // Check periodically for uninitialized references (catches duplicated objects)
            // This is a light check that only runs when needed
            if (EditorApplication.isUpdating || EditorApplication.isPlayingOrWillChangePlaymode) return;

            // Get the active scene
            var activeScene = SceneManager.GetActiveScene();
            if (!activeScene.isLoaded) return;

            // Only check every N frames to avoid performance impact
            if (Time.frameCount % 30 != 0) return;

            InitializeAllInScene(activeScene);
        }

        private static void InitializeAllInScene(Scene scene)
        {
            if (!scene.isLoaded) return;

            var rootObjects = scene.GetRootGameObjects();
            foreach (var root in rootObjects)
            {
                var monoBehaviours = root.GetComponentsInChildren<MonoBehaviour>();
                foreach (var mb in monoBehaviours)
                {
                    if (mb == null) continue;
                    try
                    {
                        KeyObjectReferenceInitializer.InitializeAllReferences(mb);
                    }
                    catch { /* Silently ignore errors to avoid spam */ }
                }
            }
        }
    }
}
#endif