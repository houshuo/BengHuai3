namespace MoleMole
{
    using System;
    using System.Runtime.InteropServices;

    public class EvtLevelState : BaseLevelEvent
    {
        public readonly int cgId;
        public readonly string endReason;
        public readonly LevelEndReason levelEndReason;
        public readonly State state;

        public EvtLevelState(State state, LevelEndReason reason = 0, int cgId = 0)
        {
            this.state = state;
            this.levelEndReason = reason;
            this.cgId = cgId;
        }

        public override string ToString()
        {
            return string.Format("level state: {0} with reason : {1} (with cgID {2})", this.state.ToString(), this.levelEndReason.ToString(), this.cgId.ToString());
        }

        public enum LevelEndReason
        {
            EndUncertainReason,
            EndWin,
            EndLoseNotMeetCondition,
            EndLoseAllDead,
            EndLoseQuit
        }

        public enum State
        {
            Start,
            EndWin,
            EndLose,
            EnterTransition,
            ExitTransition,
            PostStageReady
        }
    }
}

