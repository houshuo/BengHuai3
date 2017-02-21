namespace MoleMole
{
    using MoleMole.Config;
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public class AbilityHitExplodeRandomPosBulletMixin : AbilityHitExplodeBulletMixin
    {
        private BaseMonoEntity _attackTarget;
        private List<BulletInfo> _bulletInfoList;
        private string _bulletName;
        private int _bulletNum;
        private int _bulletNumCount;
        private float _internalTimer;
        private List<int> _posIndexOrderList;
        private HitExplodeRandomPosBulletMixin config;
        private int randPosIx;

        public AbilityHitExplodeRandomPosBulletMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config) : base(instancedAbility, instancedModifier, config)
        {
            this.config = (HitExplodeRandomPosBulletMixin) config;
            this.randPosIx = 0;
            this._bulletInfoList = new List<BulletInfo>();
        }

        public override void Core()
        {
            base.Core();
            if (this._bulletNumCount < this._bulletNum)
            {
                this.CreateBullet();
            }
            for (int i = 0; i < this._bulletInfoList.Count; i++)
            {
                if (this._bulletInfoList[i] != null)
                {
                    BulletInfo info = this._bulletInfoList[i];
                    AbilityTriggerBullet actor = Singleton<EventManager>.Instance.GetActor<AbilityTriggerBullet>(info.bulletID);
                    if (actor != null)
                    {
                        this._attackTarget = base.entity.GetAttackTarget();
                        if (this._attackTarget == null)
                        {
                            this.SelectATarget();
                        }
                        if (info.holdTimer > 0f)
                        {
                            actor.triggerBullet.SetupAtReset();
                            actor.triggerBullet.SetCollisionEnabled(false);
                            info.holdTimer -= Time.deltaTime * base.entity.TimeScale;
                            BaseMonoAvatar localAvatar = Singleton<AvatarManager>.Instance.GetLocalAvatar();
                            if (localAvatar != null)
                            {
                                actor.triggerBullet.transform.position = localAvatar.XZPosition + info.startPosRelative;
                            }
                            if (this._attackTarget != null)
                            {
                                Vector3 position = this._attackTarget.GetAttachPoint("RootNode").position;
                                actor.triggerBullet.transform.forward = position - actor.triggerBullet.transform.position;
                            }
                        }
                        else if (info.backTimer > 0f)
                        {
                            actor.triggerBullet.SetCollisionEnabled(false);
                            info.backTimer -= Time.deltaTime * base.entity.TimeScale;
                            Transform transform = actor.triggerBullet.transform;
                            transform.position -= (Vector3) (actor.triggerBullet.transform.forward.normalized * Mathf.Lerp(0f, this.config.BackDistance, 1f - (info.backTimer / this.config.BackTime)));
                            if (info.backTimer <= 0f)
                            {
                                this.FireBulletEffectSmoke(actor.triggerBullet);
                            }
                        }
                        else if (info.lifeTimer > 0f)
                        {
                            actor.triggerBullet.SetCollisionEnabled(true);
                            if (this._attackTarget != null)
                            {
                                actor.triggerBullet.SetupTracing(this._attackTarget.GetAttachPoint("RootNode").position, 99f, 0f, false);
                            }
                            else
                            {
                                actor.triggerBullet.SetupLinear();
                            }
                            info.lifeTimer -= Time.deltaTime * base.entity.TimeScale;
                        }
                        else
                        {
                            this._bulletInfoList[i] = null;
                        }
                    }
                }
            }
        }

        private void CreateBullet()
        {
            this._attackTarget = base.entity.GetAttackTarget();
            if (this._attackTarget == null)
            {
                this.SelectATarget();
            }
            switch (this.config.Type)
            {
                case HitExplodeRandomPosBulletMixin.CreateType.CreateOne:
                    this.DoCreateOne();
                    break;

                case HitExplodeRandomPosBulletMixin.CreateType.CreateAllAtSameTime:
                    this.DoCreateAllAtSameTime();
                    break;

                case HitExplodeRandomPosBulletMixin.CreateType.CreateAllInterval:
                    this.DoCreateAllInterval();
                    break;
            }
        }

        private BulletInfo CreateOneBullet()
        {
            AbilityTriggerBullet bullet = Singleton<DynamicObjectManager>.Instance.CreateAbilityLinearTriggerBullet(this._bulletName, base.actor, base.instancedAbility.Evaluate(this.config.BulletSpeed), this.config.Targetting, this.config.IgnoreTimeScale, Singleton<DynamicObjectManager>.Instance.GetNextSyncedDynamicObjectRuntimeID(), -1f);
            Vector3 zero = Vector3.zero;
            this.InitBulletPosAndForward(bullet, out zero);
            BulletInfo info = new BulletInfo {
                bulletID = bullet.runtimeID,
                backTimer = this.config.BackTime,
                holdTimer = this.config.HoldTime,
                lifeTimer = this.config.LifeTime,
                startPosRelative = zero
            };
            base._bulletAttackDatas.Add(bullet.runtimeID, DamageModelLogic.CreateAttackDataFromAttackerAnimEvent(base.actor, this.config.HitAnimEventID));
            bullet.triggerBullet.acceleration = this.config.Acceleration;
            bullet.triggerBullet.SetCollisionEnabled(false);
            return info;
        }

        private void DoCreateAllAtSameTime()
        {
            for (int i = 0; i < this._bulletNum; i++)
            {
                int num2 = this._bulletInfoList.SeekAddPosition<BulletInfo>();
                this._bulletInfoList[num2] = this.CreateOneBullet();
                this._bulletNumCount++;
                BulletInfo local1 = this._bulletInfoList[num2];
                local1.holdTimer += (this._bulletNumCount - 1) * this.config.ShootInternalTime;
            }
        }

        private void DoCreateAllInterval()
        {
            this._internalTimer -= Time.deltaTime;
            if (this._internalTimer <= 0f)
            {
                int num = this._bulletInfoList.SeekAddPosition<BulletInfo>();
                this._bulletInfoList[num] = this.CreateOneBullet();
                this._bulletNumCount++;
                BulletInfo local1 = this._bulletInfoList[num];
                local1.holdTimer += (this._bulletNumCount - 1) * this.config.ShootInternalTime;
                this._internalTimer = this.config.CreateInternalTime;
            }
        }

        private void DoCreateOne()
        {
            int num = this._bulletInfoList.SeekAddPosition<BulletInfo>();
            this._bulletInfoList[num] = this.CreateOneBullet();
            this._bulletNumCount++;
        }

        private void FireBulletEffectSmoke(MonoTriggerBullet triggerBullet)
        {
            if ((this.config.BulletEffect != null) && (this.config.BulletEffect.EffectPattern != null))
            {
                Singleton<EffectManager>.Instance.TriggerEntityEffectPattern(this.config.BulletEffect.EffectPattern, triggerBullet, this.config.BulletEffectGround);
            }
        }

        private void GeneralPosIndexOrderList(bool needShuffle)
        {
            if (this._posIndexOrderList == null)
            {
                this._posIndexOrderList = new List<int>();
                for (int i = 0; i < this.config.RandomPosPool.Length; i++)
                {
                    this._posIndexOrderList.Add(i);
                }
            }
            if (needShuffle)
            {
                this._posIndexOrderList.Shuffle<int>();
            }
        }

        private void InitBulletPosAndForward(AbilityTriggerBullet bullet, out Vector3 startPos)
        {
            Vector3 forward;
            Vector3 zero = Vector3.zero;
            if (this.config.RandomPosPool.Length > 0)
            {
                this.randPosIx = ((this.randPosIx + 1) != this.config.RandomPosPool.Length) ? (this.randPosIx + 1) : 0;
                float[] numArray = this.config.RandomPosPool[this._posIndexOrderList[this.randPosIx]];
                zero = new Vector3(numArray[0], numArray[1], numArray[2]);
            }
            startPos = bullet.triggerBullet.transform.TransformPoint(zero) - bullet.triggerBullet.transform.localPosition;
            Transform transform = bullet.triggerBullet.transform;
            transform.position += startPos;
            BaseMonoEntity entity = this._attackTarget;
            if ((entity == null) || !this.config.FaceTarget)
            {
                forward = base.entity.transform.forward;
            }
            else
            {
                forward = entity.GetAttachPoint("RootNode").position - bullet.triggerBullet.transform.position;
            }
            bullet.triggerBullet.transform.forward = forward;
            if (this.config.BackDistance > 0f)
            {
                Transform transform2 = bullet.triggerBullet.transform;
                transform2.position += (Vector3) (forward.normalized * this.config.BackDistance);
            }
        }

        public override void OnAbilityTriggered(EvtAbilityStart evt)
        {
            this._bulletNum = (this.config.Type != HitExplodeRandomPosBulletMixin.CreateType.CreateOne) ? this.config.RandomPosPool.Length : 1;
            this._bulletNumCount = 0;
            this._bulletName = this.config.BulletTypeName;
            HitExplodeTracingBulletMixinArgument abilityArgument = evt.abilityArgument as HitExplodeTracingBulletMixinArgument;
            if (abilityArgument != null)
            {
                if (abilityArgument.BulletName != null)
                {
                    this._bulletName = abilityArgument.BulletName;
                }
                if (abilityArgument.RandomBulletNames != null)
                {
                    this._bulletName = abilityArgument.RandomBulletNames[UnityEngine.Random.Range(0, abilityArgument.RandomBulletNames.Length)];
                }
            }
            this._internalTimer = 0f;
            this.GeneralPosIndexOrderList(this.config.NeedShuffle);
        }

        private void SelectATarget()
        {
            if (base.entity is BaseMonoAvatar)
            {
                (base.entity as BaseMonoAvatar).SelectTarget();
                this._attackTarget = base.entity.GetAttackTarget();
            }
            else if (base.entity is BaseMonoMonster)
            {
                this._attackTarget = base.entity.GetAttackTarget();
            }
        }

        private class BulletInfo
        {
            public float backTimer;
            public uint bulletID;
            public float holdTimer;
            public float lifeTimer;
            public Vector3 startPosRelative;
        }
    }
}

