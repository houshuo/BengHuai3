namespace MoleMole
{
    using SimpleJSON;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    public abstract class BaseMonsterAIController : BaseController, IAIController
    {
        protected BaseMonoMonster _monster;

        public BaseMonsterAIController(BaseMonoMonster monster) : base(0x101, monster)
        {
            this._monster = monster;
        }

        public override void Core()
        {
        }

        public virtual void LoadMetaDataAndInit(BaseMonoMonster aMonster, Hashtable dynamicParamTable, Dictionary<string, MethodInfo> eventHandlerDict, JSONNode aJson)
        {
            throw new NotImplementedException();
        }

        public virtual void SetActive(bool isActive)
        {
            this.active = isActive;
        }

        public void TryClearAttackTarget()
        {
            this._monster.SetAttackTarget(null);
        }

        public void TryMove(float speed)
        {
            if (this._monster.IsAnimatorInTag(MonsterData.MonsterTagGroup.FreezeDirection) || this._monster.IsMuteControl())
            {
                this._monster.OrderMove = false;
                this._monster.MoveHorizontal = false;
            }
            else
            {
                this._monster.MoveSpeedRatio = speed;
                this._monster.MoveHorizontal = false;
                this._monster.OrderMove = true;
            }
        }

        public void TryMoveHorizontal(float speed)
        {
            if (this._monster.IsAnimatorInTag(MonsterData.MonsterTagGroup.FreezeDirection) || this._monster.IsMuteControl())
            {
                this._monster.OrderMove = false;
                this._monster.MoveHorizontal = false;
            }
            else
            {
                this._monster.MoveSpeedRatio = speed;
                this._monster.MoveHorizontal = true;
                this._monster.OrderMove = true;
            }
        }

        public void TrySetAttackTarget(BaseMonoEntity attackTarget)
        {
            this._monster.SetAttackTarget(attackTarget);
        }

        public void TrySteer(Vector3 dir)
        {
            if (this._monster.IsAnimatorInTag(MonsterData.MonsterTagGroup.FreezeDirection) || this._monster.IsMuteControl())
            {
                this._monster.OrderMove = false;
            }
            else
            {
                this._monster.SteerFaceDirectionTo(Vector3.Lerp(this._monster.FaceDirection, dir, (this._monster.config.StateMachinePattern.ChangeDirLerpRatioForMove * this._monster.TimeScale) * Time.deltaTime));
            }
        }

        public void TrySteer(Vector3 dir, float lerpRatio)
        {
            if (this._monster.IsAnimatorInTag(MonsterData.MonsterTagGroup.FreezeDirection) || this._monster.IsMuteControl())
            {
                this._monster.OrderMove = false;
            }
            else
            {
                dir.Normalize();
                this._monster.SteerFaceDirectionTo(Vector3.Slerp(this._monster.FaceDirection, dir, (lerpRatio * this._monster.TimeScale) * Time.deltaTime));
            }
        }

        public void TrySteerInstant(Vector3 dir)
        {
            if (this._monster.IsAnimatorInTag(MonsterData.MonsterTagGroup.FreezeDirection) || this._monster.IsMuteControl())
            {
                this._monster.OrderMove = false;
            }
            else
            {
                this._monster.SteerFaceDirectionTo(dir);
            }
        }

        public void TryStop()
        {
            this._monster.OrderMove = false;
        }

        public bool TryUseSkill(string skillName)
        {
            if ((this._monster.IsAnimatorInTag(MonsterData.MonsterTagGroup.Throw) || this._monster.IsAnimatorInTag(MonsterData.MonsterTagGroup.ShowHit)) || this._monster.IsMuteControl())
            {
                return false;
            }
            this._monster.SetTrigger(skillName + "Trigger");
            return true;
        }

        public bool active { get; protected set; }
    }
}

