namespace MoleMole
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public class MonoUIAnimationHelper : MonoBehaviour
    {
        private Dictionary<string, Animation[]> _animMap;
        public ConfigAnimPattern[] patterns;

        [AnimationCallback]
        public void AnimCallback(string callBackStr)
        {
            Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.AnimCallBack, callBackStr));
        }

        public void Awake()
        {
            this._animMap = new Dictionary<string, Animation[]>();
            if (this.patterns != null)
            {
                foreach (ConfigAnimPattern pattern in this.patterns)
                {
                    this._animMap.Add(pattern.name, pattern.subAnims);
                }
            }
        }

        [AnimationCallback]
        public void DestroyContext(string contextName)
        {
            Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.AnimDestroyContext, contextName));
        }

        [AnimationCallback]
        public void PlayPattern(string patternName)
        {
            foreach (Animation animation in this._animMap[patternName])
            {
                if ((animation != null) && animation.gameObject.activeSelf)
                {
                    animation.Play();
                }
            }
        }

        [Serializable]
        public class ConfigAnimPattern
        {
            public string name;
            public Animation[] subAnims;
        }
    }
}

