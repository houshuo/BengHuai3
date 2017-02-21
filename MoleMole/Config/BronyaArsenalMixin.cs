namespace MoleMole.Config
{
    using MoleMole;
    using System;

    [GeneratePartialHash(CombineGeneratedFile=true)]
    public class BronyaArsenalMixin : ConfigAbilityMixin, IHashable
    {
        public string CannonEffects;
        public string ChargeEffects;
        public float ChargeTime = 3f;
        public float ClearTime = 3f;
        public float[] DelayList;
        public string ExplodeAnimEventID;
        public string ExplodeEffects;
        public float ExplodeRadius = 2f;
        public float FireIntervial = 0.1f;
        public float FireTime = 3f;
        public string HintEffects;
        public string PositionsEffect;
        public string ShakeAnimEventID;
        public string ShootEffects;

        public override BaseAbilityMixin CreateInstancedMixin(ActorAbility instancedAbility, ActorModifier instancedModifier)
        {
            return new AbilityBronyaArsenalMixin(instancedAbility, instancedModifier, this);
        }

        public void ObjectContentHashOnto(ref int lastHash)
        {
            HashUtils.ContentHashOnto(this.PositionsEffect, ref lastHash);
            if (this.DelayList != null)
            {
                foreach (float num in this.DelayList)
                {
                    HashUtils.ContentHashOnto(num, ref lastHash);
                }
            }
            HashUtils.ContentHashOnto(this.CannonEffects, ref lastHash);
            HashUtils.ContentHashOnto(this.ChargeEffects, ref lastHash);
            HashUtils.ContentHashOnto(this.HintEffects, ref lastHash);
            HashUtils.ContentHashOnto(this.ShootEffects, ref lastHash);
            HashUtils.ContentHashOnto(this.ExplodeEffects, ref lastHash);
            HashUtils.ContentHashOnto(this.ChargeTime, ref lastHash);
            HashUtils.ContentHashOnto(this.FireTime, ref lastHash);
            HashUtils.ContentHashOnto(this.ClearTime, ref lastHash);
            HashUtils.ContentHashOnto(this.FireIntervial, ref lastHash);
            HashUtils.ContentHashOnto(this.ExplodeAnimEventID, ref lastHash);
            HashUtils.ContentHashOnto(this.ExplodeRadius, ref lastHash);
            HashUtils.ContentHashOnto(this.ShakeAnimEventID, ref lastHash);
        }
    }
}

