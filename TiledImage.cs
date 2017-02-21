using System;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("UI/TiledImage")]
public class TiledImage : RawImage
{
    protected override void OnRectTransformDimensionsChange()
    {
        base.OnRectTransformDimensionsChange();
        Vector2 sizeDelta = base.rectTransform.sizeDelta;
        base.uvRect = new Rect(0f, 0f, (sizeDelta.x / ((float) base.texture.width)) * base.canvas.scaleFactor, (sizeDelta.y / ((float) base.texture.height)) * base.canvas.scaleFactor);
    }
}

