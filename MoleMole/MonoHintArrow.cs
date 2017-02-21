namespace MoleMole
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public class MonoHintArrow : MonoBehaviour
    {
        [SerializeField]
        private Animation _animation;
        private MaterialPropertyBlock _block;
        private bool _destroyUponFadeOut;
        private Color _emissivOrigineColor = Color.white;
        private AnimationState _fadeInAnimState;
        private AnimationState _fadeOutAnimState;
        private bool _isInTwinkle;
        private bool _isOriginColor = true;
        [SerializeField]
        private Renderer _renderer;
        private Color _twinkleColor = Color.gray;
        private uint _twinkleIntevalFrameCount = 2;
        private uint _twinkleSumUpFrameCount;
        private uint _twinkleTotalFrameCount = 20;
        private const string ARROW_IN_ANIM = "HintArrowIn";
        private const string ARROW_OUT_ANIM = "HintArrowOut";
        [NonSerialized]
        public BaseMonoEntity listenEntity;
        [NonSerialized]
        public uint listenRuntimID;

        public void Init(uint listenRuntimID = 0, BaseMonoEntity listenEntity = null)
        {
            this.listenRuntimID = listenRuntimID;
            this.listenEntity = listenEntity;
            this._block = new MaterialPropertyBlock();
            this._renderer.GetPropertyBlock(this._block);
            this._emissivOrigineColor = this._renderer.sharedMaterial.GetColor(InLevelData.SHADER_TINTCOLOR);
            this._block.SetColor(InLevelData.SHADER_TINTCOLOR, this._emissivOrigineColor);
            this._renderer.SetPropertyBlock(this._block);
            this._fadeInAnimState = this._animation["HintArrowIn"];
            this._fadeOutAnimState = this._animation["HintArrowOut"];
            base.gameObject.SetActive(false);
            this.state = State.Hidden;
        }

        public bool IsToBeRemove()
        {
            return ((this.listenEntity == null) || this.listenEntity.IsToBeRemove());
        }

        private void LateUpdate()
        {
            if (this.state == State.Visible)
            {
                this.LateUpdateTwinkle();
            }
        }

        private void LateUpdateTwinkle()
        {
            if (this._isInTwinkle)
            {
                if ((this._twinkleSumUpFrameCount % this._twinkleIntevalFrameCount) == 0)
                {
                    this._isOriginColor = !this._isOriginColor;
                }
                this._renderer.GetPropertyBlock(this._block);
                this._block.SetColor(InLevelData.SHADER_TINTCOLOR, !this._isOriginColor ? this._twinkleColor : this._emissivOrigineColor);
                this._renderer.SetPropertyBlock(this._block);
                this._twinkleSumUpFrameCount++;
                if (this._twinkleSumUpFrameCount >= this._twinkleTotalFrameCount)
                {
                    this.TwinkleRecoverOrigin();
                }
            }
        }

        public void SetDestroyUponFadeOut()
        {
            this._destroyUponFadeOut = true;
        }

        public void SetVisible(bool visible)
        {
            if ((this.state == State.Visible) && this._isInTwinkle)
            {
                this.TwinkleRecoverOrigin();
            }
            if (visible)
            {
                if (((!this._destroyUponFadeOut && (this.state != State.FadingIn)) && (this.state != State.Visible)) && ((this.state == State.Hidden) || (this.state == State.FadingOut)))
                {
                    base.gameObject.SetActive(true);
                    this._animation.Play("HintArrowIn");
                    this.state = State.FadingIn;
                }
            }
            else if ((this.state != State.FadingOut) && (this.state != State.Hidden))
            {
                this._animation.Play("HintArrowOut");
                this.state = State.FadingOut;
            }
        }

        public void TriggerEffect(EffectType effectType)
        {
            if (this.state == State.FadingIn)
            {
                this._animation.Stop();
            }
            else if ((this.state == State.Hidden) || (this.state == State.FadingOut))
            {
                return;
            }
            if (effectType == EffectType.Twinkle)
            {
                this._twinkleSumUpFrameCount = 0;
                this._isInTwinkle = true;
            }
        }

        private void TwinkleRecoverOrigin()
        {
            this._renderer.GetPropertyBlock(this._block);
            this._block.SetColor(InLevelData.SHADER_TINTCOLOR, this._emissivOrigineColor);
            this._renderer.SetPropertyBlock(this._block);
            this._twinkleSumUpFrameCount = 0;
            this._isInTwinkle = false;
            this._isOriginColor = true;
        }

        private void Update()
        {
            if (this.state == State.FadingIn)
            {
                if (this._animation["HintArrowIn"].normalizedTime > 1f)
                {
                    this._animation.Stop("HintArrowIn");
                    this.state = State.Visible;
                }
            }
            else if (this.state == State.FadingOut)
            {
                if (this._animation["HintArrowOut"].normalizedTime > 1f)
                {
                    if (this._destroyUponFadeOut)
                    {
                        UnityEngine.Object.Destroy(base.gameObject);
                    }
                    else
                    {
                        base.gameObject.SetActive(false);
                    }
                    this.state = State.Hidden;
                }
            }
            else if (this.state == State.Visible)
            {
            }
        }

        public State state { get; private set; }

        public enum EffectType
        {
            Twinkle,
            Count
        }

        public enum State
        {
            Visible,
            Hidden,
            FadingIn,
            FadingOut
        }
    }
}

