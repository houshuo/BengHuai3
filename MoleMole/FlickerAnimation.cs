namespace MoleMole
{
    using System;
    using UnityEngine;

    [RequireComponent(typeof(Renderer))]
    public class FlickerAnimation : MonoBehaviour
    {
        private Material _material;
        private float _time;
        public int divider = 3;
        private float emissionScaler = 1f;
        public AnimationCurve frameOverTime = AnimationCurve.Linear(0f, 0f, 1f, 1f);
        public bool isFlicker;
        public bool isLoop = true;
        public int materialId;
        public string name = "_EmissionScaler";
        public float playbackSpeed = 1f;
        public float scaler = 1f;
        private int tick;

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
            this.emissionScaler = this._material.GetFloat(this.name);
            if (this.isLoop)
            {
                this.frameOverTime.preWrapMode = WrapMode.Loop;
                this.frameOverTime.postWrapMode = WrapMode.Loop;
            }
        }

        private void Start()
        {
            this.Preparation();
        }

        public void Update()
        {
            float emissionScaler;
            if (this.isFlicker)
            {
                if ((this.tick % this.divider) == 1)
                {
                    emissionScaler = this.emissionScaler * this.scaler;
                }
                else
                {
                    emissionScaler = this.emissionScaler;
                }
                this._material.SetFloat(this.name, emissionScaler);
                this.tick++;
            }
            else
            {
                this._time += Time.deltaTime * this.playbackSpeed;
                this._time += 0.01666667f * this.playbackSpeed;
                if (!this.isLoop && (this._time > 1f))
                {
                    this._time = 1f;
                }
                emissionScaler = this.emissionScaler + (this.frameOverTime.Evaluate(this._time) * this.scaler);
                this._material.SetFloat(this.name, emissionScaler);
            }
        }
    }
}

