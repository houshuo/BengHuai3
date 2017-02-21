namespace MoleMole
{
    using MoleMole.Config;
    using System;

    public class AbilityAvatarSkillButtonHoldNormalizedTimeChargeAnimatorMixin : BaseAbilityAvatarSkillButtonHoldChargeMixin
    {
        private int _thersholdIndex;
        private AvatarSkillButtonHoldNormalizedTimeChargeAnimatorMixin config;

        public AbilityAvatarSkillButtonHoldNormalizedTimeChargeAnimatorMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config) : base(instancedAbility, instancedModifier, config)
        {
            this.config = (AvatarSkillButtonHoldNormalizedTimeChargeAnimatorMixin) config;
        }

        public override void OnAdded()
        {
            base.OnAdded();
        }

        protected override void OnBeforeToInLoop()
        {
            this._thersholdIndex = 0;
        }

        protected override void OnInLoopToAfter()
        {
            if (this.config.ChargeEndNormalizeTimeThershold != null)
            {
                int index = base._loopIx;
                if (base._loopIx == base._loopCount)
                {
                    index--;
                }
                base.actor.abilityPlugin.HandleActionTargetDispatch(this.config.ChargeEndNormalizeTimeActions[index][this._thersholdIndex], base.instancedAbility, base.instancedModifier, base.actor, null);
            }
        }

        protected override void OnMoveingToNextLoop(bool endLoop)
        {
        }

        protected override bool ShouldMoveToNextLoop()
        {
            return (base.entity.GetCurrentNormalizedTime() > this.config.ChargeLoopNormalizeTimeEnds[base._loopIx]);
        }

        protected override void UpdateInLoop()
        {
            if (this.config.ChargeEndNormalizeTimeThershold != null)
            {
                float currentNormalizedTime = base.entity.GetCurrentNormalizedTime();
                float[] numArray = this.config.ChargeEndNormalizeTimeThershold[base._loopIx];
                for (int i = this._thersholdIndex; i < numArray.Length; i++)
                {
                    if (currentNormalizedTime < numArray[i])
                    {
                        if (this._thersholdIndex != i)
                        {
                            this._thersholdIndex = i;
                        }
                        break;
                    }
                }
            }
        }
    }
}

