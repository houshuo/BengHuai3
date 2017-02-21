namespace MoleMole
{
    using FullInspector;
    using System;

    public class MaxMonsterHitAirChallenge : BaseLevelChallenge
    {
        [ShowInInspector]
        private bool _finished;
        [ShowInInspector]
        private int _tempHitAirAmount;
        public readonly int targetHitAirAmount;

        public MaxMonsterHitAirChallenge(LevelChallengeHelperPlugin helper, LevelChallengeMetaData metaData) : base(helper, metaData)
        {
            this._finished = false;
            this._tempHitAirAmount = 0;
            this.targetHitAirAmount = base._metaData.paramList[0];
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
            return string.Format("[{0}/{1}]", this._tempHitAirAmount, this.targetHitAirAmount);
        }

        public override bool IsFinished()
        {
            return this._finished;
        }

        private bool ListenBeingHit(EvtBeingHit evt)
        {
            AvatarActor actor = Singleton<EventManager>.Instance.GetActor<AvatarActor>(evt.sourceID);
            MonsterActor actor2 = Singleton<EventManager>.Instance.GetActor<MonsterActor>(evt.targetID);
            if ((actor != null) && (actor2 != null))
            {
                if (Singleton<MonsterManager>.Instance.GetMonsterByRuntimeID(actor2.runtimeID).IsAnimatorInTag(MonsterData.MonsterTagGroup.Throw))
                {
                    return false;
                }
                if (((evt.attackData.hitEffect == AttackResult.AnimatorHitEffect.ThrowAirBlow) || (evt.attackData.hitEffect == AttackResult.AnimatorHitEffect.ThrowBlow)) || ((evt.attackData.hitEffect == AttackResult.AnimatorHitEffect.ThrowUp) || (evt.attackData.hitEffect == AttackResult.AnimatorHitEffect.ThrowUpBlow)))
                {
                    this._tempHitAirAmount++;
                    if (this._tempHitAirAmount >= this.targetHitAirAmount)
                    {
                        this.Finish();
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

