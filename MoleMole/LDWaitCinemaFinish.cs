namespace MoleMole
{
    using CinemaDirector;
    using System;

    public class LDWaitCinemaFinish : BaseLDEvent
    {
        private Cutscene _cinema;

        public LDWaitCinemaFinish(Cutscene cinema)
        {
            this._cinema = cinema;
        }

        public override void OnEvent(BaseEvent evt)
        {
            EvtCinemaFinish finish = evt as EvtCinemaFinish;
            if ((finish != null) && (finish.GetCutscene() == this._cinema))
            {
                base.Done();
            }
        }
    }
}

