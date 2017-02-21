namespace MoleMole
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    [RequireComponent(typeof(Renderer))]
    public class FadeAnimation : MonoBehaviour
    {
        private bool _bFadeIn;
        private bool _bFadeOut;
        private float _elapse;
        private Material _material;
        private Color _normalColor;
        public AnimationCurve fadeInCurve;
        public float fadeInDuration;
        public AnimationCurve fadeOutCurve;
        public float fadeOutDuration;
        public string mainColorName;
        public int materialId;

        private void Init()
        {
            this._material = base.GetComponent<Renderer>().materials[this.materialId];
            this._normalColor = this._material.GetColor(this.mainColorName);
            Color color = this._normalColor;
            color.a = 0f;
            this._material.SetColor(this.mainColorName, color);
            this._bFadeIn = true;
            this._elapse = 0f;
        }

        [DebuggerHidden]
        private IEnumerator NotifyEnd(Action action)
        {
            return new <NotifyEnd>c__Iterator2C { action = action, <$>action = action, <>f__this = this };
        }

        private void OnDestroy()
        {
            if (this._material != null)
            {
                UnityEngine.Object.DestroyImmediate(this._material);
            }
        }

        private void Start()
        {
            this.Init();
        }

        public void StartFadeOut(Action action)
        {
            this._bFadeOut = true;
            this._elapse = 0f;
            base.StartCoroutine(this.NotifyEnd(action));
        }

        private void Update()
        {
            if (this._bFadeIn)
            {
                this._elapse += Time.deltaTime;
                if (this._elapse > this.fadeInDuration)
                {
                    this._bFadeIn = false;
                    this._material.SetColor(this.mainColorName, this._normalColor);
                }
                else
                {
                    Color color = this._normalColor;
                    color.a = Mathf.Lerp(0f, this._normalColor.a, this.fadeInCurve.Evaluate(this._elapse / this.fadeInDuration));
                    this._material.SetColor(this.mainColorName, color);
                }
            }
            if (this._bFadeOut)
            {
                this._elapse += Time.deltaTime;
                if (this._elapse > this.fadeOutDuration)
                {
                    this._bFadeOut = false;
                    Color color2 = this._normalColor;
                    color2.a = 0f;
                    this._material.SetColor(this.mainColorName, color2);
                }
                else
                {
                    Color color3 = this._normalColor;
                    color3.a = Mathf.Lerp(this._normalColor.a, 0f, this.fadeOutCurve.Evaluate(this._elapse / this.fadeOutDuration));
                    this._material.SetColor(this.mainColorName, color3);
                }
            }
        }

        [CompilerGenerated]
        private sealed class <NotifyEnd>c__Iterator2C : IEnumerator, IDisposable, IEnumerator<object>
        {
            internal object $current;
            internal int $PC;
            internal Action <$>action;
            internal FadeAnimation <>f__this;
            internal Action action;

            [DebuggerHidden]
            public void Dispose()
            {
                this.$PC = -1;
            }

            public bool MoveNext()
            {
                uint num = (uint) this.$PC;
                this.$PC = -1;
                switch (num)
                {
                    case 0:
                        this.$current = new WaitForSeconds(this.<>f__this.fadeOutDuration);
                        this.$PC = 1;
                        return true;

                    case 1:
                        this.action();
                        this.$PC = -1;
                        break;
                }
                return false;
            }

            [DebuggerHidden]
            public void Reset()
            {
                throw new NotSupportedException();
            }

            object IEnumerator<object>.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }

            object IEnumerator.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }
        }
    }
}

