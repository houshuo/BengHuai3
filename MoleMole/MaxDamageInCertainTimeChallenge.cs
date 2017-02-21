namespace MoleMole
{
    using FullInspector;
    using System;
    using System.Collections.Generic;
    using UniRx;
    using UnityEngine;

    public class MaxDamageInCertainTimeChallenge : BaseLevelChallenge
    {
        private List<Tuple<float, float>> _damageLs;
        [ShowInInspector]
        private bool _finished;
        [ShowInInspector]
        private bool _inStastics;
        [ShowInInspector]
        private float _stasticsTimer;
        [ShowInInspector]
        private float _tempDamageInWindow;
        [ShowInInspector]
        private float _tempMaxDamageInWindow;
        public readonly float targetDamage;
        public readonly float targetTimeWindow;

        public MaxDamageInCertainTimeChallenge(LevelChallengeHelperPlugin helper, LevelChallengeMetaData metaData) : base(helper, metaData)
        {
            this._finished = false;
            this._tempDamageInWindow = 0f;
            this._tempMaxDamageInWindow = 0f;
            this._stasticsTimer = 0f;
            this.targetTimeWindow = (float) base._metaData.paramList[0];
            this.targetDamage = (float) base._metaData.paramList[1];
            this._damageLs = new List<Tuple<float, float>>();
            this._inStastics = false;
        }

        private bool CheckStastics(float targetDamage)
        {
            this._tempDamageInWindow = 0f;
            foreach (Tuple<float, float> tuple in this._damageLs)
            {
                if (tuple != null)
                {
                    this._tempDamageInWindow += tuple.Item2;
                }
            }
            this._tempMaxDamageInWindow = Mathf.Max(this._tempMaxDamageInWindow, this._tempDamageInWindow);
            return (this._tempDamageInWindow >= targetDamage);
        }

        public override void Core()
        {
            if (this._inStastics)
            {
                this._stasticsTimer += Time.deltaTime;
                this.UpdateStastics(this._stasticsTimer, this.targetTimeWindow);
                if (this.CheckStastics(this.targetDamage))
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
            return string.Format("[{0}/{1}]", Mathf.FloorToInt(this._tempMaxDamageInWindow), Mathf.FloorToInt(this.targetDamage));
        }

        public override bool IsFinished()
        {
            return this._finished;
        }

        private bool ListenBeingHit(EvtBeingHit evt)
        {
            if (!evt.attackData.rejected)
            {
                BaseActor actor = Singleton<EventManager>.Instance.GetActor(evt.sourceID);
                if (((actor == null) || !(actor is AvatarActor)) || !this._inStastics)
                {
                    return false;
                }
                int num = this._damageLs.SeekAddPosition<Tuple<float, float>>();
                this._damageLs[num] = Tuple.Create<float, float>(this._stasticsTimer, evt.attackData.GetTotalDamage());
            }
            return false;
        }

        public override bool ListenEvent(BaseEvent evt)
        {
            if (evt is EvtBeingHit)
            {
                return this.ListenBeingHit((EvtBeingHit) evt);
            }
            return ((evt is EvtLevelState) && this.ListenLevelStatge((EvtLevelState) evt));
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
            Singleton<EventManager>.Instance.RegisterEventListener<EvtBeingHit>(base._helper.levelActor.runtimeID);
            Singleton<EventManager>.Instance.RegisterEventListener<EvtLevelState>(base._helper.levelActor.runtimeID);
        }

        public override void OnDecided()
        {
            base.OnDecided();
            Singleton<EventManager>.Instance.RemoveEventListener<EvtBeingHit>(base._helper.levelActor.runtimeID);
            Singleton<EventManager>.Instance.RemoveEventListener<EvtLevelState>(base._helper.levelActor.runtimeID);
        }

        public void StartStastics()
        {
            this._inStastics = true;
            this._tempDamageInWindow = 0f;
        }

        public void StopStastics()
        {
            this._inStastics = false;
        }

        private void UpdateStastics(float currentTime, float window)
        {
            for (int i = 0; i < this._damageLs.Count; i++)
            {
                Tuple<float, float> tuple = this._damageLs[i];
                if ((tuple != null) && ((currentTime - tuple.Item1) > window))
                {
                    this._damageLs[i] = null;
                }
            }
        }
    }
}

