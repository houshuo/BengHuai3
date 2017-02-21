namespace MoleMole
{
    using System;

    public class EvadeEntityDummy : BaseActor
    {
        private BaseMonoDynamicObject _dynamicObject;

        public override void Init(BaseMonoEntity entity)
        {
            this._dynamicObject = (BaseMonoDynamicObject) entity;
            base.runtimeID = this._dynamicObject.GetRuntimeID();
        }

        public void Kill()
        {
            this._dynamicObject.SetDied();
            Singleton<EventManager>.Instance.FireEvent(new EvtKilled(base.runtimeID), MPEventDispatchMode.Normal);
        }

        private bool OnBeingHit(EvtBeingHit evt)
        {
            if (this._dynamicObject.IsActive())
            {
                Singleton<EventManager>.Instance.FireEvent(new EvtEvadeSuccess(base.ownerID, evt.sourceID, evt.animEventID, evt.attackData), MPEventDispatchMode.CheckRemoteMode);
                this.Kill();
            }
            return true;
        }

        public override bool OnEvent(BaseEvent evt)
        {
            return ((evt is EvtBeingHit) && this.OnBeingHit((EvtBeingHit) evt));
        }

        public void Setup(uint ownerID)
        {
            base.ownerID = ownerID;
        }
    }
}

