namespace MoleMole
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;

    public class LevelChallengeDataItem
    {
        private LevelChallengeMetaData _metaData;
        public int challengeId;
        public bool Finished;

        public LevelChallengeDataItem(int challengeId, LevelMetaData levelMeta, int rewardId = 0)
        {
            this.challengeId = challengeId;
            this._metaData = LevelChallengeMetaDataReader.GetLevelChallengeMetaDataByKey(challengeId);
            if (this.IsFinishStageFastChallenge() || this.IsFinishStageVeryFastChallenge())
            {
                int num = !this.IsFinishStageFastChallenge() ? levelMeta.sonicBonusTime : levelMeta.fastBonusTime;
                this._metaData = new LevelChallengeMetaData(this._metaData.challengeId, this._metaData.conditionId, new List<int> { num }, this._metaData.diaplayTarget);
            }
            this.Finished = false;
        }

        public bool IsFinishStageFastChallenge()
        {
            return (this._metaData.conditionId == 0x1c);
        }

        public bool IsFinishStageNomalChallenge()
        {
            return (this._metaData.conditionId == 0x1b);
        }

        public bool IsFinishStageVeryFastChallenge()
        {
            return (this._metaData.conditionId == 0x1d);
        }

        public bool IsSpecialChallenge()
        {
            return ((this.IsFinishStageNomalChallenge() || this.IsFinishStageFastChallenge()) || this.IsFinishStageVeryFastChallenge());
        }

        public string DisplayTarget
        {
            get
            {
                return LocalizationGeneralLogic.GetTextWithParamArray<int>(this._metaData.diaplayTarget, this._metaData.paramList.ToArray());
            }
        }
    }
}

