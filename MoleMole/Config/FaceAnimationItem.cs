namespace MoleMole.Config
{
    using System;

    [Serializable]
    public class FaceAnimationItem
    {
        public FaceAnimationFrameBlock[] attachmentBlocks;
        public FaceAnimationFrameBlock[] leftEyeBlocks;
        public int length;
        public FaceAnimationFrameBlock[] mouthBlocks;
        public string name;
        public FaceAnimationFrameBlock[] rightEyeBlocks;
        public float timePerFrame;
    }
}

