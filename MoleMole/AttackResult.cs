namespace MoleMole
{
    using MoleMole.Config;
    using System;
    using System.Text;
    using UnityEngine;

    public class AttackResult
    {
        public float alienDamage;
        public float aniDamageRatio;
        public ConfigEntityCameraShake attackCameraShake;
        public AttackCategoryTag attackCategoryTag;
        public ConfigEntityAttackEffect attackEffectPattern;
        public ConfigEntityAttackEffect beHitEffectPattern;
        public float damage;
        public float fireDamage;
        public int frameHalt;
        public HitCollsion hitCollision;
        public AnimatorHitEffect hitEffect;
        public AnimatorHitEffectAux hitEffectAux;
        public HitEffectPattern hitEffectPattern;
        public ActorHitFlag hitFlag;
        public ActorHitLevel hitLevel;
        public ActorHitType hitType;
        public float iceDamage;
        public bool isAnimEventAttack = true;
        public bool isFromBullet;
        public bool isInComboCount = true;
        public KillEffect killEffect;
        public bool noTriggerEvadeAndDefend;
        public float plainDamage;
        public RejectType rejectState;
        public float retreatVelocity;
        public float thunderDamage;

        public void AddHitFlag(ActorHitFlag flag)
        {
            this.hitFlag |= flag;
        }

        public bool ContainHitFlag(ActorHitFlag flag)
        {
            return ((this.hitFlag & flag) != ActorHitFlag.None);
        }

        public string GetDebugOutput()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("\n--");
            builder.AppendFormat("伤害: {0}\n", this.damage);
            if (this.plainDamage > 0f)
            {
                builder.AppendFormat("元素伤害(无属性): {0}\n", this.plainDamage);
            }
            if (this.fireDamage > 0f)
            {
                builder.AppendFormat("元素伤害(火属性): {0}\n", this.fireDamage);
            }
            if (this.thunderDamage > 0f)
            {
                builder.AppendFormat("元素伤害(雷属性): {0}\n", this.thunderDamage);
            }
            if (this.iceDamage > 0f)
            {
                builder.AppendFormat("元素伤害(冰属性): {0}\n", this.iceDamage);
            }
            if (this.alienDamage > 0f)
            {
                builder.AppendFormat("元素伤害(异能属性): {0}\n", this.alienDamage);
            }
            if (this.GetTotalDamage() > this.damage)
            {
                builder.AppendFormat("总伤害: {0}\n", this.GetTotalDamage());
            }
            if (this.retreatVelocity > 0f)
            {
                builder.AppendFormat("击退: {0}\n", this.retreatVelocity);
            }
            if (this.hitLevel != ActorHitLevel.Normal)
            {
                builder.AppendFormat("攻击等级: {0}\n", this.hitLevel);
            }
            if (this.hitEffect != AnimatorHitEffect.Normal)
            {
                builder.AppendFormat("特殊 HitEffect: {0}\n", this.hitEffect);
            }
            if (this.hitEffectAux != AnimatorHitEffectAux.Normal)
            {
                builder.AppendFormat("特殊 HitEffectAux: {0}\n", this.hitEffectAux);
            }
            if (this.killEffect != KillEffect.KillNow)
            {
                builder.AppendFormat("特殊 KillEffect: {0}\n", this.killEffect);
            }
            if (!this.isAnimEventAttack)
            {
                builder.AppendFormat("!(内在攻击)\n", new object[0]);
            }
            if (!this.isInComboCount)
            {
                builder.AppendFormat("!(不算Combo)\n", new object[0]);
            }
            if (this.rejected)
            {
                builder.AppendFormat("!(无效)\n", new object[0]);
            }
            builder.AppendLine("--\n");
            return builder.ToString();
        }

        public float GetElementalDamage()
        {
            return ((((this.plainDamage + this.fireDamage) + this.thunderDamage) + this.iceDamage) + this.alienDamage);
        }

        public float GetTotalDamage()
        {
            return (((((this.damage + this.plainDamage) + this.fireDamage) + this.thunderDamage) + this.iceDamage) + this.alienDamage);
        }

        public bool rejected
        {
            get
            {
                return (this.rejectState > RejectType.Normal);
            }
        }

        [Flags]
        public enum ActorHitFlag
        {
            Count = 1,
            None = 0,
            ShieldBroken = 1
        }

        public enum ActorHitLevel
        {
            Normal,
            Mute,
            Critical
        }

        public enum ActorHitType
        {
            Melee,
            Ranged,
            Ailment
        }

        public enum AnimatorHitEffect
        {
            Mute,
            Light,
            Normal,
            FaceAttacker,
            ThrowUp,
            ThrowDown,
            KnockDown,
            ThrowUpBlow,
            ThrowBlow,
            ThrowAirBlow
        }

        public enum AnimatorHitEffectAux
        {
            Normal,
            HitLeft,
            HitRight,
            HitUpper,
            HitCentered,
            HitBack,
            HitOnLeft,
            HitOnRight
        }

        [Flags]
        public enum AttackCategoryTag
        {
            Branch = 2,
            Charge = 4,
            Defend = 0x100,
            Evade = 0x80,
            None = 0,
            Normal = 1,
            QTE = 0x40,
            SwitchIn = 0x20,
            Ultra = 8,
            Weapon = 0x10
        }

        public enum ElementType
        {
            Plain,
            Fire,
            Thunder,
            Ice,
            Alien
        }

        public class HitCollsion
        {
            public Vector3 hitDir;
            public Vector3 hitPoint;
        }

        public enum HitEffectPattern
        {
            Mute,
            OnlyAttack,
            OnlyBeHit,
            Normal
        }

        public enum RejectType
        {
            Normal,
            RejectAll,
            RejectButShowAttackEffect
        }
    }
}

