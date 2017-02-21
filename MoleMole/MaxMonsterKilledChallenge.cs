namespace MoleMole
{
    using FullInspector;
    using System;

    public class MaxMonsterKilledChallenge : BaseLevelChallenge
    {
        [ShowInInspector]
        private bool _finished;
        [ShowInInspector]
        private int _tempKilledNum;
        public readonly int targetKilledNum;

        public MaxMonsterKilledChallenge(LevelChallengeHelperPlugin helper, LevelChallengeMetaData metaData) : base(helper, metaData)
        {
            this._finished = false;
            this._tempKilledNum = 0;
            this.targetKilledNum = base._metaData.paramList[0];
        }

        private void Finish()
        {
            this._finished = true;
            this.OnDecided();
        }

        public override string GetProcessMsg()
        {
            if (this.IsFinished())
            {
                return "Succ";
            }
            return string.Format("[{0}/{1}]", this._tempKilledNum, this.targetKilledNum);
        }

        public override bool IsFinished()
        {
            return this._finished;
        }

        public override bool ListenEvent(BaseEvent evt)
        {
            return ((evt is EvtKilled) && this.ListenKilled((EvtKilled) evt));
        }

        private bool ListenKilled(EvtKilled evt)
        {
            BaseActor actor = Singleton<EventManager>.Instance.GetActor(evt.killerID);
            if (((actor != null) && (actor is AvatarActor)) && Singleton<AvatarManager>.Instance.IsLocalAvatar(actor.runtimeID))
            {
                this._tempKilledNum++;
                if (this._tempKilledNum >= this.targetKilledNum)
                {
                    this.Finish();
                }
            }
            return false;
        }

        public override void OnAdded()
        {
            Singleton<EventManager>.Instance.RegisterEventListener<EvtKilled>(base._helper.levelActor.runtimeID);
        }

        public override void OnDecided()
        {
            base.OnDecided();
            Singleton<EventManager>.Instance.RemoveEventListener<EvtKilled>(base._helper.levelActor.runtimeID);
        }
    }
}

