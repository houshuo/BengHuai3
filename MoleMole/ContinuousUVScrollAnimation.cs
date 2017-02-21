namespace MoleMole
{
    using System;
    using UnityEngine;

    [RequireComponent(typeof(Renderer))]
    public class ContinuousUVScrollAnimation : MonoBehaviour
    {
        private Material[] _materials;
        private float _offsetX;
        private float _offsetY;
        private bool _prepared;
        [Header("Drag the material here instead of in renderer")]
        public Material[] materials;
        [Header("Scroll Speed")]
        public float speed_X;
        public float speed_Y;

        private void Awake()
        {
            this.Preparation();
        }

        private void OnDestroy()
        {
            if (this._materials != null)
            {
                for (int i = 0; i < this._materials.Length; i++)
                {
                    UnityEngine.Object.DestroyImmediate(this._materials[i]);
                }
            }
        }

        private void OnValidate()
        {
            this.Preparation();
        }

        private void Preparation()
        {
            if (this.materials.Length == 0)
            {
                this._prepared = false;
            }
            else
            {
                this._materials = new Material[this.materials.Length];
                for (int i = 0; i < this._materials.Length; i++)
                {
                    this._materials[i] = new Material(this.materials[i]);
                    this._materials[i].hideFlags = HideFlags.DontSave;
                }
                base.GetComponent<Renderer>().materials = this._materials;
                this._prepared = true;
            }
        }

        public void Update()
        {
            if (this._prepared)
            {
                float deltaTime = Time.deltaTime;
                this._offsetX += this.speed_X * deltaTime;
                this._offsetY += this.speed_Y * deltaTime;
                for (int i = 0; i < this._materials.Length; i++)
                {
                    this._materials[i].SetTextureOffset("_MainTex", new Vector2(this._offsetX, this._offsetY));
                }
            }
        }
    }
}

