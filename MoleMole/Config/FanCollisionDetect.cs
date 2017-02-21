namespace MoleMole.Config
{
    using System;

    public class FanCollisionDetect : ConfigEntityAttackPattern
    {
        public float CenterYOffset;
        public float FanAngle;
        public bool FollowRootNodeY;
        public float MeleeFanAngle;
        public float MeleeRadius;
        public float OffsetZ;
        public float Radius;
        public DynamicFloat RadiusRatio = DynamicFloat.ZERO;

        public FanCollisionDetect()
        {
            base.patternMethod = new Action<string, ConfigEntityAttackPattern, IAttacker, LayerMask>(AttackPattern.FanCollisionDetectAttack);
        }
    }
}

