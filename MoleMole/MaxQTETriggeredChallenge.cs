namespace MoleMole
{
    using FullInspector;
    using System;

    public class MaxQTETriggeredChallenge : BaseLevelChallenge
    {
        [ShowInInspector]
        private bool _finished;
        [ShowInInspector]
        private int _qteTiggeredNum;
        public readonly int targetNum;

        public MaxQTETriggeredChallenge(LevelChallengeHelperPlugin helper, LevelChallengeMetaData metaData) : base(helper, metaData)
        {
            this._finished = false;
            this.targetNum = base._metaData.paramList[0];
            this._qteTiggeredNum = 0;
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
            return string.Format("[{0}/{1}]", this._qteTiggeredNum, this.targetNum);
        }

        public override bool IsFinished()
        {
            this._finished = this._qteTiggeredNum >= this.targetNum;
            return this._finished;
        }

        public override bool ListenEvent(BaseEvent evt)
        {
            return ((evt is EvtQTEFire) && this.ListenQTEFire((EvtQTEFire) evt));
        }

        private bool ListenQTEFire(EvtQTEFire evt)
        {
            if (Singleton<AvatarManager>.Instance.IsPlayerAvatar(evt.targetID))
            {
                this._qteTiggeredNum++;
            }
            if (this._qteTiggeredNum >= this.targetNum)
            {
                this.Finish();
            }
            return false;
        }

        public override void OnAdded()
        {
            Singleton<EventManager>.Instance.RegisterEventListener<EvtQTEFire>(base._helper.levelActor.runtimeID);
        }

        public override void OnDecided()
        {
            base.OnDecided();
            Singleton<EventManager>.Instance.RemoveEventListener<EvtQTEFire>(base._helper.levelActor.runtimeID);
        }
    }
}

