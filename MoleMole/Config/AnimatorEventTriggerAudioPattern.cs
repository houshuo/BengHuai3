namespace MoleMole.Config
{
    using MoleMole;
    using System;

    public class AnimatorEventTriggerAudioPattern : AnimatorEvent
    {
        public string AudioPatternName;
        public bool onlyLocalAvatar;

        public override void HandleAnimatorEvent(BaseMonoAnimatorEntity entity)
        {
            bool flag = true;
            if (this.onlyLocalAvatar && (Singleton<AvatarManager>.Instance.GetLocalAvatar().GetRuntimeID() != entity.GetRuntimeID()))
            {
                flag = false;
            }
            if (flag)
            {
                entity.TriggerAudioPattern(this.AudioPatternName);
            }
        }
    }
}

