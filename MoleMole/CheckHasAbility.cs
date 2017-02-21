namespace MoleMole
{
    using BehaviorDesigner.Runtime.Tasks;
    using System;

    [TaskCategory("Ability/CheckHasAbility")]
    public class CheckHasAbility : BehaviorDesigner.Runtime.Tasks.Action
    {
        private BaseAbilityActor _actor;
        public string abilityName;

        public override void OnStart()
        {
            BaseMonoEntity component = base.GetComponent<BaseMonoEntity>();
            this._actor = Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(component.GetRuntimeID());
        }

        public override TaskStatus OnUpdate()
        {
            if (((this._actor != null) && (this._actor.abilityPlugin != null)) && this._actor.abilityPlugin.HasAbility(this.abilityName))
            {
                return TaskStatus.Success;
            }
            return TaskStatus.Failure;
        }
    }
}

