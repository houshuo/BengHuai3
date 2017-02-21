namespace MoleMole
{
    using System;
    using System.Runtime.InteropServices;

    public class LDWaitAllMonsterLessThanSpecifiedNum : BaseLDEvent
    {
        private int _num;

        public LDWaitAllMonsterLessThanSpecifiedNum(double num = 0)
        {
            int num2 = Singleton<MonsterManager>.Instance.MonsterCount();
            if ((num != 0.0) && (num2 <= num))
            {
                base.Done();
            }
            else if (num == 0.0)
            {
            }
            this._num = (int) num;
        }

        public override void OnEvent(BaseEvent evt)
        {
            if ((evt is EvtKilled) && (Singleton<RuntimeIDManager>.Instance.ParseCategory(evt.targetID) == 4))
            {
                int num = 0;
                foreach (BaseMonoMonster monster in Singleton<MonsterManager>.Instance.GetAllMonsters())
                {
                    if (monster.IsActive())
                    {
                        num++;
                    }
                }
                if (num <= this._num)
                {
                    base.Done();
                }
            }
        }
    }
}

