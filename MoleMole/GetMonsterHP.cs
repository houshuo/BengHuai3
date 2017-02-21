namespace MoleMole
{
    using BehaviorDesigner.Runtime;
    using BehaviorDesigner.Runtime.Tasks;
    using System;

    [TaskCategory("Monster")]
    public class GetMonsterHP : BehaviorDesigner.Runtime.Tasks.Action
    {
        public SharedFloat HPRatio;

        public override void OnAwake()
        {
        }

        public override void OnStart()
        {
        }

        public override TaskStatus OnUpdate()
        {
            BaseMonoMonster component = base.GetComponent<BaseMonoMonster>();
            MonsterActor actor = Singleton<EventManager>.Instance.GetActor<MonsterActor>(component.GetRuntimeID());
            this.HPRatio.SetValue((float) (actor.HP / actor.maxHP));
            return TaskStatus.Success;
        }
    }
}

