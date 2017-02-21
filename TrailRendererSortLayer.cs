using System;
using UnityEngine;

[RequireComponent(typeof(TrailRenderer))]
public class TrailRendererSortLayer : MonoBehaviour
{
    public int sortingOrder;
    public string sortingOrderName = "Default";

    private void Start()
    {
        TrailRenderer component = base.GetComponent<TrailRenderer>();
        component.sortingLayerName = this.sortingOrderName;
        component.sortingOrder = this.sortingOrder;
    }
}

