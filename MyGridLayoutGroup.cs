using System;
using UnityEngine;
using UnityEngine.UI;

public class MyGridLayoutGroup : GridLayoutGroup
{
    public override void CalculateLayoutInputHorizontal()
    {
        base.rectChildren.Clear();
        for (int i = 0; i < base.rectTransform.childCount; i++)
        {
            RectTransform child = base.rectTransform.GetChild(i) as RectTransform;
            base.rectChildren.Add(child);
        }
        this.m_Tracker.Clear();
        int num2 = 0;
        int num3 = 0;
        if (base.m_Constraint == GridLayoutGroup.Constraint.FixedColumnCount)
        {
            num2 = num3 = base.m_ConstraintCount;
        }
        else if (base.m_Constraint == GridLayoutGroup.Constraint.FixedRowCount)
        {
            num2 = num3 = Mathf.CeilToInt((((float) base.rectChildren.Count) / ((float) base.m_ConstraintCount)) - 0.001f);
        }
        else
        {
            num2 = 1;
            num3 = Mathf.CeilToInt(Mathf.Sqrt((float) base.rectChildren.Count));
        }
        base.SetLayoutInputForAxis((base.padding.horizontal + ((base.cellSize.x + base.spacing.x) * num2)) - base.spacing.x, (base.padding.horizontal + ((base.cellSize.x + base.spacing.x) * num3)) - base.spacing.x, -1f, 0);
    }
}

