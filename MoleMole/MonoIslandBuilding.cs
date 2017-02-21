namespace MoleMole
{
    using System;
    using UnityEngine;

    public class MonoIslandBuilding : MonoBehaviour
    {
        [SerializeField]
        private Vector3 _focusOffset;
        private Vector3 _focusPos;
        [SerializeField]
        private float _foucusPitch;
        [SerializeField]
        private float _highlight_bloom_factor = 1f;
        [SerializeField]
        private Vector3 _landedOffset;
        [SerializeField]
        private float _landedPitch;
        private Vector3 _landedPos;
        private E_AlphaLerpDir _lerpDir;
        private Renderer _mainRenderer;
        private MonoIslandModel _model;
        [SerializeField]
        private float _normal_bloom_factor = 0.3f;
        private float _startTimeLerp;
        private MonoIslandBuildingsUtil _util;
        public float highlight_polygon_offset = -2000f;
        private int index_highLightMat = -1;

        public void AddHighLightMat(Renderer renderer)
        {
            if (renderer == null)
            {
                renderer = this._mainRenderer;
            }
            int num = renderer.materials.Length + 1;
            Material[] materialArray = new Material[num];
            for (int i = 0; i < renderer.materials.Length; i++)
            {
                materialArray[i] = renderer.materials[i];
            }
            materialArray[num - 1] = this._util._highLightMat;
            renderer.materials = materialArray;
            this.index_highLightMat = num - 1;
        }

        private void Awake()
        {
            this._util = base.transform.parent.GetComponent<MonoIslandBuildingsUtil>();
        }

        public float GetFocusPitch()
        {
            return this._foucusPitch;
        }

        public Vector3 GetFocusPos()
        {
            this._focusPos = base.transform.position + this._focusOffset;
            return this._focusPos;
        }

        public Vector3 GetLandedOffset()
        {
            return this._landedOffset;
        }

        public float GetLandedPitch()
        {
            return this._landedPitch;
        }

        public Vector3 GetLandedPos()
        {
            this._landedPos = base.transform.position + this._landedOffset;
            return this._landedPos;
        }

        public MonoIslandModel GetModel()
        {
            return this._model;
        }

        private void RemoveHighLightMat(Renderer renderer)
        {
            int num = renderer.materials.Length - 1;
            Material[] materialArray = new Material[num];
            for (int i = 0; i < (renderer.materials.Length - 1); i++)
            {
                materialArray[i] = renderer.materials[i];
            }
            renderer.materials = materialArray;
            this.index_highLightMat = -1;
        }

        public void SetHighLightAlpha(float t)
        {
            this._mainRenderer.materials[this.index_highLightMat].SetFloat("_Opaqueness", t);
        }

        public void SetHighLightBloomFactor(float t)
        {
            this._mainRenderer.materials[this.index_highLightMat].SetFloat("_BloomFactor", t);
        }

        public void SetPolygonOffset(float offset)
        {
            this._mainRenderer.material.SetFloat("_PolygonOffset", offset);
            Renderer[] rendererArray = this._model.GetRenderer_RenderQueue();
            for (int i = 0; i < rendererArray.Length; i++)
            {
                rendererArray[i].material.SetFloat("_PolygonOffset", offset);
            }
        }

        public void SetRenderQueue(E_IslandRenderQueue queue)
        {
            this._mainRenderer.material.renderQueue = (int) queue;
            Renderer[] rendererArray = this._model.GetRenderer_RenderQueue();
            for (int i = 0; i < rendererArray.Length; i++)
            {
                rendererArray[i].material.renderQueue = (int) queue;
            }
        }

        public void TriggerHighLight(E_AlphaLerpDir dir)
        {
            this._lerpDir = dir;
            this._startTimeLerp = Time.time;
        }

        private void Update()
        {
            this.UpdateHighLightLerp();
        }

        public void UpdateBuildingWhenExtend(string buildingPath)
        {
            Transform trans = base.transform.Find("Building");
            trans.DestroyChildren();
            Transform transform = UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>(buildingPath)).transform;
            transform.SetParent(trans, false);
            this._model = transform.GetComponent<MonoIslandModel>();
            this._mainRenderer = this._model.GetRenderer();
        }

        private void UpdateHighLightLerp()
        {
            if (this._lerpDir != E_AlphaLerpDir.None)
            {
                if ((this._lerpDir == E_AlphaLerpDir.ToLarge) && (this.index_highLightMat < 0))
                {
                    this.AddHighLightMat(this._mainRenderer);
                }
                float num = (Time.time - this._startTimeLerp) / this._util._highLightLerpDuration;
                if (num > 1f)
                {
                    float t = (this._lerpDir != E_AlphaLerpDir.ToLarge) ? 0f : 1f;
                    this.SetHighLightAlpha(t);
                    float num3 = (this._lerpDir != E_AlphaLerpDir.ToLarge) ? this._normal_bloom_factor : this._highlight_bloom_factor;
                    this.SetHighLightBloomFactor(num3);
                    float num4 = (this._lerpDir != E_AlphaLerpDir.ToLarge) ? this._util._ratioOfOpaquenessToPolygonOffsetBack : this._util._ratioOfOpaquenessToPolygonOffsetFront;
                    this.SetPolygonOffset(Mathf.Lerp(0f, this.highlight_polygon_offset, Mathf.Clamp01(t / num4)));
                    if ((this._lerpDir == E_AlphaLerpDir.ToLittle) && (this.index_highLightMat > 0))
                    {
                        this.RemoveHighLightMat(this._mainRenderer);
                    }
                    this._lerpDir = E_AlphaLerpDir.None;
                }
                else
                {
                    float num5 = (this._lerpDir != E_AlphaLerpDir.ToLarge) ? (1f - num) : num;
                    this.SetHighLightAlpha(num5);
                    float num6 = Mathf.Lerp(this._normal_bloom_factor, this._highlight_bloom_factor, (this._lerpDir != E_AlphaLerpDir.ToLarge) ? (1f - num) : num);
                    this.SetHighLightBloomFactor(num6);
                    float num7 = (this._lerpDir != E_AlphaLerpDir.ToLarge) ? this._util._ratioOfOpaquenessToPolygonOffsetBack : this._util._ratioOfOpaquenessToPolygonOffsetFront;
                    this.SetPolygonOffset(Mathf.Lerp(0f, this.highlight_polygon_offset, Mathf.Clamp01(num5 / num7)));
                }
            }
        }
    }
}

