namespace MoleMole
{
    using LuaInterface;
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public class LDEvtOnMultiMonsterKilled : BaseLDEvent
    {
        private List<MonsterActor> _monsterActorList = new List<MonsterActor>();

        public LDEvtOnMultiMonsterKilled(LuaTable monsterIDTable)
        {
            IEnumerator enumerator = monsterIDTable.Values.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    object current = enumerator.Current;
                    this._monsterActorList.Add(Singleton<EventManager>.Instance.GetActor<MonsterActor>((uint) ((double) current)));
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
            if (this._monsterActorList.Count > 0)
            {
                if (evt is EvtKilled)
                {
                    for (int i = 0; i < this._monsterActorList.Count; i++)
                    {
                        if (this._monsterActorList[i].runtimeID == evt.targetID)
                        {
                            this._monsterActorList.RemoveAt(i);
                        }
                    }
                }
                if (this._monsterActorList.Count == 0)
                {
                    base.Done();
                }
            }
        }
    }
}

