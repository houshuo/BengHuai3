namespace MoleMole
{
    using FullInspector;
    using System;
    using System.Collections.Generic;

    public class MaxUltimateSkillTriggeredByDistinctAvatarChallenge : BaseLevelChallenge
    {
        [ShowInInspector]
        private bool _finished;
        [ShowInInspector]
        private List<uint> _tempTriggeredUltimateSkillAvatarIDs;
        public readonly int targetUltimateSkillAmountByDistinctAvatar;
        private static string ULTIMATE_SKILL_NAME = "SKL02";

        public MaxUltimateSkillTriggeredByDistinctAvatarChallenge(LevelChallengeHelperPlugin helper, LevelChallengeMetaData metaData) : base(helper, metaData)
        {
            this._finished = false;
            this._tempTriggeredUltimateSkillAvatarIDs = new List<uint>();
            this.targetUltimateSkillAmountByDistinctAvatar = base._metaData.paramList[0];
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
            return string.Format("[{0}/{1}]", this._tempTriggeredUltimateSkillAvatarIDs.Count, this.targetUltimateSkillAmountByDistinctAvatar);
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
            if ((actor == null) || !(actor is AvatarActor))
            {
                return false;
            }
            if ((evt.skillID == ULTIMATE_SKILL_NAME) && !this._tempTriggeredUltimateSkillAvatarIDs.Contains(evt.targetID))
            {
                this._tempTriggeredUltimateSkillAvatarIDs.Add(evt.targetID);
            }
            if (this._tempTriggeredUltimateSkillAvatarIDs.Count >= this.targetUltimateSkillAmountByDistinctAvatar)
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

