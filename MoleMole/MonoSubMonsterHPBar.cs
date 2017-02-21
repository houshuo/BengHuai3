namespace MoleMole
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public class MonoSubMonsterHPBar : MonoBehaviour
    {
        private Action<MonoSubMonsterHPBar> _hideHPBarCallBack;
        private BaseMonoMonster _monster;
        private float _offset;
        public MonsterActor attackee;
        private HashSet<uint> attackerSet;
        public bool enable;
        private const string HEAD_TRAN_STR = "Head";

        private void Awake()
        {
            this.attackerSet = new HashSet<uint>();
            this.attackee = null;
            this.enable = false;
        }

        public void RemoveAttacker(AvatarActor attackerActor)
        {
            this.attackerSet.Remove(attackerActor.runtimeID);
            if (this.attackerSet.Count <= 0)
            {
                this.SetDisable();
            }
        }

        public void SetDisable()
        {
            this.enable = false;
            base.gameObject.SetActive(false);
            if (this._hideHPBarCallBack != null)
            {
                this._hideHPBarCallBack(this);
            }
        }

        public void SetupView(AvatarActor attacker, MonsterActor attackee, float offset, Action<MonoSubMonsterHPBar> hideHPBarCallBack = null)
        {
            base.gameObject.SetActive(true);
            if (!this.attackerSet.Contains(attacker.runtimeID))
            {
                this.attackerSet.Add(attacker.runtimeID);
            }
            this.attackee = attackee;
            this._offset = offset;
            this.enable = true;
            this._monster = attackee.entity as BaseMonoMonster;
            this._hideHPBarCallBack = hideHPBarCallBack;
        }

        private void Update()
        {
            if (this.enable)
            {
                if ((this.attackee.isAlive == 0) || !Singleton<CameraManager>.Instance.GetMainCamera().IsEntityVisible(Singleton<MonsterManager>.Instance.GetMonsterByRuntimeID(this.attackee.runtimeID)))
                {
                    this.SetDisable();
                }
                else
                {
                    base.transform.Find("HPBar").GetComponent<MonoSliderGroupWithPhase>().UpdateValue((float) this.attackee.HP, (float) this.attackee.maxHP, 0f);
                    Vector3 xZPosition = this._monster.XZPosition;
                    if (this._monster.transform.GetComponent<CapsuleCollider>() == null)
                    {
                        this.SetDisable();
                    }
                    xZPosition.y = this._monster.transform.GetComponent<CapsuleCollider>().height + this._offset;
                    base.transform.position = Singleton<CameraManager>.Instance.GetMainCamera().WorldToUIPoint(xZPosition);
                }
            }
        }
    }
}

