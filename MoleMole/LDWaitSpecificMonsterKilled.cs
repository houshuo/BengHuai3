namespace MoleMole
{
    using System;

    public class LDWaitSpecificMonsterKilled : BaseLDEvent
    {
        private MonsterActor _monsterActor;

        public LDWaitSpecificMonsterKilled(double runtimeID)
        {
            this._monsterActor = Singleton<EventManager>.Instance.GetActor<MonsterActor>((uint) runtimeID);
        }

        public override void OnEvent(BaseEvent evt)
        {
            if ((evt is EvtKilled) && (evt.targetID == this._monsterActor.runtimeID))
            {
                base.Done();
            }
        }
    }
}

