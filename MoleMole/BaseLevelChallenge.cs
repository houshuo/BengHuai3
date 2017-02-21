namespace MoleMole
{
    using FullInspector;
    using System;

    [fiInspectorOnly]
    public abstract class BaseLevelChallenge
    {
        protected LevelChallengeHelperPlugin _helper;
        protected LevelChallengeMetaData _metaData;
        public bool active;
        public readonly int challengeId;

        public BaseLevelChallenge(LevelChallengeHelperPlugin helper, LevelChallengeMetaData metaData)
        {
            this._helper = helper;
            this._metaData = metaData;
            this.challengeId = this._metaData.challengeId;
            this.active = true;
        }

        public virtual void Core()
        {
        }

        public abstract string GetProcessMsg();
        public abstract bool IsFinished();
        public virtual bool ListenEvent(BaseEvent evt)
        {
            return false;
        }

        public virtual void OnAdded()
        {
        }

        public virtual void OnDecided()
        {
            this.active = false;
        }

        public virtual bool OnEvent(BaseEvent evt)
        {
            return false;
        }

        public virtual bool OnPostEvent(BaseEvent evt)
        {
            return false;
        }
    }
}

