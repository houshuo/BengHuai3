namespace MoleMole
{
    using LuaInterface;
    using System;
    using System.Collections;

    public class LDEvtWaitLocalAvatarEnterFieldTable : BaseLDEvent
    {
        private LuaTable _fieldIDTable;

        public LDEvtWaitLocalAvatarEnterFieldTable(LuaTable fieldIDTable)
        {
            this._fieldIDTable = fieldIDTable;
        }

        public override void Dispose()
        {
            this._fieldIDTable.Dispose();
        }

        public override void OnEvent(BaseEvent evt)
        {
            if (evt is EvtFieldEnter)
            {
                EvtFieldEnter enter = (EvtFieldEnter) evt;
                if (Singleton<AvatarManager>.Instance.IsLocalAvatar(enter.otherID))
                {
                    IEnumerator enumerator = this._fieldIDTable.Values.GetEnumerator();
                    try
                    {
                        while (enumerator.MoveNext())
                        {
                            object current = enumerator.Current;
                            if (evt.targetID == ((double) current))
                            {
                                base.Done();
                            }
                        }
                    }
                    finally
                    {
                        IDisposable disposable = enumerator as IDisposable;
                        if (disposable == null)
                        {
                        }
                        disposable.Dispose();
                    }
                }
            }
        }
    }
}

