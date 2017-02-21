namespace MoleMole.Config
{
    using System;

    public abstract class ConfigEntityAttackPattern
    {
        [NonSerialized]
        public Action<string, ConfigEntityAttackPattern, IAttacker, LayerMask> patternMethod;

        protected ConfigEntityAttackPattern()
        {
        }
    }
}

