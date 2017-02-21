namespace MoleMole.Config
{
    using FullInspector;
    using MoleMole;
    using System;
    using System.Collections.Generic;

    [GeneratePartialHash]
    public class ConfigAbility : IHashable, IOnLoaded
    {
        public ConfigAbilityMixin[] AbilityMixins = ConfigAbilityMixin.EMPTY;
        public string AbilityName;
        public ConfigDynamicArguments AbilitySpecials = ConfigDynamicArguments.EMPTY;
        private const string DEFAULT_MODIFIER_NAME = "__DEFAULT_MODIFIER";
        [InspectorNullable]
        public ConfigAbilityModifier DefaultModifier;
        public static Dictionary<string, ConfigAbilityModifier> EMPTY_MODIFIERS = new Dictionary<string, ConfigAbilityModifier>();
        [NonSerialized]
        public List<BaseActionContainer> InvokeSites;
        [NonSerialized]
        public ConfigAbilityModifier[] ModifierIDMap;
        public Dictionary<string, ConfigAbilityModifier> Modifiers = EMPTY_MODIFIERS;
        public ConfigAbilityAction[] OnAbilityStart = ConfigAbilityAction.EMPTY;
        public ConfigAbilityAction[] OnAdded = ConfigAbilityAction.EMPTY;
        public ConfigAbilityAction[] OnDestroy = ConfigAbilityAction.EMPTY;
        public ConfigAbilityAction[] OnFieldEnter = ConfigAbilityAction.EMPTY;
        public ConfigAbilityAction[] OnFieldExit = ConfigAbilityAction.EMPTY;
        public ConfigAbilityAction[] OnKilled = ConfigAbilityAction.EMPTY;
        public ConfigAbilityAction[] OnRemoved = ConfigAbilityAction.EMPTY;
        [InspectorNullable]
        public string SetAbilityArgumentToOverrideMap;
        [InspectorNullable]
        public string UseAbilityArgumentAsSpecialKey;

        private void AddMixins(ConfigAbilityMixin[] mixins)
        {
            for (int i = 0; i < mixins.Length; i++)
            {
                ConfigAbilityMixin item = mixins[i];
                this.InvokeSites.Add(item);
                item.localID = this.InvokeSites.Count - 1;
                this.AddSubActions(item.GetAllSubActions());
            }
        }

        private void AddSubAction(ConfigAbilityAction action)
        {
            this.InvokeSites.Add(action);
            action.localID = this.InvokeSites.Count - 1;
        }

        private void AddSubActions(ConfigAbilityAction[] actions)
        {
            for (int i = 0; i < actions.Length; i++)
            {
                ConfigAbilityAction action = actions[i];
                this.AddSubAction(action);
                ConfigAbilityAction[][] allSubActions = action.GetAllSubActions();
                for (int j = 0; j < allSubActions.Length; j++)
                {
                    this.AddSubActions(allSubActions[j]);
                }
            }
        }

        private void AddSubActions(ConfigAbilityAction[][] actions)
        {
            for (int i = 0; i < actions.Length; i++)
            {
                this.AddSubActions(actions[i]);
            }
        }

        public void ObjectContentHashOnto(ref int lastHash)
        {
            HashUtils.ContentHashOnto(this.AbilityName, ref lastHash);
            HashUtils.ContentHashOnto(this.UseAbilityArgumentAsSpecialKey, ref lastHash);
            HashUtils.ContentHashOnto(this.SetAbilityArgumentToOverrideMap, ref lastHash);
            if (this.AbilityMixins != null)
            {
                foreach (ConfigAbilityMixin mixin in this.AbilityMixins)
                {
                    if (mixin is IHashable)
                    {
                        HashUtils.ContentHashOnto((IHashable) mixin, ref lastHash);
                    }
                }
            }
            if (this.AbilitySpecials != null)
            {
                foreach (KeyValuePair<string, object> pair in this.AbilitySpecials)
                {
                    HashUtils.ContentHashOnto(pair.Key, ref lastHash);
                    HashUtils.ContentHashOntoFallback(pair.Value, ref lastHash);
                }
            }
            if (this.Modifiers != null)
            {
                foreach (KeyValuePair<string, ConfigAbilityModifier> pair2 in this.Modifiers)
                {
                    HashUtils.ContentHashOnto(pair2.Key, ref lastHash);
                    HashUtils.ContentHashOnto((int) pair2.Value.TimeScale, ref lastHash);
                    HashUtils.ContentHashOnto((int) pair2.Value.Stacking, ref lastHash);
                    HashUtils.ContentHashOnto(pair2.Value.IsBuff, ref lastHash);
                    HashUtils.ContentHashOnto(pair2.Value.IsDebuff, ref lastHash);
                    HashUtils.ContentHashOnto(pair2.Value.IsUnique, ref lastHash);
                    if (pair2.Value.Duration != null)
                    {
                        HashUtils.ContentHashOnto(pair2.Value.Duration.isDynamic, ref lastHash);
                        HashUtils.ContentHashOnto(pair2.Value.Duration.fixedValue, ref lastHash);
                        HashUtils.ContentHashOnto(pair2.Value.Duration.dynamicKey, ref lastHash);
                    }
                    if (pair2.Value.ThinkInterval != null)
                    {
                        HashUtils.ContentHashOnto(pair2.Value.ThinkInterval.isDynamic, ref lastHash);
                        HashUtils.ContentHashOnto(pair2.Value.ThinkInterval.fixedValue, ref lastHash);
                        HashUtils.ContentHashOnto(pair2.Value.ThinkInterval.dynamicKey, ref lastHash);
                    }
                    if (pair2.Value.ModifierMixins != null)
                    {
                        foreach (ConfigAbilityMixin mixin2 in pair2.Value.ModifierMixins)
                        {
                            if (mixin2 is IHashable)
                            {
                                HashUtils.ContentHashOnto((IHashable) mixin2, ref lastHash);
                            }
                        }
                    }
                    if (pair2.Value.Properties != null)
                    {
                        IEnumerator<KeyValuePair<string, DynamicFloat>> enumerator = pair2.Value.Properties.GetEnumerator();
                        try
                        {
                            while (enumerator.MoveNext())
                            {
                                KeyValuePair<string, DynamicFloat> current = enumerator.Current;
                                HashUtils.ContentHashOnto(current.Key, ref lastHash);
                                HashUtils.ContentHashOnto(current.Value.isDynamic, ref lastHash);
                                HashUtils.ContentHashOnto(current.Value.fixedValue, ref lastHash);
                                HashUtils.ContentHashOnto(current.Value.dynamicKey, ref lastHash);
                            }
                        }
                        finally
                        {
                            if (enumerator == null)
                            {
                            }
                            enumerator.Dispose();
                        }
                    }
                    if (pair2.Value.EntityProperties != null)
                    {
                        IEnumerator<KeyValuePair<string, DynamicFloat>> enumerator4 = pair2.Value.EntityProperties.GetEnumerator();
                        try
                        {
                            while (enumerator4.MoveNext())
                            {
                                KeyValuePair<string, DynamicFloat> pair4 = enumerator4.Current;
                                HashUtils.ContentHashOnto(pair4.Key, ref lastHash);
                                HashUtils.ContentHashOnto(pair4.Value.isDynamic, ref lastHash);
                                HashUtils.ContentHashOnto(pair4.Value.fixedValue, ref lastHash);
                                HashUtils.ContentHashOnto(pair4.Value.dynamicKey, ref lastHash);
                            }
                        }
                        finally
                        {
                            if (enumerator4 == null)
                            {
                            }
                            enumerator4.Dispose();
                        }
                    }
                    HashUtils.ContentHashOnto((int) pair2.Value.State, ref lastHash);
                    if (pair2.Value.StateOption != null)
                    {
                    }
                    HashUtils.ContentHashOnto(pair2.Value.MuteStateDisplayEffect, ref lastHash);
                    HashUtils.ContentHashOnto(pair2.Value.ApplyAttackerWitchTimeRatio, ref lastHash);
                    if (pair2.Value.OnAdded != null)
                    {
                        foreach (ConfigAbilityAction action in pair2.Value.OnAdded)
                        {
                            if (action is IHashable)
                            {
                                HashUtils.ContentHashOnto((IHashable) action, ref lastHash);
                            }
                        }
                    }
                    if (pair2.Value.OnRemoved != null)
                    {
                        foreach (ConfigAbilityAction action2 in pair2.Value.OnRemoved)
                        {
                            if (action2 is IHashable)
                            {
                                HashUtils.ContentHashOnto((IHashable) action2, ref lastHash);
                            }
                        }
                    }
                    if (pair2.Value.OnBeingHit != null)
                    {
                        foreach (ConfigAbilityAction action3 in pair2.Value.OnBeingHit)
                        {
                            if (action3 is IHashable)
                            {
                                HashUtils.ContentHashOnto((IHashable) action3, ref lastHash);
                            }
                        }
                    }
                    if (pair2.Value.OnBeingHitResolved != null)
                    {
                        foreach (ConfigAbilityAction action4 in pair2.Value.OnBeingHitResolved)
                        {
                            if (action4 is IHashable)
                            {
                                HashUtils.ContentHashOnto((IHashable) action4, ref lastHash);
                            }
                        }
                    }
                    if (pair2.Value.OnAttackLanded != null)
                    {
                        foreach (ConfigAbilityAction action5 in pair2.Value.OnAttackLanded)
                        {
                            if (action5 is IHashable)
                            {
                                HashUtils.ContentHashOnto((IHashable) action5, ref lastHash);
                            }
                        }
                    }
                    if (pair2.Value.OnThinkInterval != null)
                    {
                        foreach (ConfigAbilityAction action6 in pair2.Value.OnThinkInterval)
                        {
                            if (action6 is IHashable)
                            {
                                HashUtils.ContentHashOnto((IHashable) action6, ref lastHash);
                            }
                        }
                    }
                    if (pair2.Value.OnEvadeStart != null)
                    {
                        foreach (ConfigAbilityAction action7 in pair2.Value.OnEvadeStart)
                        {
                            if (action7 is IHashable)
                            {
                                HashUtils.ContentHashOnto((IHashable) action7, ref lastHash);
                            }
                        }
                    }
                    if (pair2.Value.OnEvadeSuccess != null)
                    {
                        foreach (ConfigAbilityAction action8 in pair2.Value.OnEvadeSuccess)
                        {
                            if (action8 is IHashable)
                            {
                                HashUtils.ContentHashOnto((IHashable) action8, ref lastHash);
                            }
                        }
                    }
                    if (pair2.Value.OnDefendStart != null)
                    {
                        foreach (ConfigAbilityAction action9 in pair2.Value.OnDefendStart)
                        {
                            if (action9 is IHashable)
                            {
                                HashUtils.ContentHashOnto((IHashable) action9, ref lastHash);
                            }
                        }
                    }
                    if (pair2.Value.OnDefendSuccess != null)
                    {
                        foreach (ConfigAbilityAction action10 in pair2.Value.OnDefendSuccess)
                        {
                            if (action10 is IHashable)
                            {
                                HashUtils.ContentHashOnto((IHashable) action10, ref lastHash);
                            }
                        }
                    }
                    if (pair2.Value.OnMonsterCreated != null)
                    {
                        foreach (ConfigAbilityAction action11 in pair2.Value.OnMonsterCreated)
                        {
                            if (action11 is IHashable)
                            {
                                HashUtils.ContentHashOnto((IHashable) action11, ref lastHash);
                            }
                        }
                    }
                    if (pair2.Value.OnAvatarCreated != null)
                    {
                        foreach (ConfigAbilityAction action12 in pair2.Value.OnAvatarCreated)
                        {
                            if (action12 is IHashable)
                            {
                                HashUtils.ContentHashOnto((IHashable) action12, ref lastHash);
                            }
                        }
                    }
                    if (pair2.Value.OnKilled != null)
                    {
                        foreach (ConfigAbilityAction action13 in pair2.Value.OnKilled)
                        {
                            if (action13 is IHashable)
                            {
                                HashUtils.ContentHashOnto((IHashable) action13, ref lastHash);
                            }
                        }
                    }
                }
            }
            if (this.DefaultModifier != null)
            {
                HashUtils.ContentHashOnto((int) this.DefaultModifier.TimeScale, ref lastHash);
                HashUtils.ContentHashOnto((int) this.DefaultModifier.Stacking, ref lastHash);
                HashUtils.ContentHashOnto(this.DefaultModifier.IsBuff, ref lastHash);
                HashUtils.ContentHashOnto(this.DefaultModifier.IsDebuff, ref lastHash);
                HashUtils.ContentHashOnto(this.DefaultModifier.IsUnique, ref lastHash);
                if (this.DefaultModifier.Duration != null)
                {
                    HashUtils.ContentHashOnto(this.DefaultModifier.Duration.isDynamic, ref lastHash);
                    HashUtils.ContentHashOnto(this.DefaultModifier.Duration.fixedValue, ref lastHash);
                    HashUtils.ContentHashOnto(this.DefaultModifier.Duration.dynamicKey, ref lastHash);
                }
                if (this.DefaultModifier.ThinkInterval != null)
                {
                    HashUtils.ContentHashOnto(this.DefaultModifier.ThinkInterval.isDynamic, ref lastHash);
                    HashUtils.ContentHashOnto(this.DefaultModifier.ThinkInterval.fixedValue, ref lastHash);
                    HashUtils.ContentHashOnto(this.DefaultModifier.ThinkInterval.dynamicKey, ref lastHash);
                }
                if (this.DefaultModifier.ModifierMixins != null)
                {
                    foreach (ConfigAbilityMixin mixin3 in this.DefaultModifier.ModifierMixins)
                    {
                        if (mixin3 is IHashable)
                        {
                            HashUtils.ContentHashOnto((IHashable) mixin3, ref lastHash);
                        }
                    }
                }
                if (this.DefaultModifier.Properties != null)
                {
                    IEnumerator<KeyValuePair<string, DynamicFloat>> enumerator5 = this.DefaultModifier.Properties.GetEnumerator();
                    try
                    {
                        while (enumerator5.MoveNext())
                        {
                            KeyValuePair<string, DynamicFloat> pair5 = enumerator5.Current;
                            HashUtils.ContentHashOnto(pair5.Key, ref lastHash);
                            HashUtils.ContentHashOnto(pair5.Value.isDynamic, ref lastHash);
                            HashUtils.ContentHashOnto(pair5.Value.fixedValue, ref lastHash);
                            HashUtils.ContentHashOnto(pair5.Value.dynamicKey, ref lastHash);
                        }
                    }
                    finally
                    {
                        if (enumerator5 == null)
                        {
                        }
                        enumerator5.Dispose();
                    }
                }
                if (this.DefaultModifier.EntityProperties != null)
                {
                    IEnumerator<KeyValuePair<string, DynamicFloat>> enumerator6 = this.DefaultModifier.EntityProperties.GetEnumerator();
                    try
                    {
                        while (enumerator6.MoveNext())
                        {
                            KeyValuePair<string, DynamicFloat> pair6 = enumerator6.Current;
                            HashUtils.ContentHashOnto(pair6.Key, ref lastHash);
                            HashUtils.ContentHashOnto(pair6.Value.isDynamic, ref lastHash);
                            HashUtils.ContentHashOnto(pair6.Value.fixedValue, ref lastHash);
                            HashUtils.ContentHashOnto(pair6.Value.dynamicKey, ref lastHash);
                        }
                    }
                    finally
                    {
                        if (enumerator6 == null)
                        {
                        }
                        enumerator6.Dispose();
                    }
                }
                HashUtils.ContentHashOnto((int) this.DefaultModifier.State, ref lastHash);
                if (this.DefaultModifier.StateOption != null)
                {
                }
                HashUtils.ContentHashOnto(this.DefaultModifier.MuteStateDisplayEffect, ref lastHash);
                HashUtils.ContentHashOnto(this.DefaultModifier.ApplyAttackerWitchTimeRatio, ref lastHash);
                if (this.DefaultModifier.OnAdded != null)
                {
                    foreach (ConfigAbilityAction action14 in this.DefaultModifier.OnAdded)
                    {
                        if (action14 is IHashable)
                        {
                            HashUtils.ContentHashOnto((IHashable) action14, ref lastHash);
                        }
                    }
                }
                if (this.DefaultModifier.OnRemoved != null)
                {
                    foreach (ConfigAbilityAction action15 in this.DefaultModifier.OnRemoved)
                    {
                        if (action15 is IHashable)
                        {
                            HashUtils.ContentHashOnto((IHashable) action15, ref lastHash);
                        }
                    }
                }
                if (this.DefaultModifier.OnBeingHit != null)
                {
                    foreach (ConfigAbilityAction action16 in this.DefaultModifier.OnBeingHit)
                    {
                        if (action16 is IHashable)
                        {
                            HashUtils.ContentHashOnto((IHashable) action16, ref lastHash);
                        }
                    }
                }
                if (this.DefaultModifier.OnBeingHitResolved != null)
                {
                    foreach (ConfigAbilityAction action17 in this.DefaultModifier.OnBeingHitResolved)
                    {
                        if (action17 is IHashable)
                        {
                            HashUtils.ContentHashOnto((IHashable) action17, ref lastHash);
                        }
                    }
                }
                if (this.DefaultModifier.OnAttackLanded != null)
                {
                    foreach (ConfigAbilityAction action18 in this.DefaultModifier.OnAttackLanded)
                    {
                        if (action18 is IHashable)
                        {
                            HashUtils.ContentHashOnto((IHashable) action18, ref lastHash);
                        }
                    }
                }
                if (this.DefaultModifier.OnThinkInterval != null)
                {
                    foreach (ConfigAbilityAction action19 in this.DefaultModifier.OnThinkInterval)
                    {
                        if (action19 is IHashable)
                        {
                            HashUtils.ContentHashOnto((IHashable) action19, ref lastHash);
                        }
                    }
                }
                if (this.DefaultModifier.OnEvadeStart != null)
                {
                    foreach (ConfigAbilityAction action20 in this.DefaultModifier.OnEvadeStart)
                    {
                        if (action20 is IHashable)
                        {
                            HashUtils.ContentHashOnto((IHashable) action20, ref lastHash);
                        }
                    }
                }
                if (this.DefaultModifier.OnEvadeSuccess != null)
                {
                    foreach (ConfigAbilityAction action21 in this.DefaultModifier.OnEvadeSuccess)
                    {
                        if (action21 is IHashable)
                        {
                            HashUtils.ContentHashOnto((IHashable) action21, ref lastHash);
                        }
                    }
                }
                if (this.DefaultModifier.OnDefendStart != null)
                {
                    foreach (ConfigAbilityAction action22 in this.DefaultModifier.OnDefendStart)
                    {
                        if (action22 is IHashable)
                        {
                            HashUtils.ContentHashOnto((IHashable) action22, ref lastHash);
                        }
                    }
                }
                if (this.DefaultModifier.OnDefendSuccess != null)
                {
                    foreach (ConfigAbilityAction action23 in this.DefaultModifier.OnDefendSuccess)
                    {
                        if (action23 is IHashable)
                        {
                            HashUtils.ContentHashOnto((IHashable) action23, ref lastHash);
                        }
                    }
                }
                if (this.DefaultModifier.OnMonsterCreated != null)
                {
                    foreach (ConfigAbilityAction action24 in this.DefaultModifier.OnMonsterCreated)
                    {
                        if (action24 is IHashable)
                        {
                            HashUtils.ContentHashOnto((IHashable) action24, ref lastHash);
                        }
                    }
                }
                if (this.DefaultModifier.OnAvatarCreated != null)
                {
                    foreach (ConfigAbilityAction action25 in this.DefaultModifier.OnAvatarCreated)
                    {
                        if (action25 is IHashable)
                        {
                            HashUtils.ContentHashOnto((IHashable) action25, ref lastHash);
                        }
                    }
                }
                if (this.DefaultModifier.OnKilled != null)
                {
                    foreach (ConfigAbilityAction action26 in this.DefaultModifier.OnKilled)
                    {
                        if (action26 is IHashable)
                        {
                            HashUtils.ContentHashOnto((IHashable) action26, ref lastHash);
                        }
                    }
                }
            }
            if (this.OnAdded != null)
            {
                foreach (ConfigAbilityAction action27 in this.OnAdded)
                {
                    if (action27 is IHashable)
                    {
                        HashUtils.ContentHashOnto((IHashable) action27, ref lastHash);
                    }
                }
            }
            if (this.OnRemoved != null)
            {
                foreach (ConfigAbilityAction action28 in this.OnRemoved)
                {
                    if (action28 is IHashable)
                    {
                        HashUtils.ContentHashOnto((IHashable) action28, ref lastHash);
                    }
                }
            }
            if (this.OnAbilityStart != null)
            {
                foreach (ConfigAbilityAction action29 in this.OnAbilityStart)
                {
                    if (action29 is IHashable)
                    {
                        HashUtils.ContentHashOnto((IHashable) action29, ref lastHash);
                    }
                }
            }
            if (this.OnKilled != null)
            {
                foreach (ConfigAbilityAction action30 in this.OnKilled)
                {
                    if (action30 is IHashable)
                    {
                        HashUtils.ContentHashOnto((IHashable) action30, ref lastHash);
                    }
                }
            }
            if (this.OnDestroy != null)
            {
                foreach (ConfigAbilityAction action31 in this.OnDestroy)
                {
                    if (action31 is IHashable)
                    {
                        HashUtils.ContentHashOnto((IHashable) action31, ref lastHash);
                    }
                }
            }
            if (this.OnFieldEnter != null)
            {
                foreach (ConfigAbilityAction action32 in this.OnFieldEnter)
                {
                    if (action32 is IHashable)
                    {
                        HashUtils.ContentHashOnto((IHashable) action32, ref lastHash);
                    }
                }
            }
            if (this.OnFieldExit != null)
            {
                foreach (ConfigAbilityAction action33 in this.OnFieldExit)
                {
                    if (action33 is IHashable)
                    {
                        HashUtils.ContentHashOnto((IHashable) action33, ref lastHash);
                    }
                }
            }
        }

        public void OnLoaded()
        {
            if (this.AbilitySpecials != ConfigDynamicArguments.EMPTY)
            {
                List<string> list = new List<string>(this.AbilitySpecials.Keys);
                foreach (string str in list)
                {
                    if (this.AbilitySpecials[str] is int)
                    {
                        this.AbilitySpecials[str] = (int) this.AbilitySpecials[str];
                    }
                }
            }
            foreach (KeyValuePair<string, ConfigAbilityModifier> pair in this.Modifiers)
            {
                pair.Value.ModifierName = pair.Key;
                ConfigAbilityModifier modifier = pair.Value;
                if ((AbilityState.Undamagable | AbilityState.MaxMoveSpeed | AbilityState.Immune | AbilityState.CritUp | AbilityState.Shielded | AbilityState.PowerUp | AbilityState.AttackSpeedUp | AbilityState.MoveSpeedUp | AbilityState.Endure).ContainsState(modifier.State))
                {
                    modifier.IsBuff = true;
                }
                else if ((AbilityState.Tied | AbilityState.TargetLocked | AbilityState.Fragile | AbilityState.Weak | AbilityState.AttackSpeedDown | AbilityState.MoveSpeedDown | AbilityState.Frozen | AbilityState.Poisoned | AbilityState.Burn | AbilityState.Paralyze | AbilityState.Stun | AbilityState.Bleed).ContainsState(modifier.State))
                {
                    modifier.IsDebuff = true;
                }
                if (modifier.IsBuff)
                {
                    if (modifier.Stacking == ConfigAbilityModifier.ModifierStacking.Unique)
                    {
                        modifier.Stacking = ConfigAbilityModifier.ModifierStacking.Refresh;
                    }
                }
                else if (modifier.IsDebuff)
                {
                }
                if ((modifier.State != AbilityState.None) && (modifier.StateOption != null))
                {
                    modifier.StateOption.ChangeModifierConfig(modifier);
                }
            }
            if (this.DefaultModifier != null)
            {
                if (this.Modifiers == EMPTY_MODIFIERS)
                {
                    this.Modifiers = new Dictionary<string, ConfigAbilityModifier>();
                }
                this.DefaultModifier.ModifierName = "__DEFAULT_MODIFIER";
                this.Modifiers.Add("__DEFAULT_MODIFIER", this.DefaultModifier);
                List<ConfigAbilityAction> list2 = new List<ConfigAbilityAction>(this.OnAdded);
                ApplyModifier item = new ApplyModifier {
                    ModifierName = "__DEFAULT_MODIFIER",
                    Target = AbilityTargetting.Caster
                };
                list2.Insert(0, item);
                this.OnAdded = list2.ToArray();
                this.DefaultModifier = null;
            }
            this.InvokeSites = new List<BaseActionContainer>();
            this.AddSubActions(this.OnAdded);
            this.AddSubActions(this.OnRemoved);
            this.AddSubActions(this.OnAbilityStart);
            this.AddSubActions(this.OnKilled);
            this.AddSubActions(this.OnDestroy);
            this.AddSubActions(this.OnFieldEnter);
            this.AddSubActions(this.OnFieldExit);
            this.AddMixins(this.AbilityMixins);
            string[] array = new string[this.Modifiers.Count];
            this.Modifiers.Keys.CopyTo(array, 0);
            Array.Sort<string>(array);
            this.ModifierIDMap = new ConfigAbilityModifier[array.Length];
            for (int i = 0; i < array.Length; i++)
            {
                ConfigAbilityModifier modifier2 = this.Modifiers[array[i]];
                modifier2.localID = i;
                this.ModifierIDMap[i] = modifier2;
                this.AddSubActions(modifier2.OnAdded);
                this.AddSubActions(modifier2.OnRemoved);
                this.AddSubActions(modifier2.OnBeingHit);
                this.AddSubActions(modifier2.OnBeingHitResolved);
                this.AddSubActions(modifier2.OnAttackLanded);
                this.AddSubActions(modifier2.OnThinkInterval);
                this.AddSubActions(modifier2.OnEvadeStart);
                this.AddSubActions(modifier2.OnEvadeSuccess);
                this.AddSubActions(modifier2.OnMonsterCreated);
                this.AddSubActions(modifier2.OnAvatarCreated);
                this.AddSubActions(modifier2.OnKilled);
                this.AddMixins(modifier2.ModifierMixins);
            }
        }
    }
}

