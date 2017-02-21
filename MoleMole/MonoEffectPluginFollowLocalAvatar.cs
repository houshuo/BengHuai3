namespace MoleMole
{
    using System;
    using UnityEngine;

    public class MonoEffectPluginFollowLocalAvatar : BaseMonoEffectPlugin
    {
        [Header("Follow rotation")]
        public bool FollowRotation;

        private void FollowPosition()
        {
            BaseMonoAvatar avatar = Singleton<AvatarManager>.Instance.TryGetLocalAvatar();
            if (avatar != null)
            {
                Transform transform = avatar.transform;
                base.transform.position = transform.position + Vector3.Scale(base.transform.TransformDirection(base._effect.OffsetVec3), base._effect.transform.localScale);
                if (this.FollowRotation)
                {
                    base.transform.rotation = transform.rotation;
                }
            }
        }

        public override bool IsToBeRemove()
        {
            return false;
        }

        public void LateUpdate()
        {
            if (!this.IsToBeRemove())
            {
                this.FollowPosition();
            }
        }

        public override void SetDestroy()
        {
        }
    }
}

