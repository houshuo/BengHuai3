namespace MoleMole
{
    using FullInspector;
    using System;
    using UnityEngine;

    public class LimitTotalDamageTakenChallenge : BaseLevelChallenge
    {
        [ShowInInspector]
        private bool _finished;
        [ShowInInspector]
        private float _tempDamageTaken;
        public readonly float targetDamageTaken;

        public LimitTotalDamageTakenChallenge(LevelChallengeHelperPlugin helper, LevelChallengeMetaData metaData) : base(helper, metaData)
        {
            this._finished = true;
            this._tempDamageTaken = 0f;
            this.targetDamageTaken = (float) base._metaData.paramList[0];
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
            return string.Format("[{0}/{1}]", Mathf.FloorToInt(this._tempDamageTaken), Mathf.FloorToInt(this.targetDamageTaken));
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
                if (((actor != null) && (actor is AvatarActor)) && (Singleton<AvatarManager>.Instance.IsLocalAvatar(actor.runtimeID) && evt.attackData.IsFinalResolved()))
                {
                    this._tempDamageTaken += evt.attackData.GetTotalDamage();
                    if (this._tempDamageTaken > this.targetDamageTaken)
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

