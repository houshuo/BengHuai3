namespace MoleMole
{
    using System;

    public class LimitAvatarChallege : BaseLevelChallenge
    {
        public readonly int targetNum;

        public LimitAvatarChallege(LevelChallengeHelperPlugin helper, LevelChallengeMetaData metaData) : base(helper, metaData)
        {
            this.targetNum = base._metaData.paramList[0];
        }

        public override string GetProcessMsg()
        {
            if (this.IsFinished())
            {
                return "Succ";
            }
            return "Fail";
        }

        public override bool IsFinished()
        {
            return (Singleton<LevelScoreManager>.Instance.memberList.Count <= this.targetNum);
        }
    }
}

