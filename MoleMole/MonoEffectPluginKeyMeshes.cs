namespace MoleMole
{
    using System;
    using UnityEngine;

    public class MonoEffectPluginKeyMeshes : BaseMonoEffectPlugin
    {
        private int _curKey;
        private float[] _keyedTime;
        private float _timer;
        [Header("How many frames does the keyed transform last")]
        public int[] keyedFrames;
        [Header("Drag consequent key transforms into this.")]
        public Transform[] keyedTransforms;

        protected override void Awake()
        {
            base.Awake();
            this._timer = 0f;
            float num = 0f;
            this._keyedTime = new float[this.keyedFrames.Length];
            for (int i = 0; i < this._keyedTime.Length; i++)
            {
                this._keyedTime[i] = num + (this.keyedFrames[i] * 0.01666667f);
            }
        }

        public override bool IsToBeRemove()
        {
            return (this._curKey >= this._keyedTime.Length);
        }

        public override void SetDestroy()
        {
            this.keyedTransforms[this._curKey].gameObject.SetActive(false);
            this._curKey = this._keyedTime.Length;
        }

        public override void Setup()
        {
            for (int i = 0; i < this.keyedTransforms.Length; i++)
            {
                this.keyedTransforms[i].gameObject.SetActive(false);
            }
            this._curKey = 0;
            this.keyedTransforms[this._curKey].gameObject.SetActive(true);
            this._timer = 0f;
        }

        protected void Update()
        {
            if (this._curKey < this._keyedTime.Length)
            {
                this._timer += Time.deltaTime * base._effect.TimeScale;
                int index = this._curKey;
                while (this._timer > this._keyedTime[this._curKey])
                {
                    this._curKey++;
                    if (this._curKey == this._keyedTime.Length)
                    {
                        this.keyedTransforms[this._keyedTime.Length - 1].gameObject.SetActive(false);
                        return;
                    }
                }
                if (index != this._curKey)
                {
                    this.keyedTransforms[index].gameObject.SetActive(false);
                    this.keyedTransforms[this._curKey].gameObject.SetActive(true);
                }
            }
        }
    }
}

