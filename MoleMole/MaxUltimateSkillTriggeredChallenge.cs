namespace MoleMole
{
    using FullInspector;
    using System;

    public class MaxUltimateSkillTriggeredChallenge : BaseLevelChallenge
    {
        [ShowInInspector]
        private bool _finished;
        [ShowInInspector]
        private int _tempUltimateSkillAmount;
        public readonly int targetUltimateSkillAmount;
        private static string ULTIMATE_SKILL_NAME = "SKL02";

        public MaxUltimateSkillTriggeredChallenge(LevelChallengeHelperPlugin helper, LevelChallengeMetaData metaData) : base(helper, metaData)
        {
            this._finished = false;
            this._tempUltimateSkillAmount = 0;
            this.targetUltimateSkillAmount = base._metaData.paramList[0];
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
            return string.Format("[{0}/{1}]", this._tempUltimateSkillAmount, this.targetUltimateSkillAmount);
        }

        public override bool IsFinished()
        {
            return this._finished;
        }

        public override bool ListenEvent(BaseEvent evt)
        {
            return ((evt is EvtSkillStart) && this.ListenSkillStart((EvtSkillStart) evt));
        }

        private bool ListenSkillStart(EvtSkillStart evt)
        {
            BaseActor actor = Singleton<EventManager>.Instance.GetActor(evt.targetID);
            if (((actor == null) || !(actor is AvatarActor)) || (evt.skillID != ULTIMATE_SKILL_NAME))
            {
                return false;
            }
            this._tempUltimateSkillAmount++;
            if (this._tempUltimateSkillAmount >= this.targetUltimateSkillAmount)
            {
                this.Finish();
            }
            return true;
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

