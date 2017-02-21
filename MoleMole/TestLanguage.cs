namespace MoleMole
{
    using System;
    using UnityEngine;

    public class TestLanguage : MonoBehaviour
    {
        private int curLangIndex;
        private string language = string.Empty;
        private string[] targetLanguages = new string[] { "Chinese(PRC)", "Japanese" };

        private void OnGUI()
        {
            GUILayout.Label("=", new GUILayoutOption[0]);
            GUILayout.Label("=", new GUILayoutOption[0]);
            GUILayout.Label("=", new GUILayoutOption[0]);
            GUILayout.Label("=", new GUILayoutOption[0]);
            GUILayout.Label("=", new GUILayoutOption[0]);
            GUILayout.Label("=", new GUILayoutOption[0]);
            GUILayout.Label("=", new GUILayoutOption[0]);
            GUILayout.Label("=", new GUILayoutOption[0]);
            GUILayout.Label("=", new GUILayoutOption[0]);
            GUILayout.Label("=", new GUILayoutOption[0]);
            string language = Singleton<WwiseAudioManager>.Instance.GetLanguage();
            if (GUILayout.Button("Lang : " + ((language != null) ? language : "<null>"), new GUILayoutOption[0]))
            {
                this.curLangIndex = (this.curLangIndex + 1) % this.targetLanguages.Length;
                Singleton<WwiseAudioManager>.Instance.SetLanguage(this.targetLanguages[this.curLangIndex]);
            }
            GUILayout.Label("====================================================", new GUILayoutOption[0]);
            this.language = GUILayout.TextField(this.language, new GUILayoutOption[0]);
            if (GUILayout.Button("Set", new GUILayoutOption[0]))
            {
                Singleton<WwiseAudioManager>.Instance.SetLanguage(this.language);
            }
        }

        private void Start()
        {
            UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
        }

        private void Update()
        {
        }
    }
}

