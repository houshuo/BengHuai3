namespace MoleMole
{
    using FullInspector;
    using MoleMole.Config;
    using System;
    using System.Collections.Generic;

    public class ActorAbility : BaseActorActionContext
    {
        private BaseEvent _currentTriggerEvent;
        [ShowInInspector, InspectorCollapsedFoldout]
        private Dictionary<string, object> _overrideMap;
        public bool argumentRecieved;
        public float argumentSpecialValue;
        [InspectorCollapsedFoldout]
        public BaseAbilityActor caster;
        [InspectorCollapsedFoldout]
        public ConfigAbility config;
        public int instancedAbilityID;

        public ActorAbility(BaseAbilityActor caster, ConfigAbility config, Dictionary<string, object> overrideMap)
        {
            this.caster = caster;
            this.config = config;
            this._overrideMap = overrideMap;
            List<BaseAbilityMixin> list = new List<BaseAbilityMixin>();
            for (int i = 0; i < config.AbilityMixins.Length; i++)
            {
                BaseAbilityMixin item = caster.abilityPlugin.CreateInstancedAbilityMixin(this, null, config.AbilityMixins[i]);
                if (item != null)
                {
                    list.Add(item);
                }
            }
            base.instancedMixins = list.ToArray();
            for (int j = 0; j < base.instancedMixins.Length; j++)
            {
                base.instancedMixins[j].instancedMixinID = j;
            }
        }

        public void Attach()
        {
            base.AttachToActor(this.caster);
        }

        public void Detach()
        {
            base.DetachFromActor(this.caster);
        }

        public float Evaluate(DynamicFloat dynamicFloat)
        {
            if (!dynamicFloat.isDynamic)
            {
                return dynamicFloat.fixedValue;
            }
            if (this.argumentRecieved && (this.config.UseAbilityArgumentAsSpecialKey == dynamicFloat.dynamicKey))
            {
                return this.argumentSpecialValue;
            }
            if (!this._overrideMap.ContainsKey(dynamicFloat.dynamicKey))
            {
                return (float) this.config.AbilitySpecials[dynamicFloat.dynamicKey];
            }
            object obj2 = this._overrideMap[dynamicFloat.dynamicKey];
            if (obj2 is SafeFloat)
            {
                return (float) ((SafeFloat) obj2);
            }
            return (float) obj2;
        }

        public int Evaluate(DynamicInt dynamicInt)
        {
            if (!dynamicInt.isDynamic)
            {
                return dynamicInt.fixedValue;
            }
            if (!this._overrideMap.ContainsKey(dynamicInt.dynamicKey))
            {
                return (int) ((float) this.config.AbilitySpecials[dynamicInt.dynamicKey]);
            }
            object obj2 = this._overrideMap[dynamicInt.dynamicKey];
            if (obj2 is SafeInt32)
            {
                return (int) ((SafeInt32) obj2);
            }
            if (obj2 is SafeFloat)
            {
                return (int) ((SafeFloat) obj2);
            }
            return (int) ((float) obj2);
        }

        public string Evaluate(DynamicString dynamicStr)
        {
            if (!dynamicStr.isDynamic)
            {
                return dynamicStr.fixedValue;
            }
            if (this._overrideMap.ContainsKey(dynamicStr.dynamicKey))
            {
                return (string) this._overrideMap[dynamicStr.dynamicKey];
            }
            return (string) this.config.AbilitySpecials[dynamicStr.dynamicKey];
        }

        public override string GetDebugContextName()
        {
            return this.config.AbilityName;
        }

        public override BaseAbilityActor GetDebugOwner()
        {
            return this.caster;
        }

        public float GetFloatParam(string key)
        {
            if (!this._overrideMap.ContainsKey(key))
            {
                return (float) this.config.AbilitySpecials[key];
            }
            object obj2 = this._overrideMap[key];
            if (obj2 is SafeFloat)
            {
                return (float) ((SafeFloat) obj2);
            }
            return (float) obj2;
        }

        public bool HasParam(string key)
        {
            return this._overrideMap.ContainsKey(key);
        }

        public void SetOverrideMapValue(string key, int value)
        {
            this._overrideMap[key] = value;
        }

        public void SetOverrideMapValue(string key, object value)
        {
            if (value is int)
            {
                this._overrideMap[key] = (int) value;
            }
            if (value is float)
            {
                this._overrideMap[key] = (float) value;
            }
            else
            {
                this._overrideMap[key] = value;
            }
        }

        public void SetOverrideMapValue(string key, float value)
        {
            this._overrideMap[key] = value;
        }

        public BaseEvent CurrentTriggerEvent
        {
            get
            {
                return this._currentTriggerEvent;
            }
            set
            {
                this._currentTriggerEvent = value;
            }
        }
    }
}

