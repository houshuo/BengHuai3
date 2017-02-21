namespace MoleMole
{
    using System;
    using UnityEngine;

    public class MonoMei : BaseMonoAvatar
    {
        private bool _isAppearFXing;
        [Header("Normalized time in Born to stop the born shader effect")]
        public float BornFXNormalizedTimeStop = 0.5f;

        private void CheckBornAnimatorState(AnimatorStateInfo fromState, AnimatorStateInfo toState)
        {
            if (toState.tagHash == AvatarData.AVATAR_APPEAR_TAG)
            {
                this.SetShaderData(E_ShaderData.AppearMei, true);
                this._isAppearFXing = true;
            }
            else if ((fromState.tagHash == AvatarData.AVATAR_APPEAR_TAG) && this._isAppearFXing)
            {
                this._isAppearFXing = false;
                this.SetShaderData(E_ShaderData.AppearMei, false);
                base.onAnimatorStateChanged = (Action<AnimatorStateInfo, AnimatorStateInfo>) Delegate.Remove(base.onAnimatorStateChanged, new Action<AnimatorStateInfo, AnimatorStateInfo>(this.CheckBornAnimatorState));
            }
        }

        protected override void PostInit()
        {
            base.PostInit();
            base.onAnimatorStateChanged = (Action<AnimatorStateInfo, AnimatorStateInfo>) Delegate.Combine(base.onAnimatorStateChanged, new Action<AnimatorStateInfo, AnimatorStateInfo>(this.CheckBornAnimatorState));
        }

        public void SetBodyVisible(int show)
        {
            bool flag = show != 0;
            for (int i = 0; i < base.renderers.Length; i++)
            {
                base.renderers[i].enabled = flag;
            }
        }

        public override void TriggerSkill(int skillNum)
        {
            if (skillNum == 1)
            {
                base.SetLocomotionBool("EvadeBackward", !base.GetActiveControlData().hasSteer, false);
            }
            base.TriggerSkill(skillNum);
        }

        protected override void Update()
        {
            base.Update();
            if (this._isAppearFXing && (this.GetCurrentNormalizedTime() > this.BornFXNormalizedTimeStop))
            {
                this._isAppearFXing = false;
                this.SetShaderData(E_ShaderData.AppearMei, false);
                base.onAnimatorStateChanged = (Action<AnimatorStateInfo, AnimatorStateInfo>) Delegate.Remove(base.onAnimatorStateChanged, new Action<AnimatorStateInfo, AnimatorStateInfo>(this.CheckBornAnimatorState));
            }
        }
    }
}

