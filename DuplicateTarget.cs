using System;
using UnityEngine;

public class DuplicateTarget : MonoBehaviour
{
    public int count;
    public GameObject target;

    private void Start()
    {
        for (int i = 0; i < this.count; i++)
        {
            UnityEngine.Object.Instantiate<GameObject>(this.target).transform.parent = base.gameObject.transform;
        }
    }
}

