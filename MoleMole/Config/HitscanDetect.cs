namespace MoleMole.Config
{
    using System;

    public class HitscanDetect : ConfigEntityAttackPattern
    {
        public float CenterYOffset;
        public float MaxHitDistance;
        public float OffsetZ;

        public HitscanDetect()
        {
            base.patternMethod = new Action<string, ConfigEntityAttackPattern, IAttacker, LayerMask>(ComplexAttackPattern.HitscanDetectAttack);
        }
    }
}

