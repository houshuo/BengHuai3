namespace MoleMole
{
    using CinemaDirector;
    using System;

    public class EvtCinemaFinish : BaseEvent
    {
        private Cutscene _cutScene;

        public EvtCinemaFinish(uint targetID, Cutscene cutScene) : base(targetID)
        {
            this._cutScene = cutScene;
        }

        public Cutscene GetCutscene()
        {
            return this._cutScene;
        }

        public override string ToString()
        {
            return string.Format("{0}  Evt SlowMotionKillFinish", base.targetID);
        }
    }
}

