namespace MoleMole
{
    using MoleMole.Config;
    using System;
    using UnityEngine;

    public class AbilityAvatarWeaponOverHeatMixin : BaseAbilityMixin
    {
        private BaseMonoAnimatorEntity _animatorEntity;
        private AvatarActor _avatarActor;
        private float _coolSpeed;
        private float _currentHeatAddSpeed;
        protected string _currentSkillID;
        private float _heat;
        private bool _isAttacking;
        private bool _isOverHeat;
        private DisplayValue<float> _isOverheatDisplay;
        private DisplayValue<float> _overheatValueDisplay;
        private AvatarWeaponOverHeatMixin config;

        public AbilityAvatarWeaponOverHeatMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config) : base(instancedAbility, instancedModifier, config)
        {
            this.config = (AvatarWeaponOverHeatMixin) config;
            this._animatorEntity = (BaseMonoAnimatorEntity) base.entity;
            this._avatarActor = (AvatarActor) base.actor;
        }

        public override void Core()
        {
            base.Core();
            if (this._currentHeatAddSpeed != 0f)
            {
                DelegateUtils.UpdateField<float>(ref this._heat, this._heat + (((this._currentHeatAddSpeed * base.entity.TimeScale) * Time.deltaTime) * base.instancedAbility.Evaluate(this.config.ContinuousHeatSpeedRatio)), new Action<float, float>(this.UpdateOverheatDisplayValue));
                this._coolSpeed = 0f;
            }
            if ((this._heat > 0f) && this._animatorEntity.ContainAnimEventPredicate(this.config.IgnorePredicate))
            {
                DelegateUtils.UpdateField<float>(ref this._heat, 0f, new Action<float, float>(this.UpdateOverheatDisplayValue));
            }
            if ((this._heat > this.config.OverHeatMax) && !this._isOverHeat)
            {
                this._isOverHeat = true;
                this._isOverheatDisplay.Pub(1f);
                base.actor.abilityPlugin.HandleActionTargetDispatch(this.config.OverHeatActions, base.instancedAbility, base.instancedModifier, base.actor, null);
                this._avatarActor.SetMuteSkill(this.config.OverHeatButtonSkillID, true);
                Singleton<AvatarManager>.Instance.GetAvatarByRuntimeID(base.actor.runtimeID).IsLockDirection = false;
            }
            if (this._isOverHeat)
            {
                if (this._heat > 0f)
                {
                    if (this._coolSpeed < this.config.OverHeatCoolSpeed)
                    {
                        this._coolSpeed += (Time.deltaTime * this.config.OverHeatCoolSpeed) / this.config.ToMaxCoolSpeedTime;
                    }
                    DelegateUtils.UpdateField<float>(ref this._heat, this._heat - ((base.entity.TimeScale * Time.deltaTime) * this._coolSpeed), new Action<float, float>(this.UpdateOverheatDisplayValue));
                }
                else
                {
                    this._isOverHeat = false;
                    this._isOverheatDisplay.Pub(0f);
                    base.actor.abilityPlugin.HandleActionTargetDispatch(this.config.CoolDownActions, base.instancedAbility, base.instancedModifier, base.actor, null);
                    this._avatarActor.SetMuteSkill(this.config.OverHeatButtonSkillID, false);
                }
            }
            else if ((this._heat > 0f) && !this._isAttacking)
            {
                if (this._coolSpeed < this.config.CoolSpeed)
                {
                    this._coolSpeed += (Time.deltaTime * this.config.CoolSpeed) / this.config.ToMaxCoolSpeedTime;
                }
                DelegateUtils.UpdateField<float>(ref this._heat, this._heat - ((base.entity.TimeScale * Time.deltaTime) * this._coolSpeed), new Action<float, float>(this.UpdateOverheatDisplayValue));
            }
        }

        public override void OnAbilityTriggered(EvtAbilityStart evt)
        {
            this._heat = 0f;
        }

        public override void OnAdded()
        {
            Singleton<EventManager>.Instance.RegisterEventListener<EvtAttackStart>(base.actor.runtimeID);
            this._animatorEntity.onCurrentSkillIDChanged = (Action<string, string>) Delegate.Combine(this._animatorEntity.onCurrentSkillIDChanged, new Action<string, string>(this.SkillIDChangedCallback));
            this._isOverheatDisplay = base.actor.abilityPlugin.CreateOrGetDisplayFloat("IsOverheat", 0f, 1f, !this._isOverHeat ? 0f : 1f);
            this._overheatValueDisplay = base.actor.abilityPlugin.CreateOrGetDisplayFloat("OverheatRatio", 0f, 1f, this._heat / this.config.OverHeatMax);
        }

        public override bool OnPostEvent(BaseEvent evt)
        {
            return ((evt is EvtAttackStart) && this.OnSkillStart((EvtAttackStart) evt));
        }

        public override void OnRemoved()
        {
            Singleton<EventManager>.Instance.RemoveEventListener<EvtAttackStart>(base.actor.runtimeID);
        }

        private bool OnSkillStart(EvtAttackStart evt)
        {
            if (this.config.SkillIDs != null)
            {
                for (int i = 0; i < this.config.SkillIDs.Length; i++)
                {
                    if ((this.config.SkillIDs[i] == evt.skillID) && !this._animatorEntity.ContainAnimEventPredicate(this.config.IgnorePredicate))
                    {
                        DelegateUtils.UpdateField<float>(ref this._heat, this._heat + this.config.SkillHeatAdds[i], new Action<float, float>(this.UpdateOverheatDisplayValue));
                        this._coolSpeed = 0f;
                        break;
                    }
                }
            }
            return true;
        }

        private void SkillIDChangedCallback(string from, string to)
        {
            if (this.config.ContinuousSkillIDs != null)
            {
                for (int i = 0; i < this.config.ContinuousSkillIDs.Length; i++)
                {
                    if (from == this.config.ContinuousSkillIDs[i])
                    {
                        this._currentHeatAddSpeed = 0f;
                        this._currentSkillID = null;
                        this._isAttacking = false;
                    }
                    if (to == this.config.ContinuousSkillIDs[i])
                    {
                        this._currentHeatAddSpeed = this.config.ContinuousHeatAddSpeed[i];
                        this._currentSkillID = this.config.ContinuousSkillIDs[i];
                        this._isAttacking = true;
                    }
                }
            }
            if (this.config.NoCoolSkillIDs != null)
            {
                for (int j = 0; j < this.config.NoCoolSkillIDs.Length; j++)
                {
                    if (from == this.config.NoCoolSkillIDs[j])
                    {
                        this._isAttacking = false;
                    }
                    if (to == this.config.NoCoolSkillIDs[j])
                    {
                        this._isAttacking = true;
                    }
                }
            }
        }

        private void UpdateOverheatDisplayValue(float fromHeat, float toHeat)
        {
            this._overheatValueDisplay.Pub(Mathf.Clamp01(toHeat / this.config.OverHeatMax));
        }
    }
}

