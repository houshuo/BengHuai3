namespace MoleMole
{
    using System;

    public class LimitAvatarKilledChallenge : BaseLevelChallenge
    {
        private int _avatarKilledNum;
        public readonly int targetNum;

        public LimitAvatarKilledChallenge(LevelChallengeHelperPlugin helper, LevelChallengeMetaData metaData) : base(helper, metaData)
        {
            this.targetNum = base._metaData.paramList[0];
            this._avatarKilledNum = 0;
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
            return (this._avatarKilledNum <= this.targetNum);
        }

        public override bool ListenEvent(BaseEvent evt)
        {
            return ((evt is EvtKilled) && this.ListenKill((EvtKilled) evt));
        }

        private bool ListenKill(EvtKilled evt)
        {
            if (Singleton<RuntimeIDManager>.Instance.ParseCategory(evt.targetID) == 3)
            {
                if (!Singleton<AvatarManager>.Instance.IsPlayerAvatar(evt.targetID))
                {
                    return false;
                }
                this._avatarKilledNum++;
            }
            return true;
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

