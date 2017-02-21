namespace MoleMole
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public class MonoEffectPluginFade : BaseMonoEffectPlugin
    {
        private List<IAlphaFader> _faders;
        private State _state;
        private float _timer;
        public bool FadeEmissionColor;
        public float FadeHoldTime;
        public float FadeInTime;
        public bool FadeMainAlpha;
        public bool FadeMainColor;
        public bool FadeOpaqueness;
        public bool FadeOutlineAlpha;
        public float FadeOutTime;
        [Header("Select which properties to fade.")]
        public bool FadeTintColor = true;
        [Header("Tick this to hold forever and only uses FadeIn/FadeOut")]
        public bool HoldForever;

        protected override void Awake()
        {
            base.Awake();
            this.SetupFaders();
            this._timer = 0f;
        }

        public override bool IsToBeRemove()
        {
            return ((this._state == State.FadingOut) && (this._timer > this.FadeOutTime));
        }

        private void LerpAllFaders(float t)
        {
            for (int i = 0; i < this._faders.Count; i++)
            {
                this._faders[i].LerpAlpha(t);
            }
        }

        private void OnDestroy()
        {
            Renderer[] componentsInChildren = base.GetComponentsInChildren<Renderer>();
            for (int i = 0; i < componentsInChildren.Length; i++)
            {
                GraphicsUtils.TryCleanRendererInstancedMaterial(componentsInChildren[i]);
            }
        }

        public override void SetDestroy()
        {
            this._state = State.FadingOut;
            this._timer = 0f;
        }

        public override void Setup()
        {
            if (this.FadeInTime > 0f)
            {
                this.LerpAllFaders(0f);
                this._state = State.FadingIn;
            }
            else
            {
                this.LerpAllFaders(1f);
                this._state = State.FadingHold;
            }
            this._timer = 0f;
        }

        private void SetupFaders()
        {
            Renderer[] componentsInChildren = base.GetComponentsInChildren<Renderer>();
            this._faders = new List<IAlphaFader>();
            for (int i = 0; i < componentsInChildren.Length; i++)
            {
                Renderer renderer = componentsInChildren[i];
                if (renderer is ParticleSystemRenderer)
                {
                    GraphicsUtils.CreateAndAssignInstancedMaterial(renderer, renderer.sharedMaterial);
                    if (this.FadeTintColor && renderer.sharedMaterial.HasProperty("_TintColor"))
                    {
                        this._faders.Add(new ColorSharedMaterialFader(renderer, "_TintColor"));
                    }
                    if (this.FadeEmissionColor && renderer.sharedMaterial.HasProperty("_EmissionColor"))
                    {
                        this._faders.Add(new ColorSharedMaterialFader(renderer, "_EmissionColor"));
                    }
                    if (this.FadeMainAlpha && renderer.sharedMaterial.HasProperty("_MainAlpha"))
                    {
                        this._faders.Add(new FloatSharedMaterialFader(renderer, "_MainAlpha"));
                    }
                    if (this.FadeOutlineAlpha && renderer.sharedMaterial.HasProperty("_OutlineAlpha"))
                    {
                        this._faders.Add(new FloatSharedMaterialFader(renderer, "_OutlineAlpha"));
                    }
                    if (this.FadeMainColor && renderer.sharedMaterial.HasProperty("_Color"))
                    {
                        this._faders.Add(new ColorSharedMaterialFader(renderer, "_Color"));
                    }
                    if (this.FadeOpaqueness && renderer.sharedMaterial.HasProperty("_Opaqueness"))
                    {
                        this._faders.Add(new FloatSharedMaterialFader(renderer, "_Opaqueness"));
                    }
                }
                else
                {
                    if (this.FadeTintColor && renderer.sharedMaterial.HasProperty("_TintColor"))
                    {
                        this._faders.Add(new ColorRendererFader(renderer, "_TintColor"));
                    }
                    if (this.FadeEmissionColor && renderer.sharedMaterial.HasProperty("_EmissionColor"))
                    {
                        this._faders.Add(new ColorRendererFader(renderer, "_EmissionColor"));
                    }
                    if (this.FadeMainAlpha && renderer.sharedMaterial.HasProperty("_MainAlpha"))
                    {
                        this._faders.Add(new FloatRendererFader(renderer, "_MainAlpha"));
                    }
                    if (this.FadeOutlineAlpha && renderer.sharedMaterial.HasProperty("_OutlineAlpha"))
                    {
                        this._faders.Add(new FloatRendererFader(renderer, "_OutlineAlpha"));
                    }
                    if (this.FadeMainColor && renderer.sharedMaterial.HasProperty("_Color"))
                    {
                        this._faders.Add(new ColorRendererFader(renderer, "_Color"));
                    }
                    if (this.FadeOpaqueness && renderer.sharedMaterial.HasProperty("_Opaqueness"))
                    {
                        this._faders.Add(new FloatRendererFader(renderer, "_Opaqueness"));
                    }
                }
            }
        }

        public void Update()
        {
            if (this._state == State.FadingIn)
            {
                this._timer += Time.deltaTime * base._effect.TimeScale;
                this.LerpAllFaders(Mathf.Clamp01(this._timer / this.FadeInTime));
                if (this._timer > this.FadeInTime)
                {
                    this._timer = 0f;
                    this._state = State.FadingHold;
                    this.LerpAllFaders(1f);
                }
            }
            else if (this._state == State.FadingHold)
            {
                this._timer += Time.deltaTime * base._effect.TimeScale;
                if (!this.HoldForever && (this._timer > this.FadeHoldTime))
                {
                    this._timer = 0f;
                    this._state = State.FadingOut;
                }
            }
            else if (this._state == State.FadingOut)
            {
                this._timer += Time.deltaTime * base._effect.TimeScale;
                this.LerpAllFaders(Mathf.Clamp01(1f - (this._timer / this.FadeOutTime)));
            }
        }

        private enum State
        {
            FadingIn,
            FadingHold,
            FadingOut
        }
    }
}

