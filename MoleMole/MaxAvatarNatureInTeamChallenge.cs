namespace MoleMole
{
    using FullInspector;
    using MoleMole.Config;
    using System;
    using System.Collections.Generic;

    public class MaxAvatarNatureInTeamChallenge : BaseLevelChallenge
    {
        [ShowInInspector]
        protected bool _finished;
        public readonly EntityNature targetNature;
        public readonly int targetNum;

        public MaxAvatarNatureInTeamChallenge(LevelChallengeHelperPlugin helper, LevelChallengeMetaData metaData) : base(helper, metaData)
        {
            this._finished = false;
            this.targetNum = base._metaData.paramList[0];
            this.targetNature = base._metaData.paramList[1];
        }

        private int GetAmountOfSameEntityNatureAvatarInTeam(EntityNature entityNature)
        {
            List<AvatarDataItem> memberList = Singleton<LevelScoreManager>.Instance.memberList;
            int num = 0;
            foreach (AvatarDataItem item in memberList)
            {
                if (item.Attribute == this.targetNature)
                {
                    num++;
                }
            }
            return num;
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
            return (this.GetAmountOfSameEntityNatureAvatarInTeam(this.targetNature) >= this.targetNum);
        }

        public override void OnAdded()
        {
            this._finished = this.IsFinished();
        }
    }
}

