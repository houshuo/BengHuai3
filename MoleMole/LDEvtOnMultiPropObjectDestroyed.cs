namespace MoleMole
{
    using LuaInterface;
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public class LDEvtOnMultiPropObjectDestroyed : BaseLDEvent
    {
        private List<PropObjectActor> _propObjectActorList = new List<PropObjectActor>();

        public LDEvtOnMultiPropObjectDestroyed(LuaTable propIDTable)
        {
            IEnumerator enumerator = propIDTable.Values.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    object current = enumerator.Current;
                    this._propObjectActorList.Add(Singleton<EventManager>.Instance.GetActor<PropObjectActor>((uint) ((double) current)));
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

        public override void OnEvent(BaseEvent evt)
        {
            if (this._propObjectActorList.Count > 0)
            {
                if (evt is EvtKilled)
                {
                    for (int i = 0; i < this._propObjectActorList.Count; i++)
                    {
                        if (this._propObjectActorList[i].runtimeID == evt.targetID)
                        {
                            this._propObjectActorList.RemoveAt(i);
                        }
                    }
                }
                if (this._propObjectActorList.Count == 0)
                {
                    base.Done();
                }
            }
        }
    }
}

