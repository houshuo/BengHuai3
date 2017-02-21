namespace MoleMole
{
    using FullInspector;
    using System;

    public class LimitBeHitDownChallenge : BaseLevelChallenge
    {
        [ShowInInspector]
        private int _beHitDownNum;
        [ShowInInspector]
        private bool _finished;
        public readonly int targetDownNum;

        public LimitBeHitDownChallenge(LevelChallengeHelperPlugin helper, LevelChallengeMetaData metaData) : base(helper, metaData)
        {
            this._finished = true;
            this._beHitDownNum = 0;
            this.targetDownNum = base._metaData.paramList[0];
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
            return string.Format("[{0}/{1}]", this._beHitDownNum, this.targetDownNum);
        }

        public override bool IsFinished()
        {
            return this._finished;
        }

        private bool ListenBeingHit(EvtBeingHit evt)
        {
            if (!evt.attackData.rejected)
            {
                BaseActor actor = Singleton<EventManager>.Instance.GetActor(evt.targetID);
                if (((actor != null) && (actor is AvatarActor)) && Singleton<AvatarManager>.Instance.IsLocalAvatar(actor.runtimeID))
                {
                    if (evt.attackData.hitEffect == AttackResult.AnimatorHitEffect.KnockDown)
                    {
                        this._beHitDownNum++;
                    }
                    if (this._beHitDownNum > this.targetDownNum)
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

