namespace MoleMole.Config
{
    using System;

    public class AnimatedColliderDetect : ConfigEntityAttackPattern
    {
        public bool brokenEnemyDragged;
        public string ColliderEntryName;
        public bool DestroyOnHitWall;
        public bool DestroyOnOwnerBeHitCanceled;
        public bool dontDestroyWhenEvade;
        public bool EnableHitWallStop;
        public bool Follow;
        public string FollowAttachPoint;
        public bool FollowOwnerTimeScale;
        public string HitWallDestroyEffect;
        public bool IgnoreTimeScale;

        public AnimatedColliderDetect()
        {
            base.patternMethod = new Action<string, ConfigEntityAttackPattern, IAttacker, LayerMask>(ComplexAttackPattern.AnimatedColliderDetectAttack);
        }
    }
}

