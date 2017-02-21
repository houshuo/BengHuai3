namespace MoleMole.Config
{
    using MoleMole;
    using System;

    [GeneratePartialHash(CombineGeneratedFile=true)]
    public class ApplyLevelBuff : ConfigAbilityAction, IHashable, IOnLoaded
    {
        public string AttachLevelEffectPattern;
        public AttachModifier[] AttachModifiers = new AttachModifier[0];
        public DynamicFloat Duration = DynamicFloat.ONE;
        public bool EnteringTimeSlow = true;
        public LevelBuffType LevelBuff;
        public bool LevelBuffAllowRefresh;
        public MoleMole.Config.LevelBuffSpecial LevelBuffSpecial;
        public bool NotStartEffect;
        public LevelBuffSide OverrideCurSide;
        public bool UseOverrideCurSide;

        public ApplyLevelBuff()
        {
            base.Target = AbilityTargetting.Other;
        }

        public override void Call(ActorAbilityPlugin abilityPlugin, ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
        {
            abilityPlugin.ApplyLevelBuffHandler(actionConfig, instancedAbility, instancedModifier, target, evt);
        }

        public override ConfigAbilityAction[][] GetAllSubActions()
        {
            ConfigAbilityAction[] actionArray = new ConfigAbilityAction[this.AttachModifiers.Length];
            for (int i = 0; i < actionArray.Length; i++)
            {
                actionArray[i] = this.AttachModifiers[i];
            }
            return new ConfigAbilityAction[][] { actionArray };
        }

        public override bool GetDebugOutput(ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt, ref string output)
        {
            object[] args = new object[] { Miscs.GetDebugActorName(instancedAbility.caster), this.LevelBuff, instancedAbility.Evaluate(this.Duration), this.AttachModifiers.Length };
            output = string.Format("{0} 触发LevelBuff {1}, 持续时间 {2}, 附带挂 Modifier 数量 {3}", args);
            return true;
        }

        public override MPActorAbilityPlugin.MPAuthorityActionHandler MPGetAuthorityHandler(MPActorAbilityPlugin mpAbilityPlugin)
        {
            return new MPActorAbilityPlugin.MPAuthorityActionHandler(mpAbilityPlugin.ApplyLevelBuff_AuthorityHandler);
        }

        public override MPActorAbilityPlugin.MPRemoteActionHandler MPGetRemoteHandler(MPActorAbilityPlugin mpAbilityPlugin)
        {
            return new MPActorAbilityPlugin.MPRemoteActionHandler(mpAbilityPlugin.ApplyLevelBuff_RemoteHandler);
        }

        public void ObjectContentHashOnto(ref int lastHash)
        {
            HashUtils.ContentHashOnto((int) this.LevelBuff, ref lastHash);
            HashUtils.ContentHashOnto((int) this.LevelBuffSpecial, ref lastHash);
            HashUtils.ContentHashOnto(this.LevelBuffAllowRefresh, ref lastHash);
            HashUtils.ContentHashOnto(this.EnteringTimeSlow, ref lastHash);
            if (this.Duration != null)
            {
                HashUtils.ContentHashOnto(this.Duration.isDynamic, ref lastHash);
                HashUtils.ContentHashOnto(this.Duration.fixedValue, ref lastHash);
                HashUtils.ContentHashOnto(this.Duration.dynamicKey, ref lastHash);
            }
            if (this.AttachModifiers != null)
            {
                foreach (AttachModifier modifier in this.AttachModifiers)
                {
                    if (modifier is IHashable)
                    {
                        HashUtils.ContentHashOnto(modifier, ref lastHash);
                    }
                }
            }
            HashUtils.ContentHashOnto(this.AttachLevelEffectPattern, ref lastHash);
            HashUtils.ContentHashOnto(this.UseOverrideCurSide, ref lastHash);
            HashUtils.ContentHashOnto((int) this.OverrideCurSide, ref lastHash);
            HashUtils.ContentHashOnto(this.NotStartEffect, ref lastHash);
            HashUtils.ContentHashOnto((int) base.Target, ref lastHash);
            if ((base.TargetOption != null) && (base.TargetOption.Range != null))
            {
                HashUtils.ContentHashOnto(base.TargetOption.Range.isDynamic, ref lastHash);
                HashUtils.ContentHashOnto(base.TargetOption.Range.fixedValue, ref lastHash);
                HashUtils.ContentHashOnto(base.TargetOption.Range.dynamicKey, ref lastHash);
            }
            if (base.Predicates != null)
            {
                foreach (ConfigAbilityPredicate predicate in base.Predicates)
                {
                    if (predicate is IHashable)
                    {
                        HashUtils.ContentHashOnto((IHashable) predicate, ref lastHash);
                    }
                }
            }
        }

        public override void OnLoaded()
        {
        }
    }
}

