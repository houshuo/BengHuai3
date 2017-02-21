namespace MoleMole
{
    using FullInspector;
    using System;

    public class HelperAvatarAliveChallenge : BaseLevelChallenge
    {
        [ShowInInspector]
        private bool _finished;
        private bool _hasHelperAvatar;

        public HelperAvatarAliveChallenge(LevelChallengeHelperPlugin helper, LevelChallengeMetaData metaData) : base(helper, metaData)
        {
            this._finished = true;
            this._hasHelperAvatar = false;
        }

        private void Fail()
        {
            this._finished = false;
            this.OnDecided();
        }

        public override string GetProcessMsg()
        {
            if (this.IsFinished())
            {
                return "Doing";
            }
            return "Fail";
        }

        public override bool IsFinished()
        {
            return this._finished;
        }

        public override bool ListenEvent(BaseEvent evt)
        {
            if (evt is EvtKilled)
            {
                return this.ListenKilled((EvtKilled) evt);
            }
            return ((evt is EvtStageReady) && this.ListenStageReady((EvtStageReady) evt));
        }

        private bool ListenKilled(EvtKilled evt)
        {
            if (this._hasHelperAvatar && Singleton<AvatarManager>.Instance.IsHelperAvatar(evt.targetID))
            {
                this.Fail();
            }
            return false;
        }

        private bool ListenStageReady(EvtStageReady evt)
        {
            BaseMonoAvatar helperAvatar = Singleton<AvatarManager>.Instance.GetHelperAvatar();
            this._hasHelperAvatar = helperAvatar != null;
            this._finished = this._hasHelperAvatar;
            return false;
        }

        public override void OnAdded()
        {
        }
    }
}

