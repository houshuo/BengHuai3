namespace MoleMole
{
    using MoleMole.Config;
    using System;
    using UnityEngine;

    public class AbilityPositionDirectionHitMixin : BaseAbilityMixin
    {
        private string[] _animEventIDs;
        private float _backHitRange;
        private float _forwardAngleRangeMax;
        private float _forwardAngleRangeMin;
        private float _posAngleRangeMax;
        private float _posAngleRangeMin;
        private PositionDirectionHitMixin config;

        public AbilityPositionDirectionHitMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config) : base(instancedAbility, instancedModifier, config)
        {
            this._backHitRange = 1f;
            this.config = (PositionDirectionHitMixin) config;
            this._forwardAngleRangeMax = instancedAbility.Evaluate(this.config.ForwardAngleRangeMax);
            this._forwardAngleRangeMin = instancedAbility.Evaluate(this.config.ForwardAngleRangeMin);
            this._posAngleRangeMin = instancedAbility.Evaluate(this.config.PosAngleRangeMin);
            this._posAngleRangeMax = instancedAbility.Evaluate(this.config.PosAngleRangeMax);
            this._backHitRange = instancedAbility.Evaluate(this.config.HitRange);
            this._animEventIDs = this.config.AnimEventIDs;
        }

        private bool OnHittingOther(EvtHittingOther evt)
        {
            bool flag = false;
            if (!evt.attackData.isAnimEventAttack)
            {
                return false;
            }
            if (this._animEventIDs != null)
            {
                if (Miscs.ArrayContains<string>(this._animEventIDs, evt.animEventID))
                {
                    flag = true;
                }
            }
            else
            {
                flag = true;
            }
            if (flag)
            {
                BaseAbilityActor actor = Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(evt.toID);
                if ((actor == null) || !actor.IsActive())
                {
                    return false;
                }
                Vector3 from = base.actor.entity.transform.position - actor.entity.transform.position;
                float num = Vector3.Angle(base.actor.entity.transform.forward, actor.entity.transform.forward);
                float num2 = Vector3.Angle(from, actor.entity.transform.forward);
                bool flag2 = (num < this._forwardAngleRangeMax) && (num > this._forwardAngleRangeMin);
                bool flag3 = (num2 < this._posAngleRangeMax) && (num2 > this._posAngleRangeMin);
                bool flag4 = from.magnitude < this._backHitRange;
                if ((flag2 && flag3) && flag4)
                {
                    evt.attackData.addedDamageRatio += base.instancedAbility.Evaluate(this.config.BackDamageRatio);
                    base.actor.abilityPlugin.HandleActionTargetDispatch(this.config.Actions, base.instancedAbility, base.instancedModifier, base.actor, evt);
                }
            }
            return true;
        }

        public override bool OnPostEvent(BaseEvent evt)
        {
            return ((evt is EvtHittingOther) && this.OnHittingOther((EvtHittingOther) evt));
        }
    }
}

