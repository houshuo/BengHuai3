namespace MoleMole
{
    using BehaviorDesigner.Runtime;
    using MoleMole.Config;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.UI;

    public class MonoDebugPanel : MonoBehaviour
    {
        private List<MonsterConfigMetaData> _monsterMetaDataList;
        private string _selectedMonsterCategory;
        private List<string> _selectedMonsterCategoryList;
        private string _selectedMonsterName;
        private List<string> _selectedMonsterNameList;
        private string _selectedMonsterType;
        private List<string> _selectedMonsterTypeList;
        private Dictionary<uint, float> avatarMaxHPDict;
        private Dictionary<uint, float> avatarMaxSPDict;
        private bool isElite;
        private bool isStationary;
        private int monsterLevel = 1;
        private int monsterNumber = 1;
        private float toggleCategoryListPanelOriginHeight;
        private float toggleNameListPanelOriginHeight;
        private float toggleTypeListPanelOriginHeight;
        private uint uniqueMonsterID;

        private void Awake()
        {
            this.avatarMaxHPDict = new Dictionary<uint, float>();
            this.avatarMaxSPDict = new Dictionary<uint, float>();
        }

        private void BuildMonsterColData(MonsterColType colType)
        {
            if (colType == MonsterColType.CATEGORY)
            {
                foreach (MonsterConfigMetaData data in this._monsterMetaDataList)
                {
                    if (!this._selectedMonsterCategoryList.Contains(data.categoryName))
                    {
                        this._selectedMonsterCategoryList.Add(data.categoryName);
                    }
                }
            }
            else if (colType == MonsterColType.NAME)
            {
                foreach (MonsterConfigMetaData data2 in this._monsterMetaDataList)
                {
                    if ((data2.categoryName == this._selectedMonsterCategory) && !this._selectedMonsterNameList.Contains(data2.monsterName))
                    {
                        this._selectedMonsterNameList.Add(data2.monsterName);
                    }
                }
            }
            else if (colType == MonsterColType.TYPE)
            {
                foreach (MonsterConfigMetaData data3 in this._monsterMetaDataList)
                {
                    if (((data3.categoryName == this._selectedMonsterCategory) && (data3.monsterName == this._selectedMonsterName)) && !this._selectedMonsterTypeList.Contains(data3.typeName))
                    {
                        this._selectedMonsterTypeList.Add(data3.typeName);
                    }
                }
            }
        }

        private void BuildMonsterColItem(MonsterColType colType)
        {
            Transform parent = base.transform.Find("MonsterCategoryListPanel/MonsterToggleListPanel");
            if (this._selectedMonsterCategoryList.Count > 5)
            {
                parent.GetComponent<RectTransform>().sizeDelta = new Vector2(0f, (this.toggleCategoryListPanelOriginHeight * this._selectedMonsterCategoryList.Count) / 5f);
            }
            Transform transform2 = base.transform.Find("MonsterNameListPanel/MonsterToggleListPanel");
            if (this._selectedMonsterNameList.Count > 5)
            {
                transform2.GetComponent<RectTransform>().sizeDelta = new Vector2(0f, (this.toggleNameListPanelOriginHeight * this._selectedMonsterNameList.Count) / 5f);
            }
            Transform transform3 = base.transform.Find("MonsterTypeListPanel/MonsterToggleListPanel");
            if (this._selectedMonsterTypeList.Count > 5)
            {
                transform3.GetComponent<RectTransform>().sizeDelta = new Vector2(0f, (this.toggleTypeListPanelOriginHeight * this._selectedMonsterTypeList.Count) / 5f);
            }
            int num = 0;
            if (colType == MonsterColType.CATEGORY)
            {
                foreach (string str in this._selectedMonsterCategoryList)
                {
                    num++;
                    Transform transform = UnityEngine.Object.Instantiate<GameObject>(Miscs.LoadResource<GameObject>("UI/MonsterDebugToggle", BundleType.RESOURCE_FILE)).transform;
                    string str2 = string.Format("{0}", str);
                    transform.Find("Label").GetComponent<Text>().text = str2;
                    transform.GetComponent<MonoMonsterToggle>().SetMonsterToggleValue(this, str2, MonoMonsterToggle.ToggleColumn.CATEGORY);
                    transform.SetParent(parent, false);
                    transform.GetComponent<Toggle>().group = parent.GetComponent<ToggleGroup>();
                    transform.GetComponent<Toggle>().onValueChanged.AddListener(new UnityAction<bool>(transform.GetComponent<MonoMonsterToggle>().OnToggleValueChanged));
                }
            }
            else if (colType == MonsterColType.NAME)
            {
                foreach (string str3 in this._selectedMonsterNameList)
                {
                    num++;
                    Transform transform5 = UnityEngine.Object.Instantiate<GameObject>(Miscs.LoadResource<GameObject>("UI/MonsterDebugToggle", BundleType.RESOURCE_FILE)).transform;
                    string str4 = string.Format("{0}", str3);
                    transform5.Find("Label").GetComponent<Text>().text = str4;
                    transform5.GetComponent<MonoMonsterToggle>().SetMonsterToggleValue(this, str4, MonoMonsterToggle.ToggleColumn.NAME);
                    transform5.SetParent(transform2, false);
                    transform5.GetComponent<Toggle>().group = transform2.GetComponent<ToggleGroup>();
                    transform5.GetComponent<Toggle>().onValueChanged.AddListener(new UnityAction<bool>(transform5.GetComponent<MonoMonsterToggle>().OnToggleValueChanged));
                }
            }
            else if (colType == MonsterColType.TYPE)
            {
                foreach (string str5 in this._selectedMonsterTypeList)
                {
                    num++;
                    Transform transform6 = UnityEngine.Object.Instantiate<GameObject>(Miscs.LoadResource<GameObject>("UI/MonsterDebugToggle", BundleType.RESOURCE_FILE)).transform;
                    string str6 = string.Format("{0}", str5);
                    transform6.Find("Label").GetComponent<Text>().text = str6;
                    transform6.GetComponent<MonoMonsterToggle>().SetMonsterToggleValue(this, str6, MonoMonsterToggle.ToggleColumn.TYPE);
                    transform6.SetParent(transform3, false);
                    transform6.GetComponent<Toggle>().group = transform3.GetComponent<ToggleGroup>();
                    transform6.GetComponent<Toggle>().onValueChanged.AddListener(new UnityAction<bool>(transform6.GetComponent<MonoMonsterToggle>().OnToggleValueChanged));
                }
            }
        }

        private void ClearMonsterCol(MonsterColType colType)
        {
            this.ClearMonsterColData(colType);
            this.ClearMonsterColItem(colType);
        }

        private void ClearMonsterColData(MonsterColType colType)
        {
            if (colType == MonsterColType.CATEGORY)
            {
                this._selectedMonsterCategoryList.Clear();
                this._selectedMonsterCategory = string.Empty;
            }
            else if (colType == MonsterColType.NAME)
            {
                this._selectedMonsterNameList.Clear();
                this._selectedMonsterName = string.Empty;
            }
            else if (colType == MonsterColType.TYPE)
            {
                this._selectedMonsterTypeList.Clear();
                this._selectedMonsterType = string.Empty;
            }
        }

        private void ClearMonsterColItem(MonsterColType colType)
        {
            if (colType == MonsterColType.CATEGORY)
            {
                Transform transform = base.transform.Find("MonsterCategoryListPanel/MonsterToggleListPanel");
                for (int i = 0; i < transform.childCount; i++)
                {
                    UnityEngine.Object.Destroy(transform.GetChild(i).gameObject);
                }
            }
            else if (colType == MonsterColType.NAME)
            {
                Transform transform2 = base.transform.Find("MonsterNameListPanel/MonsterToggleListPanel");
                for (int j = 0; j < transform2.childCount; j++)
                {
                    UnityEngine.Object.Destroy(transform2.GetChild(j).gameObject);
                }
            }
            else if (colType == MonsterColType.TYPE)
            {
                Transform transform3 = base.transform.Find("MonsterTypeListPanel/MonsterToggleListPanel");
                for (int k = 0; k < transform3.childCount; k++)
                {
                    UnityEngine.Object.Destroy(transform3.GetChild(k).gameObject);
                }
            }
        }

        private void ClearMonsterNameColume()
        {
            this._selectedMonsterNameList.Clear();
            this._selectedMonsterName = string.Empty;
            Transform transform = base.transform.Find("MonsterNameListPanel/MonsterToggleListPanel");
            for (int i = 0; i < transform.childCount; i++)
            {
                UnityEngine.Object.Destroy(transform.GetChild(i).gameObject);
            }
        }

        private void ClearMonsterTypeColume()
        {
            this._selectedMonsterTypeList.Clear();
            this._selectedMonsterType = string.Empty;
            Transform transform = base.transform.Find("MonsterTypeListPanel/MonsterToggleListPanel");
            for (int i = 0; i < transform.childCount; i++)
            {
                UnityEngine.Object.Destroy(transform.GetChild(i).gameObject);
            }
        }

        public void DebugRemoveGameLogic()
        {
            base.StartCoroutine(this.DestructionIter());
        }

        private void DestroyAll<T>() where T: Component
        {
            foreach (T local in UnityEngine.Object.FindObjectsOfType<T>())
            {
                UnityEngine.Object.Destroy(local);
            }
        }

        [DebuggerHidden]
        private IEnumerator DestructionIter()
        {
            return new <DestructionIter>c__Iterator72 { <>f__this = this };
        }

        public void OnAvatarPowerfulToggleValueChanged(bool isOn)
        {
            if (isOn)
            {
                foreach (BaseMonoAvatar avatar in Singleton<AvatarManager>.Instance.GetAllAvatars())
                {
                    AvatarActor actor = Singleton<EventManager>.Instance.GetActor<AvatarActor>(avatar.GetRuntimeID());
                    foreach (AvatarActor.SKillInfo info in actor.skillInfoList)
                    {
                        info.CD = 0f;
                    }
                    this.avatarMaxHPDict[avatar.GetRuntimeID()] = (float) actor.maxHP;
                    this.avatarMaxSPDict[avatar.GetRuntimeID()] = (float) actor.maxSP;
                    actor.maxHP = 999999f;
                    actor.maxSP = 999999f;
                    DelegateUtils.UpdateField(ref actor.HP, 999999f, 0f, actor.onHPChanged);
                    DelegateUtils.UpdateField(ref actor.SP, 999999f, 0f, actor.onSPChanged);
                }
                Singleton<LevelScoreManager>.Instance.useDebugFunction = true;
            }
            else
            {
                foreach (BaseMonoAvatar avatar2 in Singleton<AvatarManager>.Instance.GetAllAvatars())
                {
                    uint runtimeID = avatar2.GetRuntimeID();
                    AvatarActor actor2 = Singleton<EventManager>.Instance.GetActor<AvatarActor>(runtimeID);
                    foreach (string str in actor2.config.Skills.Keys)
                    {
                        ConfigAvatarSkill skill = actor2.config.Skills[str];
                        string skillNameByAnimEventID = actor2.GetSkillNameByAnimEventID(str);
                        AvatarActor.SKillInfo skillInfo = actor2.GetSkillInfo(skillNameByAnimEventID);
                        if (skillInfo != null)
                        {
                            skillInfo.CD = Mathf.Max((float) 0f, (float) (skill.SkillCD + actor2.Evaluate(skill.SkillCDDelta)));
                        }
                    }
                    if (this.avatarMaxHPDict.ContainsKey(runtimeID))
                    {
                        actor2.maxHP = this.avatarMaxHPDict[runtimeID];
                        DelegateUtils.UpdateField(ref actor2.HP, this.avatarMaxHPDict[runtimeID], 0f, actor2.onHPChanged);
                    }
                    if (this.avatarMaxSPDict.ContainsKey(runtimeID))
                    {
                        actor2.maxSP = this.avatarMaxSPDict[runtimeID];
                        DelegateUtils.UpdateField(ref actor2.SP, this.avatarMaxSPDict[runtimeID], 0f, actor2.onSPChanged);
                    }
                }
            }
        }

        public void OnConfirmButtonClick()
        {
            this._selectedMonsterCategoryList.Clear();
            this._selectedMonsterNameList.Clear();
            this._selectedMonsterTypeList.Clear();
            string text = base.transform.Find("ConfirmPanel/MonsterNumber/Text").GetComponent<Text>().text;
            if (text == string.Empty)
            {
                text = "1";
            }
            if (!int.TryParse(text, out this.monsterNumber))
            {
                this.monsterNumber = 1;
            }
            string s = base.transform.Find("ConfirmPanel/MonsterLevel/Text").GetComponent<Text>().text;
            if (s == string.Empty)
            {
                s = "1";
            }
            if (!int.TryParse(s, out this.monsterLevel))
            {
                this.monsterNumber = 1;
            }
            string str3 = base.transform.Find("ConfirmPanel/UniqueID/Text").GetComponent<Text>().text;
            if (str3 == string.Empty)
            {
                str3 = "0";
            }
            if (!uint.TryParse(str3, out this.uniqueMonsterID))
            {
                this.uniqueMonsterID = 0;
            }
            this.isElite = base.transform.Find("ConfirmPanel/EliteToggle").GetComponent<Toggle>().isOn;
            if ((this.uniqueMonsterID != 0) || ((!string.IsNullOrEmpty(this._selectedMonsterCategory) && !string.IsNullOrEmpty(this._selectedMonsterName)) && !string.IsNullOrEmpty(this._selectedMonsterType)))
            {
                Vector3 xZPosition = Singleton<AvatarManager>.Instance.GetLocalAvatar().XZPosition;
                for (int i = 0; i < this.monsterNumber; i++)
                {
                    uint runtimeID = Singleton<MonsterManager>.Instance.CreateMonster(this._selectedMonsterName, this._selectedMonsterType, this.monsterLevel, true, xZPosition, Singleton<RuntimeIDManager>.Instance.GetNextRuntimeID(4), this.isElite, this.uniqueMonsterID, true, false, 0);
                    if (this.isElite)
                    {
                        MonsterActor actor = Singleton<EventManager>.Instance.GetActor<MonsterActor>(runtimeID);
                        if (((this._selectedMonsterCategory == "DeadGal") || (this._selectedMonsterCategory == "Ulysses")) || (this._selectedMonsterCategory == "Robot"))
                        {
                            actor.abilityPlugin.AddAbility(AbilityData.GetAbilityConfig("Elite_EliteShield", this._selectedMonsterName));
                        }
                    }
                }
                this.isStationary = base.transform.Find("ConfirmPanel/MonsterStationaryToggle").GetComponent<Toggle>().isOn;
                if (this.isStationary)
                {
                    this.OnMonsterStationaryToggleValueChanged(this.isStationary);
                }
            }
            base.transform.gameObject.SetActive(false);
        }

        public void OnEndLevel()
        {
            Singleton<LevelScoreManager>.Instance.useDebugFunction = true;
            base.transform.gameObject.SetActive(false);
            Singleton<LevelManager>.Instance.SetPause(true);
            Singleton<LevelManager>.Instance.levelActor.SuddenLevelEnd();
            Singleton<MainUIManager>.Instance.ShowPage(new LevelEndPageContext(EvtLevelState.LevelEndReason.EndWin, true, 0), UIType.Page);
            Singleton<WwiseAudioManager>.Instance.SetSwitch("Level_Result", "Win");
            Singleton<WwiseAudioManager>.Instance.Post("BGM_PauseMenu_Off", null, null, null);
            Singleton<WwiseAudioManager>.Instance.Post("BGM_Stage_End", null, null, null);
        }

        public void OnKillAllMonsterButtonClick()
        {
            foreach (MonsterActor actor in Singleton<EventManager>.Instance.GetActorByCategory<MonsterActor>(4))
            {
                if (actor.isAlive != 0)
                {
                    actor.ForceKill();
                }
            }
            Singleton<LevelScoreManager>.Instance.useDebugFunction = true;
        }

        public void OnMonsterCategoryToggleValueChanged(Toggle toggle)
        {
            string text = toggle.gameObject.transform.Find("Label").GetComponent<Text>().text;
            this._selectedMonsterCategory = text;
            this.ClearMonsterCol(MonsterColType.NAME);
            this.ClearMonsterCol(MonsterColType.TYPE);
            this.BuildMonsterColData(MonsterColType.NAME);
            this.BuildMonsterColItem(MonsterColType.NAME);
        }

        public void OnMonsterNameToggleValueChanged(Toggle toggle)
        {
            string text = toggle.gameObject.transform.Find("Label").GetComponent<Text>().text;
            this._selectedMonsterName = text;
            this.ClearMonsterCol(MonsterColType.TYPE);
            this.BuildMonsterColData(MonsterColType.TYPE);
            this.BuildMonsterColItem(MonsterColType.TYPE);
        }

        public void OnMonsterStationaryToggleValueChanged(bool isOn)
        {
            this.isStationary = isOn;
            if (isOn)
            {
                if (Singleton<MonsterManager>.Instance.GetAllMonsters().Count > 0)
                {
                    foreach (BaseMonoMonster monster in Singleton<MonsterManager>.Instance.GetAllMonsters())
                    {
                        MonsterActor actor = Singleton<EventManager>.Instance.GetActor<MonsterActor>(monster.GetRuntimeID());
                        actor.baseMaxHP = actor.HP = actor.maxHP = 3.402823E+38f;
                        BehaviorDesigner.Runtime.BehaviorTree component = monster.GetComponent<BehaviorDesigner.Runtime.BehaviorTree>();
                        if (component != null)
                        {
                            component.enabled = false;
                        }
                    }
                }
            }
            else
            {
                foreach (BaseMonoMonster monster2 in Singleton<MonsterManager>.Instance.GetAllMonsters())
                {
                    MonsterActor actor2 = Singleton<EventManager>.Instance.GetActor<MonsterActor>(monster2.GetRuntimeID());
                    NPCLevelMetaData nPCLevelMetaDataByKey = NPCLevelMetaDataReader.GetNPCLevelMetaDataByKey(this.monsterLevel);
                    actor2.baseMaxHP = actor2.maxHP = actor2.HP = actor2.config.CommonArguments.HP * nPCLevelMetaDataByKey.HPRatio;
                    actor2.defense = actor2.config.CommonArguments.Defence * nPCLevelMetaDataByKey.DEFRatio;
                    actor2.attack = actor2.config.CommonArguments.Attack * nPCLevelMetaDataByKey.ATKRatio;
                    actor2.PushProperty("Actor_ResistAllElementAttackRatio", nPCLevelMetaDataByKey.ElementalResistRatio);
                    if (this.isElite)
                    {
                        actor2.baseMaxHP = actor2.maxHP = actor2.HP = actor2.maxHP * actor2.config.EliteArguments.HPRatio;
                        actor2.defense *= actor2.config.EliteArguments.DefenseRatio;
                        actor2.attack *= actor2.config.EliteArguments.AttackRatio;
                    }
                    BehaviorDesigner.Runtime.BehaviorTree tree2 = monster2.GetComponent<BehaviorDesigner.Runtime.BehaviorTree>();
                    if (tree2 != null)
                    {
                        tree2.enabled = true;
                    }
                }
            }
        }

        public void OnMonsterTypeToggleValueChanged(Toggle toggle)
        {
            string text = toggle.gameObject.transform.Find("Label").GetComponent<Text>().text;
            this._selectedMonsterType = text;
        }

        public void OnShowPanelButtonClick()
        {
            this.SetupView();
            base.transform.gameObject.SetActive(true);
        }

        public void SetupView()
        {
            this._selectedMonsterCategoryList = new List<string>();
            this._selectedMonsterTypeList = new List<string>();
            this._selectedMonsterNameList = new List<string>();
            this.toggleCategoryListPanelOriginHeight = base.transform.Find("MonsterCategoryListPanel").GetComponent<RectTransform>().rect.height;
            this.toggleNameListPanelOriginHeight = base.transform.Find("MonsterNameListPanel").GetComponent<RectTransform>().rect.height;
            this.toggleTypeListPanelOriginHeight = base.transform.Find("MonsterTypeListPanel").GetComponent<RectTransform>().rect.height;
            Transform transform = base.transform.Find("MonsterCategoryListPanel/MonsterToggleListPanel");
            Transform transform2 = base.transform.Find("MonsterNameListPanel/MonsterToggleListPanel");
            Transform transform3 = base.transform.Find("MonsterTypeListPanel/MonsterToggleListPanel");
            transform.GetComponent<RectTransform>().sizeDelta = new Vector2(0f, this.toggleCategoryListPanelOriginHeight);
            transform2.GetComponent<RectTransform>().sizeDelta = new Vector2(0f, this.toggleNameListPanelOriginHeight);
            transform3.GetComponent<RectTransform>().sizeDelta = new Vector2(0f, this.toggleTypeListPanelOriginHeight);
            this.ClearMonsterCol(MonsterColType.CATEGORY);
            this.ClearMonsterCol(MonsterColType.NAME);
            this.ClearMonsterCol(MonsterColType.TYPE);
            this._monsterMetaDataList = MonsterData.GetAllMonsterConfigMetaData();
            this.BuildMonsterColData(MonsterColType.CATEGORY);
            this.BuildMonsterColItem(MonsterColType.CATEGORY);
        }

        private void Start()
        {
        }

        public void Toggle60Frames(bool is60)
        {
            if (is60)
            {
                Application.targetFrameRate = 60;
            }
            else
            {
                Application.targetFrameRate = 30;
            }
        }

        public void ToggleAlwaysLastKill(bool isOn)
        {
            MainCameraActor actor = Singleton<EventManager>.Instance.GetActor<MainCameraActor>(Singleton<CameraManager>.Instance.GetMainCamera().GetRuntimeID());
            if (isOn)
            {
                actor.SetupLastKillCloseUp();
                actor.GetPlugin<CameraActorLastKillCloseUpPlugin>().AlwaysTrigger = true;
            }
            else if (actor.HasPlugin<CameraActorLastKillCloseUpPlugin>())
            {
                actor.RemovePlugin<CameraActorLastKillCloseUpPlugin>();
            }
        }

        public void ToggleBenchmark(bool isOn)
        {
            MonoBenchmarkSwitches switches = UnityEngine.Object.FindObjectOfType<MonoBenchmarkSwitches>();
            if (switches == null)
            {
                if (isOn)
                {
                    System.Type[] components = new System.Type[] { typeof(MonoBenchmarkSwitches) };
                    new GameObject("__benchmark", components);
                }
            }
            else
            {
                switches.gameObject.SetActive(isOn);
            }
        }

        public void ToggleDistortion(bool isOn)
        {
            PostFX tfx = UnityEngine.Object.FindObjectOfType<PostFX>();
            if (tfx != null)
            {
                tfx.UseDistortion = isOn;
            }
        }

        public void ToggleDistortionDepth(bool isOn)
        {
            PostFX tfx = UnityEngine.Object.FindObjectOfType<PostFX>();
            if (tfx != null)
            {
                tfx.UseDepthTest = isOn;
            }
        }

        public void ToggleFXAA(bool isOn)
        {
            PostFX tfx = UnityEngine.Object.FindObjectOfType<PostFX>();
            if (tfx != null)
            {
                tfx.FXAA = isOn;
            }
        }

        public void TogglePostFX()
        {
            PostFX tfx = UnityEngine.Object.FindObjectOfType<PostFX>();
            if (tfx != null)
            {
                tfx.enabled = !tfx.enabled;
            }
        }

        private void Update()
        {
        }

        [CompilerGenerated]
        private sealed class <DestructionIter>c__Iterator72 : IEnumerator, IDisposable, IEnumerator<object>
        {
            internal object $current;
            internal int $PC;
            internal Rigidbody[] <$s_1860>__0;
            internal int <$s_1861>__1;
            internal MonoDebugPanel <>f__this;
            internal Rigidbody <rigidbody>__2;

            [DebuggerHidden]
            public void Dispose()
            {
                this.$PC = -1;
            }

            public bool MoveNext()
            {
                uint num = (uint) this.$PC;
                this.$PC = -1;
                switch (num)
                {
                    case 0:
                        this.$current = new WaitForEndOfFrame();
                        this.$PC = 1;
                        goto Label_0127;

                    case 1:
                        Singleton<LevelManager>.Instance.levelEntity.StopAllCoroutines();
                        Singleton<EventManager>.Instance.DropEventsAndStop();
                        Singleton<LevelDesignManager>.Instance.StopLevelDesign();
                        this.$current = null;
                        this.$PC = 2;
                        goto Label_0127;

                    case 2:
                        this.<$s_1860>__0 = UnityEngine.Object.FindObjectsOfType<Rigidbody>();
                        this.<$s_1861>__1 = 0;
                        while (this.<$s_1861>__1 < this.<$s_1860>__0.Length)
                        {
                            this.<rigidbody>__2 = this.<$s_1860>__0[this.<$s_1861>__1];
                            this.<rigidbody>__2.velocity = Vector3.zero;
                            this.<$s_1861>__1++;
                        }
                        UnityEngine.Object.Destroy(UnityEngine.Object.FindObjectOfType<MonoInLevelUICanvas>().gameObject);
                        this.<>f__this.DestroyAll<BehaviorDesigner.Runtime.BehaviorTree>();
                        this.<>f__this.DestroyAll<BaseMonoEffectPlugin>();
                        this.<>f__this.DestroyAll<BaseMonoEntity>();
                        this.<>f__this.DestroyAll<Animator>();
                        this.<>f__this.DestroyAll<MonoAuxObject>();
                        this.<>f__this.DestroyAll<MonoTheLevelV1>();
                        this.$PC = -1;
                        break;
                }
                return false;
            Label_0127:
                return true;
            }

            [DebuggerHidden]
            public void Reset()
            {
                throw new NotSupportedException();
            }

            object IEnumerator<object>.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }

            object IEnumerator.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }
        }

        private enum MonsterColType
        {
            CATEGORY,
            NAME,
            TYPE
        }
    }
}

