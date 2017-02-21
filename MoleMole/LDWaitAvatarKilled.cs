namespace MoleMole
{
    using System;
    using System.Collections.Generic;

    public class LDWaitAvatarKilled : BaseLDEvent
    {
        private int _diedAvatarNum;
        private List<uint> _playAvatarIDs = new List<uint>();
        private int _targetNum;

        public LDWaitAvatarKilled(double diedAvatarNum)
        {
            foreach (BaseMonoAvatar avatar in Singleton<AvatarManager>.Instance.GetAllPlayerAvatars())
            {
                this._playAvatarIDs.Add(avatar.GetRuntimeID());
                this._targetNum = (int) diedAvatarNum;
                this._diedAvatarNum = 0;
            }
        }

        public override void OnEvent(BaseEvent evt)
        {
            if ((evt is EvtKilled) && this._playAvatarIDs.Contains(evt.targetID))
            {
                this._diedAvatarNum++;
                if (this._diedAvatarNum >= this._targetNum)
                {
                    base.Done();
                }
            }
        }
    }
}

