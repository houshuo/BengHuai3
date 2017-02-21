namespace MoleMole
{
    using System;

    public class LDEvtMonsterHPRatio : BaseLDEvent
    {
        private MonsterActor _monsterActor;
        private float _ratio;

        public LDEvtMonsterHPRatio(double runtimeID, double ratio)
        {
            this._monsterActor = Singleton<EventManager>.Instance.GetActor<MonsterActor>((uint) runtimeID);
            this._ratio = (float) ratio;
        }

        public override void OnEvent(BaseEvent evt)
        {
            if (((evt is EvtBeingHit) && (this._monsterActor != null)) && ((this._monsterActor.runtimeID == evt.targetID) && ((this._monsterActor.HP / this._monsterActor.maxHP) < this._ratio)))
            {
                base.Done();
            }
        }
    }
}

