namespace MoleMole
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.EventSystems;

    public class MonoActScroller : MonoLevelScroller, IEventSystemHandler, IEndDragHandler
    {
        private const string MATERIAL_COLOR_PATH = "Material/ImageMonoColor";
        private const string MATERIAL_GRAY_PATH = "Material/ImageGrayscale";

        public void InitActs(int centIndex, int childNum, Action lerpEndCallBack = null, bool lerpAfterInit = true)
        {
            base._contentTrans = base.transform.Find("Content");
            base._grid = base._contentTrans.GetComponent<GridLayoutGroup>();
            base._childNum = childNum;
            base.driveByOutside = false;
            base.centerIndex = centIndex;
            base._finishInit = true;
            base._onLerpEndCallBack = lerpEndCallBack;
            base._dragDelta = 0f;
            base.Setup();
            if (lerpAfterInit)
            {
                base.state = MonoChapterScroller.State.ClickLerp;
                if (base.boundScroller != null)
                {
                    base.boundScroller.driveByOutside = true;
                    base.boundScroller.state = MonoChapterScroller.State.ClickLerp;
                    base.boundScroller.centerIndex = base.centerIndex;
                    base.driveByOutside = false;
                }
            }
            else
            {
                base._scroller.verticalNormalizedPosition = 1f - (((float) centIndex) / ((float) (base._childNum - 1)));
                this.OnEndLerp();
            }
        }

        public override void OnEndDrag(PointerEventData eventData)
        {
            base.state = MonoChapterScroller.State.Swipe;
            base._scroller.velocity = (Vector2) (base._scroller.velocity * base.initSwipeSpeedRatio);
            if (base.boundScroller != null)
            {
                base.boundScroller.state = MonoChapterScroller.State.Swipe;
            }
        }

        public override void OnEndLerp()
        {
            base.state = MonoChapterScroller.State.Idle;
            if (base.boundScroller != null)
            {
                base.boundScroller.driveByOutside = false;
                base.boundScroller.state = MonoChapterScroller.State.Idle;
                base.boundScroller.SetNormalizePositionY(base._scroller.verticalNormalizedPosition);
                base.boundScroller.OnEndLerp();
            }
            if (base._onLerpEndCallBack != null)
            {
                base._onLerpEndCallBack();
            }
            if (base.onLerpEndCallBack != null)
            {
                base.onLerpEndCallBack();
            }
        }

        public override void SetUpChildView(Transform childTrans, float distance)
        {
            if (childTrans != null)
            {
                bool isSelect = base._childIndexDict[childTrans] == base.centerIndex;
                MonoActButton component = childTrans.GetComponent<MonoActButton>();
                if ((((component != null) && isSelect) && (!component.selected && (base.state != MonoChapterScroller.State.Idle))) && (base.state != MonoChapterScroller.State.Init))
                {
                    Singleton<WwiseAudioManager>.Instance.Post("UI_Gen_Obj_Slide", null, null, null);
                }
                component.SetupStatus(isSelect);
            }
        }

        private void Update()
        {
            if (base._finishInit)
            {
                if (base._indexToChildDict.ContainsValue(null))
                {
                    base.Setup();
                }
                float verticalNormalizedPosition = base._scroller.verticalNormalizedPosition;
                if ((base.state == MonoChapterScroller.State.Lerp) || (base.state == MonoChapterScroller.State.ClickLerp))
                {
                    float b = base.CalculateCenterNormalizedPos(base.centerIndex);
                    verticalNormalizedPosition = !base.driveByOutside ? Mathf.Lerp(base._scroller.verticalNormalizedPosition, b, Time.deltaTime * base.lerpSpeed) : base._scroller.verticalNormalizedPosition;
                    base._scroller.verticalNormalizedPosition = verticalNormalizedPosition;
                    if (Mathf.Abs((float) (b - verticalNormalizedPosition)) < base.stopLerpThreshold)
                    {
                        this.OnEndLerp();
                    }
                }
                if (((base.boundScroller != null) && base.boundScroller.driveByOutside) && !base.driveByOutside)
                {
                    base.boundScroller.SetNormalizePositionY(base._scroller.verticalNormalizedPosition);
                }
            }
        }

        public override void UpdateContent()
        {
            for (int i = 0; i < base._childNum; i++)
            {
                Transform transform = base._indexToChildDict[i];
                if (transform != null)
                {
                    float distance = base.CalcualteCenterDistance(i);
                    this.SetUpChildView(base._indexToChildDict[i], distance);
                }
            }
        }
    }
}

