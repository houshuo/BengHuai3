namespace MoleMole
{
    using MoleMole.Config;
    using System;

    public class AbilityAttachModifierToSkillIDMixin : BaseAbilityMixin
    {
        private AttachModifierToSkillIDMixin config;

        public AbilityAttachModifierToSkillIDMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config) : base(instancedAbility, instancedModifier, config)
        {
            this.config = (AttachModifierToSkillIDMixin) config;
        }

        private void AddModifier()
        {
            if (base.actor.abilityPlugin.EvaluateAbilityPredicate(this.config.Predicates, base.instancedAbility, base.instancedModifier, base.actor, null))
            {
                base.actor.abilityPlugin.ApplyModifier(base.instancedAbility, this.config.ModifierName);
            }
        }

        public override void OnAdded()
        {
            base.entity.onCurrentSkillIDChanged = (Action<string, string>) Delegate.Combine(base.entity.onCurrentSkillIDChanged, new Action<string, string>(this.SkillIDChangedCallback));
            if (Miscs.ArrayContains<string>(this.config.SkillIDs, base.entity.CurrentSkillID))
            {
                if (!this.config.Inverse)
                {
                    this.AddModifier();
                }
            }
            else if (this.config.Inverse)
            {
                this.AddModifier();
            }
        }

        public override void OnRemoved()
        {
            base.entity.onCurrentSkillIDChanged = (Action<string, string>) Delegate.Remove(base.entity.onCurrentSkillIDChanged, new Action<string, string>(this.SkillIDChangedCallback));
            base.actor.abilityPlugin.TryRemoveModifier(base.instancedAbility, this.config.ModifierName);
        }

        private void RemoveModifier()
        {
            base.actor.abilityPlugin.TryRemoveModifier(base.instancedAbility, this.config.ModifierName);
        }

        private void SkillIDChangedCallback(string from, string to)
        {
            bool flag = false;
            bool flag2 = false;
            for (int i = 0; i < this.config.SkillIDs.Length; i++)
            {
                if (this.config.SkillIDs[i] == from)
                {
                    flag = true;
                }
                if (this.config.SkillIDs[i] == to)
                {
                    flag2 = true;
                }
            }
            if (!flag && flag2)
            {
                if (!this.config.Inverse)
                {
                    this.AddModifier();
                }
                else
                {
                    this.RemoveModifier();
                }
            }
            else if (flag && !flag2)
            {
                if (!this.config.Inverse)
                {
                    this.RemoveModifier();
                }
                else
                {
                    this.AddModifier();
                }
            }
        }
    }
}

