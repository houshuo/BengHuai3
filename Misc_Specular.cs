using System;
using UnityEngine;

[ExecuteInEditMode]
public class Misc_Specular : MonoBehaviour
{
    private void Start()
    {
    }

    private void Update()
    {
        base.GetComponent<Renderer>().sharedMaterial.SetVector("centerPos", base.transform.position);
    }
}

