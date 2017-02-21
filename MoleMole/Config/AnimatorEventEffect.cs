namespace MoleMole.Config
{
    using MoleMole;
    using System;

    public class AnimatorEventEffect : AnimatorEvent
    {
        public string EffectPatternName;

        public override void HandleAnimatorEvent(BaseMonoAnimatorEntity entity)
        {
            entity.TriggerEffectPattern(this.EffectPatternName);
        }
    }
}

