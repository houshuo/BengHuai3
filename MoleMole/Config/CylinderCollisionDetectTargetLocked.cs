namespace MoleMole.Config
{
    using System;

    public class CylinderCollisionDetectTargetLocked : ConfigEntityAttackPattern
    {
        public float Height;
        public float Radius;
        public DynamicFloat RadiusRatio = DynamicFloat.ZERO;

        public CylinderCollisionDetectTargetLocked()
        {
            base.patternMethod = new Action<string, ConfigEntityAttackPattern, IAttacker, LayerMask>(AttackPattern.CylinderCollisionDetectTargetLockedAttack);
        }
    }
}

