namespace MoleMole
{
    using MoleMole.Config;
    using System;

    public class AbilityStartSwitchModifierMixin : BaseAbilityMixin
    {
        private string _originIconPath;
        private float _originSpCost;
        private float _originSpNeed;
        private bool _state;
        private EntityTimer _timer;
        private OnStartSwitchModifierMixin config;

        public AbilityStartSwitchModifierMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config) : base(instancedAbility, instancedModifier, config)
        {
            this.config = (OnStartSwitchModifierMixin) config;
            this._timer = new EntityTimer(this.config.MaxDuration);
        }

        public override void Core()
        {
            base.Core();
            if ((this._state && this.config.UseLowSPForceOff) && (base.actor.SP <= 0f))
            {
                this.TurnOffModifier();
            }
            if ((this._state && this.config.UseLowHPForceOff) && (base.actor.HP <= 0f))
            {
                this.TurnOffModifier();
            }
            if (this.config.MaxDuration > 0f)
            {
                this._timer.Core(1f);
                if (this._timer.isTimeUp)
                {
                    this.TurnOffModifier();
                }
            }
        }

        public override void OnAbilityTriggered(EvtAbilityStart evt)
        {
            if (this.config.AlwaysSwitchOn)
            {
                this.TurnOnModifier();
            }
            else if (!this._state)
            {
                this.TurnOnModifier();
            }
            else
            {
                this.TurnOffModifier();
            }
        }

        public override void OnAdded()
        {
            if (this.config.OffModifierName != null)
            {
                base.actor.abilityPlugin.ApplyModifier(base.instancedAbility, this.config.OffModifierName);
            }
            AvatarActor actor = base.actor as AvatarActor;
            if (!string.IsNullOrEmpty(this.config.SkillButtonID))
            {
                this._originSpCost = (float) actor.GetSkillInfo(this.config.SkillButtonID).costSP;
                this._originSpNeed = (float) actor.GetSkillInfo(this.config.SkillButtonID).needSP;
                this._originIconPath = actor.GetSkillInfo(this.config.SkillButtonID).iconPath;
            }
        }

        public override void OnRemoved()
        {
            if (this.config.OnModifierName != null)
            {
                base.actor.abilityPlugin.TryRemoveModifier(base.instancedAbility, this.config.OnModifierName);
            }
            if (this.config.OffModifierName != null)
            {
                base.actor.abilityPlugin.TryRemoveModifier(base.instancedAbility, this.config.OffModifierName);
            }
        }

        private void TurnOffModifier()
        {
            if (this.config.OffModifierName != null)
            {
                base.actor.abilityPlugin.ApplyModifier(base.instancedAbility, this.config.OffModifierName);
            }
            base.actor.abilityPlugin.TryRemoveModifier(base.instancedAbility, this.config.OnModifierName);
            if (!string.IsNullOrEmpty(this.config.SkillButtonID))
            {
                AvatarActor actor = base.actor as AvatarActor;
                if (this.config.OnModifierSwitchToInstantTrigger)
                {
                    actor.config.Skills[this.config.SkillButtonID].IsInstantTrigger = false;
                    actor.config.Skills[this.config.SkillButtonID].InstantTriggerEvent = null;
                }
                actor.GetSkillInfo(this.config.SkillButtonID).costSP = this._originSpCost;
                actor.GetSkillInfo(this.config.SkillButtonID).needSP = this._originSpNeed;
                actor.GetSkillInfo(this.config.SkillButtonID).iconPath = this._originIconPath;
                actor.GetSkillInfo(this.config.SkillButtonID).muteHighlighted = false;
                Singleton<MainUIManager>.Instance.GetInLevelUICanvas().mainPageContext.GetSkillButtonBySkillID(this.config.SkillButtonID).RefreshSkillInfo();
            }
            this._timer.Reset(false);
            this._state = false;
        }

        private void TurnOnModifier()
        {
            base.actor.abilityPlugin.ApplyModifier(base.instancedAbility, this.config.OnModifierName);
            if (this.config.OffModifierName != null)
            {
                base.actor.abilityPlugin.TryRemoveModifier(base.instancedAbility, this.config.OffModifierName);
            }
            if (!string.IsNullOrEmpty(this.config.SkillButtonID))
            {
                AvatarActor actor = base.actor as AvatarActor;
                actor.GetSkillInfo(this.config.SkillButtonID).costSP = (SafeFloat) this.config.OnModifierReplaceCostSP;
                actor.GetSkillInfo(this.config.SkillButtonID).needSP = (SafeFloat) this.config.OnModifierReplaceCostSP;
                actor.GetSkillInfo(this.config.SkillButtonID).iconPath = this.config.OnModifierReplaceIconPath;
                actor.GetSkillInfo(this.config.SkillButtonID).muteHighlighted = true;
                Singleton<MainUIManager>.Instance.GetInLevelUICanvas().mainPageContext.GetSkillButtonBySkillID(this.config.SkillButtonID).RefreshSkillInfo();
                if (this.config.OnModifierSwitchToInstantTrigger)
                {
                    actor.config.Skills[this.config.SkillButtonID].IsInstantTrigger = true;
                    actor.config.Skills[this.config.SkillButtonID].InstantTriggerEvent = this.config.OnModifierInstantTriggerEvent;
                }
            }
            this._timer.Reset(true);
            this._state = true;
        }
    }
}

