namespace MoleMole
{
    using System;
    using System.Runtime.InteropServices;

    public class LDWaitAllMonsterClear : BaseLDEvent
    {
        private int _num;

        public LDWaitAllMonsterClear(double num = 0)
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

        public override void Core()
        {
            if (Singleton<MonsterManager>.Instance.MonsterCount() <= this._num)
            {
                base.Done();
            }
        }
    }
}

