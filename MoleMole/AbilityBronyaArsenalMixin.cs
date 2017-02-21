namespace MoleMole
{
    using MoleMole.Config;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    public class AbilityBronyaArsenalMixin : BaseAbilityMixin
    {
        private List<CannonInfo> _cannonList;
        private int _chargeIndex;
        private BaseMonoMonster _monster;
        private float _shakeTimer;
        private ArsenalState _state;
        private float _Timer;
        [CompilerGenerated]
        private static Comparison<CannonInfo> <>f__am$cache7;
        private BronyaArsenalMixin config;

        public AbilityBronyaArsenalMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config) : base(instancedAbility, instancedModifier, config)
        {
            this.config = (BronyaArsenalMixin) config;
            this._monster = base.entity as BaseMonoMonster;
            this._cannonList = new List<CannonInfo>();
            this._chargeIndex = 0;
            this._Timer = 0f;
            this._state = ArsenalState.None;
        }

        private void ClearCannons()
        {
            foreach (CannonInfo info in this._cannonList)
            {
                this.DestroyEffects(info.chargeEffects);
                this.DestroyEffects(info.cannonEffects);
            }
            this._cannonList.Clear();
        }

        public override void Core()
        {
            base.Core();
            this._Timer += Time.deltaTime * base.entity.TimeScale;
            if (this._state != ArsenalState.None)
            {
                if (this._state == ArsenalState.Charge)
                {
                    if (this._chargeIndex < this._cannonList.Count)
                    {
                        CannonInfo cannon = this._cannonList[this._chargeIndex];
                        if (this._Timer >= cannon.delay)
                        {
                            Vector3 initDir = (Vector3) (Quaternion.FromToRotation(Vector3.forward, base.entity.transform.forward) * cannon.localForward);
                            Vector3 initPos = base.entity.XZPosition + (Quaternion.FromToRotation(Vector3.forward, base.entity.transform.forward) * cannon.localPosition);
                            cannon.position = initPos;
                            cannon.forward = initDir.normalized;
                            cannon.cannonEffects = Singleton<EffectManager>.Instance.TriggerEntityEffectPatternReturnValue(this.config.CannonEffects, initPos, initDir, Vector3.one, base.entity);
                            this.UpdateEffects(cannon.cannonEffects, initPos, initDir);
                            cannon.chargeEffects = Singleton<EffectManager>.Instance.TriggerEntityEffectPatternReturnValue(this.config.ChargeEffects, initPos, initDir, Vector3.one, base.entity);
                            this.UpdateEffects(cannon.chargeEffects, initPos, initDir);
                            Singleton<EffectManager>.Instance.TriggerEntityEffectPatternReturnValue(this.config.HintEffects, this.GetTargetPosition(cannon), Vector3.forward, Vector3.one, base.entity);
                            this._chargeIndex++;
                        }
                    }
                    if (this._Timer > this.config.ChargeTime)
                    {
                        this._state = ArsenalState.Fire;
                        this._Timer = 0f;
                        this._shakeTimer = this.config.FireIntervial;
                    }
                }
                else if (this._state == ArsenalState.Fire)
                {
                    for (int i = 0; i < this._cannonList.Count; i++)
                    {
                        CannonInfo info2 = this._cannonList[i];
                        info2.fireTimer -= Time.deltaTime * base.entity.TimeScale;
                        if (info2.fireTimer <= 0f)
                        {
                            List<MonoEffect> list = Singleton<EffectManager>.Instance.TriggerEntityEffectPatternReturnValue(this.config.ShootEffects, info2.position, info2.forward, Vector3.one, base.entity);
                            this.UpdateEffects(list, info2.position, info2.forward);
                            Vector2 vector3 = (Vector2) (UnityEngine.Random.insideUnitCircle * this.config.ExplodeRadius);
                            Vector3 vector4 = new Vector3(vector3.x, 0f, vector3.y);
                            Singleton<EffectManager>.Instance.TriggerEntityEffectPatternReturnValue(this.config.ExplodeEffects, this.GetTargetPosition(info2) + vector4, Vector3.forward, Vector3.one, base.entity);
                            this.CreateExplode(this.GetTargetPosition(info2));
                            info2.fireTimer = this.config.FireIntervial;
                        }
                    }
                    this._shakeTimer -= Time.deltaTime * base.entity.TimeScale;
                    if (this._shakeTimer <= 0f)
                    {
                        ConfigMonsterAnimEvent event2 = SharedAnimEventData.ResolveAnimEvent(this._monster.config, this.config.ShakeAnimEventID);
                        if (event2.CameraShake != null)
                        {
                            AttackPattern.ActCameraShake(event2.CameraShake);
                        }
                        this._shakeTimer = this.config.FireIntervial;
                    }
                    if (this._Timer > this.config.FireTime)
                    {
                        this._state = ArsenalState.Disappear;
                        this._Timer = 0f;
                    }
                }
                else if (this._Timer > this.config.ClearTime)
                {
                    this._state = ArsenalState.None;
                    this.ClearCannons();
                }
            }
        }

        private void CreateExplode(Vector3 pos)
        {
            if (this.config.ExplodeAnimEventID != null)
            {
                List<CollisionResult> list = CollisionDetectPattern.CylinderCollisionDetectBySphere(pos, pos, this.config.ExplodeRadius, this.config.ExplodeRadius, Singleton<EventManager>.Instance.GetAbilityHitboxTargettingMask(base.actor.runtimeID, MixinTargetting.Enemy));
                for (int i = 0; i < list.Count; i++)
                {
                    CollisionResult result = list[i];
                    BaseMonoEntity collisionResultEntity = AttackPattern.GetCollisionResultEntity(result.entity);
                    if (collisionResultEntity != null)
                    {
                        Singleton<EventManager>.Instance.FireEvent(new EvtHittingOther(base.actor.runtimeID, collisionResultEntity.GetRuntimeID(), this.config.ExplodeAnimEventID, result.hitPoint, result.hitForward), MPEventDispatchMode.Normal);
                    }
                }
            }
        }

        private void DestroyEffects(List<MonoEffect> _effects)
        {
            if (_effects != null)
            {
                foreach (MonoEffect effect in _effects)
                {
                    effect.SetDestroyImmediately();
                }
                _effects.Clear();
            }
        }

        private Vector3 GetTargetPosition(CannonInfo cannon)
        {
            float num = Mathf.Abs((float) (cannon.position.y / cannon.forward.y));
            return (cannon.position + ((Vector3) (cannon.forward * num)));
        }

        private void InitCannonInfo()
        {
            MonoEffect effect = Singleton<EffectManager>.Instance.TriggerEntityEffectPatternReturnValue(this.config.PositionsEffect, base.entity.XZPosition, base.entity.transform.forward, Vector3.one, base.entity)[0];
            int num = Mathf.Min(this.config.DelayList.Length, effect.transform.childCount);
            for (int i = 0; i < num; i++)
            {
                CannonInfo item = new CannonInfo {
                    localPosition = effect.transform.GetChild(i).localPosition
                };
                this._cannonList.Add(item);
            }
            if (<>f__am$cache7 == null)
            {
                <>f__am$cache7 = (x, y) => (int) (x.localPosition.magnitude - y.localPosition.magnitude);
            }
            this._cannonList.Sort(<>f__am$cache7);
            for (int j = 0; j < this._cannonList.Count; j++)
            {
                float angle = 22f - Mathf.Clamp(this._cannonList[j].localPosition.y, 2f, 6f);
                this._cannonList[j].delay = this.config.DelayList[j];
                this._cannonList[j].localForward = (Vector3) (Quaternion.AngleAxis(angle, Vector3.right) * Vector3.forward);
                this._cannonList[j].fireTimer = UnityEngine.Random.Range(this.config.FireIntervial, this.config.FireIntervial * 2f);
            }
            effect.SetDestroy();
        }

        public override void OnAbilityTriggered(EvtAbilityStart evt)
        {
            this._Timer = 0f;
            this.ClearCannons();
            this.InitCannonInfo();
            this._state = ArsenalState.Charge;
            this._chargeIndex = 0;
        }

        private void UpdateEffects(List<MonoEffect> _effects, Vector3 _pos, Vector3 _dir)
        {
            if (_effects != null)
            {
                foreach (MonoEffect effect in _effects)
                {
                    effect.transform.position = _pos;
                    effect.transform.forward = _dir;
                }
            }
        }

        private enum ArsenalState
        {
            None,
            Charge,
            Fire,
            Disappear
        }

        private class CannonInfo
        {
            public List<MonoEffect> cannonEffects;
            public List<MonoEffect> chargeEffects;
            public float delay;
            public float fireTimer;
            public Vector3 forward;
            public Vector3 localForward;
            public Vector3 localPosition;
            public Vector3 position;
            public Vector3 targetPositon = Vector3.zero;
        }
    }
}

