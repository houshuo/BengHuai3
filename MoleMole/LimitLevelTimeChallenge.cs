namespace MoleMole
{
    using FullInspector;
    using System;

    public class LimitLevelTimeChallenge : BaseLevelChallenge
    {
        [ShowInInspector]
        private bool _finished;
        public readonly float targetLevelTime;

        public LimitLevelTimeChallenge(LevelChallengeHelperPlugin helper, LevelChallengeMetaData metaData) : base(helper, metaData)
        {
            this._finished = true;
            this.targetLevelTime = (float) base._metaData.paramList[0];
        }

        public override string GetProcessMsg()
        {
            LevelActorTimerPlugin plugin = base._helper.levelActor.GetPlugin<LevelActorTimerPlugin>();
            float num = (plugin == null) ? 0f : plugin.Timer;
            return string.Format("[{0}/{1}]", (int) num, (int) this.targetLevelTime);
        }

        public override bool IsFinished()
        {
            LevelActorTimerPlugin plugin = base._helper.levelActor.GetPlugin<LevelActorTimerPlugin>();
            this._finished = (plugin != null) ? (plugin.Timer <= this.targetLevelTime) : false;
            return this._finished;
        }
    }
}

