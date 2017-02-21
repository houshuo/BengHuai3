using System;
using UnityEngine;
using UnityEngine.UI;

public class CircleButton : Button, ICanvasRaycastFilter
{
    public float radius = 50f;

    public bool IsRaycastLocationValid(Vector2 sp, Camera eventCamera)
    {
        return (Vector2.Distance(sp, base.transform.position) < this.radius);
    }
}

