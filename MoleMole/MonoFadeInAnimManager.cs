namespace MoleMole
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public class MonoFadeInAnimManager : MonoBehaviour
    {
        private Dictionary<string, AnimationItem> _animMap;
        public AnimationItem[] animationList = new AnimationItem[0];

        public void Awake()
        {
            this._animMap = new Dictionary<string, AnimationItem>();
            foreach (AnimationItem item in this.animationList)
            {
                this._animMap.Add(item.name, item);
            }
            this.InitAllFadeInItem();
        }

        private bool CheckNeedWaitNextStep(StepItem step)
        {
            foreach (FadeInItem item in step.fadeInList)
            {
                if (item.gameObject.activeSelf)
                {
                    return true;
                }
            }
            return false;
        }

        private void HideAllObjectInAnim(AnimationItem anim)
        {
            foreach (StepItem item in anim.stepList)
            {
                foreach (FadeInItem item2 in item.fadeInList)
                {
                    if (item2.gameObject.activeSelf)
                    {
                        CanvasGroup component = item2.gameObject.GetComponent<CanvasGroup>();
                        if (item2.type == FadeInItem.Type.FadeOut)
                        {
                            component.alpha = 1f;
                        }
                        else
                        {
                            component.alpha = 0f;
                        }
                    }
                }
            }
        }

        private void InitAllFadeInItem()
        {
            foreach (AnimationItem item in this.animationList)
            {
                foreach (StepItem item2 in item.stepList)
                {
                    foreach (FadeInItem item3 in item2.fadeInList)
                    {
                        item3.anchoredPosition = item3.gameObject.GetComponent<RectTransform>().anchoredPosition;
                    }
                }
            }
        }

        public bool IsAnimationPlaying(string name)
        {
            AnimationItem item = this._animMap[name];
            return item.isPlaying;
        }

        public void Play(string name, bool isReverseOrder = false, Action endCallBack = null)
        {
            AnimationItem anim = this._animMap[name];
            anim.isReverseOrder = isReverseOrder;
            anim.AnimationAllEndCallBack = endCallBack;
            if (base.gameObject.activeInHierarchy)
            {
                base.StartCoroutine(this.PlayAnim(anim));
            }
        }

        [DebuggerHidden]
        private IEnumerator PlayAnim(AnimationItem anim)
        {
            return new <PlayAnim>c__Iterator74 { anim = anim, <$>anim = anim, <>f__this = this };
        }

        [DebuggerHidden]
        private IEnumerator PlayStep(AnimationItem anim, StepItem step)
        {
            return new <PlayStep>c__Iterator75 { anim = anim, step = step, <$>anim = anim, <$>step = step };
        }

        [CompilerGenerated]
        private sealed class <PlayAnim>c__Iterator74 : IEnumerator, IDisposable, IEnumerator<object>
        {
            internal object $current;
            internal int $PC;
            internal MonoFadeInAnimManager.AnimationItem <$>anim;
            internal MonoFadeInAnimManager <>f__this;
            internal int <i>__0;
            internal int <nextIndex>__1;
            internal MonoFadeInAnimManager.AnimationItem anim;

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
                        this.anim.isPlaying = true;
                        if (!this.anim.isReverseOrder)
                        {
                            this.<>f__this.HideAllObjectInAnim(this.anim);
                        }
                        this.<i>__0 = 0;
                        while (this.<i>__0 < this.anim.stepList.Length)
                        {
                            if (this.<>f__this.gameObject.activeInHierarchy)
                            {
                                this.<>f__this.StartCoroutine(this.<>f__this.PlayStep(this.anim, this.anim.stepList[!this.anim.isReverseOrder ? this.<i>__0 : ((this.anim.stepList.Length - this.<i>__0) - 1)]));
                            }
                            this.<nextIndex>__1 = this.<i>__0 + 1;
                            if ((this.<nextIndex>__1 < this.anim.stepList.Length) && this.<>f__this.CheckNeedWaitNextStep(this.anim.stepList[!this.anim.isReverseOrder ? this.<nextIndex>__1 : ((this.anim.stepList.Length - this.<nextIndex>__1) - 1)]))
                            {
                                this.$current = new WaitForSeconds(this.anim.STEP_INTERVAL);
                                this.$PC = 1;
                                return true;
                            }
                        Label_015E:
                            this.<i>__0++;
                        }
                        this.anim.isPlaying = false;
                        this.anim.isReverseOrder = false;
                        if (this.anim.AnimationAllEndCallBack != null)
                        {
                            this.anim.AnimationAllEndCallBack();
                            this.anim.AnimationAllEndCallBack = null;
                        }
                        this.$PC = -1;
                        break;

                    case 1:
                        goto Label_015E;
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

        [CompilerGenerated]
        private sealed class <PlayStep>c__Iterator75 : IEnumerator, IDisposable, IEnumerator<object>
        {
            internal object $current;
            internal int $PC;
            internal MonoFadeInAnimManager.AnimationItem <$>anim;
            internal MonoFadeInAnimManager.StepItem <$>step;
            internal MonoFadeInAnimManager.FadeInItem[] <$s_1929>__1;
            internal int <$s_1930>__2;
            internal MonoFadeInAnimManager.FadeInItem[] <$s_1931>__12;
            internal int <$s_1932>__13;
            internal CanvasGroup <canvasGroup>__15;
            internal CanvasGroup <canvasGroup>__4;
            internal float <delta>__7;
            internal MonoFadeInAnimManager.FadeInItem <fadeIn>__14;
            internal MonoFadeInAnimManager.FadeInItem <fadeIn>__3;
            internal float <position_normalized>__9;
            internal float <progress_normalized>__8;
            internal float <ratio>__5;
            internal float <ratio>__6;
            internal float <timer>__0;
            internal float <x>__10;
            internal float <y>__11;
            internal MonoFadeInAnimManager.AnimationItem anim;
            internal MonoFadeInAnimManager.StepItem step;

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
                        this.<timer>__0 = 0f;
                        break;

                    case 1:
                        break;

                    default:
                        goto Label_0428;
                }
                if (this.<timer>__0 <= this.anim.ALPHA_FADE_DURATION)
                {
                    this.<$s_1929>__1 = this.step.fadeInList;
                    this.<$s_1930>__2 = 0;
                    while (this.<$s_1930>__2 < this.<$s_1929>__1.Length)
                    {
                        this.<fadeIn>__3 = this.<$s_1929>__1[this.<$s_1930>__2];
                        if (this.<fadeIn>__3.gameObject.activeSelf)
                        {
                            this.<canvasGroup>__4 = this.<fadeIn>__3.gameObject.GetComponent<CanvasGroup>();
                            this.<timer>__0 += Time.unscaledDeltaTime;
                            if (this.<fadeIn>__3.type != MonoFadeInAnimManager.FadeInItem.Type.FadeOut)
                            {
                                this.<ratio>__5 = !this.anim.isReverseOrder ? (this.<timer>__0 / this.anim.ALPHA_FADE_DURATION) : ((this.anim.ALPHA_FADE_DURATION - this.<timer>__0) / this.anim.ALPHA_FADE_DURATION);
                                this.<canvasGroup>__4.alpha = Mathf.Lerp(0f, 1f, this.<ratio>__5);
                            }
                            else
                            {
                                this.<ratio>__6 = !this.anim.isReverseOrder ? ((this.anim.ALPHA_FADE_DURATION - this.<timer>__0) / this.anim.ALPHA_FADE_DURATION) : (this.<timer>__0 / this.anim.ALPHA_FADE_DURATION);
                                this.<canvasGroup>__4.alpha = Mathf.Lerp(0f, 1f, this.<ratio>__6);
                            }
                            if ((this.<fadeIn>__3.type == MonoFadeInAnimManager.FadeInItem.Type.WithMoveLeft) || (this.<fadeIn>__3.type == MonoFadeInAnimManager.FadeInItem.Type.WithMoveRight))
                            {
                                this.<delta>__7 = (this.<fadeIn>__3.type != MonoFadeInAnimManager.FadeInItem.Type.WithMoveLeft) ? -this.anim.MOVE_DISTANCE : this.anim.MOVE_DISTANCE;
                                this.<progress_normalized>__8 = !this.anim.isReverseOrder ? (this.<timer>__0 / this.anim.MOVE_FADE_DURATION) : ((this.anim.ALPHA_FADE_DURATION - this.<timer>__0) / this.anim.ALPHA_FADE_DURATION);
                                this.<position_normalized>__9 = 0f;
                                if (this.<fadeIn>__3.curve == MonoFadeInAnimManager.FadeInItem.Curve.Quadratic)
                                {
                                    this.<position_normalized>__9 = (2f * this.<progress_normalized>__8) - (this.<progress_normalized>__8 * this.<progress_normalized>__8);
                                }
                                else
                                {
                                    this.<position_normalized>__9 = this.<progress_normalized>__8;
                                }
                                this.<x>__10 = Mathf.Lerp(this.<fadeIn>__3.anchoredPosition.x + this.<delta>__7, this.<fadeIn>__3.anchoredPosition.x, this.<position_normalized>__9);
                                this.<y>__11 = this.<fadeIn>__3.anchoredPosition.y;
                                this.<fadeIn>__3.gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(this.<x>__10, this.<y>__11);
                            }
                        }
                        this.<$s_1930>__2++;
                    }
                    this.$current = null;
                    this.$PC = 1;
                    return true;
                }
                this.<$s_1931>__12 = this.step.fadeInList;
                this.<$s_1932>__13 = 0;
                while (this.<$s_1932>__13 < this.<$s_1931>__12.Length)
                {
                    this.<fadeIn>__14 = this.<$s_1931>__12[this.<$s_1932>__13];
                    if (this.<fadeIn>__14.gameObject.activeSelf)
                    {
                        this.<canvasGroup>__15 = this.<fadeIn>__14.gameObject.GetComponent<CanvasGroup>();
                        this.<canvasGroup>__15.alpha = (this.<fadeIn>__14.type != MonoFadeInAnimManager.FadeInItem.Type.FadeOut) ? ((float) 1) : ((float) 0);
                        if ((this.<fadeIn>__14.type == MonoFadeInAnimManager.FadeInItem.Type.WithMoveLeft) || (this.<fadeIn>__14.type == MonoFadeInAnimManager.FadeInItem.Type.WithMoveRight))
                        {
                            this.<fadeIn>__14.gameObject.GetComponent<RectTransform>().anchoredPosition = this.<fadeIn>__14.anchoredPosition;
                        }
                    }
                    this.<$s_1932>__13++;
                }
                this.$PC = -1;
            Label_0428:
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

        [Serializable]
        public class AnimationItem
        {
            public float ALPHA_FADE_DURATION = 0.2f;
            public Action AnimationAllEndCallBack;
            [HideInInspector]
            public bool isPlaying;
            public bool isReverseOrder;
            public float MOVE_DISTANCE = 50f;
            public float MOVE_FADE_DURATION = 0.2f;
            public string name;
            public float STEP_INTERVAL = 0.05f;
            public MonoFadeInAnimManager.StepItem[] stepList;
        }

        [Serializable]
        public class FadeInItem
        {
            [HideInInspector]
            public Vector2 anchoredPosition;
            public Curve curve;
            public GameObject gameObject;
            public Type type;

            public enum Curve
            {
                Linear,
                Quadratic
            }

            public enum Type
            {
                Normal,
                FadeOut,
                WithMoveLeft,
                WithMoveRight
            }
        }

        [Serializable]
        public class StepItem
        {
            public MonoFadeInAnimManager.FadeInItem[] fadeInList;
        }
    }
}

