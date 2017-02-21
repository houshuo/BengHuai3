namespace MoleMole.Config
{
    using System;

    public class ConfigMonsterCommonArguments : ConfigEntityCommonArguments
    {
        public float Attack;
        public float BePushedSpeedRatio;
        public float BePushedSpeedRatioThrow;
        public float Defence;
        public float? FadeInHeight;
        public float HitboxInactiveDelay = 0.45f;
        public float HP;
        public bool UseEliteShader = true;
        public bool UseSwitchShader = true;
        public float UseTransparentShaderDistanceThreshold = 2.1f;
    }
}

