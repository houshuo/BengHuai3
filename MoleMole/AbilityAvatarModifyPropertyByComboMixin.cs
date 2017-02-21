namespace MoleMole
{
    using MoleMole.Config;
    using System;
    using UnityEngine;

    public class AbilityAvatarModifyPropertyByComboMixin : BaseAbilityMixin
    {
        private float _perComboDelta;
        private int _propertyIx;
        private AvatarModifyPropertyByCombo config;

        public AbilityAvatarModifyPropertyByComboMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config) : base(instancedAbility, instancedModifier, config)
        {
            this.config = (AvatarModifyPropertyByCombo) config;
            if (this.config.MaxValueCombo == null)
            {
                this._perComboDelta = instancedAbility.Evaluate(this.config.PerComboDelta);
            }
            else
            {
                this._perComboDelta = instancedAbility.Evaluate(this.config.MaxValue) / ((float) instancedAbility.Evaluate(this.config.MaxValueCombo));
            }
        }

        public override void OnAdded()
        {
            Singleton<LevelManager>.Instance.levelActor.onLevelComboChanged = (Action<int, int>) Delegate.Combine(Singleton<LevelManager>.Instance.levelActor.onLevelComboChanged, new Action<int, int>(this.UpdateAttackSpeedByCombo));
            this._propertyIx = base.actor.PushProperty(this.config.Property, 0f);
            this.UpdateAttackSpeedByCombo(0, (int) Singleton<LevelManager>.Instance.levelActor.levelCombo);
        }

        public override void OnRemoved()
        {
            Singleton<LevelManager>.Instance.levelActor.onLevelComboChanged = (Action<int, int>) Delegate.Remove(Singleton<LevelManager>.Instance.levelActor.onLevelComboChanged, new Action<int, int>(this.UpdateAttackSpeedByCombo));
            base.actor.PopProperty(this.config.Property, this._propertyIx);
        }

        private void UpdateAttackSpeedByCombo(int from, int to)
        {
            float num = Mathf.Clamp(base.instancedAbility.Evaluate(this.config.Initial) + (to * this._perComboDelta), base.instancedAbility.Evaluate(this.config.MinValue), base.instancedAbility.Evaluate(this.config.MaxValue));
            base.actor.SetPropertyByStackIndex(this.config.Property, this._propertyIx, num);
        }
    }
}

