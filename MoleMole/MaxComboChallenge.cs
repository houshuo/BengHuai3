namespace MoleMole
{
    using FullInspector;
    using System;

    public class MaxComboChallenge : BaseLevelChallenge
    {
        [ShowInInspector]
        private bool _finished;
        [ShowInInspector]
        private int _maxCombo;
        public readonly int targetMaxCombo;

        public MaxComboChallenge(LevelChallengeHelperPlugin helper, LevelChallengeMetaData metaData) : base(helper, metaData)
        {
            this._finished = false;
            this._maxCombo = 0;
            this.targetMaxCombo = base._metaData.paramList[0];
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
            return string.Format("[{0}/{1}]", this._maxCombo, this.targetMaxCombo);
        }

        public override bool IsFinished()
        {
            return this._finished;
        }

        public override bool ListenEvent(BaseEvent evt)
        {
            return ((evt is EvtLevelState) && this.ListenLevelState((EvtLevelState) evt));
        }

        private bool ListenLevelState(EvtLevelState evt)
        {
            if (evt.state == EvtLevelState.State.Start)
            {
                Singleton<LevelManager>.Instance.levelActor.onLevelComboChanged = (Action<int, int>) Delegate.Combine(Singleton<LevelManager>.Instance.levelActor.onLevelComboChanged, new Action<int, int>(this.OnLevelComboChanged));
                Singleton<EventManager>.Instance.RemoveEventListener<EvtLevelState>(base._helper.levelActor.runtimeID);
            }
            return false;
        }

        public override void OnAdded()
        {
            Singleton<EventManager>.Instance.RegisterEventListener<EvtLevelState>(base._helper.levelActor.runtimeID);
        }

        public override void OnDecided()
        {
            base.OnDecided();
            Singleton<LevelManager>.Instance.levelActor.onLevelComboChanged = (Action<int, int>) Delegate.Remove(Singleton<LevelManager>.Instance.levelActor.onLevelComboChanged, new Action<int, int>(this.OnLevelComboChanged));
        }

        private void OnLevelComboChanged(int from, int to)
        {
            if (to > from)
            {
                int num = to;
                if (this._maxCombo < num)
                {
                    this._maxCombo = num;
                    if (this._maxCombo >= this.targetMaxCombo)
                    {
                        this.Finish();
                    }
                }
            }
        }
    }
}

