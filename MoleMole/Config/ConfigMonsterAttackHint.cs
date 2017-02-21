namespace MoleMole.Config
{
    using System;

    public abstract class ConfigMonsterAttackHint
    {
        public float InnerInflateDuration = 0.9f;
        public float InnerStartDelay = 0.2f;
        public HintOffsetBase OffsetBase;

        protected ConfigMonsterAttackHint()
        {
        }
    }
}

