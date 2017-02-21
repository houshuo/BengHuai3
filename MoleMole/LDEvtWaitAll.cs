namespace MoleMole
{
    using LuaInterface;
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public class LDEvtWaitAll : BaseLDEvent
    {
        private List<BaseLDEvent> _LDEventList = new List<BaseLDEvent>();

        public LDEvtWaitAll(LuaTable LDEventTable)
        {
            IEnumerator enumerator = LDEventTable.Values.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    object current = enumerator.Current;
                    BaseLDEvent item = Singleton<LevelDesignManager>.Instance.CreateLDEventFromTable((LuaTable) current);
                    this._LDEventList.Add(item);
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

        public override void Core()
        {
            foreach (BaseLDEvent event2 in this._LDEventList)
            {
                if (event2.isDone)
                {
                    this._LDEventList.Remove(event2);
                }
            }
            if (this._LDEventList.Count == 0)
            {
                base.Done();
            }
        }
    }
}

