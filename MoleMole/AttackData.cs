namespace MoleMole
{
    using MoleMole.Config;
    using System;

    public class AttackData : AttackResult
    {
        public float addedAttackerAlienDamageRatio;
        public float addedAttackerFireDamageRatio;
        public float addedAttackerIceDamageRatio;
        public float addedAttackerNormalDamageRatio;
        public float addedAttackerThunderDamageRatio;
        public float addedAttackRatio;
        public float addedDamageRatio;
        public float attackeeAddedDamageTakeRatio;
        public float attackeeAniDefenceRatio;
        public EntityClass attackeeClass;
        public EntityNature attackeeNature;
        public float attackerAddedAllDamageReduceRatio;
        public float attackerAddedAttackValue;
        public float attackerAlienDamage;
        public float attackerAlienDamagePercentage;
        public float attackerAniDamageRatio;
        public float attackerAttackPercentage;
        public float attackerAttackValue;
        public ushort attackerCategory;
        public EntityClass attackerClass;
        public float attackerCritChance;
        public float attackerCritDamageRatio;
        public float attackerFireDamage;
        public float attackerFireDamagePercentage;
        public float attackerIceDamage;
        public float attackerIceDamagePercentage;
        public int attackerLevel;
        public EntityNature attackerNature;
        public float attackerNormalDamage;
        public float attackerNormalDamagePercentage;
        public float attackerShieldDamageDelta;
        public float attackerShieldDamageRatio;
        public float attackerThunderDamage;
        public float attackerThunderDamagePercentage;
        public float natureDamageRatio;
        public int noBreakFrameHaltAdd;
        public AttackDataStep resolveStep;

        public AttackData Clone()
        {
            return (AttackData) base.MemberwiseClone();
        }

        public bool IsFinalResolved()
        {
            return (this.resolveStep == AttackDataStep.FinalResolved);
        }

        public void Reject(AttackResult.RejectType rejectType)
        {
            this.resolveStep = AttackDataStep.FinalResolved;
            base.rejectState = rejectType;
        }

        public enum AttackDataStep
        {
            AttackerResolved,
            AttackeeResolved,
            FinalResolved
        }
    }
}

