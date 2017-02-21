using System;
using UnityEngine;

public class MonoDestroyInTime : MonoBehaviour
{
    private float _timer;
    public float time;

    private void Start()
    {
        this._timer = this.time;
    }

    private void Update()
    {
        this._timer -= Time.deltaTime;
        if (this._timer <= 0f)
        {
            UnityEngine.Object.Destroy(base.gameObject);
        }
    }
}

