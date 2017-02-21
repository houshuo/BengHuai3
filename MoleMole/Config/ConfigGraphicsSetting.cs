namespace MoleMole.Config
{
    using System;
    using System.Collections.Generic;

    public class ConfigGraphicsSetting
    {
        public float ContrastDelta;
        [NonSerialized]
        public Dictionary<PostEffectQualityGrade, int> PostFxGradeBufferSize = new Dictionary<PostEffectQualityGrade, int>();
        public int RecommendResolutionX;
        public int RecommendResolutionY;
        [NonSerialized]
        public Dictionary<ResolutionQualityGrade, int> ResolutionPercentage = new Dictionary<ResolutionQualityGrade, int>();
        public ResolutionQualityGrade ResolutionQuality;
        public int TargetFrameRate;
        public ConfigGraphicsVolatileSetting VolatileSetting;
    }
}

