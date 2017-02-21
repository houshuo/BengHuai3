namespace MoleMole
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;

    [RequireComponent(typeof(Animation))]
    public class MonoAnimationinSequence : MonoBehaviour
    {
        private Animation _animation;
        private Action<Transform> _endCallBack;
        private bool _needReset;
        private OnAnimationEnd _onAnimationEnd;
        private float _timer = -1f;
        public string animationName;
        public string audioPattern;
        public float audioPatternDelay;

        public void AnimationEnd()
        {
            if (this._endCallBack != null)
            {
                this._endCallBack(base.transform);
            }
            if (this._onAnimationEnd != null)
            {
                this._onAnimationEnd();
            }
        }

        private void Awake()
        {
            this._animation = base.GetComponent<Animation>();
        }

        public void Play()
        {
            if (string.IsNullOrEmpty(this.animationName))
            {
                base.transform.GetComponent<Animation>().Play();
            }
            else
            {
                base.transform.GetComponent<Animation>().PlayQueued(this.animationName);
            }
            if (!string.IsNullOrEmpty(this.audioPattern))
            {
                if (this.audioPatternDelay == 0f)
                {
                    Singleton<WwiseAudioManager>.Instance.Post(this.audioPattern, null, null, null);
                }
                else
                {
                    this._timer = this.audioPatternDelay;
                }
            }
        }

        public void SetAnimationEndCallBack(OnAnimationEnd callBack, Action<Transform> endCallBack = null)
        {
            this._onAnimationEnd = callBack;
            this._endCallBack = endCallBack;
        }

        public void TryResetToAnimationFirstFrame()
        {
            if (!this._needReset)
            {
                this._needReset = true;
            }
            else
            {
                AnimationState state;
                if (!string.IsNullOrEmpty(this.animationName))
                {
                    this._animation.clip = this._animation[this.animationName].clip;
                    state = this._animation[this.animationName];
                }
                else
                {
                    state = this._animation[this._animation.clip.name];
                }
                state.enabled = true;
                state.time = 0f;
                state.weight = 1f;
                this._animation.Sample();
                state.enabled = false;
            }
        }

        private void Update()
        {
            if (this._timer >= 0f)
            {
                this._timer -= Time.deltaTime;
                if (this._timer < 0f)
                {
                    Singleton<WwiseAudioManager>.Instance.Post(this.audioPattern, null, null, null);
                }
            }
        }
    }
}

