namespace MoleMole.Config
{
    using FullInspector;
    using MoleMole;
    using System;

    public class AnimatorEventTriggerEffectPattern : AnimatorEvent
    {
        public string EffectPatternName;
        [InspectorNullable]
        public string Predicate1;
        [InspectorNullable]
        public string Predicate2;

        public override void HandleAnimatorEvent(BaseMonoAnimatorEntity entity)
        {
            entity.TriggerEffectPattern(this.EffectPatternName, this.Predicate1, this.Predicate2);
        }
    }
}

