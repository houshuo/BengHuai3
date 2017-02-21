namespace MoleMole.Config
{
    using System;

    public class FanCollisionWithHeightDetect : FanCollisionDetect
    {
        public float Height;

        public FanCollisionWithHeightDetect()
        {
            base.patternMethod = new Action<string, ConfigEntityAttackPattern, IAttacker, LayerMask>(ComplexAttackPattern.FanCollisionWithHeightDetectAttack);
        }
    }
}

