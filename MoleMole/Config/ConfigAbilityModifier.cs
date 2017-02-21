namespace MoleMole.Config
{
    using MoleMole;
    using System;
    using System.Collections.Generic;

    public class ConfigAbilityModifier
    {
        public bool ApplyAttackerWitchTimeRatio;
        public DynamicFloat Duration;
        public SortedList<string, DynamicFloat> EntityProperties;
        public bool IsBuff;
        public bool IsDebuff;
        public bool IsUnique;
        [NonSerialized]
        public int localID;
        public ConfigAbilityMixin[] ModifierMixins;
        [NonSerialized]
        public string ModifierName;
        public bool MuteStateDisplayEffect;
        public ConfigAbilityAction[] OnAdded;
        public ConfigAbilityAction[] OnAttackLanded;
        public ConfigAbilityAction[] OnAvatarCreated;
        public ConfigAbilityAction[] OnBeingHit;
        public ConfigAbilityAction[] OnBeingHitResolved;
        public ConfigAbilityAction[] OnDefendStart;
        public ConfigAbilityAction[] OnDefendSuccess;
        public ConfigAbilityAction[] OnEvadeStart;
        public ConfigAbilityAction[] OnEvadeSuccess;
        public ConfigAbilityAction[] OnKilled;
        public ConfigAbilityAction[] OnMonsterCreated;
        public ConfigAbilityAction[] OnRemoved;
        public ConfigAbilityAction[] OnThinkInterval;
        public SortedList<string, DynamicFloat> Properties;
        public ModifierStacking Stacking;
        public AbilityState State;
        public ConfigAbilityStateOption StateOption;
        public DynamicFloat ThinkInterval;
        public ModifierTimeScale TimeScale;

        public ConfigAbilityModifier()
        {
            DynamicFloat num = new DynamicFloat {
                fixedValue = 0f
            };
            this.Duration = num;
            num = new DynamicFloat {
                fixedValue = 0f
            };
            this.ThinkInterval = num;
            this.ModifierMixins = new ConfigAbilityMixin[0];
            this.Properties = new SortedList<string, DynamicFloat>();
            this.EntityProperties = new SortedList<string, DynamicFloat>();
            this.OnAdded = ConfigAbilityAction.EMPTY;
            this.OnRemoved = ConfigAbilityAction.EMPTY;
            this.OnBeingHit = ConfigAbilityAction.EMPTY;
            this.OnBeingHitResolved = ConfigAbilityAction.EMPTY;
            this.OnAttackLanded = ConfigAbilityAction.EMPTY;
            this.OnThinkInterval = ConfigAbilityAction.EMPTY;
            this.OnEvadeStart = ConfigAbilityAction.EMPTY;
            this.OnEvadeSuccess = ConfigAbilityAction.EMPTY;
            this.OnDefendStart = ConfigAbilityAction.EMPTY;
            this.OnDefendSuccess = ConfigAbilityAction.EMPTY;
            this.OnMonsterCreated = ConfigAbilityAction.EMPTY;
            this.OnAvatarCreated = ConfigAbilityAction.EMPTY;
            this.OnKilled = ConfigAbilityAction.EMPTY;
            this.localID = -1;
        }

        public enum ModifierStacking
        {
            Unique,
            Refresh,
            Prolong,
            Multiple
        }

        public enum ModifierTimeScale
        {
            Owner,
            Caster,
            Level
        }
    }
}

