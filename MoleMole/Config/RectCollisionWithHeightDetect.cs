namespace MoleMole.Config
{
    using System;

    public class RectCollisionWithHeightDetect : RectCollisionDetect
    {
        public float Height;

        public RectCollisionWithHeightDetect()
        {
            base.patternMethod = new Action<string, ConfigEntityAttackPattern, IAttacker, LayerMask>(ComplexAttackPattern.RectCollisionWithHeightDetectAttack);
        }
    }
}

