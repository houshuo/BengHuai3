namespace MoleMole
{
    using FullInspector;
    using System;

    public class LimitBeHitChallenge : BaseLevelChallenge
    {
        [ShowInInspector]
        private int _beHitNum;
        [ShowInInspector]
        private bool _finished;
        public readonly int targetNum;

        public LimitBeHitChallenge(LevelChallengeHelperPlugin helper, LevelChallengeMetaData metaData) : base(helper, metaData)
        {
            this._finished = true;
            this._beHitNum = 0;
            this.targetNum = base._metaData.paramList[0];
        }

        private void Fail()
        {
            this._finished = false;
            this.OnDecided();
        }

        public override string GetProcessMsg()
        {
            if (!this.IsFinished())
            {
                return "Fail";
            }
            return string.Format("[{0}/{1}]", this._beHitNum, this.targetNum);
        }

        public override bool IsFinished()
        {
            return this._finished;
        }

        private bool ListenBeingHit(EvtBeingHit evt)
        {
            if (!evt.attackData.rejected)
            {
                if (!evt.attackData.isAnimEventAttack)
                {
                    return false;
                }
                BaseActor actor = Singleton<EventManager>.Instance.GetActor(evt.targetID);
                if (((actor != null) && (actor is AvatarActor)) && (Singleton<AvatarManager>.Instance.IsLocalAvatar(actor.runtimeID) && evt.attackData.isInComboCount))
                {
                    this._beHitNum++;
                    if (this._beHitNum > this.targetNum)
                    {
                        this.Fail();
                    }
                }
            }
            return false;
        }

        public override bool ListenEvent(BaseEvent evt)
        {
            return ((evt is EvtBeingHit) && this.ListenBeingHit((EvtBeingHit) evt));
        }

        public override void OnAdded()
        {
            Singleton<EventManager>.Instance.RegisterEventListener<EvtBeingHit>(base._helper.levelActor.runtimeID);
        }

        public override void OnDecided()
        {
            base.OnDecided();
            Singleton<EventManager>.Instance.RemoveEventListener<EvtBeingHit>(base._helper.levelActor.runtimeID);
        }
    }
}

