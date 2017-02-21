namespace MoleMole
{
    using System;
    using UnityEngine;

    [ExecuteInEditMode, RequireComponent(typeof(Renderer))]
    public class DiscreteUVScrollAnimation : MonoBehaviour
    {
        private Material _material;
        private float _offset_X;
        private float _offset_Y;
        private float _step_X;
        private float _step_Y;
        [Header("Drag the material here instead of in renderer")]
        public Material material;
        [Header("Original offsets (tile as unit)")]
        public int origOffset_X;
        public int origOffset_Y;
        public float playbackSpeed = 1f;
        [Header("Map the normalized uv square to a block with X*Y tiles in the texture")]
        public int screen_X = 1;
        public int screen_Y = 1;
        [Header("Scroll speed (tile as unit)")]
        public int speed_X;
        public int speed_Y;
        public int tiles_X = 1;
        public int tiles_Y = 1;

        private void Awake()
        {
            this.Preparation();
        }

        private void OnDestroy()
        {
            if (this._material != null)
            {
                UnityEngine.Object.DestroyImmediate(this._material);
            }
        }

        private void OnDisable()
        {
        }

        private void OnEnable()
        {
            this.Preparation();
        }

        private void Preparation()
        {
            if (this.material == null)
            {
                base.enabled = false;
            }
            else
            {
                this._material = new Material(this.material);
                this._material.hideFlags = HideFlags.DontSave;
                base.GetComponent<Renderer>().material = this._material;
                this._step_X = 1f / ((float) this.tiles_X);
                this._step_Y = 1f / ((float) this.tiles_Y);
                this._material.SetTextureScale("_MainTex", new Vector2(this._step_X * this.screen_X, this._step_Y * this.screen_Y));
            }
        }

        public void Update()
        {
            float deltaTime = Time.deltaTime;
            this._offset_X += this.speed_X * deltaTime;
            this._offset_Y += this.speed_Y * deltaTime;
            Vector2 offset = new Vector2 {
                x = ((int) this._offset_X) * this._step_X,
                y = ((int) this._offset_Y) * this._step_Y
            };
            this._material.SetTextureScale("_MainTex", new Vector2(this._step_X * this.screen_X, this._step_Y * this.screen_Y));
            this._material.SetTextureOffset("_MainTex", offset);
        }
    }
}

