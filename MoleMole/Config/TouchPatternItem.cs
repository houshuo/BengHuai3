namespace MoleMole.Config
{
    using MoleMole;
    using System;

    [Serializable]
    public class TouchPatternItem
    {
        public ReactionPattern advanceReactionPattern;
        public int advanceTime;
        public BodyPartType bodyPartType;
        public int heartLevel;
        public ReactionPattern reactionPattern;
    }
}

