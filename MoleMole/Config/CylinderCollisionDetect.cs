namespace MoleMole.Config
{
    using System;

    public class CylinderCollisionDetect : ConfigEntityAttackPattern
    {
        public float CenterZOffset;
        public float Height;
        public float Radius;

        public CylinderCollisionDetect()
        {
            base.patternMethod = new Action<string, ConfigEntityAttackPattern, IAttacker, LayerMask>(AttackPattern.CylinderCollisionDetectAttack);
        }
    }
}

