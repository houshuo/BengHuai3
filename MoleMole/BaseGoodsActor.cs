namespace MoleMole
{
    using System;

    public abstract class BaseGoodsActor : BaseActor
    {
        protected MonoGoods _entity;

        protected BaseGoodsActor()
        {
        }

        public abstract void DoGoodsLogic(uint avatarRuntimeID);
        public override void Init(BaseMonoEntity entity)
        {
            this._entity = entity as MonoGoods;
            base.runtimeID = this._entity.GetRuntimeID();
        }

        protected void Kill()
        {
            this._entity.SetDied();
            Singleton<EventManager>.Instance.FireEvent(new EvtKilled(base.runtimeID), MPEventDispatchMode.Normal);
        }

        public override bool OnEvent(BaseEvent evt)
        {
            return ((evt is EvtFieldEnter) && this.OnFieldEnter((EvtFieldEnter) evt));
        }

        private bool OnFieldEnter(EvtFieldEnter evt)
        {
            uint otherID = evt.otherID;
            this.DoGoodsLogic(otherID);
            return true;
        }
    }
}

