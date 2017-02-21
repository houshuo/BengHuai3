namespace MoleMole.Config
{
    using MoleMole;
    using System;

    [GeneratePartialHash(CombineGeneratedFile=true)]
    public class TriggerFieldExMixin : ConfigAbilityMixin, IHashable
    {
        public bool ApplyAttackerWitchTimeRatio;
        public float CreateEffectDelay;
        public MixinEffect CreationEffect;
        public DynamicFloat CreationXOffset = DynamicFloat.ZERO;
        public DynamicFloat CreationZOffset = DynamicFloat.ZERO;
        public bool DestoryAfterSwitch;
        public MixinEffect DestroyEffect;
        public DynamicFloat Duration;
        public MixinEffect DurationEffect;
        public bool Follow;
        public bool IncludeSelf;
        public DynamicFloat NoAttackTargetZOffset = DynamicFloat.ZERO;
        public ConfigAbilityAction[] OnDestroyCasterActions = ConfigAbilityAction.EMPTY;
        public ConfigAbilityAction[] OnDestroyTargetActions = ConfigAbilityAction.EMPTY;
        public ConfigAbilityAction[] OnStartCasterActions = ConfigAbilityAction.EMPTY;
        public DynamicFloat Radius;
        public string[] TargetModifiers = Miscs.EMPTY_STRINGS;
        public MixinTargetting Targetting = MixinTargetting.Enemy;
        public bool TriggerOnAdded;
        public PositionType TriggerPositionType = PositionType.Target;

        public override BaseAbilityMixin CreateInstancedMixin(ActorAbility instancedAbility, ActorModifier instancedModifier)
        {
            return new AbilityTriggerFieldExMixin(instancedAbility, instancedModifier, this);
        }

        public void ObjectContentHashOnto(ref int lastHash)
        {
            if (this.CreationEffect != null)
            {
                HashUtils.ContentHashOnto(this.CreationEffect.EffectPattern, ref lastHash);
                HashUtils.ContentHashOnto(this.CreationEffect.AudioPattern, ref lastHash);
            }
            HashUtils.ContentHashOnto(this.CreateEffectDelay, ref lastHash);
            if (this.DurationEffect != null)
            {
                HashUtils.ContentHashOnto(this.DurationEffect.EffectPattern, ref lastHash);
                HashUtils.ContentHashOnto(this.DurationEffect.AudioPattern, ref lastHash);
            }
            if (this.DestroyEffect != null)
            {
                HashUtils.ContentHashOnto(this.DestroyEffect.EffectPattern, ref lastHash);
                HashUtils.ContentHashOnto(this.DestroyEffect.AudioPattern, ref lastHash);
            }
            HashUtils.ContentHashOnto(this.DestoryAfterSwitch, ref lastHash);
            HashUtils.ContentHashOnto((int) this.TriggerPositionType, ref lastHash);
            if (this.NoAttackTargetZOffset != null)
            {
                HashUtils.ContentHashOnto(this.NoAttackTargetZOffset.isDynamic, ref lastHash);
                HashUtils.ContentHashOnto(this.NoAttackTargetZOffset.fixedValue, ref lastHash);
                HashUtils.ContentHashOnto(this.NoAttackTargetZOffset.dynamicKey, ref lastHash);
            }
            if (this.CreationZOffset != null)
            {
                HashUtils.ContentHashOnto(this.CreationZOffset.isDynamic, ref lastHash);
                HashUtils.ContentHashOnto(this.CreationZOffset.fixedValue, ref lastHash);
                HashUtils.ContentHashOnto(this.CreationZOffset.dynamicKey, ref lastHash);
            }
            if (this.CreationXOffset != null)
            {
                HashUtils.ContentHashOnto(this.CreationXOffset.isDynamic, ref lastHash);
                HashUtils.ContentHashOnto(this.CreationXOffset.fixedValue, ref lastHash);
                HashUtils.ContentHashOnto(this.CreationXOffset.dynamicKey, ref lastHash);
            }
            if (this.Radius != null)
            {
                HashUtils.ContentHashOnto(this.Radius.isDynamic, ref lastHash);
                HashUtils.ContentHashOnto(this.Radius.fixedValue, ref lastHash);
                HashUtils.ContentHashOnto(this.Radius.dynamicKey, ref lastHash);
            }
            HashUtils.ContentHashOnto((int) this.Targetting, ref lastHash);
            if (this.Duration != null)
            {
                HashUtils.ContentHashOnto(this.Duration.isDynamic, ref lastHash);
                HashUtils.ContentHashOnto(this.Duration.fixedValue, ref lastHash);
                HashUtils.ContentHashOnto(this.Duration.dynamicKey, ref lastHash);
            }
            HashUtils.ContentHashOnto(this.Follow, ref lastHash);
            HashUtils.ContentHashOnto(this.IncludeSelf, ref lastHash);
            HashUtils.ContentHashOnto(this.TriggerOnAdded, ref lastHash);
            HashUtils.ContentHashOnto(this.ApplyAttackerWitchTimeRatio, ref lastHash);
            if (this.TargetModifiers != null)
            {
                foreach (string str in this.TargetModifiers)
                {
                    HashUtils.ContentHashOnto(str, ref lastHash);
                }
            }
            if (this.OnStartCasterActions != null)
            {
                foreach (ConfigAbilityAction action in this.OnStartCasterActions)
                {
                    if (action is IHashable)
                    {
                        HashUtils.ContentHashOnto((IHashable) action, ref lastHash);
                    }
                }
            }
            if (this.OnDestroyCasterActions != null)
            {
                foreach (ConfigAbilityAction action2 in this.OnDestroyCasterActions)
                {
                    if (action2 is IHashable)
                    {
                        HashUtils.ContentHashOnto((IHashable) action2, ref lastHash);
                    }
                }
            }
            if (this.OnDestroyTargetActions != null)
            {
                foreach (ConfigAbilityAction action3 in this.OnDestroyTargetActions)
                {
                    if (action3 is IHashable)
                    {
                        HashUtils.ContentHashOnto((IHashable) action3, ref lastHash);
                    }
                }
            }
        }

        public enum PositionType
        {
            Caster,
            AttackTarget,
            Target
        }
    }
}

