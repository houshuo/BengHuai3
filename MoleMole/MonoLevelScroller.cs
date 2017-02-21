namespace MoleMole
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;

    [RequireComponent(typeof(ScrollRect))]
    public class MonoLevelScroller : MonoBehaviour, IEventSystemHandler, IDragHandler, IEndDragHandler, IBeginDragHandler
    {
        protected Dictionary<Transform, int> _childIndexDict;
        protected int _childNum;
        protected Transform _contentTrans;
        protected float _dragDelta;
        protected bool _finishInit;
        protected GridLayoutGroup _grid;
        protected Dictionary<int, Transform> _indexToChildDict;
        protected Action _onLerpEndCallBack;
        private int _originalCenterIndex;
        protected ScrollRect _scroller;
        protected float _selectedOffset;
        public MonoLevelScroller boundScroller;
        public int centerIndex;
        public bool driveByOutside;
        public float initSwipeSpeedRatio = 1f;
        public float lerpSpeed = 5f;
        private const string LEVEL_LINES_ANI_STR = "LevelLinesFadeIn";
        public MonoChapterScroller.Movement moveType = MonoChapterScroller.Movement.Vertical;
        public Action onLerpEndCallBack;
        public MonoChapterScroller.State state;
        public float stopLerpThreshold = 0.001f;
        public float stopSwipeSpeedThreshold = 0.1f;

        private void Awake()
        {
            this._scroller = base.GetComponent<ScrollRect>();
            this._scroller.onValueChanged.AddListener(new UnityAction<Vector2>(this.OnValueChanged));
        }

        protected float CaculateNormalizedPositionDelta(float absDelat)
        {
            float num = absDelat / this._contentTrans.GetComponent<RectTransform>().rect.width;
            if (this.moveType == MonoChapterScroller.Movement.Vertical)
            {
                num = absDelat / this._contentTrans.GetComponent<RectTransform>().rect.height;
            }
            return num;
        }

        protected float CalcualteCenterDistance(int index)
        {
            float num2 = this.CalculateCenterNormalizedPos(index);
            return Mathf.Abs((float) (this._scroller.verticalNormalizedPosition - num2));
        }

        protected float CalculateCenterNormalizedPos(int index)
        {
            return (1f - (((float) index) / (this._childNum - 1f)));
        }

        public void ClickToChangeCenter(Transform child)
        {
            this.centerIndex = this._childIndexDict[child];
            this.state = MonoChapterScroller.State.ClickLerp;
            if (this.boundScroller != null)
            {
                this.boundScroller.driveByOutside = true;
                this.boundScroller.state = MonoChapterScroller.State.ClickLerp;
                this.boundScroller.centerIndex = this.centerIndex;
                this.driveByOutside = false;
            }
        }

        public Transform GetCenterTransform()
        {
            return this._indexToChildDict[this.centerIndex];
        }

        public void InitLevelPanels(int centIndex, int childNum, Action lerpEndCallBack = null, bool lerpAfterInit = true)
        {
            this._contentTrans = base.transform.Find("Content");
            this._grid = this._contentTrans.GetComponent<GridLayoutGroup>();
            this._childNum = childNum;
            this.driveByOutside = false;
            this.centerIndex = centIndex;
            this._finishInit = true;
            this._onLerpEndCallBack = lerpEndCallBack;
            this._dragDelta = 0f;
            this.Setup();
            if (!lerpAfterInit)
            {
                this._scroller.verticalNormalizedPosition = 1f - (((float) centIndex) / ((float) (this._childNum - 1)));
                this.OnEndLerp();
            }
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            this.state = MonoChapterScroller.State.Drag;
            this.driveByOutside = false;
            if (this.boundScroller != null)
            {
                this.boundScroller.driveByOutside = true;
                this.boundScroller.state = MonoChapterScroller.State.Drag;
            }
            this._originalCenterIndex = this.centerIndex;
            this._dragDelta = 0f;
            if (this.moveType == MonoChapterScroller.Movement.Horizontal)
            {
                this._dragDelta += eventData.delta.x;
            }
            else
            {
                this._dragDelta += eventData.delta.y;
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (this.moveType == MonoChapterScroller.Movement.Horizontal)
            {
                this._dragDelta += eventData.delta.x;
            }
            else
            {
                this._dragDelta += eventData.delta.y;
            }
        }

        public virtual void OnEndDrag(PointerEventData eventData)
        {
            if (this.moveType == MonoChapterScroller.Movement.Horizontal)
            {
                this._dragDelta += eventData.delta.x;
            }
            else
            {
                this._dragDelta += eventData.delta.y;
            }
            int num = this._originalCenterIndex;
            if (this.moveType == MonoChapterScroller.Movement.Horizontal)
            {
                if (this._dragDelta > 0f)
                {
                    num--;
                }
                else
                {
                    num++;
                }
            }
            if (this.moveType == MonoChapterScroller.Movement.Vertical)
            {
                if (this._dragDelta > 0f)
                {
                    num++;
                }
                else
                {
                    num--;
                }
            }
            num = Mathf.Clamp(num, 0, this._childNum - 1);
            this.ClickToChangeCenter(this._indexToChildDict[num]);
        }

        public virtual void OnEndLerp()
        {
            this.state = MonoChapterScroller.State.Idle;
            if (this.boundScroller != null)
            {
                this.boundScroller.driveByOutside = false;
            }
            if (this._onLerpEndCallBack != null)
            {
                this._onLerpEndCallBack();
            }
            this._scroller.velocity = Vector2.zero;
        }

        public virtual void OnValueChanged(Vector2 normalizedPos)
        {
            float f = (this.moveType != MonoChapterScroller.Movement.Horizontal) ? this._scroller.velocity.y : this._scroller.velocity.x;
            if ((this.state == MonoChapterScroller.State.Swipe) && (Mathf.Abs(f) < this.stopSwipeSpeedThreshold))
            {
                this.state = MonoChapterScroller.State.Lerp;
            }
            for (int i = 0; i < this._childNum; i++)
            {
                float distance = this.CalcualteCenterDistance(i);
                this.SetUpChildView(this._indexToChildDict[i], distance);
                if ((distance <= this._selectedOffset) && (this.state != MonoChapterScroller.State.ClickLerp))
                {
                    this.centerIndex = i;
                }
            }
        }

        public void SetNormalizedPosition(Vector2 normalizedPosition)
        {
            this._scroller.normalizedPosition = normalizedPosition;
        }

        public void SetNormalizePositionY(float positionY)
        {
            Vector2 vector = new Vector2(this._scroller.normalizedPosition.x, positionY);
            this._scroller.normalizedPosition = vector;
        }

        public void Setup()
        {
            this._childIndexDict = new Dictionary<Transform, int>();
            this._indexToChildDict = new Dictionary<int, Transform>();
            int num = 0;
            IEnumerator enumerator = this._contentTrans.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    Transform current = (Transform) enumerator.Current;
                    if (current.GetComponent<MonoItemStatus>().isValid)
                    {
                        this._childIndexDict.Add(current, num);
                        this._indexToChildDict.Add(num, current);
                        num++;
                    }
                }
            }
            finally
            {
                IDisposable disposable = enumerator as IDisposable;
                if (disposable == null)
                {
                }
                disposable.Dispose();
            }
            this._selectedOffset = (1f / (this._childNum - 1f)) / 2f;
            this.UpdateContent();
            this.state = MonoChapterScroller.State.Lerp;
        }

        public virtual void SetUpChildView(Transform childTrans, float distance)
        {
        }

        public void ShowItemByIndex(int index)
        {
            if (index < this._childNum)
            {
                this.SetNormalizePositionY(1f - (((float) index) / ((float) (this._childNum - 1))));
            }
        }

        public void Update()
        {
            if (this._finishInit)
            {
                if (this._indexToChildDict.ContainsValue(null))
                {
                    this.Setup();
                }
                float verticalNormalizedPosition = this._scroller.verticalNormalizedPosition;
                if (this.state == MonoChapterScroller.State.Swipe)
                {
                    float f = (this.moveType != MonoChapterScroller.Movement.Horizontal) ? this._scroller.velocity.y : this._scroller.velocity.x;
                    if (Mathf.Abs(f) < this.stopSwipeSpeedThreshold)
                    {
                        this.state = MonoChapterScroller.State.Lerp;
                    }
                }
                if ((this.state == MonoChapterScroller.State.Lerp) || (this.state == MonoChapterScroller.State.ClickLerp))
                {
                    float b = this.CalculateCenterNormalizedPos(this.centerIndex);
                    verticalNormalizedPosition = !this.driveByOutside ? Mathf.Lerp(this._scroller.verticalNormalizedPosition, b, Time.deltaTime * 5f) : this._scroller.verticalNormalizedPosition;
                    this._scroller.verticalNormalizedPosition = verticalNormalizedPosition;
                    if (Mathf.Approximately(b, verticalNormalizedPosition))
                    {
                        this.OnEndLerp();
                    }
                }
                if (((this.boundScroller != null) && this.boundScroller.driveByOutside) && !this.driveByOutside)
                {
                    this.boundScroller.SetNormalizePositionY(this._scroller.verticalNormalizedPosition);
                }
            }
        }

        public virtual void UpdateContent()
        {
        }
    }
}

