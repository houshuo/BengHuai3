namespace MoleMole
{
    using System;
    using UnityEngine;

    [RequireComponent(typeof(Renderer))]
    public class UVAnimation : MonoBehaviour
    {
        private Material _material;
        private Material _material2;
        private Vector3 _referenceStartPos;
        public float curveScaler = 1f;
        public float curveSpeed = 1f;
        public AnimationCurve frameOverTime = AnimationCurve.Linear(0f, 0f, 1f, 1f);
        public int materialId;
        public int materialId2 = -1;
        public Vector2 referenceDisplacementPerCycle = Vector2.one;
        [Range(0f, 1f)]
        public float referenceStartPhaseX;
        [Range(0f, 1f)]
        public float referenceStartPhaseY;
        public Transform referenceTransform;
        public float scrollX;
        public float scrollY;
        public float speed2Ratio = 1f;
        public string TexName;
        public bool useCurve;

        private void OnDestroy()
        {
            if (this._material != null)
            {
                UnityEngine.Object.DestroyImmediate(this._material);
            }
        }

        private void Preparation()
        {
            this._material = base.GetComponent<Renderer>().materials[this.materialId];
            if (this.materialId2 >= 0)
            {
                this._material2 = base.GetComponent<Renderer>().materials[this.materialId2];
            }
            this.frameOverTime.preWrapMode = WrapMode.Loop;
            this.frameOverTime.postWrapMode = WrapMode.Loop;
            if (this.referenceTransform != null)
            {
                this._referenceStartPos = this.referenceTransform.position;
            }
        }

        private void Start()
        {
            this.Preparation();
        }

        public void Update()
        {
            Vector2 vector;
            if (this.referenceTransform == null)
            {
                if (this.useCurve)
                {
                    vector = (Vector2) ((Vector2.one * this.frameOverTime.Evaluate(Time.time * this.curveSpeed)) * this.curveScaler);
                }
                else
                {
                    vector = (Vector2) (Vector2.one * Time.time);
                }
            }
            else
            {
                Vector2 vector2 = new Vector2();
                Vector3 vector3 = this.referenceTransform.position - this._referenceStartPos;
                vector2.x = (vector3.x / this.referenceDisplacementPerCycle.x) + this.referenceStartPhaseX;
                vector2.y = (vector3.y / this.referenceDisplacementPerCycle.y) + this.referenceStartPhaseY;
                if (this.useCurve)
                {
                    vector = new Vector2();
                    vector = new Vector2(this.frameOverTime.Evaluate(vector2.x * this.curveSpeed) * this.curveScaler, this.frameOverTime.Evaluate(vector2.y * this.curveSpeed) * this.curveScaler);
                }
                else
                {
                    vector = vector2;
                }
            }
            if (string.IsNullOrEmpty(this.TexName))
            {
                this._material.SetTextureOffset("_MainTex", new Vector2((vector.x * this.scrollX) % 1f, (vector.y * this.scrollY) % 1f));
                if (this.materialId2 >= 0)
                {
                    this._material2.SetTextureOffset("_DistortionTex", new Vector2(((vector.x * this.scrollX) * this.speed2Ratio) % 1f, ((vector.y * this.scrollY) * this.speed2Ratio) % 1f));
                }
            }
            else
            {
                this._material.SetTextureOffset(this.TexName, new Vector2((vector.x * this.scrollX) % 1f, (vector.y * this.scrollY) % 1f));
                if (this.materialId2 >= 0)
                {
                    this._material2.SetTextureOffset("_DistortionTex", new Vector2(((vector.x * this.scrollX) * this.speed2Ratio) % 1f, ((vector.y * this.scrollY) * this.speed2Ratio) % 1f));
                }
            }
        }
    }
}

