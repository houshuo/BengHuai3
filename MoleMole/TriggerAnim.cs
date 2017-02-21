namespace MoleMole
{
    using BehaviorDesigner.Runtime.Tasks;
    using System;

    public class TriggerAnim : BehaviorDesigner.Runtime.Tasks.Action
    {
        private BaseMonoAnimatorEntity _entity;
        public bool completely;
        public EntityType entityType = EntityType.None;
        public bool onlyTriggerInSkill;
        public string skillName = string.Empty;
        public string triggerName = string.Empty;

        public override void OnAwake()
        {
            this._entity = base.GetComponent<BaseMonoAnimatorEntity>();
        }

        public override TaskStatus OnUpdate()
        {
            if (this.onlyTriggerInSkill)
            {
                if ((this.entityType == EntityType.Avatar) && (base.GetComponent<BaseMonoAvatar>().CurrentSkillID != this.skillName))
                {
                    return TaskStatus.Failure;
                }
                if ((this.entityType == EntityType.Monster) && (base.GetComponent<BaseMonoMonster>().CurrentSkillID != this.skillName))
                {
                    return TaskStatus.Failure;
                }
            }
            if (this._entity.IsActive())
            {
                if (!this.completely)
                {
                    this._entity.SetTrigger(this.triggerName + "Trigger");
                }
                else
                {
                    this._entity.SetTrigger(this.triggerName);
                }
            }
            return TaskStatus.Success;
        }

        public enum EntityType
        {
            Avatar,
            Monster,
            None
        }
    }
}

