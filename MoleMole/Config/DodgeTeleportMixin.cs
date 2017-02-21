namespace MoleMole.Config
{
    using MoleMole;
    using System;

    [GeneratePartialHash(CombineGeneratedFile=true)]
    public class DodgeTeleportMixin : ConfigAbilityMixin, IHashable
    {
        public DynamicFloat Angle = DynamicFloat.ZERO;
        public bool CanHitTrigger;
        public DynamicFloat CDTime = DynamicFloat.ONE;
        public TeleportDirectionMode DirectionMode;
        public float Distance = 1f;
        public bool NeedFade;
        public ConfigAbilityPredicate[] Predicates = ConfigAbilityPredicate.EMPTY;
        public string SpawnPoint = "Center";
        public ConfigAbilityAction[] TeleportActions = ConfigAbilityAction.EMPTY;
        public string[] TeleportSkillIDs;
        public float TeleportTime = 0.1f;

        public override BaseAbilityMixin CreateInstancedMixin(ActorAbility instancedAbility, ActorModifier instancedModifier)
        {
            return new AbilityDodgeTeleportMixin(instancedAbility, instancedModifier, this);
        }

        public void ObjectContentHashOnto(ref int lastHash)
        {
            if (this.TeleportSkillIDs != null)
            {
                foreach (string str in this.TeleportSkillIDs)
                {
                    HashUtils.ContentHashOnto(str, ref lastHash);
                }
            }
            HashUtils.ContentHashOnto(this.CanHitTrigger, ref lastHash);
            HashUtils.ContentHashOnto(this.Distance, ref lastHash);
            HashUtils.ContentHashOnto(this.TeleportTime, ref lastHash);
            HashUtils.ContentHashOnto((int) this.DirectionMode, ref lastHash);
            if (this.Angle != null)
            {
                HashUtils.ContentHashOnto(this.Angle.isDynamic, ref lastHash);
                HashUtils.ContentHashOnto(this.Angle.fixedValue, ref lastHash);
                HashUtils.ContentHashOnto(this.Angle.dynamicKey, ref lastHash);
            }
            HashUtils.ContentHashOnto(this.SpawnPoint, ref lastHash);
            if (this.CDTime != null)
            {
                HashUtils.ContentHashOnto(this.CDTime.isDynamic, ref lastHash);
                HashUtils.ContentHashOnto(this.CDTime.fixedValue, ref lastHash);
                HashUtils.ContentHashOnto(this.CDTime.dynamicKey, ref lastHash);
            }
            HashUtils.ContentHashOnto(this.NeedFade, ref lastHash);
            if (this.TeleportActions != null)
            {
                foreach (ConfigAbilityAction action in this.TeleportActions)
                {
                    if (action is IHashable)
                    {
                        HashUtils.ContentHashOnto((IHashable) action, ref lastHash);
                    }
                }
            }
            if (this.Predicates != null)
            {
                foreach (ConfigAbilityPredicate predicate in this.Predicates)
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

