namespace MoleMole.Config
{
    using System;

    public class RectCollisionDetect : ConfigEntityAttackPattern
    {
        public float CenterYOffset;
        public float Distance;
        public float OffsetZ;
        public float Width;

        public RectCollisionDetect()
        {
            base.patternMethod = new Action<string, ConfigEntityAttackPattern, IAttacker, LayerMask>(AttackPattern.RectCollisionDetectAttack);
        }
    }
}

