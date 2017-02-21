using System;
using UnityEngine;

[ExecuteInEditMode]
public class CharacterRender : MonoBehaviour
{
    public float mipMapBias = -1f;
    public Texture[] textures;

    private void OnDisable()
    {
        this.SetMipMapBias(0f);
    }

    private void OnEnable()
    {
        this.SetMipMapBias(this.mipMapBias);
    }

    private void SetMipMapBias(float bias)
    {
        foreach (Texture texture in this.textures)
        {
            texture.mipMapBias = bias;
        }
    }

    private void Start()
    {
        this.SetMipMapBias(this.mipMapBias);
    }
}

