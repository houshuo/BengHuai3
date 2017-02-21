namespace MoleMole
{
    using BehaviorDesigner.Runtime;
    using BehaviorDesigner.Runtime.Tasks;
    using System;

    [TaskIcon("{SkinColor}SequenceIcon.png"), TaskDescription("Clear AttackNum On End")]
    public class AttackSequence : Composite
    {
        public SharedInt avatarAttackNum;
        private int currentChildIndex;
        private TaskStatus executionStatus;
        public SharedBool IsAttacking;

        public override bool CanExecute()
        {
            return ((this.currentChildIndex < base.children.Count) && (this.executionStatus != TaskStatus.Failure));
        }

        public override int CurrentChildIndex()
        {
            return this.currentChildIndex;
        }

        public override void OnChildExecuted(TaskStatus childStatus)
        {
            this.currentChildIndex++;
            this.executionStatus = childStatus;
        }

        public override void OnConditionalAbort(int childIndex)
        {
            this.currentChildIndex = childIndex;
            this.executionStatus = TaskStatus.Inactive;
        }

        public override void OnEnd()
        {
            this.executionStatus = TaskStatus.Inactive;
            this.currentChildIndex = 0;
            if (this.IsAttacking.Value)
            {
                Singleton<LevelManager>.Instance.levelActor.GetPlugin<LevelAIPlugin>().RemoveAttackingMonster(base.GetComponent<BaseMonoMonster>());
                this.IsAttacking.Value = false;
            }
        }
    }
}

