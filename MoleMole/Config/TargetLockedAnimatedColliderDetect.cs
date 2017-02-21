namespace MoleMole.Config
{
    using System;

    public class TargetLockedAnimatedColliderDetect : ConfigEntityAttackPattern
    {
        public bool brokenEnemyDragged;
        public string ColliderEntryName;
        public bool DestroyOnOwnerBeHitCanceled;
        public bool dontDestroyWhenEvade;
        public bool LockX;
        public float MaxLockDistance;
        public float ScatteringDistance;
        public bool StopOnFirstContact;

        public TargetLockedAnimatedColliderDetect()
        {
            base.patternMethod = new Action<string, ConfigEntityAttackPattern, IAttacker, LayerMask>(ComplexAttackPattern.TargetLockedAnimatedColliderDetectAttack);
        }
    }
}

