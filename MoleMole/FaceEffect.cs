namespace MoleMole
{
    using System;
    using UnityEngine;

    public class FaceEffect
    {
        private MonoFaceEffect _effect;

        private GameObject GetEffectByName(string name)
        {
            int index = 0;
            int length = this._effect.items.Length;
            while (index < length)
            {
                if (this._effect.items[index].name == name)
                {
                    return this._effect.items[index].effect;
                }
                index++;
            }
            return null;
        }

        public void HideAll()
        {
            int index = 0;
            int length = this._effect.items.Length;
            while (index < length)
            {
                this._effect.items[index].effect.SetActive(false);
                index++;
            }
        }

        public void HideEffect(string name)
        {
            GameObject effectByName = this.GetEffectByName(name);
            if (effectByName != null)
            {
                effectByName.SetActive(false);
            }
        }

        public void Init(MonoFaceEffect effect)
        {
            this._effect = effect;
        }

        public void ShowEffect(string name)
        {
            GameObject effectByName = this.GetEffectByName(name);
            if (effectByName != null)
            {
                effectByName.SetActive(true);
            }
        }

        public void Uninit()
        {
            if (this._effect != null)
            {
                UnityEngine.Object.Destroy(this._effect.gameObject);
                this._effect = null;
            }
        }
    }
}

