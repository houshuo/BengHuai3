namespace MoleMole
{
    using System;

    public class EvtStoryState : BaseEvent
    {
        private State _state;

        public EvtStoryState(uint targetID, State state) : base(targetID)
        {
            this._state = state;
        }

        public override string ToString()
        {
            return string.Format("{0}  Evt Story State : {1}", base.targetID, this._state.ToString());
        }

        public State StoryState
        {
            get
            {
                return this._state;
            }
        }

        public enum State
        {
            None,
            Begin,
            Finish
        }
    }
}

