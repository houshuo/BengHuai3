namespace MoleMole
{
    using System;
    using UnityEngine;

    public class LDEvtWaitForSeconds : BaseLDEvent
    {
        private float timeLeft;

        public LDEvtWaitForSeconds(double t)
        {
            this.timeLeft = (float) t;
        }

        public override void Core()
        {
            this.timeLeft -= Singleton<LevelManager>.Instance.levelEntity.TimeScale * Time.deltaTime;
            if (this.timeLeft <= 0f)
            {
                base.Done();
            }
        }
    }
}

