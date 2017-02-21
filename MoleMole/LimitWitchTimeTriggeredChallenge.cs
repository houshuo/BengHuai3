namespace MoleMole
{
    using FullInspector;
    using System;

    public class LimitWitchTimeTriggeredChallenge : BaseLevelChallenge
    {
        [ShowInInspector]
        private bool _finished;
        [ShowInInspector]
        private int _witchTimeTiggered;
        public readonly int targetNum;

        public LimitWitchTimeTriggeredChallenge(LevelChallengeHelperPlugin helper, LevelChallengeMetaData metaData) : base(helper, metaData)
        {
            this._finished = false;
            this.targetNum = base._metaData.paramList[0];
            this._witchTimeTiggered = 0;
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
            return string.Format("[{0}/{1}]", this._witchTimeTiggered, this.targetNum);
        }

        public override bool IsFinished()
        {
            this._finished = this._witchTimeTiggered >= this.targetNum;
            return this._finished;
        }

        public override bool ListenEvent(BaseEvent evt)
        {
            return ((evt is EvtSkillStart) && this.ListenSkillStart((EvtSkillStart) evt));
        }

        private bool ListenSkillStart(EvtSkillStart evt)
        {
            BaseActor actor = Singleton<EventManager>.Instance.GetActor(evt.targetID);
            if (((actor is AvatarActor) && (actor as AvatarActor).config.Skills.ContainsKey(evt.skillID)) && ((actor as AvatarActor).config.Skills[evt.skillID].SkillCategoryTag != null))
            {
                for (int i = 0; i < (actor as AvatarActor).config.Skills[evt.skillID].SkillCategoryTag.Length; i++)
                {
                    if ((actor as AvatarActor).config.Skills[evt.skillID].SkillCategoryTag[i] == AttackResult.AttackCategoryTag.Evade)
                    {
                        this._witchTimeTiggered++;
                        break;
                    }
                }
            }
            if (this._witchTimeTiggered >= this.targetNum)
            {
                this.Finish();
            }
            return false;
        }

        public override void OnAdded()
        {
            Singleton<EventManager>.Instance.RegisterEventListener<EvtSkillStart>(base._helper.levelActor.runtimeID);
        }

        public override void OnDecided()
        {
            base.OnDecided();
            Singleton<EventManager>.Instance.RemoveEventListener<EvtSkillStart>(base._helper.levelActor.runtimeID);
        }
    }
}

