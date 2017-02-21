namespace MoleMole.Config
{
    using MoleMole;
    using System;

    public class ConfigEntityAttackEffect : IndexedConfig<ConfigEntityAttackEffect>
    {
        public AttackEffectTriggerAt AttackEffectTriggerPos;
        public string EffectPattern;
        public bool MuteAttackEffect;
        public string SwitchName;

        public override int CompareTo(ConfigEntityAttackEffect other)
        {
            if (other == null)
            {
                return 1;
            }
            int num = IndexedConfig.Compare(this.EffectPattern, other.EffectPattern);
            if (num != 0)
            {
                return num;
            }
            num = IndexedConfig.Compare(this.SwitchName, other.SwitchName);
            if (num != 0)
            {
                return num;
            }
            num = this.MuteAttackEffect.CompareTo(other.MuteAttackEffect);
            if (num != 0)
            {
                return num;
            }
            return this.AttackEffectTriggerPos.CompareTo(other.AttackEffectTriggerPos);
        }

        public override int ContentHash()
        {
            int lastHash = 0;
            HashUtils.ContentHashOnto(this.EffectPattern, ref lastHash);
            HashUtils.ContentHashOnto(this.SwitchName, ref lastHash);
            HashUtils.ContentHashOnto(this.MuteAttackEffect, ref lastHash);
            HashUtils.ContentHashOnto((int) this.AttackEffectTriggerPos, ref lastHash);
            return 0;
        }
    }
}

