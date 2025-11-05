using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace EinheitsKiste
{
    public static partial class DebugTools
    {
        public static void DrawLine(Vector3 start, Vector3 end) => Debug.DrawLine(start, end, Color.red, Mathf.Infinity, false);
        public static void DrawLine(Vector3 start, Vector3 end, Color color) => Debug.DrawLine(start, end, color, Mathf.Infinity, false);
        public static void DrawLine(Vector3 start, Vector3 end, float duration) => Debug.DrawLine(start, end, Color.red, duration, false);
        public static void DrawLine(Vector3 start, Vector3 end, Color color, float duration) => Debug.DrawLine(start, end, color, duration, false);

        public static void DrawPoint(Vector3 point, float size = .1f, bool depthTest = false) =>
            DrawPoint(point, Color.red, size, depthTest);

        public static void DrawPoint(Vector3 point, Color color, float size = .1f, bool depthTest = false)
        {
            Vector3[] vectors = new Vector3[] {
                Vector3.up,
                Vector3.down,
                Vector3.right,
                Vector3.left,
                Vector3.forward,
                Vector3.back,
            };
            foreach (Vector3 vector in vectors)
            {
                Vector3 end = vector * size + point;
                Debug.DrawLine(point, end, color, Time.deltaTime, depthTest);
            }
        }

        public static void LogAll(params object[] list)
        {
            string[] parameters = new string[list.Length];
            for (int i = 0; i < list.Length; i++)
            {
                parameters[i] = list[i].ToString();
            }
            Log(parameters);
        }

        public static void Log(IEnumerable message)
        {
            try
            {
                Debug.Log($"{message} with length: {((ICollection)message).Count}");
            }
            catch
            {
                int count = 0;
                while (message.GetEnumerator().MoveNext())
                    count++;
                Debug.Log($"{message} with length: {count}");
            }
            foreach (object item in message)
            {
                try
                {
                    Log($"\t{(IEnumerable)item}");
                }
                catch
                {
                    Debug.Log($"\t{item}");
                }
            }
        }

        public static void Log(IDictionary message)
        {
            try
            {
                Debug.Log($"{message} with length: {message.Keys.Count}");
            }
            catch
            {
                Debug.Log(message);
                if (message.GetType() == typeof(string))
                    return;
            }
            foreach (object key in message.Keys)
            {
                try
                {
                    Debug.Log($"\t{key}: {message[key]}");
                }
                catch
                {
                    Debug.Log($"\t{key}: <No Item>");
                }
            }
        }

        public static void LogTransformPath(GameObject gameObject)
        {
            Transform parent = gameObject.transform.parent;
            List<Transform> parents = new();
            while (parent != null)
            {
                parents.Add(parent);
                parent = parent.parent;
            }

            parents.Reverse();

            string path = "/";
            foreach (var p in parents)
                path += $"{p.gameObject.name}/";

            Debug.Log(path + gameObject.name);
        }
    }
}
