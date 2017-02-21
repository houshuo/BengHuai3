namespace MoleMole
{
    using BehaviorDesigner.Runtime;
    using BehaviorDesigner.Runtime.Tasks;
    using System;
    using UnityEngine;

    [TaskCategory("Ability/TriggerAbility")]
    public class TriggerAbility : BehaviorDesigner.Runtime.Tasks.Action
    {
        private BaseMonoEntity _entity;
        public SharedFloat abilityArgument;
        public string abilityName;
        public bool isRandom;
        public float randomMax;
        public float randomMin;
        public SharedFloat ratio;

        public override void OnAwake()
        {
            this._entity = base.GetComponent<BaseMonoEntity>();
        }

        public override TaskStatus OnUpdate()
        {
            EvtAbilityStart evt = new EvtAbilityStart(this._entity.GetRuntimeID(), null) {
                abilityName = this.abilityName
            };
            float num = 1f;
            if (this.ratio.IsShared)
            {
                num = this.ratio.Value;
            }
            if (this.isRandom)
            {
                evt.abilityArgument = UnityEngine.Random.Range(this.randomMin, this.randomMax) * num;
            }
            else
            {
                evt.abilityArgument = this.abilityArgument.Value * num;
            }
            Singleton<EventManager>.Instance.FireEvent(evt, MPEventDispatchMode.Normal);
            return TaskStatus.Success;
        }
    }
}

