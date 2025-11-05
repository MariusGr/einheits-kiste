using System.Collections.Generic;
using UnityEngine;

namespace EinheitsKiste
{
    // source: https://discussions.unity.com/t/how-to-draw-debug-text-into-scene/14023/6
    public class DebugTextDrawer : MonoBehaviour
    {
        public class DebugTextData
        {
            public Vector3 position;
            public string text;
            public int fontSize;
            public Color? color;
            public float eraseTime;
        }

        public List<DebugTextData> Strings = new();

        public void OnDrawGizmos()
        {
            foreach (var stringData in Strings)
            {
                GUIStyle style = new();
                Color color = stringData.color ?? Color.green;
                style.normal.textColor = color;
                style.fontSize = stringData.fontSize;

#if UNITY_EDITOR
                UnityEditor.Handles.color = color;
                UnityEditor.Handles.Label(stringData.position, stringData.text, style);
#endif
            }
        }

        private static DebugTextDrawer m_instance;
        public static DebugTextDrawer instance
        {
            get
            {
                if (m_instance == null)
                {
                    m_instance = FindFirstObjectByType<DebugTextDrawer>();
                    if (m_instance == null)
                    {
                        var go = new GameObject("DeleteMeLater");
                        m_instance = go.AddComponent<DebugTextDrawer>();
                    }
                }
                return m_instance;
            }
        }

        public static void DrawText(Vector3 position, string text, int fontSize = 11, float duration = Mathf.Infinity)
            => DrawText(position, text, Color.red, fontSize, duration);

        public static void DrawText(Vector3 position, string text, Color color, int fontSize = 11, float duration = Mathf.Infinity)
        {
            instance.Strings.Add(new DebugTextData
            {
                text = text,
                fontSize = fontSize,
                color = color,
                position = position,
                eraseTime = Time.time + duration
            });

            List<DebugTextData> toBeRemoved = new();

            foreach (var item in instance.Strings)
            {
                if (item.eraseTime <= Time.time)
                    toBeRemoved.Add(item);
            }

            foreach (var rem in toBeRemoved)
                instance.Strings.Remove(rem);
        }
    }
}