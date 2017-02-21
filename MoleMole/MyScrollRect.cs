namespace MoleMole
{
    using System;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;

    public class MyScrollRect : ScrollRect
    {
        private int _childCount;
        private SlideWithScrollOneByOne _slideWithScroll;
        private float _step;

        private float GetScrollPosition(float position)
        {
            float num;
            this._childCount = base.transform.Find("Content").childCount;
            this._step = 1f / ((float) (this._childCount - 1));
            int num2 = Mathf.FloorToInt(position / this._step);
            if (num2 < 0)
            {
                num2 = 0;
            }
            if (num2 > this._childCount)
            {
                num2 = this._childCount;
            }
            if ((position - (num2 * this._step)) >= (this._step / 2f))
            {
                num = (num2 + 1) * this._step;
            }
            else
            {
                num = num2 * this._step;
            }
            if (num > 1f)
            {
                num = 1f;
            }
            if (num < 0f)
            {
                num = 0f;
            }
            return num;
        }

        public override void OnEndDrag(PointerEventData eventData)
        {
            base.OnEndDrag(eventData);
            this._slideWithScroll.SetTargetPosition(this.GetScrollPosition(base.horizontalNormalizedPosition));
        }

        protected override void Start()
        {
            base.Start();
            this._slideWithScroll = base.transform.GetComponent<SlideWithScrollOneByOne>();
        }
    }
}

