namespace MoleMole
{
    using System;
    using UnityEngine;

    [RequireComponent(typeof(Renderer))]
    public class MonoMeiHairFadeAnimation : MonoBehaviour
    {
        private FadeParams _currentParams;
        private Shader _fadingShader;
        private FadeParams _fromParams;
        private MaterialPropertyBlock _mpb;
        private Shader _normalShader;
        private FadeParams _originalParams;
        private Renderer _renderer;
        private float _timer;
        private FadeParams _toParams;
        public float fadeDuration;
        private static readonly string FADING_SHADER_PATH = "miHoYo/Character/Avatar UI Hair";
        private static readonly string NORMAL_SHADER_PATH = "miHoYo/Character/Avatar";
        public FadeParams paramsForStigmata;
        public FadeParams paramsForWeapon;

        private void ApplyFade()
        {
            this._renderer.GetPropertyBlock(this._mpb);
            this._mpb.SetFloat("_DirectionalFadeOffset", this._currentParams.offset);
            this._mpb.SetFloat("_DirectionalFadeRange", this._currentParams.range);
            this._mpb.SetFloat("_DirectionalFadeValue", this._currentParams.value);
            this._renderer.SetPropertyBlock(this._mpb);
        }

        public void CancelFade()
        {
            this.FadeToParams(this._originalParams);
        }

        public void FadeForStigmataTab()
        {
            this.FadeToParams(this.paramsForStigmata);
        }

        public void FadeForWeaponTab()
        {
            this.FadeToParams(this.paramsForWeapon);
        }

        private void FadeToParams(FadeParams targetParams)
        {
            this._timer = 0f;
            this._fromParams = this._currentParams;
            this._toParams = targetParams;
        }

        private void Start()
        {
            this._originalParams = new FadeParams();
            this._originalParams.value = 1f;
            this._currentParams = this._originalParams;
            this._fromParams = this._originalParams;
            this._timer = 0f;
            this._renderer = base.GetComponent<Renderer>();
            this._mpb = new MaterialPropertyBlock();
            this._normalShader = Shader.Find(NORMAL_SHADER_PATH);
            this._fadingShader = Shader.Find(FADING_SHADER_PATH);
        }

        private void Update()
        {
            if ((this._timer < this.fadeDuration) && (this._toParams != null))
            {
                this._timer += Time.deltaTime;
                FadeParams @params = FadeParams.Lerp(this._fromParams, this._toParams, Mathf.Clamp01(this._timer / this.fadeDuration));
                bool flag = this._currentParams.value < 0.99;
                bool flag2 = @params.value < 0.99;
                if (flag2 && !flag)
                {
                    for (int i = 0; i < this._renderer.materials.Length; i++)
                    {
                        this._renderer.materials[i].shader = this._fadingShader;
                    }
                }
                else if (!flag2 && flag)
                {
                    for (int j = 0; j < this._renderer.materials.Length; j++)
                    {
                        this._renderer.materials[j].shader = this._normalShader;
                    }
                }
                this._currentParams = @params;
                this.ApplyFade();
            }
        }

        [Serializable]
        public class FadeParams
        {
            public float offset = 1f;
            public float range = 0.01f;
            public float value = 1f;

            public bool IsValidFade()
            {
                return (this.value < 0.9999);
            }

            public static MonoMeiHairFadeAnimation.FadeParams Lerp(MonoMeiHairFadeAnimation.FadeParams params1, MonoMeiHairFadeAnimation.FadeParams params2, float t)
            {
                MonoMeiHairFadeAnimation.FadeParams @params = new MonoMeiHairFadeAnimation.FadeParams();
                if (!params1.IsValidFade())
                {
                    @params.offset = params2.offset;
                    @params.range = params2.range;
                }
                if (!params2.IsValidFade())
                {
                    @params.offset = params1.offset;
                    @params.range = params1.range;
                }
                else
                {
                    @params.offset = Mathf.Lerp(params1.offset, params2.offset, t);
                    @params.range = Mathf.Lerp(params1.range, params2.range, t);
                }
                @params.value = Mathf.Lerp(params1.value, params2.value, t);
                return @params;
            }
        }
    }
}

