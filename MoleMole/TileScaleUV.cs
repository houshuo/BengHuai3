namespace MoleMole
{
    using System;
    using UnityEngine;

    [ExecuteInEditMode]
    public class TileScaleUV : MonoBehaviour
    {
        private Material[] _materials;
        [Header("Target Renderer")]
        public Renderer targetRenderer;

        private void Awake()
        {
            if (Application.isPlaying)
            {
                this._materials = this.targetRenderer.materials;
            }
            else
            {
                this._materials = this.targetRenderer.sharedMaterials;
            }
            this.SyncMaterialTiling();
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

        private void SyncMaterialTiling()
        {
            for (int i = 0; i < this._materials.Length; i++)
            {
                this._materials[i].SetTextureScale("_MainTex", new Vector2(base.transform.localScale.x, 1f));
            }
        }

        private void Update()
        {
            if (Application.isPlaying)
            {
                if (base.transform.hasChanged)
                {
                    this.SyncMaterialTiling();
                }
            }
            else
            {
                this.SyncMaterialTiling();
            }
        }
    }
}

