namespace MoleMole
{
    using System;
    using UnityEngine;

    [ExecuteInEditMode]
    public class MonoAnimationControlRenderer : MonoBehaviour
    {
        private MaterialPropertyBlock _block;
        private int _colorPropertyID;
        private int _floatPropertyID;
        private int _floatPropertyID2;
        private int _floatPropertyID3;
        private int _vectorPropertyID;
        private int _vectorPropertyID2;
        [Header("Keyed color Property, leave key string empty to not take effect")]
        public string colorPropertyKey;
        [Header("Keyed float Property, leave key string empty to not take effect")]
        public string floatPropertyKey;
        [Header("Keyed float Property, leave key string empty to not take effect")]
        public string floatPropertyKey2;
        [Header("Keyed float Property, leave key string empty to not take effect")]
        public string floatPropertyKey3;
        public Color keyedColor;
        public float keyedFloat;
        public float keyedFloat2;
        public float keyedFloat3;
        public Vector4 keyedVector;
        public Vector4 keyedVector2;
        [Header("Target Renderers")]
        public Renderer[] targetRenderers = new Renderer[0];
        [Header("Keyed Vector Property, leave key string empty to not take effect")]
        public string vectorPropertyKey;
        [Header("Keyed Vector Property #2, leave key string empty to not take effect")]
        public string vectorPropertyKey2;

        private void Awake()
        {
            this.SetPropertyIDs();
            this._block = new MaterialPropertyBlock();
        }

        private void SetPropertyIDs()
        {
            if (!string.IsNullOrEmpty(this.vectorPropertyKey))
            {
                this._vectorPropertyID = Shader.PropertyToID(this.vectorPropertyKey);
            }
            if (!string.IsNullOrEmpty(this.vectorPropertyKey2))
            {
                this._vectorPropertyID2 = Shader.PropertyToID(this.vectorPropertyKey2);
            }
            if (!string.IsNullOrEmpty(this.floatPropertyKey))
            {
                this._floatPropertyID = Shader.PropertyToID(this.floatPropertyKey);
            }
            if (!string.IsNullOrEmpty(this.floatPropertyKey2))
            {
                this._floatPropertyID2 = Shader.PropertyToID(this.floatPropertyKey2);
            }
            if (!string.IsNullOrEmpty(this.floatPropertyKey3))
            {
                this._floatPropertyID3 = Shader.PropertyToID(this.floatPropertyKey3);
            }
            if (!string.IsNullOrEmpty(this.colorPropertyKey))
            {
                this._colorPropertyID = Shader.PropertyToID(this.colorPropertyKey);
            }
        }

        private void SyncTargetRenderers()
        {
            for (int i = 0; i < this.targetRenderers.Length; i++)
            {
                Renderer renderer = this.targetRenderers[i];
                renderer.GetPropertyBlock(this._block);
                if (this._vectorPropertyID != 0)
                {
                    this._block.SetVector(this._vectorPropertyID, this.keyedVector);
                }
                if (this._vectorPropertyID2 != 0)
                {
                    this._block.SetVector(this._vectorPropertyID2, this.keyedVector2);
                }
                if (this._floatPropertyID != 0)
                {
                    this._block.SetFloat(this._floatPropertyID, this.keyedFloat);
                }
                if (this._floatPropertyID2 != 0)
                {
                    this._block.SetFloat(this._floatPropertyID2, this.keyedFloat2);
                }
                if (this._floatPropertyID3 != 0)
                {
                    this._block.SetFloat(this._floatPropertyID3, this.keyedFloat3);
                }
                if (this._colorPropertyID != 0)
                {
                    this._block.SetColor(this._colorPropertyID, this.keyedColor);
                }
                renderer.SetPropertyBlock(this._block);
            }
        }

        private void Update()
        {
            this.SyncTargetRenderers();
        }
    }
}

