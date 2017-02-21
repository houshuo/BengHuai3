namespace MoleMole
{
    using LuaInterface;
    using System;
    using UnityEngine;

    public class LetMeCrash : MonoBehaviour
    {
        public static string orUpload = string.Empty;
        public static string orUrl = string.Empty;

        private void Awake()
        {
            UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
        }

        private string DrawField(string label, string val)
        {
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUILayout.Label(label, new GUILayoutOption[0]);
            string str = GUILayout.TextField((val != null) ? val : string.Empty, new GUILayoutOption[0]);
            GUILayout.EndHorizontal();
            return str;
        }

        private void OnGUI()
        {
            GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.MinWidth(200f), GUILayout.MinHeight(100f) };
            if (GUILayout.Button("Let Me Crash", options))
            {
                LuaDLL.lua_call(IntPtr.Zero, 1, 1);
            }
            orUrl = this.DrawField("Url", orUrl);
            orUpload = this.DrawField("Upload", orUpload);
        }
    }
}

