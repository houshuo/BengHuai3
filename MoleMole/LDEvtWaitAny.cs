namespace MoleMole
{
    using LuaInterface;
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public class LDEvtWaitAny : BaseLDEvent
    {
        private List<BaseLDEvent> _LDEventList = new List<BaseLDEvent>();

        public LDEvtWaitAny(LuaTable LDEventTable)
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
            bool flag = false;
            foreach (BaseLDEvent event2 in this._LDEventList)
            {
                if (event2.isDone)
                {
                    flag = true;
                    break;
                }
            }
            if (flag)
            {
                foreach (BaseLDEvent event3 in this._LDEventList)
                {
                    event3.Done();
                }
                base.Done();
            }
        }
    }
}

