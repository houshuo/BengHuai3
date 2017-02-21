namespace MoleMole
{
    using MoleMole.Config;
    using System;

    [Serializable]
    public class FaceAnimationConvertItem
    {
        public ConfigFaceAnimation config;
        public TestMatInfoProvider leftEyeProvider;
        public TestMatInfoProvider mouthProvider;
        public TestMatInfoProvider rightEyeProvider;
    }
}

