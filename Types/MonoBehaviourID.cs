// https://www.reddit.com/r/Unity3D/comments/fdc2on/easily_generate_unique_ids_for_your_game_objects/
using System.Linq;
using System;
using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif

namespace EinheitsKiste
{
    public class MonoBehaviourID : MonoBehaviour
    {
        [SerializeField] private string _id;
        [SerializeField] private bool _maintainIdAutomatically = false;

#if UNITY_EDITOR
        private void OnValidate()
        {
            Initialize();

            // Make sure that IDs in assets are unique
            if (gameObject.scene != null || IsUnique(_id)) return;

            Debug.LogError($"MonoBehaviourID: ID {_id} is not unique in scene {gameObject.scene.name}.", this);
        }

        private bool IsUnique(string guid) =>
            Resources.FindObjectsOfTypeAll<MonoBehaviourID>()
                .Count(x => x.Value == guid && x != this) == 0;

        private void ResetId()
        {
            _id = Guid.NewGuid().ToString();


            // Perform the rest only if we are not running Tests
            if (Environment.StackTrace.Contains("UnityEngine.TestRunner"))
                return;

            PrefabUtility.RecordPrefabInstancePropertyModifications(this);
            EditorUtility.SetDirty(this);
            EditorSceneManager.MarkSceneDirty(gameObject.scene);
            Undo.RecordObject(this, "Assign GUID");

            Debug.Log($"Setting new ID on object {gameObject.name}: {_id}");
        }

        public bool Initialize()
        {
            bool reset = string.IsNullOrEmpty(_id) || _maintainIdAutomatically && !IsUnique(_id);
            if (reset)
                ResetId();
            return reset;
        }
#endif

        public string Value
        {
            get { return _id; }
            set { _id = value; }
        }

        public override bool Equals(object other)
        {
            if (other is MonoBehaviourID otherId) return _id == otherId._id;
            return false;
        }

        public bool IsValid() => !string.IsNullOrEmpty(_id) && _id != "0" && _id != "00000000-0000-0000-0000-000000000000";
        public override int GetHashCode() => _id?.GetHashCode() ?? 0;
        public override string ToString() => _id;
    }
}
