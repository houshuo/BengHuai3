namespace MoleMole
{
    using CinemaDirector;
    using System;

    public class EvtCinemaReceiveMessage : BaseEvent
    {
        private Cutscene _cutScene;
        private string _messageID;

        public EvtCinemaReceiveMessage(uint targetID, Cutscene cutScene, string messageID) : base(targetID)
        {
            this._messageID = string.Empty;
            this._cutScene = cutScene;
            this._messageID = messageID;
        }

        public Cutscene GetCutscene()
        {
            return this._cutScene;
        }

        public string GetMessageID()
        {
            return this._messageID;
        }

        public override string ToString()
        {
            return string.Format("{0}  Evt SlowMotionKillFinish", base.targetID);
        }
    }
}

