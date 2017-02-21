namespace MoleMole
{
    using System;
    using UnityEngine;

    [ExecuteInEditMode, RequireComponent(typeof(Camera)), AddComponentMenu("Image Effects/PostFX")]
    public class PostFX : PostFXWithResScale
    {
        protected override void OnEnable()
        {
            base.OnEnable();
            base.internalBufferSize = PostFXBase.InternalBufferSizeEnum.SIZE_128;
            base.CameraResScale = PostFXWithResScale.CAMERA_RES_SCALE.RES_100;
        }
    }
}

