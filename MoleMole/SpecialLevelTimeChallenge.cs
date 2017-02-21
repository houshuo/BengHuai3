namespace MoleMole
{
    using FullInspector;
    using System;

    public class SpecialLevelTimeChallenge : BaseLevelChallenge
    {
        [ShowInInspector]
        private bool _finished;
        public readonly float targetLevelTime;

        public SpecialLevelTimeChallenge(LevelChallengeHelperPlugin helper, LevelChallengeMetaData metaData) : base(helper, metaData)
        {
            this._finished = true;
            if (metaData.conditionId != 0x1b)
            {
                LevelMetaData levelMetaDataByKey = LevelMetaDataReader.GetLevelMetaDataByKey(Singleton<LevelScoreManager>.Instance.LevelId);
                this.targetLevelTime = (metaData.conditionId != 0x1d) ? ((float) levelMetaDataByKey.fastBonusTime) : ((float) levelMetaDataByKey.sonicBonusTime);
            }
            else
            {
                this.targetLevelTime = (float) base._metaData.paramList[0];
            }
        }

        public override string GetProcessMsg()
        {
            if (base._metaData.conditionId == 0x1b)
            {
                return string.Format(string.Empty, new object[0]);
            }
            LevelActorTimerPlugin plugin = base._helper.levelActor.GetPlugin<LevelActorTimerPlugin>();
            float num = (plugin == null) ? 0f : plugin.Timer;
            return string.Format("[{0}/{1}]", (int) num, (int) this.targetLevelTime);
        }

        public override bool IsFinished()
        {
            LevelActorTimerPlugin plugin = base._helper.levelActor.GetPlugin<LevelActorTimerPlugin>();
            if (plugin == null)
            {
                this._finished = false;
            }
            else if (base._metaData.conditionId == 0x1b)
            {
                this._finished = true;
            }
            else
            {
                this._finished = plugin.Timer <= this.targetLevelTime;
            }
            return this._finished;
        }
    }
}

