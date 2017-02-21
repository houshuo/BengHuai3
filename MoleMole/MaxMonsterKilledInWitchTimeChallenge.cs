namespace MoleMole
{
    using FullInspector;
    using System;
    using UnityEngine;

    public class MaxMonsterKilledInWitchTimeChallenge : BaseLevelChallenge
    {
        [ShowInInspector]
        private bool _finished;
        private bool _inStastics;
        [ShowInInspector]
        private int _tempKilledAmount;
        [ShowInInspector]
        private int _tempMaxKilledAmount;
        public readonly int targetKilledAmount;

        public MaxMonsterKilledInWitchTimeChallenge(LevelChallengeHelperPlugin helper, LevelChallengeMetaData metaData) : base(helper, metaData)
        {
            this._finished = false;
            this._tempKilledAmount = 0;
            this._tempMaxKilledAmount = 0;
            this.targetKilledAmount = base._metaData.paramList[0];
            this._inStastics = false;
        }

        private void Finish()
        {
            this._finished = true;
            this.StopStastics();
            this.OnDecided();
        }

        public override string GetProcessMsg()
        {
            if (this.IsFinished())
            {
                return "Succ";
            }
            return string.Format("[{0}/{1}]", this._tempMaxKilledAmount, this.targetKilledAmount);
        }

        public override bool IsFinished()
        {
            return this._finished;
        }

        public override bool ListenEvent(BaseEvent evt)
        {
            if (evt is EvtLevelBuffState)
            {
                return this.ListenLevelBuffState((EvtLevelBuffState) evt);
            }
            return ((evt is EvtKilled) && this.ListenKilled((EvtKilled) evt));
        }

        private bool ListenKilled(EvtKilled evt)
        {
            AvatarActor actor = Singleton<EventManager>.Instance.GetActor<AvatarActor>(evt.killerID);
            MonsterActor actor2 = Singleton<EventManager>.Instance.GetActor<MonsterActor>(evt.targetID);
            if (((actor == null) || (actor2 == null)) || !this._inStastics)
            {
                return false;
            }
            this._tempKilledAmount++;
            this._tempMaxKilledAmount = Mathf.Max(this._tempMaxKilledAmount, this._tempKilledAmount);
            if (this._tempKilledAmount >= this.targetKilledAmount)
            {
                this.Finish();
            }
            return true;
        }

        private bool ListenLevelBuffState(EvtLevelBuffState evt)
        {
            if ((evt.state == LevelBuffState.Start) && (evt.levelBuff == LevelBuffType.WitchTime))
            {
                this.StartStastics();
            }
            if ((evt.state == LevelBuffState.Stop) && (evt.levelBuff == LevelBuffType.WitchTime))
            {
                this.StopStastics();
            }
            return false;
        }

        public override void OnAdded()
        {
            Singleton<EventManager>.Instance.RegisterEventListener<EvtKilled>(base._helper.levelActor.runtimeID);
            Singleton<EventManager>.Instance.RegisterEventListener<EvtLevelBuffState>(base._helper.levelActor.runtimeID);
        }

        public override void OnDecided()
        {
            base.OnDecided();
            Singleton<EventManager>.Instance.RemoveEventListener<EvtKilled>(base._helper.levelActor.runtimeID);
            Singleton<EventManager>.Instance.RemoveEventListener<EvtLevelBuffState>(base._helper.levelActor.runtimeID);
        }

        public void StartStastics()
        {
            this._inStastics = true;
        }

        public void StopStastics()
        {
            this._inStastics = false;
        }
    }
}

