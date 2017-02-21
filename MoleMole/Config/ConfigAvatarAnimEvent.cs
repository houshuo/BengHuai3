namespace MoleMole.Config
{
    using FullInspector;

    public class ConfigAvatarAnimEvent : ConfigEntityAnimEvent
    {
        [InspectorNullable]
        public ConfigAvatarCameraAction CameraAction;
        [InspectorNullable]
        public ConfigLastKillCameraAnimation LastKillCameraAnimation;
        [InspectorNullable]
        public ConfigMissionSpecificKill MissionSpecificKill;
        [InspectorNullable]
        public ConfigAvatarPhysicsProperty PhysicsProperty;
        [InspectorNullable]
        public ConfigWitchTimeResume WitchTimeResume;
    }
}

