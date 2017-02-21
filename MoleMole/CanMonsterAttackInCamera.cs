namespace MoleMole
{
    using BehaviorDesigner.Runtime;
    using BehaviorDesigner.Runtime.Tasks;
    using System;
    using UnityEngine;

    [TaskCategory("Monster")]
    public class CanMonsterAttackInCamera : BehaviorDesigner.Runtime.Tasks.Action
    {
        private IAIEntity _aiEntity;
        public SharedFloat AttackCD;
        public SharedInt avatarBeAttackMaxNum;
        public SharedInt avatarBeAttackNum;
        private const float CAMERA_CHECK_FAR_OFFSET = 0f;
        private const float CAMERA_CHECK_FOV_OFFSET = -5f;
        private const float CAMERA_CHECK_NEAR_OFFSET = 5f;
        public SharedFloat resetAttackCDPos;
        public SharedFloat resetAttackCDTime;

        public TaskStatus CheckCanAttackWithMaxNum()
        {
            if (this.avatarBeAttackNum.Value >= this.avatarBeAttackMaxNum.Value)
            {
                this.AttackCD.Value = this.resetAttackCDTime.Value;
                return TaskStatus.Failure;
            }
            return TaskStatus.Success;
        }

        public override void OnAwake()
        {
        }

        public override void OnStart()
        {
        }

        public override TaskStatus OnUpdate()
        {
            BaseMonoEntity component = base.GetComponent<BaseMonoMonster>();
            this._aiEntity = (BaseMonoMonster) component;
            if (this._aiEntity.GetProperty("AI_IgnoreMaxAttackNumChance") > 0f)
            {
                return TaskStatus.Success;
            }
            if (!Singleton<CameraManager>.Instance.GetMainCamera().IsEntityVisibleInCustomOffset(component, -5f, 5f, 0f))
            {
                if (Singleton<CameraManager>.Instance.GetMainCamera().GetVisibleMonstersCountWithOffset(-5f, 5f, 0f) < this.avatarBeAttackMaxNum.Value)
                {
                    return this.CheckCanAttackWithMaxNum();
                }
                if (UnityEngine.Random.value < this.resetAttackCDPos.Value)
                {
                    this.AttackCD.Value = this.resetAttackCDTime.Value;
                    return TaskStatus.Failure;
                }
            }
            return this.CheckCanAttackWithMaxNum();
        }
    }
}

