namespace MoleMole.Config
{
    using FullInspector;
    using System;

    public class SubEffect
    {
        [InspectorNullable]
        public EffectCreationOp onCreate;
        public string predicate;
        public string prefabPath;
    }
}

