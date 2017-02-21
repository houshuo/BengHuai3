namespace MoleMole
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class MonoSubMonsterHPBarContainer : MonoBehaviour
    {
        private Dictionary<uint, MonoSubMonsterHPBar> _avatarHPBarMap;
        private Dictionary<uint, MonoSubMonsterHPBar> _subMonsterHPBarMap;
        public MonsterActor localAvatarLockedMonsterActor;
        public GameObject SubMonsterHPBarGO;

        private void Awake()
        {
            this._avatarHPBarMap = new Dictionary<uint, MonoSubMonsterHPBar>();
            this._subMonsterHPBarMap = new Dictionary<uint, MonoSubMonsterHPBar>();
        }

        private GameObject GetAvailableSubMonsterHPBar()
        {
            IEnumerator enumerator = base.transform.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    Transform current = (Transform) enumerator.Current;
                    if (!current.GetComponent<MonoSubMonsterHPBar>().enable)
                    {
                        return current.gameObject;
                    }
                }
            }
            finally
            {
                IDisposable disposable = enumerator as IDisposable;
                if (disposable == null)
                {
                }
                disposable.Dispose();
            }
            return UnityEngine.Object.Instantiate<GameObject>(this.SubMonsterHPBarGO);
        }

        private bool IsLocalAvatarLockedMonsterBoss()
        {
            if ((this.localAvatarLockedMonsterActor != null) && (this.localAvatarLockedMonsterActor.uniqueMonsterID > 0))
            {
                UniqueMonsterMetaData uniqueMonsterMetaData = MonsterData.GetUniqueMonsterMetaData(this.localAvatarLockedMonsterActor.uniqueMonsterID);
                if ((uniqueMonsterMetaData != null) && (uniqueMonsterMetaData.hpPhaseNum > 1))
                {
                    return true;
                }
            }
            return false;
        }

        private bool IsMonsterBoss(MonsterActor monster)
        {
            if ((monster != null) && (monster.uniqueMonsterID > 0))
            {
                UniqueMonsterMetaData uniqueMonsterMetaData = MonsterData.GetUniqueMonsterMetaData(monster.uniqueMonsterID);
                if ((uniqueMonsterMetaData != null) && (uniqueMonsterMetaData.hpPhaseNum > 1))
                {
                    return true;
                }
            }
            return false;
        }

        public void OnAttackLandedEvt(EvtAttackLanded evt)
        {
            if ((Singleton<RuntimeIDManager>.Instance.ParseCategory(evt.targetID) == 3) && (Singleton<RuntimeIDManager>.Instance.ParseCategory(evt.attackeeID) == 4))
            {
                AvatarActor attackerActor = Singleton<EventManager>.Instance.GetActor<AvatarActor>(evt.targetID);
                if (((attackerActor != null) && (attackerActor.runtimeID != Singleton<AvatarManager>.Instance.GetLocalAvatar().GetRuntimeID())) && !(attackerActor is AvatarMirrorActor))
                {
                    BaseMonoAvatar entity = attackerActor.entity as BaseMonoAvatar;
                    if (entity.IsAnimatorInTag(AvatarData.AvatarTagGroup.AttackTargetLeadDirection))
                    {
                        MonsterActor actor = Singleton<EventManager>.Instance.GetActor<MonsterActor>(evt.attackeeID);
                        if ((this.localAvatarLockedMonsterActor == null) || (this.localAvatarLockedMonsterActor.runtimeID != actor.runtimeID))
                        {
                            if (this.IsMonsterBoss(actor))
                            {
                                this.SetAllSubHPBarDisable();
                            }
                            else
                            {
                                MonoSubMonsterHPBar component;
                                if (this._subMonsterHPBarMap.ContainsKey(actor.runtimeID))
                                {
                                    component = this._subMonsterHPBarMap[actor.runtimeID];
                                }
                                else
                                {
                                    GameObject availableSubMonsterHPBar = this.GetAvailableSubMonsterHPBar();
                                    availableSubMonsterHPBar.transform.SetParent(base.transform, false);
                                    component = availableSubMonsterHPBar.GetComponent<MonoSubMonsterHPBar>();
                                }
                                if (this._avatarHPBarMap.ContainsKey(attackerActor.runtimeID) && (this._avatarHPBarMap[attackerActor.runtimeID] != component))
                                {
                                    this._avatarHPBarMap[attackerActor.runtimeID].RemoveAttacker(attackerActor);
                                }
                                component.SetupView(attackerActor, actor, 0.1f, new Action<MonoSubMonsterHPBar>(this.OnHideHPBar));
                                this._avatarHPBarMap[attackerActor.runtimeID] = component;
                                this._subMonsterHPBarMap[actor.runtimeID] = component;
                            }
                        }
                    }
                }
            }
        }

        private void OnHideHPBar(MonoSubMonsterHPBar hpBar)
        {
            Dictionary<uint, MonoSubMonsterHPBar> dictionary = new Dictionary<uint, MonoSubMonsterHPBar>();
            foreach (KeyValuePair<uint, MonoSubMonsterHPBar> pair in this._avatarHPBarMap)
            {
                if (pair.Value != hpBar)
                {
                    dictionary[pair.Key] = pair.Value;
                }
            }
            this._avatarHPBarMap = dictionary;
            this._subMonsterHPBarMap.Remove(hpBar.attackee.runtimeID);
        }

        public void OnTargetMonsterChange(MonsterActor targetBefore, MonsterActor targetAfter)
        {
            this.localAvatarLockedMonsterActor = targetAfter;
            if (this.IsLocalAvatarLockedMonsterBoss())
            {
                this.SetAllSubHPBarDisable();
            }
            if ((this.localAvatarLockedMonsterActor != null) && this._subMonsterHPBarMap.ContainsKey(this.localAvatarLockedMonsterActor.runtimeID))
            {
                this._subMonsterHPBarMap[this.localAvatarLockedMonsterActor.runtimeID].SetDisable();
            }
        }

        private void SetAllSubHPBarDisable()
        {
            foreach (KeyValuePair<uint, MonoSubMonsterHPBar> pair in this._avatarHPBarMap)
            {
                pair.Value.SetDisable();
            }
        }
    }
}

