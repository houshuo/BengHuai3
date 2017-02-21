namespace MoleMole
{
    using FullInspector;
    using System;
    using UnityEngine;

    public class MaxAllDamageInWitchTimeChallenge : BaseLevelChallenge
    {
        [ShowInInspector]
        private bool _finished;
        private bool _inStastics;
        [ShowInInspector]
        private float _tempAllDamage;
        public readonly float targetDamage;

        public MaxAllDamageInWitchTimeChallenge(LevelChallengeHelperPlugin helper, LevelChallengeMetaData metaData) : base(helper, metaData)
        {
            this._finished = false;
            this._tempAllDamage = 0f;
            this.targetDamage = (float) base._metaData.paramList[0];
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
            return string.Format("[{0}/{1}]", Mathf.FloorToInt(this._tempAllDamage), Mathf.FloorToInt(this.targetDamage));
        }

        public override bool IsFinished()
        {
            return this._finished;
        }

        private bool ListenBeingHit(EvtBeingHit evt)
        {
            if (evt.attackData.rejected)
            {
                return false;
            }
            BaseActor actor = Singleton<EventManager>.Instance.GetActor(evt.sourceID);
            if (((actor == null) || !(actor is AvatarActor)) || (!this._inStastics || !Singleton<LevelManager>.Instance.levelActor.IsLevelBuffActive(LevelBuffType.WitchTime)))
            {
                return false;
            }
            this._tempAllDamage += evt.attackData.damage;
            if (this._tempAllDamage >= this.targetDamage)
            {
                this.Finish();
            }
            return true;
        }

        public override bool ListenEvent(BaseEvent evt)
        {
            if (evt is EvtBeingHit)
            {
                return this.ListenBeingHit((EvtBeingHit) evt);
            }
            return ((evt is EvtLevelBuffState) && this.ListenLevelBuffState((EvtLevelBuffState) evt));
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
            Singleton<EventManager>.Instance.RegisterEventListener<EvtBeingHit>(base._helper.levelActor.runtimeID);
            Singleton<EventManager>.Instance.RegisterEventListener<EvtLevelBuffState>(base._helper.levelActor.runtimeID);
        }

        public override void OnDecided()
        {
            base.OnDecided();
            Singleton<EventManager>.Instance.RemoveEventListener<EvtBeingHit>(base._helper.levelActor.runtimeID);
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

