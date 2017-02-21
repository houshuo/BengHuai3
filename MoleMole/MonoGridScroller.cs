namespace MoleMole
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;

    [RequireComponent(typeof(ScrollRect))]
    public class MonoGridScroller : MonoBehaviour, IEventSystemHandler, IEndDragHandler
    {
        private float _bottom;
        private Vector2 _cellSize = Vector2.zero;
        private int _col;
        private bool _initialized;
        private bool _isFirstRefresh;
        private int _itemCount;
        private OnChange _onChange;
        private Vector2 _padding = Vector2.zero;
        private int _preRefreshIndex;
        private int _row;
        private ScrollRect _scroller;
        private Rect _scrollerRect;
        private HashSet<int> _showIndexSet = new HashSet<int>();
        private Vector2 _spacing = Vector2.zero;
        private int _transCount;
        private Dictionary<int, RectTransform> _transDict = new Dictionary<int, RectTransform>();
        private HashSet<int> _transIndexSet = new HashSet<int>();
        public RectTransform grid;
        public Transform itemPrefab;
        public Movement moveType;
        public string scrollAudioPatternName = "UI_Gen_Obj_Slide";
        public MonoScrollBar scrollBar;
        public MonoScrollBarAutoHide scrollBarAutoHide;

        public void AddChildren(int addCount)
        {
            this._itemCount += addCount;
            int num = this._transCount;
            this._transCount = this.CalculateTransCount();
            if (this._transCount > num)
            {
                for (int i = num; i < this._transCount; i++)
                {
                    this.AddNewChildByIndex(i);
                }
            }
            this.RefreshIndexSet();
            this.InitTransform(null);
        }

        private void AddNewChildByIndex(int index)
        {
            Transform trans = this.grid.transform.AddChildFromPrefab(this.itemPrefab, index.ToString());
            this.InitChild(trans.GetComponent<RectTransform>(), index);
            this._onChange(trans, index);
            this._transIndexSet.Add(index);
            this._transDict.Add(index, trans.GetComponent<RectTransform>());
        }

        private int CalculateTransCount()
        {
            int num = this._col * this._row;
            if (num > this._itemCount)
            {
                num = this._itemCount;
            }
            return num;
        }

        private void ChangeToIndex(int from, int to)
        {
            if (this._transDict.ContainsKey(from))
            {
                RectTransform transform = this._transDict[from];
                transform.anchoredPosition = this.IndexToPosition(to, transform.pivot);
                this._transDict.Remove(from);
                this._transDict.Add(to, transform);
            }
        }

        private void Clear()
        {
            this._transIndexSet.Clear();
            this._transDict.Clear();
            this._showIndexSet.Clear();
            if (this.grid != null)
            {
                this.grid.DestroyChildren();
            }
        }

        private void DoScroll(float delta)
        {
            Rect rect = this._scroller.content.rect;
            if (this.moveType == Movement.Horizontal)
            {
                float num = rect.width - this._scrollerRect.width;
                float num2 = (this._scroller.horizontalNormalizedPosition * num) + delta;
                float num3 = (num != 0f) ? Mathf.Clamp((float) (num2 / num), (float) 0f, (float) 1f) : 0f;
                this._scroller.horizontalNormalizedPosition = num3;
            }
            else
            {
                float num4 = rect.height - this._scrollerRect.height;
                float num5 = (this._scroller.verticalNormalizedPosition * num4) + delta;
                float num6 = (num4 != 0f) ? Mathf.Clamp((float) (num5 / num4), (float) 0f, (float) 1f) : 0f;
                this._scroller.verticalNormalizedPosition = num6;
            }
        }

        public Dictionary<int, RectTransform> GetItemDict()
        {
            return this._transDict;
        }

        public Transform GetItemTransByIndex(int index)
        {
            if (this._transDict.ContainsKey(index))
            {
                return this._transDict[index];
            }
            return null;
        }

        public int GetMaxItemCountWithouScroll()
        {
            return (this._col * this._row);
        }

        public Vector2 GetNormalizedPosition()
        {
            return this._scroller.normalizedPosition;
        }

        private Vector2 IndexToPosition(int index, Vector2 pivot)
        {
            Vector2 vector;
            if (this.moveType == Movement.Horizontal)
            {
                vector = new Vector2(this.ItemSize.x * (index / this._row), -this.ItemSize.y * (index % this._row));
            }
            else
            {
                vector = new Vector2(this.ItemSize.x * (index % this._col), -this.ItemSize.y * (index / this._col));
            }
            return ((vector + this._padding) + new Vector2(pivot.x * this._cellSize.x, (pivot.y - 1f) * this._cellSize.y));
        }

        public void Init(OnChange onChange, int itemCount, Vector2? normalizedPosition = new Vector2?())
        {
            if (this._initialized)
            {
                this._onChange = onChange;
                if (itemCount > this._itemCount)
                {
                    this.AddChildren(itemCount - this._itemCount);
                }
                else if (itemCount < this._itemCount)
                {
                    this.RemoveChildren(this._itemCount - itemCount);
                }
                this.InitTransform(normalizedPosition);
                this.RefreshCurrent();
                this._itemCount = itemCount;
            }
            else
            {
                this._onChange = onChange;
                this._itemCount = itemCount;
                this.Clear();
                this.InitScroller();
                this.InitGrid();
                this.InitChildren();
                this.InitTransform(normalizedPosition);
            }
            this.InitScorllBar();
            this._isFirstRefresh = true;
            this._initialized = true;
            this._preRefreshIndex = 0;
        }

        private void InitChild(RectTransform rectTrans, int index)
        {
            rectTrans.anchorMax = new Vector2(0f, 1f);
            rectTrans.anchorMin = new Vector2(0f, 1f);
            rectTrans.sizeDelta = this._cellSize;
            rectTrans.anchoredPosition = this.IndexToPosition(index, rectTrans.pivot);
        }

        private void InitChildren()
        {
            LayoutElement component = this._scroller.GetComponent<LayoutElement>();
            if (component != null)
            {
                this._col = Mathf.CeilToInt(component.preferredWidth / ((this.ItemSize.x * this.grid.localScale.x) + this._spacing.x));
                this._row = Mathf.CeilToInt(component.preferredHeight / ((this.ItemSize.y * this.grid.localScale.y) + this._spacing.y));
            }
            else
            {
                this._col = Mathf.CeilToInt(this._scrollerRect.width / ((this.ItemSize.x * this.grid.localScale.x) + this._spacing.x));
                this._row = Mathf.CeilToInt(this._scrollerRect.height / ((this.ItemSize.y * this.grid.localScale.y) + this._spacing.y));
            }
            if (this.moveType == Movement.Horizontal)
            {
                this._row = this.grid.GetComponent<GridLayoutGroup>().constraintCount;
                this._col += 2;
            }
            else
            {
                this._col = this.grid.GetComponent<GridLayoutGroup>().constraintCount;
                this._row += 2;
            }
            this._transCount = this.CalculateTransCount();
            for (int i = 0; i < this._transCount; i++)
            {
                this.AddNewChildByIndex(i);
            }
        }

        private void InitGrid()
        {
            GridLayoutGroup component = this.grid.GetComponent<GridLayoutGroup>();
            this._cellSize = component.cellSize;
            this._spacing = component.spacing;
            this._padding.x = component.padding.left;
            this._padding.y = -component.padding.top;
            this._bottom = component.padding.bottom;
            this.grid.GetComponent<GridLayoutGroup>().enabled = false;
        }

        private void InitScorllBar()
        {
            if (this.scrollBar != null)
            {
                LayoutElement component = this._scroller.GetComponent<LayoutElement>();
                int num = 0;
                int num2 = 0;
                if (component != null)
                {
                    num = Mathf.FloorToInt(component.preferredWidth / ((this.ItemSize.x * this.grid.localScale.x) + this._spacing.x));
                    num2 = Mathf.FloorToInt(component.preferredHeight / ((this.ItemSize.y * this.grid.localScale.y) + this._spacing.y));
                }
                else
                {
                    num = Mathf.FloorToInt(this._scrollerRect.width / ((this.ItemSize.x * this.grid.localScale.x) + this._spacing.x));
                    num2 = Mathf.FloorToInt(this._scrollerRect.height / ((this.ItemSize.y * this.grid.localScale.y) + this._spacing.y));
                }
                bool isVisible = false;
                if (this.moveType == Movement.Vertical)
                {
                    if (num2 < (this._itemCount / num))
                    {
                        isVisible = true;
                    }
                }
                else if (num < (this._itemCount / num2))
                {
                    isVisible = true;
                }
                this.scrollBar.SetVisible(isVisible);
            }
        }

        private void InitScroller()
        {
            this._scroller = base.GetComponent<ScrollRect>();
            this._scroller.onValueChanged.AddListener(new UnityAction<Vector2>(this.OnValueChanged));
            this._scrollerRect = this._scroller.GetComponent<RectTransform>().rect;
            if (this.moveType == Movement.Horizontal)
            {
                this._scroller.vertical = false;
                this._scroller.horizontal = true;
            }
            else
            {
                this._scroller.vertical = true;
                this._scroller.horizontal = false;
            }
        }

        private void InitTransform(Vector2? normalizedPosition = new Vector2?())
        {
            if (this.moveType == Movement.Horizontal)
            {
                int num = Mathf.CeilToInt((this._itemCount * 1f) / ((float) this._row));
                this.grid.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, ((this._cellSize.x * num) + (this._spacing.x * (num - 1))) + Mathf.Abs(this._padding.x));
            }
            else
            {
                int num2 = Mathf.CeilToInt((this._itemCount * 1f) / ((float) this._col));
                this.grid.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, (((this._cellSize.y * num2) + (this._spacing.y * (num2 - 1))) + Mathf.Abs(this._padding.y)) + this._bottom);
            }
            if (normalizedPosition.HasValue)
            {
                this._scroller.normalizedPosition = normalizedPosition.Value;
            }
            this.OnValueChanged(this._scroller.normalizedPosition);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (((this._itemCount > this.GetMaxItemCountWithouScroll()) && (this.scrollBarAutoHide != null)) && this.scrollBarAutoHide.hidebyDefault)
            {
                float velocity = (this.moveType != Movement.Horizontal) ? this._scroller.velocity.y : this._scroller.velocity.x;
                this.scrollBarAutoHide.UpdateStatus(velocity);
            }
        }

        public void OnValueChanged(Vector2 normalizedPosition)
        {
            this.ProcessSlideAudio();
            if (this._transCount != this._itemCount)
            {
                this.RefreshIndexSet();
                if (this.scrollBarAutoHide != null)
                {
                    if (this.scrollBarAutoHide.hidebyDefault)
                    {
                        float velocity = (this.moveType != Movement.Horizontal) ? this._scroller.velocity.y : this._scroller.velocity.x;
                        this.scrollBarAutoHide.UpdateStatus(velocity);
                    }
                    else if (this.moveType == Movement.Horizontal)
                    {
                        this.scrollBarAutoHide.UpdateStatus(this._itemCount > (this._col - 2));
                    }
                    else
                    {
                        this.scrollBarAutoHide.UpdateStatus(this._itemCount > (this._row - 2));
                    }
                }
            }
        }

        private void ProcessSlideAudio()
        {
            if (!string.IsNullOrEmpty(this.scrollAudioPatternName))
            {
                int num = 0;
                if (this.moveType == Movement.Horizontal)
                {
                    float num2 = -this.grid.GetComponent<RectTransform>().anchoredPosition.x;
                    num = (int) (num2 / this.ItemSize.x);
                }
                else
                {
                    num = (int) (this.grid.GetComponent<RectTransform>().anchoredPosition.y / this.ItemSize.y);
                }
                if (!this._isFirstRefresh && (this._preRefreshIndex != num))
                {
                    Singleton<WwiseAudioManager>.Instance.Post(this.scrollAudioPatternName, null, null, null);
                }
                this._isFirstRefresh = false;
                this._preRefreshIndex = num;
            }
        }

        public void RefreshCurrent()
        {
            foreach (int num in this._transIndexSet)
            {
                if (this._onChange != null)
                {
                    this._onChange(this._transDict[num], num);
                }
            }
        }

        public void RefreshCurrentByIndex(int index)
        {
            if (this._transIndexSet.Contains(index) && (this._onChange != null))
            {
                this._onChange(this._transDict[index], index);
            }
        }

        private void RefreshIndexSet()
        {
            int startIndex = 0;
            if (this.moveType == Movement.Horizontal)
            {
                float num2 = -this.grid.GetComponent<RectTransform>().anchoredPosition.x;
                int num3 = (int) (num2 / this.ItemSize.x);
                startIndex = num3 * this._row;
            }
            else
            {
                int num5 = (int) (this.grid.GetComponent<RectTransform>().anchoredPosition.y / this.ItemSize.y);
                startIndex = num5 * this._col;
            }
            this.SwapIndex(startIndex);
        }

        private void RemoveChildByIndex(int i)
        {
            if (this._transDict.ContainsKey(i))
            {
                Transform transform = this._transDict[i];
                transform.gameObject.SetActive(false);
                UnityEngine.Object.Destroy(transform.gameObject);
                this._transCount--;
                this._transIndexSet.Remove(i);
                this._transDict.Remove(i);
            }
        }

        public void RemoveChildren(int removeCount)
        {
            this._itemCount -= removeCount;
            int num = this._transCount;
            int num2 = this.CalculateTransCount();
            removeCount = num - num2;
            int num3 = Enumerable.Max(this._transIndexSet);
            for (int i = num3; i > (num3 - removeCount); i--)
            {
                this.RemoveChildByIndex(i);
            }
            this.RefreshIndexSet();
            this.InitTransform(null);
        }

        public void ScrollToBegin()
        {
            this.SetNormalizedPosition(new Vector2(0f, 1f));
        }

        public void ScrollToEnd()
        {
            this.SetNormalizedPosition(new Vector2(1f, 0f));
        }

        public void ScrollToNextItem()
        {
            float delta = (this.moveType != Movement.Horizontal) ? -this._cellSize.y : this._cellSize.x;
            this.DoScroll(delta);
        }

        public void ScrollToNextPage()
        {
            float delta = (this.moveType != Movement.Horizontal) ? -this._scrollerRect.height : this._scrollerRect.width;
            this.DoScroll(delta);
        }

        public void ScrollToPreItem()
        {
            float delta = (this.moveType != Movement.Horizontal) ? this._cellSize.y : -this._cellSize.x;
            this.DoScroll(delta);
        }

        public void ScrollToPrevPage()
        {
            float delta = (this.moveType != Movement.Horizontal) ? this._scrollerRect.height : -this._scrollerRect.width;
            this.DoScroll(delta);
        }

        public void SetNormalizedPosition(Vector2 normalizedPosition)
        {
            this._scroller.normalizedPosition = normalizedPosition;
        }

        private void SwapIndex(int startIndex)
        {
            this._showIndexSet.Clear();
            for (int i = 0; i < this._transCount; i++)
            {
                int item = i + startIndex;
                if (item >= this._itemCount)
                {
                    item = startIndex - ((item - this._itemCount) + 1);
                }
                else if (item < 0)
                {
                    item += this._transCount;
                }
                if (item < 0)
                {
                    item = 0;
                }
                this._showIndexSet.Add(item);
            }
            if (!this._showIndexSet.SetEquals(this._transIndexSet))
            {
                IEnumerator<int> enumerator = Enumerable.Except<int>(this._showIndexSet, this._transIndexSet).GetEnumerator();
                IEnumerator<int> enumerator2 = Enumerable.Except<int>(this._transIndexSet, this._showIndexSet).GetEnumerator();
                while (enumerator.MoveNext() && enumerator2.MoveNext())
                {
                    this.ChangeToIndex(enumerator2.Current, enumerator.Current);
                    this._onChange(this._transDict[enumerator.Current], enumerator.Current);
                }
                HashSet<int> set = this._transIndexSet;
                this._transIndexSet = this._showIndexSet;
                this._showIndexSet = set;
            }
        }

        public Vector2 ItemSize
        {
            get
            {
                return (this._spacing + this._cellSize);
            }
        }

        public enum Movement
        {
            Horizontal,
            Vertical
        }

        public delegate void OnChange(Transform trans, int index);
    }
}

