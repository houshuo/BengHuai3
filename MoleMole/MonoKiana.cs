namespace MoleMole
{
    using System;
    using UnityEngine;

    public class MonoKiana : BaseMonoAvatar
    {
        private bool _isAppearFXing;
        [Header("Normalized time in Born to stop the born shader effect")]
        public float BornFXNormalizedStopTime = 0.5f;
        public Renderer LeftWeapon;
        [Header("When exiting from these skill IDs set weapon visible will be reset.")]
        public string[] ResetWeaponShownSkillIDs;
        public Renderer RightWeapon;

        private void CheckBornAnimatorState(AnimatorStateInfo fromState, AnimatorStateInfo toState)
        {
            if (toState.tagHash == AvatarData.AVATAR_APPEAR_TAG)
            {
                this.SetShaderData(E_ShaderData.AppearKiana, true);
                this._isAppearFXing = true;
            }
            else if ((fromState.tagHash == AvatarData.AVATAR_APPEAR_TAG) && this._isAppearFXing)
            {
                this._isAppearFXing = false;
                this.SetShaderData(E_ShaderData.AppearKiana, false);
                base.onAnimatorStateChanged = (Action<AnimatorStateInfo, AnimatorStateInfo>) Delegate.Remove(base.onAnimatorStateChanged, new Action<AnimatorStateInfo, AnimatorStateInfo>(this.CheckBornAnimatorState));
            }
        }

        protected override void PostInit()
        {
            base.PostInit();
            if (this.ResetWeaponShownSkillIDs.Length > 0)
            {
                base.onCurrentSkillIDChanged = (Action<string, string>) Delegate.Combine(base.onCurrentSkillIDChanged, new Action<string, string>(this.ResetWeaponBySkillID));
            }
            base.onAnimatorStateChanged = (Action<AnimatorStateInfo, AnimatorStateInfo>) Delegate.Combine(base.onAnimatorStateChanged, new Action<AnimatorStateInfo, AnimatorStateInfo>(this.CheckBornAnimatorState));
        }

        private void ResetWeaponBySkillID(string from, string to)
        {
            if (Miscs.ArrayContains<string>(this.ResetWeaponShownSkillIDs, from))
            {
                this.SetWeaponVisible(1);
            }
        }

        private void SetWeaponVisible(int show)
        {
            bool flag = show != 0;
            if (this.LeftWeapon != null)
            {
                this.LeftWeapon.enabled = flag;
            }
            if (this.RightWeapon != null)
            {
                this.RightWeapon.enabled = flag;
            }
        }

        protected override void Update()
        {
            base.Update();
            if (this._isAppearFXing && (this.GetCurrentNormalizedTime() > this.BornFXNormalizedStopTime))
            {
                this._isAppearFXing = false;
                this.SetShaderData(E_ShaderData.AppearKiana, false);
                base.onAnimatorStateChanged = (Action<AnimatorStateInfo, AnimatorStateInfo>) Delegate.Remove(base.onAnimatorStateChanged, new Action<AnimatorStateInfo, AnimatorStateInfo>(this.CheckBornAnimatorState));
            }
        }
    }
}

