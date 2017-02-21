namespace MoleMole.Config
{
    using System;

    public class ConfigEntityCommonArguments
    {
        public float CameraMinAngleRatio = 1f;
        public EntityClass Class;
        public float CollisionLevel;
        public float CollisionRadius = 0.5f;
        public float CreateCollisionHeight;
        public float CreateCollisionRadius;
        public float CreatePosYOffset;
        public string DefaultAnimEventPredicate = "NormalMode";
        public string[] EffectPredicates = Miscs.EMPTY_STRINGS;
        public bool HasLowPrefab;
        public EntityNature Nature;
        public string[] PreloadEffectPatternGroups = Miscs.EMPTY_STRINGS;
        public string[] RequestSoundBankNames = Miscs.EMPTY_STRINGS;
        public EntityRoleName RoleName;
    }
}

