namespace MoleMole
{
    using System;
    using UnityEngine;

    public class AmbientSound : MonoBehaviour
    {
        public string enterEventName;
        public string exitEventName;

        private void Awake()
        {
            if (!string.IsNullOrEmpty(this.enterEventName))
            {
                Singleton<WwiseAudioManager>.Instance.Post(this.enterEventName, null, null, null);
            }
        }

        private void OnDestroy()
        {
            if (!string.IsNullOrEmpty(this.exitEventName))
            {
                Singleton<WwiseAudioManager>.Instance.Post(this.exitEventName, null, null, null);
            }
        }
    }
}

