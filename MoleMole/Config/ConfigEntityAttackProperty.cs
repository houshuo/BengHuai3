namespace MoleMole.Config
{
    using FullInspector;
    using MoleMole;
    using System;
    using System.Text;

    public class ConfigEntityAttackProperty : IOnLoaded
    {
        public float AddedDamageValue;
        public float AlienDamage;
        public float AlienDamagePercentage;
        public float AniDamageRatio;
        public MixinTargetting AttackTargetting = MixinTargetting.Enemy;
        [InspectorNullable]
        public AttackResult.AttackCategoryTag[] CategoryTag;
        [NonSerialized]
        public AttackResult.AttackCategoryTag CategoryTagCombined;
        public float DamagePercentage;
        public float FireDamage;
        public float FireDamagePercentage;
        public int FrameHalt;
        public AttackResult.AnimatorHitEffect HitEffect = AttackResult.AnimatorHitEffect.Normal;
        public AttackResult.AnimatorHitEffectAux HitEffectAux;
        public AttackResult.ActorHitType HitType;
        public float IceDamage;
        public float IceDamagePercentage;
        public bool IsAnimEventAttack = true;
        public bool IsInComboCount = true;
        public MoleMole.Config.KillEffect KillEffect;
        public int NoBreakFrameHaltAdd = 2;
        public float NormalDamage;
        public float NormalDamagePercentage;
        public bool NoTriggerEvadeAndDefend;
        public float RetreatVelocity;
        public float SPRecover;
        public float ThunderDamage;
        public float ThunderDamagePercentage;
        public float WitchTimeRatio = 1f;

        public string GetDebugOutput()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("\n--");
            builder.AppendFormat("攻击力百分比: {0}\n", this.DamagePercentage);
            if (this.AddedDamageValue > 0f)
            {
                builder.AppendFormat("额外基础伤害: {0}\n", this.AddedDamageValue);
            }
            if (this.NormalDamage > 0f)
            {
                builder.AppendFormat("元素伤害(无属性): {0}\n", this.NormalDamage);
            }
            if (this.FireDamage > 0f)
            {
                builder.AppendFormat("元素伤害(火属性): {0}\n", this.FireDamage);
            }
            if (this.ThunderDamage > 0f)
            {
                builder.AppendFormat("元素伤害(雷属性): {0}\n", this.ThunderDamage);
            }
            if (this.AlienDamage > 0f)
            {
                builder.AppendFormat("元素伤害(异能属性): {0}\n", this.AlienDamage);
            }
            if (this.RetreatVelocity > 0f)
            {
                builder.AppendFormat("击退: {0}\n", this.RetreatVelocity);
            }
            if (this.HitEffect != AttackResult.AnimatorHitEffect.Normal)
            {
                builder.AppendFormat("特殊 HitEffect: {0}\n", this.HitEffect);
            }
            if (this.KillEffect != MoleMole.Config.KillEffect.KillNow)
            {
                builder.AppendFormat("特殊 KillEffect: {0}\n", this.KillEffect);
            }
            if (!this.IsAnimEventAttack)
            {
                builder.AppendFormat("!(内在攻击)\n", new object[0]);
            }
            if (!this.IsInComboCount)
            {
                builder.AppendFormat("!(不算Combo)\n", new object[0]);
            }
            builder.AppendLine("--\n");
            return builder.ToString();
        }

        public void OnLoaded()
        {
            if (this.CategoryTag != null)
            {
                for (int i = 0; i < this.CategoryTag.Length; i++)
                {
                    this.CategoryTagCombined |= this.CategoryTag[i];
                }
            }
        }
    }
}

