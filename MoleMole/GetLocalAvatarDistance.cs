namespace MoleMole
{
    using BehaviorDesigner.Runtime;
    using BehaviorDesigner.Runtime.Tasks;
    using UnityEngine;

    public class GetLocalAvatarDistance : Action
    {
        public SharedFloat Distance;

        public override TaskStatus OnUpdate()
        {
            Vector3 xZPosition = base.GetComponent<BaseMonoAvatar>().XZPosition;
            Vector3 b = Singleton<AvatarManager>.Instance.GetLocalAvatar().XZPosition;
            this.Distance.SetValue(Vector3.Distance(xZPosition, b));
            return TaskStatus.Success;
        }
    }
}

