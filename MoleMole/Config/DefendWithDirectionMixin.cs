namespace MoleMole.Config
{
    using MoleMole;
    using System;

    [GeneratePartialHash(CombineGeneratedFile=true)]
    public class DefendWithDirectionMixin : ConfigAbilityMixin, IHashable
    {
        public bool AlwaysDefend;
        public float BreakDefendAniDamageRatio = 10f;
        public float DefendAngle = 90f;
        public float DefendDamageReduce = 1f;
        public bool DefendElemental = true;
        public ConfigAbilityAction[] DefendFailActions = ConfigAbilityAction.EMPTY;
        public float DefendNormalizedTimeStart;
        public float DefendNormalizedTimeStop = 1f;
        public ConfigAbilityPredicate[] DefendPredicates = ConfigAbilityPredicate.EMPTY;
        public string[] DefendSkillIDs;
        public ConfigAbilityAction[] DefendSuccessActions = ConfigAbilityAction.EMPTY;
        public AttackResult.AnimatorHitEffect DefendSuccessHitEffect;
        public bool ReverseAngle;

        public override BaseAbilityMixin CreateInstancedMixin(ActorAbility instancedAbility, ActorModifier instancedModifier)
        {
            return new AbilityDefendWithDirectionMixin(instancedAbility, instancedModifier, this);
        }

        public override ConfigAbilityAction[][] GetAllSubActions()
        {
            return new ConfigAbilityAction[][] { this.DefendSuccessActions, this.DefendFailActions };
        }

        public void ObjectContentHashOnto(ref int lastHash)
        {
            if (this.DefendSkillIDs != null)
            {
                foreach (string str in this.DefendSkillIDs)
                {
                    HashUtils.ContentHashOnto(str, ref lastHash);
                }
            }
            HashUtils.ContentHashOnto(this.AlwaysDefend, ref lastHash);
            HashUtils.ContentHashOnto(this.DefendNormalizedTimeStart, ref lastHash);
            HashUtils.ContentHashOnto(this.DefendNormalizedTimeStop, ref lastHash);
            HashUtils.ContentHashOnto(this.DefendAngle, ref lastHash);
            HashUtils.ContentHashOnto(this.ReverseAngle, ref lastHash);
            HashUtils.ContentHashOnto(this.BreakDefendAniDamageRatio, ref lastHash);
            HashUtils.ContentHashOnto(this.DefendDamageReduce, ref lastHash);
            HashUtils.ContentHashOnto(this.DefendElemental, ref lastHash);
            HashUtils.ContentHashOnto((int) this.DefendSuccessHitEffect, ref lastHash);
            if (this.DefendSuccessActions != null)
            {
                foreach (ConfigAbilityAction action in this.DefendSuccessActions)
                {
                    if (action is IHashable)
                    {
                        HashUtils.ContentHashOnto((IHashable) action, ref lastHash);
                    }
                }
            }
            if (this.DefendFailActions != null)
            {
                foreach (ConfigAbilityAction action2 in this.DefendFailActions)
                {
                    if (action2 is IHashable)
                    {
                        HashUtils.ContentHashOnto((IHashable) action2, ref lastHash);
                    }
                }
            }
            if (this.DefendPredicates != null)
            {
                foreach (ConfigAbilityPredicate predicate in this.DefendPredicates)
                {
                    if (predicate is IHashable)
                    {
                        HashUtils.ContentHashOnto((IHashable) predicate, ref lastHash);
                    }
                }
            }
        }
    }
}

