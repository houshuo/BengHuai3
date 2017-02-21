namespace MoleMole
{
    using BehaviorDesigner.Runtime;
    using BehaviorDesigner.Runtime.Tasks;
    using System;

    [TaskCategory("Monster")]
    public class SetAttackNum : BehaviorDesigner.Runtime.Tasks.Action
    {
        private IAIEntity _aiEntity;
        private bool _isTargetLocalAvatar;
        private LevelAIPlugin _levelAIPlugin;
        public SetType attackType;
        public SharedInt AvatarBeAttackNum;
        public SharedBool IsAttacking;
        private BaseMonoMonster monster;

        public override void OnAwake()
        {
            this.monster = base.GetComponent<BaseMonoMonster>();
            this._aiEntity = this.monster;
            this._levelAIPlugin = Singleton<LevelManager>.Instance.levelActor.GetPlugin<LevelAIPlugin>();
        }

        public override void OnStart()
        {
        }

        public override TaskStatus OnUpdate()
        {
            BaseMonoAvatar attackTarget = this._aiEntity.AttackTarget as BaseMonoAvatar;
            if (attackTarget == null)
            {
                this._isTargetLocalAvatar = false;
            }
            else
            {
                this._isTargetLocalAvatar = Singleton<AvatarManager>.Instance.IsLocalAvatar(attackTarget.GetRuntimeID());
            }
            if (this._isTargetLocalAvatar)
            {
                if (this.attackType == SetType.BEGIN)
                {
                    this._levelAIPlugin.AddAttackingMonster(this.monster);
                    this.IsAttacking.Value = true;
                }
                else
                {
                    this._levelAIPlugin.RemoveAttackingMonster(this.monster);
                    this.IsAttacking.Value = false;
                }
            }
            return TaskStatus.Success;
        }

        public enum SetType
        {
            BEGIN,
            END
        }
    }
}

