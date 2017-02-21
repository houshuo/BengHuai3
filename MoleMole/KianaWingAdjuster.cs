namespace MoleMole
{
    using System;
    using UnityEngine;

    [RequireComponent(typeof(Renderer))]
    public class KianaWingAdjuster : MonoBehaviour
    {
        private MaterialPropertyBlock _mpb;
        private float _oldEmissionScalerScalerWithoutHDR = 1f;
        private float _oldOpaquenessScalerWithoutHDR = 1f;
        private Renderer _renderer;
        private static readonly string EMISSION_SCALER_SCALER_NAME = "_EmissionScalerScalerWithoutHDR";
        public float emissionScalerScalerWithoutHDR = 0.22f;
        private static readonly string OPAQUENESS_SCALER_NAME = "_OpaquenessScalerWithoutHDR";
        public float opaquenessScalerWithoutHDR = 2f;

        private void OnDestroy()
        {
            GraphicsSettingUtil.onPostFXChanged = (Action<bool>) Delegate.Remove(GraphicsSettingUtil.onPostFXChanged, new Action<bool>(this.SettingHDR));
        }

        private void SettingHDR(bool postFXEnable)
        {
            if (base.gameObject.activeSelf)
            {
                bool supportHDR = false;
                PostFX tfx = UnityEngine.Object.FindObjectOfType<PostFX>();
                if ((tfx != null) && tfx.enabled)
                {
                    supportHDR = tfx.SupportHDR;
                }
                if (!supportHDR)
                {
                    this._mpb.SetFloat(OPAQUENESS_SCALER_NAME, this.opaquenessScalerWithoutHDR);
                    this._mpb.SetFloat(EMISSION_SCALER_SCALER_NAME, this.emissionScalerScalerWithoutHDR);
                }
                else
                {
                    this._mpb.SetFloat(OPAQUENESS_SCALER_NAME, this._oldOpaquenessScalerWithoutHDR);
                    this._mpb.SetFloat(EMISSION_SCALER_SCALER_NAME, this._oldEmissionScalerScalerWithoutHDR);
                }
                this._renderer.SetPropertyBlock(this._mpb);
            }
        }

        private void Start()
        {
            GraphicsSettingUtil.onPostFXChanged = (Action<bool>) Delegate.Combine(GraphicsSettingUtil.onPostFXChanged, new Action<bool>(this.SettingHDR));
            this._renderer = base.GetComponent<Renderer>();
            this._mpb = new MaterialPropertyBlock();
            this._renderer.GetPropertyBlock(this._mpb);
            this.SettingHDR(false);
        }
    }
}

