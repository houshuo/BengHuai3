namespace MoleMole.Config
{
    using MoleMole;
    using System;

    [GeneratePartialHash(CombineGeneratedFile=true)]
    public class PositionDirectionHitMixin : ConfigAbilityMixin, IHashable
    {
        public ConfigAbilityAction[] Actions;
        public string[] AnimEventIDs;
        public DynamicFloat BackDamageRatio;
        public DynamicFloat ForwardAngleRangeMax;
        public DynamicFloat ForwardAngleRangeMin;
        public DynamicFloat HitRange;
        public DynamicFloat PosAngleRangeMax;
        public DynamicFloat PosAngleRangeMin;

        public PositionDirectionHitMixin()
        {
            DynamicFloat num = new DynamicFloat {
                fixedValue = 180f
            };
            this.ForwardAngleRangeMax = num;
            this.ForwardAngleRangeMin = DynamicFloat.ZERO;
            this.BackDamageRatio = DynamicFloat.ONE;
            this.PosAngleRangeMin = DynamicFloat.ZERO;
            num = new DynamicFloat {
                fixedValue = 180f
            };
            this.PosAngleRangeMax = num;
            this.HitRange = DynamicFloat.ZERO;
            this.Actions = ConfigAbilityAction.EMPTY;
        }

        public override BaseAbilityMixin CreateInstancedMixin(ActorAbility instancedAbility, ActorModifier instancedModifier)
        {
            return new AbilityPositionDirectionHitMixin(instancedAbility, instancedModifier, this);
        }

        public void ObjectContentHashOnto(ref int lastHash)
        {
            if (this.AnimEventIDs != null)
            {
                foreach (string str in this.AnimEventIDs)
                {
                    HashUtils.ContentHashOnto(str, ref lastHash);
                }
            }
            if (this.ForwardAngleRangeMax != null)
            {
                HashUtils.ContentHashOnto(this.ForwardAngleRangeMax.isDynamic, ref lastHash);
                HashUtils.ContentHashOnto(this.ForwardAngleRangeMax.fixedValue, ref lastHash);
                HashUtils.ContentHashOnto(this.ForwardAngleRangeMax.dynamicKey, ref lastHash);
            }
            if (this.ForwardAngleRangeMin != null)
            {
                HashUtils.ContentHashOnto(this.ForwardAngleRangeMin.isDynamic, ref lastHash);
                HashUtils.ContentHashOnto(this.ForwardAngleRangeMin.fixedValue, ref lastHash);
                HashUtils.ContentHashOnto(this.ForwardAngleRangeMin.dynamicKey, ref lastHash);
            }
            if (this.BackDamageRatio != null)
            {
                HashUtils.ContentHashOnto(this.BackDamageRatio.isDynamic, ref lastHash);
                HashUtils.ContentHashOnto(this.BackDamageRatio.fixedValue, ref lastHash);
                HashUtils.ContentHashOnto(this.BackDamageRatio.dynamicKey, ref lastHash);
            }
            if (this.PosAngleRangeMin != null)
            {
                HashUtils.ContentHashOnto(this.PosAngleRangeMin.isDynamic, ref lastHash);
                HashUtils.ContentHashOnto(this.PosAngleRangeMin.fixedValue, ref lastHash);
                HashUtils.ContentHashOnto(this.PosAngleRangeMin.dynamicKey, ref lastHash);
            }
            if (this.PosAngleRangeMax != null)
            {
                HashUtils.ContentHashOnto(this.PosAngleRangeMax.isDynamic, ref lastHash);
                HashUtils.ContentHashOnto(this.PosAngleRangeMax.fixedValue, ref lastHash);
                HashUtils.ContentHashOnto(this.PosAngleRangeMax.dynamicKey, ref lastHash);
            }
            if (this.HitRange != null)
            {
                HashUtils.ContentHashOnto(this.HitRange.isDynamic, ref lastHash);
                HashUtils.ContentHashOnto(this.HitRange.fixedValue, ref lastHash);
                HashUtils.ContentHashOnto(this.HitRange.dynamicKey, ref lastHash);
            }
            if (this.Actions != null)
            {
                foreach (ConfigAbilityAction action in this.Actions)
                {
                    if (action is IHashable)
                    {
                        HashUtils.ContentHashOnto((IHashable) action, ref lastHash);
                    }
                }
            }
        }
    }
}

