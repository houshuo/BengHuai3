namespace MoleMole
{
    using System;
    using UnityEngine;

    public class MonoEffectPluginTrail : BaseMonoEffectPlugin
    {
        protected int _curFrame;
        public Transform AniAnchorTransform;
        public Vector3[] FramePosList;
        public Transform TrailRendererTransform;

        protected override void Awake()
        {
            base.Awake();
            this._curFrame = 0;
            this.AniAnchorTransform.gameObject.SetActive(false);
        }

        public override bool IsToBeRemove()
        {
            return ((null == this.TrailRendererTransform) || (this._curFrame >= this.FramePosList.Length));
        }

        public override void SetDestroy()
        {
            if (this.TrailRendererTransform != null)
            {
                UnityEngine.Object.Destroy(this.TrailRendererTransform.gameObject);
            }
        }

        public override void Setup()
        {
            this.AniAnchorTransform.gameObject.SetActive(true);
            this._curFrame = 0;
        }

        protected virtual void Update()
        {
            if (this._curFrame < this.FramePosList.Length)
            {
                this.AniAnchorTransform.localPosition = this.FramePosList[this._curFrame];
                this.TrailRendererTransform.localPosition = this.AniAnchorTransform.localPosition;
                this._curFrame++;
            }
        }
    }
}

