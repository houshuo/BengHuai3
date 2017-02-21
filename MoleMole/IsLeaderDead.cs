namespace MoleMole
{
    using BehaviorDesigner.Runtime.Tasks;
    using MoleMole.Config;
    using System;

    [TaskCategory("Group")]
    public class IsLeaderDead : Conditional
    {
        public SharedGroupAttackType attackType;
        public SharedGroupMoveType moveType;
        public SharedEntity targetEntity;

        public override void OnAwake()
        {
            base.OnAwake();
        }

        public override TaskStatus OnUpdate()
        {
            if ((this.targetEntity.Value != null) && this.targetEntity.Value.IsActive())
            {
                return TaskStatus.Failure;
            }
            if ((((ConfigGroupAIMinionOld.AttackType) this.attackType.Value) == ConfigGroupAIMinionOld.AttackType.Free) && (((ConfigGroupAIMinionOld.MoveType) this.moveType.Value) == ConfigGroupAIMinionOld.MoveType.Free))
            {
                return TaskStatus.Failure;
            }
            this.attackType.Value = ConfigGroupAIMinionOld.AttackType.Free;
            this.moveType.Value = ConfigGroupAIMinionOld.MoveType.Free;
            return TaskStatus.Success;
        }
    }
}

