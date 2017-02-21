namespace MoleMole
{
    using FullInspector;
    using MoleMole.Config;
    using System;
    using System.Collections.Generic;

    public class ActorModifier : BaseActorActionContext
    {
        [InspectorCollapsedFoldout]
        public ConfigAbilityModifier config;
        public int instancedModifierID;
        [InspectorCollapsedFoldout]
        public BaseAbilityActor owner;
        [InspectorCollapsedFoldout]
        public ActorAbility parentAbility;
        public int[] stackIndices;

        public ActorModifier(ActorAbility parentAbility, BaseAbilityActor owner, ConfigAbilityModifier config)
        {
            this.parentAbility = parentAbility;
            this.config = config;
            this.owner = owner;
            List<BaseAbilityMixin> list = new List<BaseAbilityMixin>();
            for (int i = 0; i < config.ModifierMixins.Length; i++)
            {
                BaseAbilityMixin item = owner.abilityPlugin.CreateInstancedAbilityMixin(parentAbility, this, config.ModifierMixins[i]);
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
            this.stackIndices = new int[config.Properties.Count + config.EntityProperties.Count];
        }

        public void Attach()
        {
            for (int i = 0; i < this.config.Properties.Count; i++)
            {
                string propertyKey = this.config.Properties.Keys[i];
                float num2 = this.parentAbility.Evaluate(this.config.Properties.Values[i]);
                this.stackIndices[i] = this.owner.PushProperty(propertyKey, num2);
            }
            int count = this.config.Properties.Count;
            for (int j = 0; j < this.config.EntityProperties.Count; j++)
            {
                string str2 = this.config.EntityProperties.Keys[j];
                float num5 = this.parentAbility.Evaluate(this.config.EntityProperties.Values[j]);
                this.stackIndices[count + j] = this.owner.PushProperty(str2, num5);
            }
            if (this.config.State != AbilityState.None)
            {
                this.owner.AddAbilityState(this.config.State, this.config.MuteStateDisplayEffect);
            }
            base.AttachToActor(this.owner);
        }

        public void Detach()
        {
            for (int i = 0; i < this.config.Properties.Count; i++)
            {
                string propertyKey = this.config.Properties.Keys[i];
                this.owner.PopProperty(propertyKey, this.stackIndices[i]);
            }
            int count = this.config.Properties.Count;
            for (int j = 0; j < this.config.EntityProperties.Count; j++)
            {
                string str2 = this.config.EntityProperties.Keys[j];
                this.owner.PopProperty(str2, this.stackIndices[count + j]);
            }
            if (this.config.State != AbilityState.None)
            {
                this.owner.RemoveAbilityState(this.config.State);
            }
            base.DetachFromActor(this.owner);
        }

        public override string GetDebugContextName()
        {
            return string.Format("{0} -> {1}", this.parentAbility.config.AbilityName, this.config.ModifierName);
        }

        public override BaseAbilityActor GetDebugOwner()
        {
            return this.owner;
        }
    }
}

