namespace MoleMole
{
    using FullInspector;
    using MoleMole.Config;
    using System;
    using System.Collections.Generic;

    public class MaxAvatarClassInTeamChallenge : BaseLevelChallenge
    {
        [ShowInInspector]
        protected bool _finished;
        public readonly EntityClass targetClass;
        public readonly int targetNum;

        public MaxAvatarClassInTeamChallenge(LevelChallengeHelperPlugin helper, LevelChallengeMetaData metaData) : base(helper, metaData)
        {
            this._finished = false;
            this.targetNum = base._metaData.paramList[0];
            this.targetClass = base._metaData.paramList[1];
        }

        private int GetAmountOfSameEntityClassAvatarInTeam(EntityClass entityClass)
        {
            List<AvatarDataItem> memberList = Singleton<LevelScoreManager>.Instance.memberList;
            int num = 0;
            foreach (AvatarDataItem item in memberList)
            {
                if (item.ClassId == entityClass)
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
            return (this.GetAmountOfSameEntityClassAvatarInTeam(this.targetClass) >= this.targetNum);
        }

        public override void OnAdded()
        {
            this._finished = this.IsFinished();
        }
    }
}

