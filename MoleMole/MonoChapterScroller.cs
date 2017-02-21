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
    public class MonoChapterScroller : MonoBehaviour, IEventSystemHandler, IEndDragHandler, IBeginDragHandler
    {
        private int _centerIndex;
        private Action<int> _changeSelectCallBack;
        private Dictionary<Transform, int> _childIndexMap;
        private int _childNum;
        private List<Transform> _childSort;
        private Dictionary<Transform, float> _diatanceMap;
        private GridLayoutGroup _gridLayout;
        private InitState _initState;
        private ScrollRect _scroller;
        public RectTransform content;
        public float lerpSpeed = 5f;
        public Movement moveType;
        public State state;
        public float stopLerpSpeedThreshold;

        private void Awake()
        {
            this.InitScroller();
        }

        private float CaculateNormalizedPositionDelta(float absDelat)
        {
            float num = absDelat / this.content.rect.width;
            if (this.moveType == Movement.Vertical)
            {
                num = absDelat / this.content.rect.height;
            }
            return num;
        }

        private float CalcualteCenterDistance(Transform child)
        {
            float num2 = this.CalculateCenterNormalizedPos(this._childIndexMap[child]);
            if (this.moveType == Movement.Horizontal)
            {
                return Mathf.Abs((float) (this._scroller.horizontalNormalizedPosition - num2));
            }
            return Mathf.Abs((float) (this._scroller.verticalNormalizedPosition - num2));
        }

        private float CalculateCenterNormalizedPos(int index)
        {
            return Mathf.Clamp((float) (((float) index) / (this._childNum - 1f)), (float) 0f, (float) 1f);
        }

        public void ClickToChangeCenter(Transform child)
        {
            if (this._childIndexMap == null)
            {
                this.InitChildIndexMap();
            }
            this._centerIndex = this._childIndexMap[child];
            this.state = State.ClickLerp;
        }

        private Vector2 IndexToPosition(int index, Vector2 pivot)
        {
            Vector2 vector;
            if (this.moveType == Movement.Horizontal)
            {
                vector = new Vector2(this._gridLayout.padding.left + ((this._gridLayout.cellSize.x + this._gridLayout.spacing.x) * index), 0f);
            }
            else
            {
                vector = new Vector2(0f, -(this._gridLayout.padding.top + ((this._gridLayout.cellSize.y + this._gridLayout.spacing.y) * index)));
            }
            return (vector + new Vector2(pivot.x * this._gridLayout.cellSize.x, (pivot.y - 1f) * this._gridLayout.cellSize.y));
        }

        private void Init()
        {
            this.state = State.Init;
            this.InitGrid();
            this.InitChildren();
            this.InitTransform();
            this.InitCenter();
        }

        public void Init(int initCenterIndex, int childNum, Action<int> changeSelectCallback = null)
        {
            this._childNum = childNum;
            this._centerIndex = initCenterIndex;
            this.state = State.Init;
            this._initState = InitState.EnableGird;
            this._changeSelectCallBack = changeSelectCallback;
        }

        private void InitCenter()
        {
            float num = this.CalculateCenterNormalizedPos(this._centerIndex);
            if (this.moveType == Movement.Horizontal)
            {
                this._scroller.horizontalNormalizedPosition = num;
            }
            else
            {
                this._scroller.verticalNormalizedPosition = num;
            }
            this.UpdateChildren();
        }

        private void InitChildIndexMap()
        {
            if (this._childIndexMap == null)
            {
                this._childIndexMap = new Dictionary<Transform, int>();
            }
            else
            {
                this._childIndexMap.Clear();
            }
            int num = 0;
            IEnumerator enumerator = this.content.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    Transform current = (Transform) enumerator.Current;
                    this._childIndexMap.Add(current, num);
                    num++;
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
        }

        private void InitChildren()
        {
            this._diatanceMap = new Dictionary<Transform, float>();
            this._childSort = new List<Transform>();
            this._childIndexMap = new Dictionary<Transform, int>();
            int num = 0;
            IEnumerator enumerator = this.content.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    Transform current = (Transform) enumerator.Current;
                    this._childIndexMap.Add(current, num);
                    RectTransform component = current.GetComponent<RectTransform>();
                    component.anchoredPosition = this.IndexToPosition(num, component.pivot);
                    component.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, this._gridLayout.cellSize.x);
                    component.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, this._gridLayout.cellSize.y);
                    component.anchorMin = new Vector2(0f, 1f);
                    component.anchorMax = new Vector2(0f, 1f);
                    num++;
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
        }

        private void InitGrid()
        {
            this._gridLayout = this.content.GetComponent<GridLayoutGroup>();
            this._gridLayout.enabled = false;
        }

        private void InitScroller()
        {
            this._scroller = base.GetComponent<ScrollRect>();
            this._scroller.onValueChanged.AddListener(new UnityAction<Vector2>(this.OnValueChanged));
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

        private void InitTransform()
        {
            if (this.moveType == Movement.Horizontal)
            {
                float size = ((this._gridLayout.padding.left + this._gridLayout.padding.right) + (this._gridLayout.cellSize.x * this._childNum)) + (this._gridLayout.spacing.x * (this._childNum - 1));
                this.content.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size);
            }
            else
            {
                float num2 = ((this._gridLayout.padding.top + this._gridLayout.padding.bottom) + (this._gridLayout.cellSize.y * this._childNum)) + (this._gridLayout.spacing.y * (this._childNum - 1));
                this.content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, num2);
            }
        }

        public bool IsCenter(Transform child)
        {
            return (((child != null) && (this._childIndexMap != null)) && (this._childIndexMap[child] == this._centerIndex));
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            this.state = State.Drag;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            this.state = State.Swipe;
        }

        public void OnValueChanged(Vector2 normalizedPos)
        {
            float f = (this.moveType != Movement.Horizontal) ? this._scroller.velocity.y : this._scroller.velocity.x;
            if ((this.state == State.Swipe) && (Mathf.Abs(f) < this.stopLerpSpeedThreshold))
            {
                this.state = State.Lerp;
            }
            this.UpdateChildren();
        }

        private void SetChildView(Transform child, float distance)
        {
            float num = (1f / (this._childNum - 1f)) / 2f;
            child.localScale = (Vector3) (Vector3.one * Mathf.Clamp((float) (1f - ((0.08f / num) * distance)), (float) 0f, (float) 1f));
            if ((distance <= num) && (this.state != State.ClickLerp))
            {
                this._centerIndex = this._childIndexMap[child];
            }
            MonoActivityEntryButton component = child.GetComponent<MonoActivityEntryButton>();
            MonoChapterButton button2 = child.GetComponent<MonoChapterButton>();
            if (component != null)
            {
                bool flag = distance <= num;
                if (((component.selected != flag) && flag) && ((this.state != State.Init) && (this.state != State.Idle)))
                {
                    Singleton<WwiseAudioManager>.Instance.Post("UI_Gen_Obj_Slide", null, null, null);
                }
                component.UpdateView(distance <= num);
            }
            else if (button2 != null)
            {
                bool flag2 = distance <= num;
                if (((button2.selected != flag2) && flag2) && ((this.state != State.Init) && (this.state != State.Idle)))
                {
                    Singleton<WwiseAudioManager>.Instance.Post("UI_Gen_Obj_Slide", null, null, null);
                }
                button2.UpdateView(distance <= num);
            }
        }

        private void Update()
        {
            float horizontalNormalizedPosition;
            float num2;
            switch (this.state)
            {
                case State.Init:
                    switch (this._initState)
                    {
                        case InitState.EnableGird:
                            this._gridLayout = this.content.GetComponent<GridLayoutGroup>();
                            this._gridLayout.enabled = true;
                            this._initState = InitState.Init;
                            return;

                        case InitState.Init:
                            this.Init();
                            this.state = State.Idle;
                            return;
                    }
                    return;

                case State.Idle:
                case State.Drag:
                    return;

                case State.Lerp:
                case State.ClickLerp:
                    float num3;
                    horizontalNormalizedPosition = this._scroller.horizontalNormalizedPosition;
                    num2 = this.CalculateCenterNormalizedPos(this._centerIndex);
                    if (this.moveType != Movement.Horizontal)
                    {
                        num3 = Mathf.Lerp(this._scroller.verticalNormalizedPosition, num2, Time.deltaTime * this.lerpSpeed);
                        this._scroller.verticalNormalizedPosition = num3;
                        horizontalNormalizedPosition = num3;
                        break;
                    }
                    num3 = Mathf.Lerp(this._scroller.horizontalNormalizedPosition, num2, Time.deltaTime * this.lerpSpeed);
                    this._scroller.horizontalNormalizedPosition = num3;
                    horizontalNormalizedPosition = num3;
                    break;

                default:
                    return;
            }
            if (Mathf.Abs((float) (num2 - horizontalNormalizedPosition)) <= 0.001f)
            {
                if (this._changeSelectCallBack != null)
                {
                    this._changeSelectCallBack(this._centerIndex);
                }
                this.state = State.Idle;
            }
        }

        private void UpdateChildren()
        {
            if (this._childNum > 1)
            {
                this._diatanceMap.Clear();
                IEnumerator enumerator = this.content.GetEnumerator();
                try
                {
                    while (enumerator.MoveNext())
                    {
                        Transform current = (Transform) enumerator.Current;
                        if (!this._childIndexMap.ContainsKey(current))
                        {
                            this.InitChildIndexMap();
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
                IEnumerator enumerator2 = this.content.GetEnumerator();
                try
                {
                    while (enumerator2.MoveNext())
                    {
                        Transform key = (Transform) enumerator2.Current;
                        this._diatanceMap.Add(key, this.CalcualteCenterDistance(key));
                        this.SetChildView(key, this._diatanceMap[key]);
                    }
                }
                finally
                {
                    IDisposable disposable2 = enumerator2 as IDisposable;
                    if (disposable2 == null)
                    {
                    }
                    disposable2.Dispose();
                }
                this._childSort.Clear();
                this._childSort.AddRange(this._diatanceMap.Keys);
                this._childSort.Sort((Comparison<Transform>) ((lobj, robj) => Mathf.FloorToInt(this._diatanceMap[lobj] - this._diatanceMap[robj])));
                for (int i = 0; i < this._childSort.Count; i++)
                {
                    int index = (this._childSort.Count - 1) - i;
                    this._childSort[i].SetSiblingIndex(index);
                }
            }
        }

        public enum InitState
        {
            EnableGird,
            Init
        }

        public enum Movement
        {
            Horizontal,
            Vertical
        }

        public enum State
        {
            Init,
            Idle,
            Drag,
            Lerp,
            ClickLerp,
            Swipe
        }
    }
}

