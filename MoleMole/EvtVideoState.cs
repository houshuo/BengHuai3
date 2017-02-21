namespace MoleMole
{
    using System;

    public class EvtVideoState : BaseEvent
    {
        private State _state;

        public EvtVideoState(uint targetID, State state) : base(targetID)
        {
            this._state = state;
        }

        public override string ToString()
        {
            return string.Format("{0}  Evt Video State : {1}", base.targetID, this._state.ToString());
        }

        public State VideoState
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

