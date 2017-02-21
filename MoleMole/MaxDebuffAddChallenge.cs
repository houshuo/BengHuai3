namespace MoleMole
{
    using FullInspector;
    using System;
    using System.Collections.Generic;

    public class MaxDebuffAddChallenge : BaseLevelChallenge
    {
        [ShowInInspector]
        private List<uint> _addDebuffMonsterList;
        [ShowInInspector]
        private bool _finished;
        public readonly int targetDebuffAddNum;

        public MaxDebuffAddChallenge(LevelChallengeHelperPlugin helper, LevelChallengeMetaData metaData) : base(helper, metaData)
        {
            this._finished = false;
            this.targetDebuffAddNum = base._metaData.paramList[0];
            this._addDebuffMonsterList = new List<uint>();
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
            return string.Format("[{0}/{1}]", this._addDebuffMonsterList.Count, this.targetDebuffAddNum);
        }

        public override bool IsFinished()
        {
            return this._finished;
        }

        private bool ListenBuffAdd(EvtBuffAdd evt)
        {
            BaseActor actor = Singleton<EventManager>.Instance.GetActor(evt.targetID);
            bool flag = (evt.abilityState & (AbilityState.Tied | AbilityState.TargetLocked | AbilityState.Fragile | AbilityState.Weak | AbilityState.AttackSpeedDown | AbilityState.MoveSpeedDown | AbilityState.Frozen | AbilityState.Poisoned | AbilityState.Burn | AbilityState.Paralyze | AbilityState.Stun | AbilityState.Bleed)) != AbilityState.None;
            if (((actor != null) && (actor is MonsterActor)) && flag)
            {
                if (!this._addDebuffMonsterList.Contains(actor.runtimeID))
                {
                    this._addDebuffMonsterList.Add(actor.runtimeID);
                }
                if (this._addDebuffMonsterList.Count >= this.targetDebuffAddNum)
                {
                    this.Finish();
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

