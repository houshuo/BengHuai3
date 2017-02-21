namespace MoleMole.Config
{
    using System;

    public class StaticHitBoxDetect : RectCollisionDetect
    {
        public bool Enable = true;
        public float LengthRatio = 1f;
        public HitBoxResetType ResetType;
        public float SizeRatio = 1f;
        public bool UseOwnerCenterForRetreatDirection;

        public StaticHitBoxDetect()
        {
            base.patternMethod = new Action<string, ConfigEntityAttackPattern, IAttacker, LayerMask>(ComplexAttackPattern.StaticHitBoxDetectAttack);
        }

        public enum HitBoxResetType
        {
            None,
            WithInside,
            WithoutInside
        }
    }
}

