namespace MoleMole
{
    using System;

    public class AvatarStastics
    {
        public SafeFloat avatarActiveWeaponSkillDamage = 0f;
        public SafeInt32 avatarActiveWeaponSkillTimes = 0;
        public SafeFloat avatarBeDamaged = 0f;
        public SafeInt32 avatarBeingBreakTimes = 0;
        public SafeInt32 avatarBeingHitTimes = 0;
        public SafeInt32 avatarBreakTimes = 0;
        public SafeFloat avatarDamage = 0f;
        public SafeInt32 avatarEffectHitTimes = 0;
        public SafeInt32 avatarEvadeEffectTimes = 0;
        public SafeInt32 avatarEvadeSuccessTimes = 0;
        public SafeInt32 avatarEvadeTimes = 0;
        public SafeInt32 avatarHitTimes = 0;
        public SafeInt32 avatarID = 0;
        public SafeInt32 avatarLevel = 1;
        public SafeInt32 avatarSkill01Times = 0;
        public SafeInt32 avatarSkill02Times = 0;
        public SafeInt32 avatarSpecialAttackTimes = 0;
        public SafeInt32 avatarStar = 1;
        public SafeFloat battleTime = 0f;
        public SafeFloat behitCriticalDamageMax = 0f;
        public SafeFloat behitNormalDamageMax = 0f;
        public SafeFloat beRestrictedDamage = 0f;
        public SafeFloat beRestrictedDamageRatio = 0f;
        public SafeFloat comboMax = 0f;
        public SafeFloat dps = 0f;
        public SafeFloat hitCriticalDamageMax = 0f;
        public SafeFloat hitNormalDamageMax = 0f;
        public SafeFloat hpBegin = 0f;
        public SafeFloat hpGain = 0f;
        public SafeFloat hpMax = 0f;
        public SafeBool isAlive = 1;
        public SafeBool isOnStage = 0;
        public SafeFloat normalDamage = 0f;
        public SafeFloat normalDamageRatio = 0f;
        public SafeFloat onStageTime = 0f;
        public SafeFloat restrictionDamage = 0f;
        public SafeFloat restrictionDamageRatio = 0f;
        public static int SELF_SP_RECOVE_UPBOUND = 10;
        public SafeFloat selfSPRecover = 0f;
        public SafeFloat spBegin = 0f;
        public SafeFloat spMax = 0f;
        public SafeFloat SpRecover = 0f;
        public SafeFloat spUse = 0f;
        public SafeInt32 stageID = 0;
        public SafeInt32 swapInTimes = 0;
        public SafeInt32 swapOutTimes = 0;

        public AvatarStastics(int avatarID)
        {
            this.avatarID = avatarID;
        }
    }
}

