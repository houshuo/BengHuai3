namespace MoleMole
{
    using System;
    using UnityEngine;

    public class FadePlugin : BaseEntityFuncPlugin
    {
        private bool _active;
        private MaterialFader[] _faders;
        private float _fadeTime;
        private float _from;
        private float _timer;
        private float _to;
        public const int EXECUTION_ORDER = 0x238d;

        public FadePlugin(BaseMonoEntity entity) : base(entity)
        {
        }

        public override void Core()
        {
            if (this._active)
            {
                this._timer += Time.deltaTime * base._entity.TimeScale;
                this.LerpAlpha(Mathf.Lerp(this._from, this._to, this._timer / this._fadeTime));
                if (this._timer > this._fadeTime)
                {
                    this._active = false;
                }
            }
        }

        public override void FixedCore()
        {
        }

        public override bool IsActive()
        {
            return this._active;
        }

        public bool IsLastActiveFadeOut()
        {
            return (this._to < this._from);
        }

        private void LerpAlpha(float t)
        {
            for (int i = 0; i < this._faders.Length; i++)
            {
                this._faders[i].LerpAlpha(t);
            }
        }

        public void StartFade(float from, float to, float time)
        {
            if (!this._active)
            {
                this._from = from;
                this._to = to;
                this._fadeTime = time;
                this._timer = 0f;
                this._active = true;
                Material[] allMaterials = ((IFadeOff) base._entity).GetAllMaterials();
                this._faders = new MaterialFader[allMaterials.Length * 2];
                for (int i = 0; i < allMaterials.Length; i++)
                {
                    if (allMaterials[i].HasProperty("_MainAlpha"))
                    {
                        this._faders[2 * i] = new FloatFader(allMaterials[i], "_MainAlpha");
                        this._faders[2 * i].LerpAlpha(this._from);
                        this._faders[(2 * i) + 1] = new NopFader();
                    }
                    else if (allMaterials[i].HasProperty("_Color"))
                    {
                        this._faders[2 * i] = new ColorFader(allMaterials[i], "_Color");
                        this._faders[2 * i].LerpAlpha(this._from);
                        this._faders[(2 * i) + 1] = new ColorFader(allMaterials[i], "_OutlineColor");
                        this._faders[(2 * i) + 1].LerpAlpha(this._from);
                    }
                    else
                    {
                        this._faders[2 * i] = new NopFader();
                        this._faders[(2 * i) + 1] = new NopFader();
                    }
                }
            }
        }
    }
}

