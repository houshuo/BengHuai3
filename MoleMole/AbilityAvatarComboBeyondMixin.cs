namespace MoleMole
{
    using MoleMole.Config;
    using System;

    public class AbilityAvatarComboBeyondMixin : BaseAbilityMixin
    {
        private AvatarActor _avatarActor;
        private AvatarComboBeyondMixin config;

        public AbilityAvatarComboBeyondMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config) : base(instancedAbility, instancedModifier, config)
        {
            this.config = (AvatarComboBeyondMixin) config;
            this._avatarActor = base.actor as AvatarActor;
        }

        private void ApplyStepModifier(int step)
        {
            if (step != 0)
            {
                int index = step - 1;
                this._avatarActor.abilityPlugin.ApplyModifier(base.instancedAbility, this.config.ModifierNames[index]);
            }
        }

        private int EvaluateComboStep(int combo)
        {
            int num = 0;
            for (int i = 0; i < this.config.ComboSteps.Length; i++)
            {
                if (combo < base.instancedAbility.Evaluate(this.config.ComboSteps[i]))
                {
                    return num;
                }
                num++;
            }
            return num;
        }

        public override void OnAdded()
        {
            Singleton<LevelManager>.Instance.levelActor.onLevelComboChanged = (Action<int, int>) Delegate.Combine(Singleton<LevelManager>.Instance.levelActor.onLevelComboChanged, new Action<int, int>(this.UpdateAttackByCombo));
        }

        public override void OnRemoved()
        {
            Singleton<LevelManager>.Instance.levelActor.onLevelComboChanged = (Action<int, int>) Delegate.Remove(Singleton<LevelManager>.Instance.levelActor.onLevelComboChanged, new Action<int, int>(this.UpdateAttackByCombo));
        }

        private void RemoveStepModifier(int step)
        {
            if (step != 0)
            {
                int index = step - 1;
                string modifierName = this.config.ModifierNames[index];
                this._avatarActor.abilityPlugin.TryRemoveModifier(base.instancedAbility, modifierName);
            }
        }

        private void UpdateAttackByCombo(int from, int to)
        {
            int step = this.EvaluateComboStep(from);
            int num2 = this.EvaluateComboStep(to);
            if (step != num2)
            {
                this.RemoveStepModifier(step);
                this.ApplyStepModifier(num2);
            }
        }
    }
}

