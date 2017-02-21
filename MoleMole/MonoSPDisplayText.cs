namespace MoleMole
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.UI;

    public class MonoSPDisplayText : MonoBehaviour
    {
        private float _emitScalerGrowSpeed = 0.2f;
        private bool _isSPBrightAnimationPlaying;
        private Material _spMaterial;
        private const float MAX_SP_BAR_EMIT_SCALER = 1.5f;
        private const float MIN_SP_BAR_EMIT_SCALER = 1f;

        private float GetSPBarEmissionScaler()
        {
            return this._spMaterial.GetFloat("_EmissionScaler");
        }

        private void PlayDisplayAnimation()
        {
            Animation component = base.transform.Find("DisplayText").GetComponent<Animation>();
            if (component != null)
            {
                if (component.isPlaying)
                {
                    component.Rewind();
                }
                component.Play("DisplaySP", PlayMode.StopAll);
            }
        }

        private void PlaySPAddAnimation()
        {
            this._isSPBrightAnimationPlaying = true;
            this.SetSPBarEmissionScaler(1f);
        }

        private void SetSPBarEmissionScaler(float scaler)
        {
            if (this._spMaterial != null)
            {
                float num = Mathf.Clamp(scaler, 1f, 1.5f);
                this._spMaterial.SetFloat("_EmissionScaler", num);
            }
        }

        public void SetupView(float spBefore, float spAfter, float delta, bool showText = false)
        {
            Text component = base.transform.Find("DisplayText/Text").GetComponent<Text>();
            this._spMaterial = base.transform.Find("Bar/MaskSlider/Slider/Fill").GetComponent<ImageForSmoothMask>().material;
            this._isSPBrightAnimationPlaying = false;
            int num = UIUtil.FloorToIntCustom(delta);
            if (delta > 0f)
            {
                component.text = string.Format("+{0}", num);
                if (showText)
                {
                    base.transform.Find("DisplayText").gameObject.SetActive(true);
                    this.PlayDisplayAnimation();
                }
                else
                {
                    base.transform.Find("DisplayText").gameObject.SetActive(false);
                }
                this.PlaySPAddAnimation();
            }
            if (delta < 0f)
            {
                component.text = string.Format("{0}", num);
            }
        }

        public void Update()
        {
            if (this._isSPBrightAnimationPlaying)
            {
                float sPBarEmissionScaler = this.GetSPBarEmissionScaler();
                if (sPBarEmissionScaler < 1.5f)
                {
                    float scaler = Mathf.Clamp((float) (sPBarEmissionScaler + this._emitScalerGrowSpeed), (float) 1f, (float) 1.5f);
                    this.SetSPBarEmissionScaler(scaler);
                }
                else
                {
                    this._isSPBrightAnimationPlaying = false;
                    this.SetSPBarEmissionScaler(1f);
                }
            }
        }
    }
}

