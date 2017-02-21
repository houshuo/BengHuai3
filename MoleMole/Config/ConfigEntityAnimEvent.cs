namespace MoleMole.Config
{
    using FullInspector;
    using System;

    public abstract class ConfigEntityAnimEvent
    {
        [InspectorNullable]
        public ConfigEntityAttackEffect AttackEffect;
        [InspectorNullable]
        public ConfigEntityAttackPattern AttackPattern;
        [InspectorNullable]
        public ConfigEntityAttackProperty AttackProperty;
        [InspectorNullable]
        public ConfigEntityCameraShake CameraShake;
        public string Predicate = "Always";
        public string Predicate2 = "Always";
        [InspectorNullable]
        public ConfigTimeSlow TimeSlow;
        [InspectorNullable]
        public ConfigEntityTriggerAbility TriggerAbility;
        [InspectorNullable]
        public ConfigTriggerEffectPattern TriggerEffectPattern;
        [InspectorNullable]
        public ConfigTintCamera TriggerTintCamera;

        protected ConfigEntityAnimEvent()
        {
        }
    }
}

