namespace MoleMole
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public class MonoEffectPluginAudio : BaseMonoEffectPlugin
    {
        private List<EffectAudioItem> _itemList = new List<EffectAudioItem>();
        private float _timer;
        public EffectAudioItem[] enterPatternName;
        public string[] exitPatternName;

        public override bool IsToBeRemove()
        {
            return false;
        }

        private void OnDisable()
        {
            this._timer = 0f;
            this._itemList.Clear();
            int index = 0;
            int length = this.exitPatternName.Length;
            while (index < length)
            {
                if ((base._effect.owner == null) || (base._effect.owner.GetComponent<Collider>() == null))
                {
                    Singleton<WwiseAudioManager>.Instance.Post(this.exitPatternName[index], null, null, null);
                }
                else
                {
                    Singleton<WwiseAudioManager>.Instance.Post(this.exitPatternName[index], base._effect.owner.gameObject, null, null);
                }
                index++;
            }
        }

        private void OnEnable()
        {
            this._timer = 0f;
            this._itemList.AddRange(this.enterPatternName);
        }

        public override void SetDestroy()
        {
        }

        public override void Setup()
        {
        }

        private void Update()
        {
            this._timer += Time.deltaTime;
            for (int i = 0; i < this._itemList.Count; i++)
            {
                if (this._timer >= this._itemList[i].delayTime)
                {
                    if ((base._effect.owner == null) || (base._effect.owner.GetComponent<Collider>() == null))
                    {
                        Singleton<WwiseAudioManager>.Instance.Post(this._itemList[i].eventName, null, null, null);
                    }
                    else
                    {
                        Singleton<WwiseAudioManager>.Instance.Post(this._itemList[i].eventName, base._effect.owner.gameObject, null, null);
                    }
                    this._itemList.RemoveAt(i);
                    i--;
                }
            }
        }
    }
}

