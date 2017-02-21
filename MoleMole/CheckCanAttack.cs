namespace MoleMole
{
    using BehaviorDesigner.Runtime.Tasks;

    [TaskCategory("Avatar")]
    public class CheckCanAttack : BaseAvatarAction
    {
        public override TaskStatus OnUpdate()
        {
            if (base._avatarActor.CanUseSkill("ATK"))
            {
                return TaskStatus.Success;
            }
            return TaskStatus.Failure;
        }
    }
}

