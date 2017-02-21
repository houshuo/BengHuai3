using System;
using UnityEngine;

[RequireComponent(typeof(RectTransform), typeof(Collider2D))]
public class RaycastMask : MonoBehaviour, ICanvasRaycastFilter
{
    private Collider2D myCollider;
    private RectTransform rectTransform;

    private void Awake()
    {
        this.myCollider = base.GetComponent<Collider2D>();
        this.rectTransform = base.GetComponent<RectTransform>();
    }

    public bool IsRaycastLocationValid(Vector2 screenPos, Camera eventCamera)
    {
        Vector3 zero = Vector3.zero;
        RectTransformUtility.ScreenPointToWorldPointInRectangle(this.rectTransform, screenPos, eventCamera, out zero);
        return this.myCollider.OverlapPoint(zero);
    }
}

