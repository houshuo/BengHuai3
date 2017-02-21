namespace MoleMole
{
    using CinemaDirector;
    using System;

    public class LDWaitCinemaReceiveMessage : BaseLDEvent
    {
        private Cutscene _cinema;
        private string _messageID = string.Empty;

        public LDWaitCinemaReceiveMessage(Cutscene cinema, string messageID)
        {
            this._cinema = cinema;
            this._messageID = messageID;
        }

        public override void OnEvent(BaseEvent evt)
        {
            EvtCinemaReceiveMessage message = evt as EvtCinemaReceiveMessage;
            if (((message != null) && (message.GetCutscene() == this._cinema)) && (message.GetMessageID() == this._messageID))
            {
                base.Done();
            }
        }
    }
}

