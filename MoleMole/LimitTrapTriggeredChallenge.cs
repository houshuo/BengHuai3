namespace MoleMole
{
    using FullInspector;
    using System;

    public class LimitTrapTriggeredChallenge : BaseLevelChallenge
    {
        [ShowInInspector]
        private bool _finished;
        [ShowInInspector]
        private int _trapTriggeredNum;
        public readonly int targetNum;

        public LimitTrapTriggeredChallenge(LevelChallengeHelperPlugin helper, LevelChallengeMetaData metaData) : base(helper, metaData)
        {
            this._finished = true;
            this._trapTriggeredNum = 0;
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
            return string.Format("[{0}/{1}]", this._trapTriggeredNum, this.targetNum);
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
                BaseActor actor2 = Singleton<EventManager>.Instance.GetActor(evt.sourceID);
                if ((((actor != null) && (actor is AvatarActor)) && (Singleton<AvatarManager>.Instance.IsLocalAvatar(actor.runtimeID) && (actor2 != null))) && (actor2 is PropObjectActor))
                {
                    PropObjectActor actor3 = actor2 as PropObjectActor;
                    if ((actor3 != null) && (actor3.entity is MonoTriggerProp))
                    {
                        this._trapTriggeredNum++;
                        if (this._trapTriggeredNum >= this.targetNum)
                        {
                            this.Fail();
                        }
                    }
                }
            }
            return false;
        }

        public override bool ListenEvent(BaseEvent evt)
        {
            if (evt is EvtBeingHit)
            {
                return this.ListenBeingHit((EvtBeingHit) evt);
            }
            return ((evt is EvtFieldEnter) && this.ListenFieldEnter((EvtFieldEnter) evt));
        }

        private bool ListenFieldEnter(EvtFieldEnter evt)
        {
            BaseActor actor = Singleton<EventManager>.Instance.GetActor(evt.targetID);
            BaseActor actor2 = Singleton<EventManager>.Instance.GetActor(evt.otherID);
            if ((((actor != null) && (actor is PropObjectActor)) && ((actor2 != null) && (actor2 is AvatarActor))) && Singleton<AvatarManager>.Instance.IsLocalAvatar(actor2.runtimeID))
            {
                PropObjectActor actor3 = actor as PropObjectActor;
                if ((actor3 != null) && (actor3.entity is MonoTriggerProp))
                {
                    this._trapTriggeredNum++;
                    if (this._trapTriggeredNum >= this.targetNum)
                    {
                        this.Fail();
                    }
                }
            }
            return false;
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

