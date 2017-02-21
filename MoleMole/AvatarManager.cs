namespace MoleMole
{
    using MoleMole.Config;
    using proto;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UniRx;
    using UnityEngine;

    public class AvatarManager
    {
        private Dictionary<uint, BaseMonoAvatar> _avatarDict = new Dictionary<uint, BaseMonoAvatar>();
        private List<BaseMonoAvatar> _avatarLs = new List<BaseMonoAvatar>();
        private uint _helperAvatarRuntimeID;
        private uint _localAvatarRuntimeID = 0;
        private List<BaseMonoAvatar> _playerAvatars = new List<BaseMonoAvatar>();
        private List<Tuple<string, GameObject>> _preloadedAvatar = new List<Tuple<string, GameObject>>();
        [CompilerGenerated]
        private static Predicate<BaseMonoAvatar> <>f__am$cache8;
        public Action<BaseMonoAvatar, BaseMonoAvatar> onLocalAvatarChanged;

        private AvatarManager()
        {
        }

        private int Compare(BaseMonoAvatar a, BaseMonoAvatar b)
        {
            if ((a.GetRuntimeID() == this._localAvatarRuntimeID) && (b.GetRuntimeID() != this._localAvatarRuntimeID))
            {
                return -1;
            }
            if ((a.GetRuntimeID() != this._localAvatarRuntimeID) && (b.GetRuntimeID() == this._localAvatarRuntimeID))
            {
                return 1;
            }
            if (a.IsActive() && !b.IsActive())
            {
                return -1;
            }
            if (!a.IsActive() && b.IsActive())
            {
                return 1;
            }
            return 0;
        }

        public void Core()
        {
            this.RemoveAllRemoveableAvatars();
        }

        public uint CreateAvatar(AvatarDataItem avatarDataItem, bool isLocal, Vector3 initPos, Vector3 initDir, uint runtimeID, bool isLeader, bool leaderSkillOn, bool isHelper = false, bool useLow = false)
        {
            BaseMonoAvatar component = null;
            string avatarRegistryKey = avatarDataItem.AvatarRegistryKey;
            GameObject obj2 = null;
            int index = -1;
            for (int i = 0; i < this._preloadedAvatar.Count; i++)
            {
                if (this._preloadedAvatar[i].Item1 == avatarRegistryKey)
                {
                    obj2 = this._preloadedAvatar[i].Item2;
                    index = i;
                    break;
                }
            }
            if (obj2 != null)
            {
                obj2.GetComponent<BaseMonoAvatar>().Enable();
                this._preloadedAvatar.RemoveAt(index);
            }
            else
            {
                useLow = useLow || (!GlobalVars.AVATAR_USE_DYNAMIC_BONE || (Singleton<LevelManager>.Instance.levelActor.levelMode == LevelActor.Mode.Multi));
                obj2 = (GameObject) UnityEngine.Object.Instantiate(Miscs.LoadResource<GameObject>(AvatarData.GetPrefabResPath(avatarRegistryKey, useLow), BundleType.RESOURCE_FILE), InLevelData.CREATE_INIT_POS, Quaternion.Euler(0f, 200f, 0f));
            }
            component = obj2.GetComponent<BaseMonoAvatar>();
            if (runtimeID == 0)
            {
                runtimeID = Singleton<RuntimeIDManager>.Instance.GetNextRuntimeID(3);
            }
            component.Init(isLocal, runtimeID, avatarDataItem.AvatarRegistryKey, avatarDataItem.GetWeapon().ID, initPos, initDir, isLeader);
            bool isPlayerAvatar = !isHelper;
            this.RegisterAvatar(component, isLocal, isPlayerAvatar, isHelper);
            AvatarActor actor = Singleton<EventManager>.Instance.CreateActor<AvatarActor>(component);
            actor.InitAvatarDataItem(avatarDataItem, isLocal, isHelper, isLeader, leaderSkillOn);
            actor.InitGalTouchBuff(avatarDataItem);
            actor.PostInit();
            if (Singleton<LevelScoreManager>.Instance.LevelType == 4)
            {
                EndlessAvatarHp endlessAvatarHPData = Singleton<EndlessModule>.Instance.GetEndlessAvatarHPData(avatarDataItem.avatarID);
                actor.HP = (actor.maxHP * endlessAvatarHPData.get_hp_percent()) / 100f;
                actor.SP = (actor.maxSP * endlessAvatarHPData.get_sp_percent()) / 100f;
            }
            ConfigAvatar config = component.config;
            for (int j = 0; j < config.CommonArguments.PreloadEffectPatternGroups.Length; j++)
            {
                Singleton<EffectManager>.Instance.PreloadEffectGroup(config.CommonArguments.PreloadEffectPatternGroups[j], false);
            }
            if (component is MonoBronya)
            {
                if (actor.HasAppliedAbilityName("Weapon_Additional_BronyaLaser"))
                {
                    Singleton<EffectManager>.Instance.PreloadEffectGroup("Bronya_Laser_Effects", false);
                }
                else
                {
                    Singleton<EffectManager>.Instance.PreloadEffectGroup("Bronya_Gun_Effects", false);
                }
            }
            for (int k = 0; k < config.CommonArguments.RequestSoundBankNames.Length; k++)
            {
                Singleton<WwiseAudioManager>.Instance.ManualPrepareBank(config.CommonArguments.RequestSoundBankNames[k]);
            }
            return component.GetRuntimeID();
        }

        private void CreateAvatarHelper()
        {
            if ((Singleton<LevelScoreManager>.Instance.friendDetailItem != null) && (this._helperAvatarRuntimeID == 0))
            {
                FriendDetailDataItem friendDetailItem = Singleton<LevelScoreManager>.Instance.friendDetailItem;
                AvatarDataItem leaderAvatar = friendDetailItem.leaderAvatar;
                bool leaderSkillOn = Singleton<FriendModule>.Instance.IsMyFriend(friendDetailItem.uid);
                Singleton<AvatarManager>.Instance.CreateAvatar(leaderAvatar, false, InLevelData.CREATE_INIT_POS, InLevelData.CREATE_INIT_FORWARD, Singleton<RuntimeIDManager>.Instance.GetNextRuntimeID(3), false, leaderSkillOn, true, true);
            }
        }

        public uint CreateAvatarMirror(BaseMonoAvatar owner, Vector3 initPos, Vector3 initDir, string AIName, float hpRatio)
        {
            BaseMonoAvatar component = ((GameObject) UnityEngine.Object.Instantiate(Miscs.LoadResource<GameObject>(AvatarData.GetPrefabResPath(owner.AvatarTypeName, true), BundleType.RESOURCE_FILE), initPos, Quaternion.LookRotation(initDir))).GetComponent<BaseMonoAvatar>();
            component.Init(false, Singleton<RuntimeIDManager>.Instance.GetNextRuntimeID(3), owner.AvatarTypeName, owner.EquipedWeaponID, initPos, initDir, false);
            this.RegisterAvatar(component, false, false, false);
            AvatarMirrorActor actor = Singleton<EventManager>.Instance.CreateActor<AvatarMirrorActor>(component);
            actor.InitFromAvatarActor(Singleton<EventManager>.Instance.GetActor<AvatarActor>(owner.GetRuntimeID()), hpRatio);
            actor.PostInit();
            component.TriggerSwitchIn();
            BTreeAvatarAIController activeAIController = component.GetActiveAIController() as BTreeAvatarAIController;
            if (string.IsNullOrEmpty(AIName))
            {
                activeAIController.SetActive(false);
            }
            else
            {
                activeAIController.ChangeBehavior(AIName);
                component.ForceUseAIController();
            }
            return component.GetRuntimeID();
        }

        public void CreateTeamAvatars()
        {
            List<AvatarDataItem> memberList = Singleton<LevelScoreManager>.Instance.memberList;
            for (int i = 0; i < memberList.Count; i++)
            {
                bool isLocal = i == 0;
                bool isLeader = i == 0;
                Singleton<AvatarManager>.Instance.CreateAvatar(memberList[i], isLocal, InLevelData.CREATE_INIT_POS, InLevelData.CREATE_INIT_FORWARD, Singleton<RuntimeIDManager>.Instance.GetNextRuntimeID(3), isLeader, isLeader, false, false);
            }
            this.CreateAvatarHelper();
            this._preloadedAvatar.Clear();
        }

        public void Destroy()
        {
            for (int i = 0; i < this._preloadedAvatar.Count; i++)
            {
                if ((this._preloadedAvatar[i] != null) && (this._preloadedAvatar[i].Item2 != null))
                {
                    UnityEngine.Object.DestroyImmediate(this._preloadedAvatar[i].Item2);
                }
            }
            for (int j = 0; j < this._avatarLs.Count; j++)
            {
                if (this._avatarLs[j] != null)
                {
                    UnityEngine.Object.DestroyImmediate(this._avatarLs[j]);
                }
            }
            this.onLocalAvatarChanged = null;
        }

        public int GetActiveAvatarCount()
        {
            int num = 0;
            for (int i = 0; i < this._avatarLs.Count; i++)
            {
                if (this._avatarLs[i].IsActive())
                {
                    num++;
                }
            }
            return num;
        }

        public List<BaseMonoAvatar> GetAllAvatars()
        {
            return this._avatarLs;
        }

        public List<BaseMonoAvatar> GetAllPlayerAvatars()
        {
            return this._playerAvatars;
        }

        public BaseMonoAvatar GetAvatarByRuntimeID(uint runtimeID)
        {
            return this._avatarDict[runtimeID];
        }

        public BaseMonoAvatar GetFirstAliveAvatar()
        {
            List<BaseMonoAvatar> list = this._playerAvatars;
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].IsAlive())
                {
                    return list[i];
                }
            }
            return null;
        }

        public BaseMonoAvatar GetHelperAvatar()
        {
            return (((this._helperAvatarRuntimeID == 0) || !this._avatarDict.ContainsKey(this._helperAvatarRuntimeID)) ? null : this._avatarDict[this._helperAvatarRuntimeID]);
        }

        public BaseMonoAvatar GetLocalAvatar()
        {
            return this._avatarDict[this._localAvatarRuntimeID];
        }

        public BaseMonoAvatar GetTeamLeader()
        {
            if (<>f__am$cache8 == null)
            {
                <>f__am$cache8 = avatar => avatar.isLeader;
            }
            return this.GetAllAvatars().Find(<>f__am$cache8);
        }

        public void HideHelperAvatar(bool force)
        {
            BaseMonoAvatar helperAvatar = this.GetHelperAvatar();
            if (helperAvatar != null)
            {
                Singleton<EventManager>.Instance.GetActor<AvatarActor>(helperAvatar.GetRuntimeID()).GetPlugin<AvatarHelperStatePlugin>().TriggerSwitchOut(force);
            }
        }

        public void InitAtAwake()
        {
        }

        public void InitAtStart()
        {
        }

        public void InitAvatarsPos(List<MonoSpawnPoint> avatarSpawnPointList)
        {
            Vector3[] vectorArray = new Vector3[] { Vector3.zero, Vector3.zero, Vector3.zero };
            switch (Singleton<LevelManager>.Instance.levelActor.levelMode)
            {
                case LevelActor.Mode.Single:
                    vectorArray = InLevelData.SINGLE_MODE_AVATAR_INIT_POS_LIST;
                    break;

                case LevelActor.Mode.Multi:
                case LevelActor.Mode.NetworkedMP:
                    vectorArray = InLevelData.MUTIL_MODE_AVATAR_INIT_POS_LIST;
                    break;

                case LevelActor.Mode.MultiRemote:
                    vectorArray = InLevelData.MUTIL_REMOTE_MODE_AVATAR_INIT_POS_LIST;
                    break;
            }
            List<BaseMonoAvatar> allPlayerAvatars = Singleton<AvatarManager>.Instance.GetAllPlayerAvatars();
            if (Singleton<LevelManager>.Instance.levelActor.levelMode != LevelActor.Mode.NetworkedMP)
            {
                allPlayerAvatars.Sort(new Comparison<BaseMonoAvatar>(this.Compare));
            }
            for (int i = 0; i < allPlayerAvatars.Count; i++)
            {
                if (allPlayerAvatars[i].IsActive())
                {
                    if (Singleton<LevelManager>.Instance.levelActor.levelMode == LevelActor.Mode.MultiRemote)
                    {
                        allPlayerAvatars[i].transform.position = avatarSpawnPointList[i].transform.TransformPoint(vectorArray[i]);
                        allPlayerAvatars[i].transform.forward = avatarSpawnPointList[i].transform.forward;
                    }
                    else
                    {
                        allPlayerAvatars[i].transform.position = avatarSpawnPointList[0].transform.TransformPoint(vectorArray[i]);
                        allPlayerAvatars[i].transform.forward = avatarSpawnPointList[0].transform.forward;
                    }
                }
                else
                {
                    allPlayerAvatars[i].transform.position = InLevelData.CREATE_INIT_POS;
                }
            }
        }

        public bool IsHelperAvatar(uint runtimeID)
        {
            return (runtimeID == this._helperAvatarRuntimeID);
        }

        public bool IsLocalAvatar(uint runtimeID)
        {
            return (runtimeID == this._localAvatarRuntimeID);
        }

        public bool IsPlayerAvatar(BaseMonoAvatar avatar)
        {
            return this._playerAvatars.Contains(avatar);
        }

        public bool IsPlayerAvatar(uint avatarID)
        {
            for (int i = 0; i < this._playerAvatars.Count; i++)
            {
                if (this._playerAvatars[i].GetRuntimeID() == avatarID)
                {
                    return true;
                }
            }
            return false;
        }

        private void OnPlayerAvatarDie(BaseMonoAvatar avatar)
        {
            if (!this.IsLocalAvatar(avatar.GetRuntimeID()))
            {
                avatar.gameObject.SetActive(false);
            }
            else
            {
                Singleton<LevelManager>.Instance.gameMode.HandleLocalPlayerAvatarDie(avatar);
            }
        }

        public void PreloadAvatar(string avatarType, bool useLow)
        {
            GameObject obj3 = (GameObject) UnityEngine.Object.Instantiate(Miscs.LoadResource<GameObject>(AvatarData.GetPrefabResPath(avatarType, useLow), BundleType.RESOURCE_FILE), InLevelData.CREATE_INIT_POS, Quaternion.Euler(0f, 200f, 0f));
            obj3.GetComponent<BaseMonoAnimatorEntity>().Preload();
            this._preloadedAvatar.Add(new Tuple<string, GameObject>(avatarType, obj3));
        }

        public void PreloadTeamAvatars()
        {
            List<AvatarDataItem> list = new List<AvatarDataItem>();
            LevelScoreManager instance = Singleton<LevelScoreManager>.Instance;
            if ((instance != null) && (instance.memberList != null))
            {
                list.AddRange(instance.memberList);
            }
            bool useLow = !GlobalVars.AVATAR_USE_DYNAMIC_BONE || (Singleton<LevelManager>.Instance.levelActor.levelMode == LevelActor.Mode.Multi);
            for (int i = 0; i < list.Count; i++)
            {
                this.PreloadAvatar(list[i].AvatarRegistryKey, useLow);
            }
            if ((instance != null) && (instance.friendDetailItem != null))
            {
                AvatarDataItem leaderAvatar = instance.friendDetailItem.leaderAvatar;
                list.Add(leaderAvatar);
                this.PreloadAvatar(leaderAvatar.AvatarRegistryKey, true);
            }
        }

        public void RegisterAvatar(BaseMonoAvatar avatar, bool isLocal, bool isPlayerAvatar, bool isHelperAvatar)
        {
            this._avatarLs.Add(avatar);
            this._avatarDict.Add(avatar.GetRuntimeID(), avatar);
            if (isLocal)
            {
                this._localAvatarRuntimeID = avatar.GetRuntimeID();
            }
            if (isHelperAvatar)
            {
                this._helperAvatarRuntimeID = avatar.GetRuntimeID();
            }
            if (isPlayerAvatar)
            {
                this._playerAvatars.Add(avatar);
                avatar.onDie = (Action<BaseMonoAvatar>) Delegate.Combine(avatar.onDie, new Action<BaseMonoAvatar>(this.OnPlayerAvatarDie));
            }
        }

        public void RemoveAllAvatars()
        {
            for (int i = 0; i < this._avatarLs.Count; i++)
            {
                BaseMonoAvatar avatar = this._avatarLs[i];
                if (!avatar.IsToBeRemove())
                {
                    avatar.SetDied(KillEffect.KillImmediately);
                }
                this.RemoveAvatarByRuntimeID(avatar.GetRuntimeID());
                i--;
            }
        }

        private void RemoveAllRemoveableAvatars()
        {
            for (int i = 0; i < this._avatarLs.Count; i++)
            {
                BaseMonoAvatar avatar = this._avatarLs[i];
                if (avatar.IsToBeRemove())
                {
                    this.RemoveAvatarByRuntimeID(avatar.GetRuntimeID());
                    i--;
                }
            }
        }

        public bool RemoveAvatarByRuntimeID(uint runtimeID)
        {
            BaseMonoAvatar item = this._avatarDict[runtimeID];
            Singleton<EventManager>.Instance.TryRemoveActor(runtimeID);
            UnityEngine.Object.Destroy(item.gameObject);
            bool flag = true;
            flag &= this._playerAvatars.Remove(item);
            flag &= this._avatarDict.Remove(runtimeID);
            return (flag & this._avatarLs.Remove(item));
        }

        public void SetAllAvatarVisibility(bool visible)
        {
            for (int i = 0; i < this._avatarLs.Count; i++)
            {
                BaseMonoAvatar avatar = this._avatarLs[i];
                for (int j = 0; j < avatar.renderers.Length; j++)
                {
                    avatar.renderers[j].enabled = visible;
                }
            }
        }

        public void SetAutoBattle(bool isAuto)
        {
            this.isAutoBattle = isAuto;
            this.GetLocalAvatar().RefreshController();
        }

        public void SetAvatarVisibility(bool visible, BaseMonoAvatar avatar)
        {
            if (this._avatarLs.Contains(avatar))
            {
                for (int i = 0; i < avatar.renderers.Length; i++)
                {
                    avatar.renderers[i].enabled = visible;
                }
            }
        }

        public void SetLocalAvatar(uint runtimeID)
        {
            uint previousAvatarID = this._localAvatarRuntimeID;
            this._localAvatarRuntimeID = runtimeID;
            if (this.onLocalAvatarChanged != null)
            {
                this.onLocalAvatarChanged(this._avatarDict[previousAvatarID], this._avatarDict[this._localAvatarRuntimeID]);
            }
            Singleton<EventManager>.Instance.FireEvent(new EvtLocalAvatarChanged(runtimeID, previousAvatarID), MPEventDispatchMode.Normal);
        }

        public void SetMuteAllAvatarControl(bool mute)
        {
            foreach (BaseMonoAvatar avatar in this._avatarDict.Values)
            {
                avatar.SetCountedMuteControl(mute);
                if (avatar.IsActive())
                {
                    avatar.OrderMove = false;
                    avatar.ClearAttackTriggers();
                }
            }
        }

        public void SetPause(bool pause)
        {
            foreach (BaseMonoAvatar avatar in this._avatarDict.Values)
            {
                avatar.SetPause(pause);
            }
        }

        public void ShowHelperAvater()
        {
            BaseMonoAvatar helperAvatar = this.GetHelperAvatar();
            if (helperAvatar != null)
            {
                AvatarActor actor = Singleton<EventManager>.Instance.GetActor<AvatarActor>(helperAvatar.GetRuntimeID());
                List<BaseMonoAvatar> allPlayerAvatars = Singleton<AvatarManager>.Instance.GetAllPlayerAvatars();
                int num = 0;
                foreach (BaseMonoAvatar avatar2 in allPlayerAvatars)
                {
                    AvatarActor actor2 = Singleton<EventManager>.Instance.GetActor<AvatarActor>(avatar2.GetRuntimeID());
                    if (actor2 != null)
                    {
                        num = Math.Max((int) actor2.level, num);
                    }
                }
                float num2 = (num <= actor.level) ? (AvatarLevelMetaDataReader.GetAvatarLevelMetaDataByKey(num).avatarAssistConf / AvatarLevelMetaDataReader.GetAvatarLevelMetaDataByKey((int) actor.level).avatarAssistConf) : 1f;
                actor.attack *= num2;
                actor.GetPlugin<AvatarHelperStatePlugin>().TriggerSwitchIn();
            }
        }

        public BaseMonoAvatar TryGetAvatarByRuntimeID(uint runtimeID)
        {
            BaseMonoAvatar avatar;
            this._avatarDict.TryGetValue(runtimeID, out avatar);
            return avatar;
        }

        public BaseMonoAvatar TryGetLocalAvatar()
        {
            return (!this._avatarDict.ContainsKey(this._localAvatarRuntimeID) ? null : this._avatarDict[this._localAvatarRuntimeID]);
        }

        public bool isAutoBattle { get; private set; }
    }
}

