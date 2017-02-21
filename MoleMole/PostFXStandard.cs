namespace MoleMole
{
    using System;
    using UnityEngine;

    [RequireComponent(typeof(Camera)), AddComponentMenu("Image Effects/PostFX"), ExecuteInEditMode]
    public class PostFXStandard : PostFXBase
    {
        public LayerMask cullingMask;

        [ImageEffectTransformsToLDR]
        public void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            base.DoPostProcess(source, destination);
        }
    }
}

