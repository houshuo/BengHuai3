namespace MoleMole
{
    using MoleMole.Config;
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public class AbilityMonsterSummonAttack : BaseAbilityMixin
    {
        private MonsterSummonMixin config;
        private List<SummonItem> summonList;
        private List<SummonItem> summonListDelete;

        public AbilityMonsterSummonAttack(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config) : base(instancedAbility, instancedModifier, config)
        {
            this.summonList = new List<SummonItem>();
            this.summonListDelete = new List<SummonItem>();
            this.config = (MonsterSummonMixin) config;
        }

        private Vector3 AdjustLevelCollision(Vector3 origin, Vector3 offset)
        {
            float num = 0.2f;
            Vector3 vector = offset;
            int num2 = 4;
            for (int i = 0; i < num2; i++)
            {
                RaycastHit hit;
                Ray ray = new Ray(origin + ((Vector3) (Vector3.up * num)), vector.normalized);
                if (!Physics.Raycast(ray, out hit, vector.magnitude, (((int) 1) << InLevelData.OBSTACLE_COLLIDER_LAYER) | (((int) 1) << InLevelData.STAGE_COLLIDER_LAYER)))
                {
                    break;
                }
                vector = (Vector3) (Quaternion.AngleAxis(360f / ((float) num2), Vector3.up) * vector);
            }
            return (origin + vector);
        }

        private Vector3 CalculateSummonPosition(Vector3 summonPosition)
        {
            BaseMonoAvatar localAvatar = Singleton<AvatarManager>.Instance.GetLocalAvatar();
            if (localAvatar != null)
            {
                RaycastHit hit;
                bool flag = false;
                Vector3 start = new Vector3(localAvatar.transform.position.x, 0.1f, localAvatar.transform.position.z);
                Vector3 end = new Vector3(summonPosition.x, 0.1f, summonPosition.z);
                if (Physics.Linecast(start, end, out hit, (((int) 1) << InLevelData.OBSTACLE_COLLIDER_LAYER) | (((int) 1) << InLevelData.STAGE_COLLIDER_LAYER)))
                {
                    flag = true;
                }
                if (flag)
                {
                    Vector3 point = hit.point;
                    Vector3 vector8 = start - end;
                    Vector3 normalized = vector8.normalized;
                    float num = 0.1f;
                    Vector3 vector5 = point + ((Vector3) (normalized * num));
                    return new Vector3(vector5.x, summonPosition.y, vector5.z);
                }
            }
            return summonPosition;
        }

        private Vector3 CalculateSummonPosition(bool baseType, float distance, float angle)
        {
            Vector3 zero = Vector3.zero;
            BaseMonoEntity attackTarget = null;
            if (baseType)
            {
                attackTarget = base.entity.GetAttackTarget();
            }
            else
            {
                attackTarget = base.entity;
            }
            if (attackTarget != null)
            {
                zero = this.AdjustLevelCollision(attackTarget.XZPosition, (Vector3) ((Quaternion.Euler(0f, angle, 0f) * attackTarget.transform.forward) * distance));
            }
            return zero;
        }

        public override void Core()
        {
            float num = Time.deltaTime * base.entity.TimeScale;
            int num2 = 0;
            int count = this.summonList.Count;
            while (num2 < count)
            {
                SummonItem item = this.summonList[num2];
                item.effectTimer -= num;
                item.summonTimer -= num;
                if (!item.effectFired && (item.effectTimer <= 0f))
                {
                    this.SummonEffect(item.summonPosition, this.config.SummonEffect);
                    item.effectFired = true;
                }
                if (item.summonTimer <= 0f)
                {
                    this.SummonMonster(item.summon, item.summonPosition);
                }
                if ((item.effectTimer <= 0f) && (item.summonTimer <= 0f))
                {
                    this.summonListDelete.Add(item);
                }
                num2++;
            }
            if (this.summonListDelete.Count > 0)
            {
                int num4 = 0;
                int num5 = this.summonListDelete.Count;
                while (num4 < num5)
                {
                    this.summonList.Remove(this.summonListDelete[num4]);
                    num4++;
                }
                this.summonListDelete.Clear();
            }
        }

        public override void OnAbilityTriggered(EvtAbilityStart evt)
        {
            this.OnSummon(evt);
        }

        private void OnSummon(EvtAbilityStart evt)
        {
            MonsterSummonMixinArgument abilityArgument = evt.abilityArgument as MonsterSummonMixinArgument;
            BaseMonoAnimatorEntity animEntity = base.entity as BaseMonoAnimatorEntity;
            for (int i = 2; i < 6; i++)
            {
                if (animEntity != null)
                {
                    animEntity.StartFadeAnimatorLayerWeight(i, 0f, 0.01f);
                }
            }
            if (abilityArgument != null)
            {
                MixinSummonItem item = this.config.SummonMonsters[abilityArgument.SummonMonsterIndex];
                this.OrderSummonMonster(item.EffectDelay, item.SummonDelay, item, evt);
                this.SetAnimatorLayer(item, animEntity);
            }
            else
            {
                int index = 0;
                int length = this.config.SummonMonsters.Length;
                while (index < length)
                {
                    MixinSummonItem item2 = this.config.SummonMonsters[index];
                    this.OrderSummonMonster(item2.EffectDelay, item2.SummonDelay, item2, evt);
                    this.SetAnimatorLayer(item2, animEntity);
                    index++;
                }
            }
        }

        private void OrderSummonMonster(float effectDelay, float summonDelay, MixinSummonItem item, EvtAbilityStart evt)
        {
            SummonItem item2 = new SummonItem {
                effectTimer = effectDelay,
                summonTimer = effectDelay + summonDelay,
                summon = item
            };
            if (evt.hitCollision != null)
            {
                item2.summonPosition = evt.hitCollision.hitPoint;
                item2.summonPosition.y = 0f;
                item2.summonPosition = this.CalculateSummonPosition(item2.summonPosition);
            }
            else
            {
                item2.summonPosition = this.CalculateSummonPosition(item.BaseOnTarget, item.Distance, item.Angle);
                item2.summonPosition = this.CalculateSummonPosition(item2.summonPosition);
            }
            this.summonList.Add(item2);
        }

        private void SetAnimatorLayer(MixinSummonItem item, BaseMonoAnimatorEntity animEntity)
        {
            if (item.UseCoffinAnim && (animEntity != null))
            {
                animEntity.StartFadeAnimatorLayerWeight(item.CoffinIndex + 1, 1f, 0.01f);
            }
        }

        private void SummonEffect(Vector3 position, MixinEffect effect)
        {
            if (effect != null)
            {
                if (effect.EffectPattern != null)
                {
                    base.entity.FireEffect(effect.EffectPattern, position, Vector3.forward);
                }
                if (effect.AudioPattern != null)
                {
                    base.entity.PlayAudio(effect.AudioPattern);
                }
            }
        }

        private void SummonMonster(MixinSummonItem item, Vector3 position)
        {
            uint runtimeID = Singleton<MonsterManager>.Instance.CreateMonster(base.instancedAbility.Evaluate(item.MonsterName), base.instancedAbility.Evaluate(item.TypeName), Singleton<LevelScoreManager>.Instance.NPCHardLevel, true, position, 0, false, 0, true, false, 0);
            BaseAbilityActor actor = Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(runtimeID);
            actor.ownerID = base.actor.runtimeID;
            if (item.Abilities != null)
            {
                foreach (KeyValuePair<string, ConfigEntityAbilityEntry> pair in item.Abilities)
                {
                    actor.abilityPlugin.AddAbility(AbilityData.GetAbilityConfig(pair.Value.AbilityName, pair.Value.AbilityOverride));
                }
            }
        }

        private class SummonItem
        {
            public bool effectFired;
            public float effectTimer;
            public MixinSummonItem summon;
            public Vector3 summonPosition;
            public float summonTimer;
        }
    }
}

