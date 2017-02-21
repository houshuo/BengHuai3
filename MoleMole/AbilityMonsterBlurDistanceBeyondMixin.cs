namespace MoleMole
{
    using MoleMole.Config;
    using System;
    using UnityEngine;

    public class AbilityMonsterBlurDistanceBeyondMixin : BaseAbilityMixin
    {
        private BaseAbilityActor _abilityActor;
        private bool _inDistance;
        private MonsterBlurDistanceBeyondMixin config;

        public AbilityMonsterBlurDistanceBeyondMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config) : base(instancedAbility, instancedModifier, config)
        {
            this.config = (MonsterBlurDistanceBeyondMixin) config;
            this._abilityActor = base.actor;
            this._inDistance = true;
        }

        private void AddModifiers()
        {
            if ((this.config.ModifierNames != null) && (this.config.ModifierNames.Length > 0))
            {
                int index = 0;
                int length = this.config.ModifierNames.Length;
                while (index < length)
                {
                    this._abilityActor.abilityPlugin.ApplyModifier(base.instancedAbility, this.config.ModifierNames[index]);
                    index++;
                }
            }
        }

        private void ControlModifierByIsInDistance(bool isInDistance)
        {
            this._inDistance = isInDistance;
            if (this._inDistance)
            {
                this.SetMonsterBlur(false);
                this.RemoveModifiers();
            }
            else
            {
                this.SetMonsterBlur(true);
                this.AddModifiers();
            }
        }

        public override void Core()
        {
            bool isInDistance = Vector3.Distance(Singleton<AvatarManager>.Instance.GetLocalAvatar().XZPosition, this._abilityActor.entity.XZPosition) < base.instancedAbility.Evaluate(this.config.Distance);
            if (isInDistance != this._inDistance)
            {
                this.ControlModifierByIsInDistance(isInDistance);
            }
        }

        public override void OnAdded()
        {
        }

        public override void OnRemoved()
        {
        }

        private void RemoveModifiers()
        {
            if ((this.config.ModifierNames != null) && (this.config.ModifierNames.Length > 0))
            {
                int index = 0;
                int length = this.config.ModifierNames.Length;
                while (index < length)
                {
                    this._abilityActor.abilityPlugin.TryRemoveModifier(base.instancedAbility, this.config.ModifierNames[index]);
                    index++;
                }
            }
        }

        private void SetMonsterBlur(bool isBlur)
        {
            if (this._abilityActor != null)
            {
                this._abilityActor.entity.SetCountedIsGhost(isBlur);
                this._abilityActor.entity.SetCountedDenySelect(isBlur, false);
            }
        }
    }
}

