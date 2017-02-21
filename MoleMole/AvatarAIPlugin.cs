namespace MoleMole
{
    using BehaviorDesigner.Runtime;
    using System;

    public class AvatarAIPlugin : BaseActorPlugin
    {
        protected BehaviorDesigner.Runtime.BehaviorTree _aiTree;
        private LevelActor _levelActor;
        protected AvatarActor _owner;

        public AvatarAIPlugin(AvatarActor owner)
        {
            this._owner = owner;
            this._aiTree = this._owner.avatar.GetComponent<BehaviorDesigner.Runtime.BehaviorTree>();
            this._levelActor = Singleton<LevelManager>.Instance.levelActor;
        }

        private bool ListenAttackStart(EvtAttackStart evt)
        {
            if ((this._owner.avatar.AttackTarget != null) && (this._owner.avatar.AttackTarget.GetRuntimeID() == evt.targetID))
            {
                this._aiTree.SendEvent("AITargetAttackStart_0");
            }
            return false;
        }

        public override bool ListenEvent(BaseEvent evt)
        {
            if (evt is EvtStageReady)
            {
                return this.ListenStageReady((EvtStageReady) evt);
            }
            if (!this._owner.avatar.IsAIActive())
            {
                return false;
            }
            return ((evt is EvtAttackStart) && this.ListenAttackStart((EvtAttackStart) evt));
        }

        private bool ListenStageReady(EvtStageReady evt)
        {
            if (evt.isBorn)
            {
                this.Preparation();
                Singleton<EventManager>.Instance.RemoveEventListener<EvtStageReady>(this._owner.runtimeID);
            }
            return false;
        }

        public override void OnAdded()
        {
            if (this._levelActor.levelState == LevelActor.LevelState.LevelRunning)
            {
                this.Preparation();
            }
            else
            {
                Singleton<EventManager>.Instance.RegisterEventListener<EvtStageReady>(this._owner.runtimeID);
            }
            Singleton<EventManager>.Instance.RegisterEventListener<EvtAttackStart>(this._owner.runtimeID);
        }

        private void Preparation()
        {
        }
    }
}

