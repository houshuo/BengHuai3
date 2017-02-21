namespace MoleMole.Config
{
    using MoleMole;
    using System;

    [GeneratePartialHash(CombineGeneratedFile=true)]
    public class AvatarDodgeTeleportMixin : DodgeTeleportMixin, IHashable
    {
        public bool DodgeMeleeAttack;
        public float MeleeDistance = 2f;

        public override BaseAbilityMixin CreateInstancedMixin(ActorAbility instancedAbility, ActorModifier instancedModifier)
        {
            return new AbilityAvatarDodgeTeleportMixin(instancedAbility, instancedModifier, this);
        }

        public void ObjectContentHashOnto(ref int lastHash)
        {
            HashUtils.ContentHashOnto(this.DodgeMeleeAttack, ref lastHash);
            HashUtils.ContentHashOnto(this.MeleeDistance, ref lastHash);
            if (base.TeleportSkillIDs != null)
            {
                foreach (string str in base.TeleportSkillIDs)
                {
                    HashUtils.ContentHashOnto(str, ref lastHash);
                }
            }
            HashUtils.ContentHashOnto(base.CanHitTrigger, ref lastHash);
            HashUtils.ContentHashOnto(base.Distance, ref lastHash);
            HashUtils.ContentHashOnto(base.TeleportTime, ref lastHash);
            HashUtils.ContentHashOnto((int) base.DirectionMode, ref lastHash);
            if (base.Angle != null)
            {
                HashUtils.ContentHashOnto(base.Angle.isDynamic, ref lastHash);
                HashUtils.ContentHashOnto(base.Angle.fixedValue, ref lastHash);
                HashUtils.ContentHashOnto(base.Angle.dynamicKey, ref lastHash);
            }
            HashUtils.ContentHashOnto(base.SpawnPoint, ref lastHash);
            if (base.CDTime != null)
            {
                HashUtils.ContentHashOnto(base.CDTime.isDynamic, ref lastHash);
                HashUtils.ContentHashOnto(base.CDTime.fixedValue, ref lastHash);
                HashUtils.ContentHashOnto(base.CDTime.dynamicKey, ref lastHash);
            }
            HashUtils.ContentHashOnto(base.NeedFade, ref lastHash);
            if (base.TeleportActions != null)
            {
                foreach (ConfigAbilityAction action in base.TeleportActions)
                {
                    if (action is IHashable)
                    {
                        HashUtils.ContentHashOnto((IHashable) action, ref lastHash);
                    }
                }
            }
            if (base.Predicates != null)
            {
                foreach (ConfigAbilityPredicate predicate in base.Predicates)
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

