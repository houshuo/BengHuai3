namespace MoleMole
{
    using BehaviorDesigner.Runtime.Tasks;
    using System;

    public class SetSkillAnimatorPattern : BehaviorDesigner.Runtime.Tasks.Action
    {
        public string animatorEventPatternName;
        public string skillName;

        public override TaskStatus OnUpdate()
        {
            BaseMonoAnimatorEntity component = base.GetComponent<BaseMonoAnimatorEntity>();
            if (component is BaseMonoMonster)
            {
                (component as BaseMonoMonster).SetSoleSkillAnimatorEventPattern(this.skillName, this.animatorEventPatternName);
            }
            return TaskStatus.Success;
        }
    }
}

