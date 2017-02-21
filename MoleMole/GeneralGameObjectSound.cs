namespace MoleMole
{
    using System;
    using UnityEngine;

    public class GeneralGameObjectSound : MonoBehaviour
    {
        public string enterEventName;
        public string exitEventName;

        private void OnDisable()
        {
            this.PlayPatterns(this.exitEventName);
        }

        private void OnEnable()
        {
            this.PlayPatterns(this.enterEventName);
        }

        private void PlayPatterns(string content)
        {
            char[] separator = new char[] { ';' };
            string[] strArray = content.Split(separator);
            int index = 0;
            int length = strArray.Length;
            while (index < length)
            {
                if (!string.IsNullOrEmpty(strArray[index]))
                {
                    Singleton<WwiseAudioManager>.Instance.Post(strArray[index], null, null, null);
                }
                index++;
            }
        }

        [AnimationCallback]
        private void TriggerAudioPattern(string name)
        {
            this.PlayPatterns(name);
        }
    }
}

