namespace MoleMole
{
    using BehaviorDesigner.Runtime.Tasks;
    using System;

    [TaskCategory("Monster"), TaskDescription("Check whether target in is any ability state, e.g. stun, paralyze, etc.")]
    public class CheckTargetAbilityState : BehaviorDesigner.Runtime.Tasks.Action
    {
        private IAIEntity _aiEntity;
        public string state;

        public override void OnAwake()
        {
            BaseMonoAnimatorEntity component = base.GetComponent<BaseMonoAnimatorEntity>();
            this._aiEntity = (BaseMonoMonster) component;
        }

        public override void OnStart()
        {
        }

        public override TaskStatus OnUpdate()
        {
            BaseMonoAnimatorEntity attackTarget = this._aiEntity.AttackTarget as BaseMonoAnimatorEntity;
            if (attackTarget != null)
            {
                AbilityState targetState = (AbilityState) ((int) Enum.Parse(typeof(AbilityState), this.state));
                BaseAbilityActor actor = Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(attackTarget.GetRuntimeID());
                if ((actor != null) && actor.abilityState.ContainsState(targetState))
                {
                    return TaskStatus.Success;
                }
            }
            return TaskStatus.Failure;
        }
    }
}

