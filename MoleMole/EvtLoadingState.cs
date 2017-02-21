namespace MoleMole
{
    using System;

    public class EvtLoadingState : BaseLevelEvent
    {
        public readonly State state;

        public EvtLoadingState(State state)
        {
            this.state = state;
        }

        public override string ToString()
        {
            return string.Format("Loading state: {0}", this.state.ToString());
        }

        public enum State
        {
            Start,
            Destroy
        }
    }
}

