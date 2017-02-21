namespace MoleMole
{
    using FullInspector;
    using System;

    public class LimitBeAddedDebuffChallenge : BaseLevelChallenge
    {
        [ShowInInspector]
        private bool _finished;
        [ShowInInspector]
        private int _tempBeAddedDebuffNum;
        public readonly int targetBeAddedDebuffNum;

        public LimitBeAddedDebuffChallenge(LevelChallengeHelperPlugin helper, LevelChallengeMetaData metaData) : base(helper, metaData)
        {
            this._finished = true;
            this._tempBeAddedDebuffNum = 0;
            this.targetBeAddedDebuffNum = base._metaData.paramList[0];
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
                return "Succ";
            }
            return "Fail";
        }

        public override bool IsFinished()
        {
            return this._finished;
        }

        private bool ListenBuffAdd(EvtBuffAdd evt)
        {
            BaseActor actor = Singleton<EventManager>.Instance.GetActor(evt.targetID);
            bool flag = (evt.abilityState & (AbilityState.Tied | AbilityState.TargetLocked | AbilityState.Fragile | AbilityState.Weak | AbilityState.AttackSpeedDown | AbilityState.MoveSpeedDown | AbilityState.Frozen | AbilityState.Poisoned | AbilityState.Burn | AbilityState.Paralyze | AbilityState.Stun | AbilityState.Bleed)) != AbilityState.None;
            if (((actor != null) && (actor is AvatarActor)) && (Singleton<AvatarManager>.Instance.IsLocalAvatar(actor.runtimeID) && flag))
            {
                this._tempBeAddedDebuffNum++;
                if (this._tempBeAddedDebuffNum > this.targetBeAddedDebuffNum)
                {
                    this.Fail();
                }
            }
            return false;
        }

        public override bool ListenEvent(BaseEvent evt)
        {
            return ((evt is EvtBuffAdd) && this.ListenBuffAdd((EvtBuffAdd) evt));
        }

        public override void OnAdded()
        {
            Singleton<EventManager>.Instance.RegisterEventListener<EvtBuffAdd>(base._helper.levelActor.runtimeID);
        }

        public override void OnDecided()
        {
            base.OnDecided();
            Singleton<EventManager>.Instance.RemoveEventListener<EvtBuffAdd>(base._helper.levelActor.runtimeID);
        }
    }
}

