namespace MoleMole
{
    using System;

    public class EvtTutorialState : BaseLevelEvent
    {
        public readonly State state;

        public EvtTutorialState(State state)
        {
            this.state = state;
        }

        public override string ToString()
        {
            return string.Format("level state: {0}", this.state.ToString());
        }

        public enum State
        {
            Start
        }
    }
}

