namespace MoleMole
{
    using System;

    public class EvtCamearaAnimState : BaseEvent
    {
        private State _state;

        public EvtCamearaAnimState(uint targetID, State state) : base(targetID)
        {
            this._state = state;
        }

        public override string ToString()
        {
            return string.Format("{0}  Evt Camera Anim State : {1}", base.targetID, this._state.ToString());
        }

        public State CameraAnimState
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

