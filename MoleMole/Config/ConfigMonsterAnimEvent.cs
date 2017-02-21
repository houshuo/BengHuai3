namespace MoleMole.Config
{
    using FullInspector;

    public class ConfigMonsterAnimEvent : ConfigEntityAnimEvent
    {
        [InspectorNullable]
        public ConfigMonsterAttackHint AttackHint;
        [InspectorNullable]
        public ConfigEntityPhysicsProperty PhysicsProperty;
    }
}

