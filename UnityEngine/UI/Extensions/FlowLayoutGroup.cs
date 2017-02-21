namespace UnityEngine.UI.Extensions
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;

    [AddComponentMenu("Layout/Extensions/Flow Layout Group")]
    public class FlowLayoutGroup : LayoutGroup
    {
        private float _layoutHeight;
        private readonly IList<RectTransform> _rowList = new List<RectTransform>();
        public bool ChildForceExpandHeight;
        public bool ChildForceExpandWidth;
        public bool ExpandHorizontalSpacing;
        public float SpacingX;
        public float SpacingY;

        public override void CalculateLayoutInputHorizontal()
        {
            base.CalculateLayoutInputHorizontal();
            float totalMin = (this.GetGreatestMinimumChildWidth() + base.padding.left) + base.padding.right;
            base.SetLayoutInputForAxis(totalMin, -1f, -1f, 0);
        }

        public override void CalculateLayoutInputVertical()
        {
            this._layoutHeight = this.SetLayout(base.rectTransform.rect.width, 1, true);
        }

        private float CalculateRowVerticalOffset(float groupHeight, float yOffset, float currentRowHeight)
        {
            if (this.IsLowerAlign)
            {
                return ((groupHeight - yOffset) - currentRowHeight);
            }
            if (this.IsMiddleAlign)
            {
                return (((groupHeight * 0.5f) - (this._layoutHeight * 0.5f)) + yOffset);
            }
            return yOffset;
        }

        public float GetGreatestMinimumChildWidth()
        {
            float b = 0f;
            for (int i = 0; i < base.rectChildren.Count; i++)
            {
                b = Mathf.Max(LayoutUtility.GetMinWidth(base.rectChildren[i]), b);
            }
            return b;
        }

        protected void LayoutRow(IList<RectTransform> contents, float rowWidth, float rowHeight, float maxWidth, float xOffset, float yOffset, int axis)
        {
            float pos = xOffset;
            if (!this.ChildForceExpandWidth && this.IsCenterAlign)
            {
                pos += (maxWidth - rowWidth) * 0.5f;
            }
            else if (!this.ChildForceExpandWidth && this.IsRightAlign)
            {
                pos += maxWidth - rowWidth;
            }
            float num2 = 0f;
            float num3 = 0f;
            if (this.ChildForceExpandWidth)
            {
                num2 = (maxWidth - rowWidth) / ((float) this._rowList.Count);
            }
            else if (this.ExpandHorizontalSpacing)
            {
                num3 = (maxWidth - rowWidth) / ((float) (this._rowList.Count - 1));
                if (this._rowList.Count > 1)
                {
                    if (this.IsCenterAlign)
                    {
                        pos -= (num3 * 0.5f) * (this._rowList.Count - 1);
                    }
                    else if (this.IsRightAlign)
                    {
                        pos -= num3 * (this._rowList.Count - 1);
                    }
                }
            }
            for (int i = 0; i < this._rowList.Count; i++)
            {
                int num5 = !this.IsLowerAlign ? i : ((this._rowList.Count - 1) - i);
                RectTransform rect = this._rowList[num5];
                float a = LayoutUtility.GetPreferredSize(rect, 0) + num2;
                float preferredSize = LayoutUtility.GetPreferredSize(rect, 1);
                if (this.ChildForceExpandHeight)
                {
                    preferredSize = rowHeight;
                }
                a = Mathf.Min(a, maxWidth);
                float num8 = yOffset;
                if (this.IsMiddleAlign)
                {
                    num8 += (rowHeight - preferredSize) * 0.5f;
                }
                else if (this.IsLowerAlign)
                {
                    num8 += rowHeight - preferredSize;
                }
                if (this.ExpandHorizontalSpacing && (i > 0))
                {
                    pos += num3;
                }
                if (axis == 0)
                {
                    base.SetChildAlongAxis(rect, 0, pos, a);
                }
                else
                {
                    base.SetChildAlongAxis(rect, 1, num8, preferredSize);
                }
                if (i < (this._rowList.Count - 1))
                {
                    pos += a + this.SpacingX;
                }
            }
        }

        public float SetLayout(float width, int axis, bool layoutInput)
        {
            float height = base.rectTransform.rect.height;
            float b = (base.rectTransform.rect.width - base.padding.left) - base.padding.right;
            float yOffset = !this.IsLowerAlign ? ((float) base.padding.top) : ((float) base.padding.bottom);
            float rowWidth = 0f;
            float currentRowHeight = 0f;
            for (int i = 0; i < base.rectChildren.Count; i++)
            {
                int num7 = !this.IsLowerAlign ? i : ((base.rectChildren.Count - 1) - i);
                RectTransform rect = base.rectChildren[num7];
                float preferredSize = LayoutUtility.GetPreferredSize(rect, 0);
                float num9 = LayoutUtility.GetPreferredSize(rect, 1);
                preferredSize = Mathf.Min(preferredSize, b);
                if ((rowWidth + preferredSize) > b)
                {
                    rowWidth -= this.SpacingX;
                    if (!layoutInput)
                    {
                        float num10 = this.CalculateRowVerticalOffset(height, yOffset, currentRowHeight);
                        this.LayoutRow(this._rowList, rowWidth, currentRowHeight, b, (float) base.padding.left, num10, axis);
                    }
                    this._rowList.Clear();
                    yOffset += currentRowHeight;
                    yOffset += this.SpacingY;
                    currentRowHeight = 0f;
                    rowWidth = 0f;
                }
                rowWidth += preferredSize;
                this._rowList.Add(rect);
                if (num9 > currentRowHeight)
                {
                    currentRowHeight = num9;
                }
                if (i < (base.rectChildren.Count - 1))
                {
                    rowWidth += this.SpacingX;
                }
            }
            if (!layoutInput)
            {
                float num11 = this.CalculateRowVerticalOffset(height, yOffset, currentRowHeight);
                rowWidth -= this.SpacingX;
                this.LayoutRow(this._rowList, rowWidth, currentRowHeight, b - ((this._rowList.Count <= 1) ? 0f : this.SpacingX), (float) base.padding.left, num11, axis);
            }
            this._rowList.Clear();
            yOffset += currentRowHeight;
            yOffset += !this.IsLowerAlign ? ((float) base.padding.bottom) : ((float) base.padding.top);
            if (layoutInput && (axis == 1))
            {
                base.SetLayoutInputForAxis(yOffset, yOffset, -1f, axis);
            }
            return yOffset;
        }

        public override void SetLayoutHorizontal()
        {
            this.SetLayout(base.rectTransform.rect.width, 0, false);
        }

        public override void SetLayoutVertical()
        {
            this.SetLayout(base.rectTransform.rect.width, 1, false);
        }

        protected bool IsCenterAlign
        {
            get
            {
                return (((base.childAlignment == TextAnchor.LowerCenter) || (base.childAlignment == TextAnchor.MiddleCenter)) || (base.childAlignment == TextAnchor.UpperCenter));
            }
        }

        protected bool IsLowerAlign
        {
            get
            {
                return (((base.childAlignment == TextAnchor.LowerLeft) || (base.childAlignment == TextAnchor.LowerRight)) || (base.childAlignment == TextAnchor.LowerCenter));
            }
        }

        protected bool IsMiddleAlign
        {
            get
            {
                return (((base.childAlignment == TextAnchor.MiddleLeft) || (base.childAlignment == TextAnchor.MiddleRight)) || (base.childAlignment == TextAnchor.MiddleCenter));
            }
        }

        protected bool IsRightAlign
        {
            get
            {
                return (((base.childAlignment == TextAnchor.LowerRight) || (base.childAlignment == TextAnchor.MiddleRight)) || (base.childAlignment == TextAnchor.UpperRight));
            }
        }
    }
}

