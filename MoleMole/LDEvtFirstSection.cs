namespace MoleMole
{
    using System;
    using UnityEngine;

    public class LDEvtFirstSection : BaseLDEvent
    {
        private BaseMonoAvatar _localAvatar;

        public LDEvtFirstSection(string sectionLevelAnim)
        {
            if (!Singleton<LevelManager>.Instance.levelActor.HasPlugin<DevLevelActorPlugin>())
            {
                Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.DestroyLoadingScene, null));
                Singleton<MainUIManager>.Instance.GetInLevelUICanvas().FadeInStageTransitPanel(0.18f, false, null, null);
            }
            if (!string.IsNullOrEmpty(sectionLevelAnim))
            {
                Singleton<LevelDesignManager>.Instance.PlayCameraAnimationOnEnv(sectionLevelAnim, false, false, true, CameraAnimationCullingType.CullAvatars);
            }
            BaseMonoAvatar localAvatar = Singleton<AvatarManager>.Instance.GetLocalAvatar();
            localAvatar.onAnimatorStateChanged = (Action<AnimatorStateInfo, AnimatorStateInfo>) Delegate.Combine(localAvatar.onAnimatorStateChanged, new Action<AnimatorStateInfo, AnimatorStateInfo>(this.WaitAppearAnimCallback));
            this._localAvatar = localAvatar;
        }

        public override void Core()
        {
            if (this._localAvatar.IsAnimatorInTag(AvatarData.AvatarTagGroup.AllowTriggerInput) && (Singleton<LevelManager>.Instance.levelActor.levelState == LevelActor.LevelState.LevelRunning))
            {
                this._localAvatar.onAnimatorStateChanged = (Action<AnimatorStateInfo, AnimatorStateInfo>) Delegate.Remove(this._localAvatar.onAnimatorStateChanged, new Action<AnimatorStateInfo, AnimatorStateInfo>(this.WaitAppearAnimCallback));
                base.Done();
            }
        }

        private void WaitAppearAnimCallback(AnimatorStateInfo fromState, AnimatorStateInfo toState)
        {
            if (fromState.tagHash == AvatarData.AVATAR_APPEAR_TAG)
            {
                BaseMonoAvatar localAvatar = Singleton<AvatarManager>.Instance.GetLocalAvatar();
                localAvatar.onAnimatorStateChanged = (Action<AnimatorStateInfo, AnimatorStateInfo>) Delegate.Remove(localAvatar.onAnimatorStateChanged, new Action<AnimatorStateInfo, AnimatorStateInfo>(this.WaitAppearAnimCallback));
                base.Done();
            }
        }
    }
}

