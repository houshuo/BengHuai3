namespace MoleMole
{
    using BehaviorDesigner.Runtime.Tasks;
    using System;

    public abstract class BaseAvatarAction : BehaviorDesigner.Runtime.Tasks.Action
    {
        protected BaseMonoAvatar _avatar;
        protected AvatarActor _avatarActor;
        protected AvatarAIPlugin _avatarAIPlugin;

        protected BaseAvatarAction()
        {
        }

        public override void OnAwake()
        {
            this._avatar = base.GetComponent<BaseMonoAvatar>();
            this._avatarActor = Singleton<EventManager>.Instance.GetActor<AvatarActor>(this._avatar.GetRuntimeID());
            this._avatarAIPlugin = this._avatarActor.GetPlugin<AvatarAIPlugin>();
        }
    }
}

