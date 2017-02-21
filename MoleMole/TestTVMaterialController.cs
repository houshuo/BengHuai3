namespace MoleMole
{
    using System;
    using UnityEngine;
    using UnityEngine.UI;

    [RequireComponent(typeof(RawImage))]
    public class TestTVMaterialController : MonoBehaviour
    {
        private Material _material;
        private RawImage _rawImage;
        public float bloomFactor;
        public float colorScatterStrength;
        public float distortionAmplitude;
        public float distortionAnimationSpeed;
        public float distortionFrequency;
        public bool enable;
        public float noiseStrength;

        private void Start()
        {
            this._rawImage = base.GetComponent<RawImage>();
            this._material = this._rawImage.material;
            this.enable = false;
            this.distortionFrequency = this._material.GetFloat("_DistortionFrequency");
            this.distortionAmplitude = this._material.GetFloat("_DistortionAmplitude");
            this.distortionAnimationSpeed = this._material.GetFloat("_DistortionAnmSpeed");
            this.colorScatterStrength = this._material.GetFloat("_ColorScatterStrength");
            this.noiseStrength = this._material.GetFloat("_NoiseStrength");
            this.bloomFactor = this._material.GetFloat("_BloomFactor");
        }

        private void Update()
        {
            this.distortionAmplitude = Mathf.Clamp(this.distortionAmplitude, 0f, 1f);
            this.colorScatterStrength = Mathf.Clamp(this.colorScatterStrength, -1f, 1f);
            if ((this._material != null) && this.enable)
            {
                this._material.SetFloat("_DistortionFrequency", this.distortionFrequency);
                this._material.SetFloat("_DistortionAmplitude", this.distortionAmplitude);
                this._material.SetFloat("_DistortionAnmSpeed", this.distortionAnimationSpeed);
                this._material.SetFloat("_ColorScatterStrength", this.colorScatterStrength);
                this._material.SetFloat("_NoiseStrength", this.noiseStrength);
                this._material.SetFloat("_BloomFactor", this.bloomFactor);
            }
        }
    }
}

