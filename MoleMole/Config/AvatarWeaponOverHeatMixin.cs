namespace MoleMole.Config
{
    using MoleMole;
    using System;

    [GeneratePartialHash(CombineGeneratedFile=true)]
    public class AvatarWeaponOverHeatMixin : ConfigAbilityMixin, IHashable
    {
        public float[] ContinuousHeatAddSpeed;
        public DynamicFloat ContinuousHeatSpeedRatio = DynamicFloat.ONE;
        public string[] ContinuousSkillIDs;
        public ConfigAbilityAction[] CoolDownActions = ConfigAbilityAction.EMPTY;
        public float CoolSpeed;
        public string IgnorePredicate;
        public string[] NoCoolSkillIDs;
        public ConfigAbilityAction[] OverHeatActions = ConfigAbilityAction.EMPTY;
        public string OverHeatButtonSkillID;
        public float OverHeatCoolSpeed;
        public int OverHeatLayer = 2;
        public float OverHeatMax;
        public float[] SkillHeatAdds;
        public string[] SkillIDs;
        public float ToMaxCoolSpeedTime;

        public override BaseAbilityMixin CreateInstancedMixin(ActorAbility instancedAbility, ActorModifier instancedModifier)
        {
            return new AbilityAvatarWeaponOverHeatMixin(instancedAbility, instancedModifier, this);
        }

        public void ObjectContentHashOnto(ref int lastHash)
        {
            HashUtils.ContentHashOnto(this.OverHeatMax, ref lastHash);
            HashUtils.ContentHashOnto(this.CoolSpeed, ref lastHash);
            HashUtils.ContentHashOnto(this.OverHeatLayer, ref lastHash);
            HashUtils.ContentHashOnto(this.OverHeatCoolSpeed, ref lastHash);
            HashUtils.ContentHashOnto(this.ToMaxCoolSpeedTime, ref lastHash);
            HashUtils.ContentHashOnto(this.OverHeatButtonSkillID, ref lastHash);
            if (this.SkillIDs != null)
            {
                foreach (string str in this.SkillIDs)
                {
                    HashUtils.ContentHashOnto(str, ref lastHash);
                }
            }
            if (this.NoCoolSkillIDs != null)
            {
                foreach (string str2 in this.NoCoolSkillIDs)
                {
                    HashUtils.ContentHashOnto(str2, ref lastHash);
                }
            }
            if (this.SkillHeatAdds != null)
            {
                foreach (float num3 in this.SkillHeatAdds)
                {
                    HashUtils.ContentHashOnto(num3, ref lastHash);
                }
            }
            if (this.ContinuousSkillIDs != null)
            {
                foreach (string str3 in this.ContinuousSkillIDs)
                {
                    HashUtils.ContentHashOnto(str3, ref lastHash);
                }
            }
            if (this.ContinuousHeatAddSpeed != null)
            {
                foreach (float num6 in this.ContinuousHeatAddSpeed)
                {
                    HashUtils.ContentHashOnto(num6, ref lastHash);
                }
            }
            if (this.ContinuousHeatSpeedRatio != null)
            {
                HashUtils.ContentHashOnto(this.ContinuousHeatSpeedRatio.isDynamic, ref lastHash);
                HashUtils.ContentHashOnto(this.ContinuousHeatSpeedRatio.fixedValue, ref lastHash);
                HashUtils.ContentHashOnto(this.ContinuousHeatSpeedRatio.dynamicKey, ref lastHash);
            }
            HashUtils.ContentHashOnto(this.IgnorePredicate, ref lastHash);
            if (this.OverHeatActions != null)
            {
                foreach (ConfigAbilityAction action in this.OverHeatActions)
                {
                    if (action is IHashable)
                    {
                        HashUtils.ContentHashOnto((IHashable) action, ref lastHash);
                    }
                }
            }
            if (this.CoolDownActions != null)
            {
                foreach (ConfigAbilityAction action2 in this.CoolDownActions)
                {
                    if (action2 is IHashable)
                    {
                        HashUtils.ContentHashOnto((IHashable) action2, ref lastHash);
                    }
                }
            }
        }
    }
}

