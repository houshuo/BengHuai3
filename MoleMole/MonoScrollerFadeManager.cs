namespace MoleMole
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using UnityEngine;
    using UnityEngine.UI;

    public class MonoScrollerFadeManager : MonoBehaviour
    {
        private List<RectTransform> _itemList;
        private ScrollRect _scroller;
        [SerializeField]
        private float FadeInDuration = 0.5f;
        [SerializeField]
        private float FirstDelay = 0.1f;
        [SerializeField]
        private float NextItemFadeInDelay = 0.3f;

        private void BeginFadeIn()
        {
            this._scroller.enabled = false;
            foreach (RectTransform transform in this._itemList)
            {
                transform.GetComponent<CanvasGroup>().alpha = 0f;
            }
        }

        private void EndFadeIn()
        {
            if (this._scroller == null)
            {
                this._scroller = base.GetComponent<ScrollRect>();
            }
            this._scroller.enabled = true;
        }

        public void Init(Dictionary<int, RectTransform> itemDict, Dictionary<int, RectTransform> oldItemList, Func<RectTransform, RectTransform, bool> isEqual)
        {
            this._scroller = base.GetComponent<ScrollRect>();
            SortedDictionary<int, RectTransform> dictionary = new SortedDictionary<int, RectTransform>();
            foreach (KeyValuePair<int, RectTransform> pair in itemDict)
            {
                dictionary.Add(pair.Key, pair.Value);
            }
            if (oldItemList == null)
            {
                this._itemList = Enumerable.ToList<RectTransform>(dictionary.Values);
            }
            else
            {
                dictionary.Clear();
                foreach (KeyValuePair<int, RectTransform> pair2 in itemDict)
                {
                    bool flag = true;
                    if (oldItemList.ContainsValue(pair2.Value))
                    {
                        flag = !oldItemList.ContainsKey(pair2.Key) ? true : !isEqual(pair2.Value, oldItemList[pair2.Key]);
                    }
                    if (flag)
                    {
                        dictionary.Add(pair2.Key, pair2.Value);
                    }
                }
                this._itemList = Enumerable.ToList<RectTransform>(dictionary.Values);
            }
        }

        public void Play()
        {
            base.StartCoroutine(this.PlayAll());
        }

        [DebuggerHidden]
        private IEnumerator PlayAll()
        {
            return new <PlayAll>c__Iterator76 { <>f__this = this };
        }

        [DebuggerHidden]
        private IEnumerator PlayStep(List<RectTransform> tranList, int num, int lastItemIndex)
        {
            return new <PlayStep>c__Iterator77 { num = num, tranList = tranList, lastItemIndex = lastItemIndex, <$>num = num, <$>tranList = tranList, <$>lastItemIndex = lastItemIndex, <>f__this = this };
        }

        public void Reset()
        {
            if ((this._scroller != null) && !this._scroller.enabled)
            {
                foreach (RectTransform transform in this._itemList)
                {
                    transform.GetComponent<CanvasGroup>().alpha = 1f;
                }
                this.EndFadeIn();
            }
        }

        [CompilerGenerated]
        private sealed class <PlayAll>c__Iterator76 : IEnumerator, IDisposable, IEnumerator<object>
        {
            internal object $current;
            internal int $PC;
            internal MonoScrollerFadeManager <>f__this;
            internal int <itemIndex>__1;
            internal int <num>__0;
            internal float <prePositionY>__3;
            internal List<RectTransform> <transList>__2;

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
                        this.<>f__this.BeginFadeIn();
                        this.$current = new WaitForSeconds(this.<>f__this.FirstDelay);
                        this.$PC = 1;
                        return true;

                    case 1:
                        this.<num>__0 = 0;
                        this.<itemIndex>__1 = 0;
                        goto Label_0201;

                    default:
                        goto Label_022E;
                }
            Label_01CA:
                this.<>f__this.StartCoroutine(this.<>f__this.PlayStep(this.<transList>__2, this.<num>__0, this.<itemIndex>__1));
                this.<num>__0++;
            Label_0201:
                if (this.<itemIndex>__1 < this.<>f__this._itemList.Count)
                {
                    this.<transList>__2 = new List<RectTransform>();
                    while ((this.<itemIndex>__1 < this.<>f__this._itemList.Count) && (this.<>f__this._itemList[this.<itemIndex>__1] == null))
                    {
                        this.<itemIndex>__1++;
                    }
                    if (this.<itemIndex>__1 >= this.<>f__this._itemList.Count)
                    {
                        this.<>f__this.EndFadeIn();
                        goto Label_0201;
                    }
                    this.<prePositionY>__3 = this.<>f__this._itemList[this.<itemIndex>__1].localPosition.y;
                    while (this.<itemIndex>__1 < this.<>f__this._itemList.Count)
                    {
                        if (this.<>f__this._itemList[this.<itemIndex>__1] == null)
                        {
                            this.<itemIndex>__1++;
                            break;
                        }
                        if (this.<>f__this._itemList[this.<itemIndex>__1].localPosition.y != this.<prePositionY>__3)
                        {
                            break;
                        }
                        this.<transList>__2.Add(this.<>f__this._itemList[this.<itemIndex>__1]);
                        this.<itemIndex>__1++;
                    }
                    goto Label_01CA;
                }
                this.<>f__this.EndFadeIn();
                this.$PC = -1;
            Label_022E:
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
        private sealed class <PlayStep>c__Iterator77 : IEnumerator, IDisposable, IEnumerator<object>
        {
            internal object $current;
            internal int $PC;
            internal int <$>lastItemIndex;
            internal int <$>num;
            internal List<RectTransform> <$>tranList;
            internal List<RectTransform>.Enumerator <$s_1948>__3;
            internal List<CanvasGroup>.Enumerator <$s_1949>__5;
            internal List<CanvasGroup>.Enumerator <$s_1950>__7;
            internal MonoScrollerFadeManager <>f__this;
            internal CanvasGroup <canvasGroup>__6;
            internal CanvasGroup <canvasGroup>__8;
            internal float <delay>__0;
            internal List<CanvasGroup> <itemCanvasGroupList>__2;
            internal RectTransform <rectTrans>__4;
            internal float <timer>__1;
            internal int lastItemIndex;
            internal int num;
            internal List<RectTransform> tranList;

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
                        this.<delay>__0 = this.num * this.<>f__this.NextItemFadeInDelay;
                        this.$current = new WaitForSeconds(this.<delay>__0);
                        this.$PC = 1;
                        goto Label_02F1;

                    case 1:
                        if ((this.tranList != null) && (this.tranList.Count >= 1))
                        {
                            this.<timer>__1 = 0f;
                            this.<itemCanvasGroupList>__2 = new List<CanvasGroup>();
                            this.<$s_1948>__3 = this.tranList.GetEnumerator();
                            try
                            {
                                while (this.<$s_1948>__3.MoveNext())
                                {
                                    this.<rectTrans>__4 = this.<$s_1948>__3.Current;
                                    if (this.<rectTrans>__4 != null)
                                    {
                                        this.<itemCanvasGroupList>__2.Add(this.<rectTrans>__4.GetComponent<CanvasGroup>());
                                    }
                                }
                            }
                            finally
                            {
                                this.<$s_1948>__3.Dispose();
                            }
                            break;
                        }
                        if (this.lastItemIndex >= (this.<>f__this._itemList.Count - 1))
                        {
                            this.<>f__this.EndFadeIn();
                        }
                        goto Label_02EF;

                    case 2:
                        break;

                    default:
                        goto Label_02EF;
                }
                while (this.<timer>__1 <= this.<>f__this.FadeInDuration)
                {
                    this.<timer>__1 += Time.deltaTime;
                    this.<$s_1949>__5 = this.<itemCanvasGroupList>__2.GetEnumerator();
                    try
                    {
                        while (this.<$s_1949>__5.MoveNext())
                        {
                            this.<canvasGroup>__6 = this.<$s_1949>__5.Current;
                            if (this.<canvasGroup>__6 == null)
                            {
                                if (this.lastItemIndex >= (this.<>f__this._itemList.Count - 1))
                                {
                                    this.<>f__this.EndFadeIn();
                                }
                                goto Label_02EF;
                            }
                            this.<canvasGroup>__6.alpha = Mathf.Lerp(0f, 1f, this.<timer>__1 / this.<>f__this.FadeInDuration);
                        }
                    }
                    finally
                    {
                        this.<$s_1949>__5.Dispose();
                    }
                    this.$current = null;
                    this.$PC = 2;
                    goto Label_02F1;
                }
                this.<$s_1950>__7 = this.<itemCanvasGroupList>__2.GetEnumerator();
                try
                {
                    while (this.<$s_1950>__7.MoveNext())
                    {
                        this.<canvasGroup>__8 = this.<$s_1950>__7.Current;
                        if (this.<canvasGroup>__8 == null)
                        {
                            if (this.lastItemIndex >= (this.<>f__this._itemList.Count - 1))
                            {
                                this.<>f__this.EndFadeIn();
                            }
                            goto Label_02EF;
                        }
                        this.<canvasGroup>__8.alpha = 1f;
                    }
                }
                finally
                {
                    this.<$s_1950>__7.Dispose();
                }
                if (this.lastItemIndex >= (this.<>f__this._itemList.Count - 1))
                {
                    this.<>f__this.EndFadeIn();
                }
                this.$PC = -1;
            Label_02EF:
                return false;
            Label_02F1:
                return true;
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

