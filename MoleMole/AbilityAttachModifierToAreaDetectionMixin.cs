namespace MoleMole
{
    using MoleMole.Config;
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public class AbilityAttachModifierToAreaDetectionMixin : BaseAbilityMixin
    {
        private BaseMonoEntity _bufferEntity;
        private float _delayTimer;
        private bool _hasEntity;
        private AttachModifierToAreaDetectionMixin config;

        public AbilityAttachModifierToAreaDetectionMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config) : base(instancedAbility, instancedModifier, config)
        {
            this.config = (AttachModifierToAreaDetectionMixin) config;
        }

        private void ApplyModifiers()
        {
            for (int i = 0; i < this.config.ModifierNames.Length; i++)
            {
                string modifierName = this.config.ModifierNames[i];
                base.actor.abilityPlugin.ApplyModifier(base.instancedAbility, modifierName);
            }
        }

        public override void Core()
        {
            if (base.entity.IsActive())
            {
                this._delayTimer -= Time.deltaTime * base.entity.TimeScale;
                if (this._delayTimer <= 0f)
                {
                    if (this._hasEntity)
                    {
                        if ((this._bufferEntity == null) || (Vector3.Distance(base.actor.entity.XZPosition, this._bufferEntity.XZPosition) > base.instancedAbility.Evaluate(this.config.Radius)))
                        {
                            this._bufferEntity = null;
                            if (!this.FindEntity())
                            {
                                this._hasEntity = false;
                                if (!this.config.IsInvert)
                                {
                                    this.RemoveModifiers();
                                }
                                else
                                {
                                    this.ApplyModifiers();
                                }
                            }
                        }
                    }
                    else if (this.FindEntity())
                    {
                        this._hasEntity = true;
                        if (!this.config.IsInvert)
                        {
                            this.ApplyModifiers();
                        }
                        else
                        {
                            this.RemoveModifiers();
                        }
                    }
                    this._delayTimer = this.config.Delay;
                }
            }
        }

        private bool FindEntity()
        {
            List<BaseMonoMonster> allMonsters = Singleton<MonsterManager>.Instance.GetAllMonsters();
            for (int i = 0; i < allMonsters.Count; i++)
            {
                BaseMonoMonster monster = allMonsters[i];
                if (Vector3.Distance(base.actor.entity.XZPosition, monster.XZPosition) <= base.instancedAbility.Evaluate(this.config.Radius))
                {
                    this._bufferEntity = monster;
                    return true;
                }
            }
            return false;
        }

        private void OnActiveChanged(bool active)
        {
            if (!active)
            {
                this._hasEntity = false;
                this._bufferEntity = null;
                this.RemoveModifiers();
            }
            else
            {
                this._delayTimer = this.config.Delay;
            }
        }

        public override void OnAdded()
        {
            base.entity.onActiveChanged = (Action<bool>) Delegate.Combine(base.entity.onActiveChanged, new Action<bool>(this.OnActiveChanged));
            this._delayTimer = this.config.Delay;
        }

        public override void OnRemoved()
        {
            base.entity.onActiveChanged = (Action<bool>) Delegate.Remove(base.entity.onActiveChanged, new Action<bool>(this.OnActiveChanged));
            this.RemoveModifiers();
        }

        private void RemoveModifiers()
        {
            for (int i = 0; i < this.config.ModifierNames.Length; i++)
            {
                string modifierName = this.config.ModifierNames[i];
                base.actor.abilityPlugin.TryRemoveModifier(base.instancedAbility, modifierName);
            }
        }
    }
}

