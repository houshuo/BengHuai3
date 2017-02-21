namespace MoleMole
{
    using FullInspector;
    using System;

    public class MaxBoxOpenedChallenge : BaseLevelChallenge
    {
        [ShowInInspector]
        private bool _finished;
        [ShowInInspector]
        private int _tempBoxOpenedNum;
        public readonly int targetBoxOpenedNum;

        public MaxBoxOpenedChallenge(LevelChallengeHelperPlugin helper, LevelChallengeMetaData metaData) : base(helper, metaData)
        {
            this._finished = false;
            this._tempBoxOpenedNum = 0;
            this.targetBoxOpenedNum = base._metaData.paramList[0];
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
            return string.Format("[{0}/{1}]", this._tempBoxOpenedNum, this.targetBoxOpenedNum);
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
            BaseActor actor2 = Singleton<EventManager>.Instance.GetActor(evt.targetID);
            if ((((actor != null) && (actor is AvatarActor)) && (Singleton<AvatarManager>.Instance.IsLocalAvatar(actor.runtimeID) && (actor2 != null))) && (actor2 is PropObjectActor))
            {
                PropObjectActor actor3 = actor2 as PropObjectActor;
                if ((actor3 != null) && actor3.config.Name.Contains("Box"))
                {
                    this._tempBoxOpenedNum++;
                    if (this._tempBoxOpenedNum >= this.targetBoxOpenedNum)
                    {
                        this.Finish();
                    }
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

