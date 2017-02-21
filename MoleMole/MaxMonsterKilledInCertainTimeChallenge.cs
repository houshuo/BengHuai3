namespace MoleMole
{
    using FullInspector;
    using System;
    using System.Collections.Generic;
    using UniRx;
    using UnityEngine;

    public class MaxMonsterKilledInCertainTimeChallenge : BaseLevelChallenge
    {
        [ShowInInspector]
        private bool _finished;
        [ShowInInspector]
        private bool _inStastics;
        private List<Tuple<float, int>> _killedAmountLs;
        [ShowInInspector]
        private float _stasticsTimer;
        [ShowInInspector]
        private int _tempKilledAmountInWindow;
        [ShowInInspector]
        private int _tempMaxKilledAmountInWindow;
        public readonly int targetKilledAmount;
        public readonly float targetTimeWindow;

        public MaxMonsterKilledInCertainTimeChallenge(LevelChallengeHelperPlugin helper, LevelChallengeMetaData metaData) : base(helper, metaData)
        {
            this._finished = false;
            this._tempKilledAmountInWindow = 0;
            this._tempMaxKilledAmountInWindow = 0;
            this._stasticsTimer = 0f;
            this.targetTimeWindow = (float) base._metaData.paramList[0];
            this.targetKilledAmount = base._metaData.paramList[1];
            this._killedAmountLs = new List<Tuple<float, int>>();
            this._inStastics = false;
        }

        private bool CheckStastics(int targetKilledAmount)
        {
            this._tempKilledAmountInWindow = 0;
            foreach (Tuple<float, int> tuple in this._killedAmountLs)
            {
                if (tuple != null)
                {
                    this._tempKilledAmountInWindow += tuple.Item2;
                }
            }
            this._tempMaxKilledAmountInWindow = Mathf.Max(this._tempMaxKilledAmountInWindow, this._tempKilledAmountInWindow);
            return (this._tempKilledAmountInWindow >= targetKilledAmount);
        }

        public override void Core()
        {
            if (this._inStastics)
            {
                this._stasticsTimer += Time.deltaTime;
                this.UpdateStastics(this._stasticsTimer, this.targetTimeWindow);
                if (this.CheckStastics(this.targetKilledAmount))
                {
                    this.Finish();
                }
            }
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
            return string.Format("[{0}/{1}]", this._tempMaxKilledAmountInWindow, this.targetKilledAmount);
        }

        public override bool IsFinished()
        {
            return this._finished;
        }

        public override bool ListenEvent(BaseEvent evt)
        {
            if (evt is EvtLevelState)
            {
                return this.ListenLevelStatge((EvtLevelState) evt);
            }
            return ((evt is EvtKilled) && this.ListenKilled((EvtKilled) evt));
        }

        private bool ListenKilled(EvtKilled evt)
        {
            AvatarActor actor = Singleton<EventManager>.Instance.GetActor<AvatarActor>(evt.killerID);
            MonsterActor actor2 = Singleton<EventManager>.Instance.GetActor<MonsterActor>(evt.targetID);
            if (((actor != null) && (actor2 != null)) && this._inStastics)
            {
                int num = this._killedAmountLs.SeekAddPosition<Tuple<float, int>>();
                this._killedAmountLs[num] = Tuple.Create<float, int>(this._stasticsTimer, 1);
            }
            return false;
        }

        private bool ListenLevelStatge(EvtLevelState evt)
        {
            if (evt.state == EvtLevelState.State.Start)
            {
                this.StartStastics();
            }
            return false;
        }

        public override void OnAdded()
        {
            Singleton<EventManager>.Instance.RegisterEventListener<EvtKilled>(base._helper.levelActor.runtimeID);
            Singleton<EventManager>.Instance.RegisterEventListener<EvtLevelState>(base._helper.levelActor.runtimeID);
        }

        public override void OnDecided()
        {
            base.OnDecided();
            Singleton<EventManager>.Instance.RemoveEventListener<EvtKilled>(base._helper.levelActor.runtimeID);
            Singleton<EventManager>.Instance.RemoveEventListener<EvtLevelState>(base._helper.levelActor.runtimeID);
        }

        public void StartStastics()
        {
            this._inStastics = true;
            this._tempKilledAmountInWindow = 0;
        }

        public void StopStastics()
        {
            this._inStastics = false;
        }

        private void UpdateStastics(float currentTime, float window)
        {
            for (int i = 0; i < this._killedAmountLs.Count; i++)
            {
                Tuple<float, int> tuple = this._killedAmountLs[i];
                if ((tuple != null) && ((currentTime - tuple.Item1) > window))
                {
                    this._killedAmountLs[i] = null;
                }
            }
        }
    }
}

