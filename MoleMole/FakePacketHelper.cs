namespace MoleMole
{
    using proto;
    using SimpleJSON;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    public static class FakePacketHelper
    {
        private static Dictionary<int, uint> _weaponUidSet;
        private static JSONNode inputDict;
        private const string JSON_FILE = "DataDev/FakePacketRsp.json";
        private static uint uid;

        private static void AddMaterialIntoRsp(GetEquipmentDataRsp rsp, int addNum)
        {
            List<ItemMetaData> itemList = ItemMetaDataReader.GetItemList();
            int num = 0;
            foreach (ItemMetaData data in itemList)
            {
                num++;
                if (num > addNum)
                {
                    break;
                }
                Material item = new Material();
                item.set_id((uint) data.ID);
                item.set_num(100);
                rsp.get_material_list().Add(item);
            }
        }

        private static void AddStigmataIntoRsp(GetEquipmentDataRsp rsp, int addNum)
        {
            List<StigmataMetaData> itemList = StigmataMetaDataReader.GetItemList();
            int num = 0;
            foreach (StigmataMetaData data in itemList)
            {
                num++;
                if (num > addNum)
                {
                    break;
                }
                Stigmata item = new Stigmata();
                item.set_unique_id(GenerateNewUid());
                item.set_id((uint) data.ID);
                item.set_level(1);
                item.set_exp(0);
                rsp.get_stigmata_list().Add(item);
            }
        }

        private static void AddWeaponIntoRsp(GetEquipmentDataRsp rsp, int addNum)
        {
            List<WeaponMetaData> itemList = WeaponMetaDataReader.GetItemList();
            int num = 0;
            foreach (WeaponMetaData data in itemList)
            {
                num++;
                if (num > addNum)
                {
                    break;
                }
                Weapon item = new Weapon();
                item.set_unique_id(GenerateNewUid());
                item.set_id((uint) data.ID);
                item.set_level(1);
                item.set_exp(0);
                rsp.get_weapon_list().Add(item);
                num++;
            }
            List<AvatarMetaData> list2 = AvatarMetaDataReader.GetItemList();
            _weaponUidSet = new Dictionary<int, uint>();
            foreach (AvatarMetaData data2 in list2)
            {
                WeaponMetaData weaponMetaDataByKey = WeaponMetaDataReader.GetWeaponMetaDataByKey(data2.initialWeapon);
                Weapon weapon2 = new Weapon();
                weapon2.set_unique_id(GenerateNewUid());
                weapon2.set_id((uint) weaponMetaDataByKey.ID);
                weapon2.set_level(1);
                weapon2.set_exp(0);
                rsp.get_weapon_list().Add(weapon2);
                _weaponUidSet[data2.avatarID] = weapon2.get_unique_id();
            }
        }

        public static void AutoTestForAndroid()
        {
            GameObject target = new GameObject("FakeAutoTest");
            UnityEngine.Object.DontDestroyOnLoad(target);
            MonoBehaviour behaviour = target.AddComponent<MonoBehaviour>();
            MiHoYoGameData.DeleteAllData();
            behaviour.StartCoroutine(DoAutoTestForAndroid());
        }

        [DebuggerHidden]
        private static IEnumerator DoAutoTestForAndroid()
        {
            return new <DoAutoTestForAndroid>c__Iterator4B();
        }

        public static void FakeConnectDispatch()
        {
            string aJSON = "{\"account_url\" : \"fake\"}";
            Singleton<NetworkManager>.Instance.DispatchSeverData = new DispatchServerDataItem(JSON.Parse(aJSON));
        }

        private static uint GenerateNewUid()
        {
            return ++uid;
        }

        public static AvatarReviveRsp GetAvatarReviveRsp()
        {
            AvatarReviveRsp rsp = new AvatarReviveRsp();
            rsp.set_retcode(0);
            rsp.set_revive_times(1);
            return rsp;
        }

        private static string GetClassNameByType(System.Type type)
        {
            char[] separator = new char[] { "."[0] };
            string[] strArray = type.ToString().Split(separator);
            return strArray[strArray.Length - 1];
        }

        public static GetAskAddFriendListRsp GetFakeAskAddFriendListRsp()
        {
            GetAskAddFriendListRsp rsp = new GetAskAddFriendListRsp();
            List<AvatarMetaData> itemList = AvatarMetaDataReader.GetItemList();
            for (int i = 0; i < 100; i++)
            {
                PlayerFriendBriefData item = new PlayerFriendBriefData();
                item.set_uid((uint) (i + 0x2710));
                item.set_nickname("friend_" + i);
                item.set_level((uint) UnityEngine.Random.Range(1, 100));
                item.set_avatar_combat((uint) UnityEngine.Random.Range(100, 0x2710));
                item.set_avatar_star((uint) UnityEngine.Random.Range(1, 5));
                item.set_avatar_id((uint) itemList[UnityEngine.Random.Range(0, itemList.Count)].avatarID);
                rsp.get_ask_list().Add(item);
            }
            return rsp;
        }

        public static GetAvatarDataRsp GetFakeAvatarDataRsp()
        {
            GetAvatarDataRsp rsp = new GetAvatarDataRsp();
            List<int> list = new List<int> { 0x191, 0x192, 0x193, 0x194 };
            foreach (AvatarMetaData data in AvatarMetaDataReader.GetItemList())
            {
                if (!list.Contains(data.avatarID))
                {
                    Avatar item = new Avatar();
                    item.set_avatar_id((uint) data.avatarID);
                    item.set_star((uint) data.unlockStar);
                    item.set_level(10);
                    item.set_exp(0);
                    item.set_fragment(100);
                    item.set_stigmata_unique_id_1(0);
                    item.set_stigmata_unique_id_2(0);
                    item.set_stigmata_unique_id_3(0);
                    item.set_touch_goodfeel(0);
                    item.set_today_has_add_goodfeel(0);
                    item.set_stage_goodfeel(0);
                    item.set_weapon_unique_id(_weaponUidSet[data.avatarID]);
                    rsp.get_avatar_list().Add(item);
                }
            }
            return rsp;
        }

        public static GetEquipmentDataRsp GetFakeEquipmentDataRsp()
        {
            int addNum = 100;
            int num2 = 10;
            int num3 = 10;
            GetEquipmentDataRsp rsp = new GetEquipmentDataRsp();
            AddWeaponIntoRsp(rsp, addNum);
            AddStigmataIntoRsp(rsp, num2);
            AddMaterialIntoRsp(rsp, num3);
            return rsp;
        }

        public static GetFinishGuideDataRsp GetFakeFinishGuideDataRsp()
        {
            GetFinishGuideDataRsp rsp = new GetFinishGuideDataRsp();
            rsp.set_retcode(0);
            foreach (LevelTutorialMetaData data in LevelTutorialMetaDataReader.GetItemList())
            {
                rsp.get_guide_id_list().Add((uint) data.tutorialId);
            }
            foreach (TutorialData data2 in TutorialDataReader.GetItemList())
            {
                rsp.get_guide_id_list().Add((uint) data2.id);
            }
            return rsp;
        }

        public static GetFriendListRsp GetFakeFriendListRsp()
        {
            GetFriendListRsp rsp = new GetFriendListRsp();
            List<AvatarMetaData> itemList = AvatarMetaDataReader.GetItemList();
            for (int i = 0; i < 100; i++)
            {
                PlayerFriendBriefData item = new PlayerFriendBriefData();
                item.set_uid((uint) (i + 0x2710));
                item.set_nickname("friend_" + i);
                item.set_level((uint) UnityEngine.Random.Range(1, 100));
                item.set_avatar_combat((uint) UnityEngine.Random.Range(100, 0x2710));
                item.set_avatar_star((uint) UnityEngine.Random.Range(1, 5));
                item.set_avatar_id((uint) itemList[UnityEngine.Random.Range(0, itemList.Count)].avatarID);
                rsp.get_friend_list().Add(item);
            }
            return rsp;
        }

        public static GetAvatarTeamDataRsp GetFakeGetAvatarTeamDataRsp()
        {
            GetAvatarTeamDataRsp rsp = new GetAvatarTeamDataRsp();
            rsp.set_retcode(0);
            List<StageType> list = new List<StageType> { 1 };
            List<uint> collection = new List<uint> { 0x65, 0x66 };
            foreach (StageType type in list)
            {
                AvatarTeam item = new AvatarTeam();
                item.set_stage_type(type);
                item.get_avatar_id_list().AddRange(collection);
                rsp.get_avatar_team_list().Add(item);
            }
            return rsp;
        }

        public static GetGachaDisplayRsp GetFakeGetGachaDisplayRsp()
        {
            GetGachaDisplayRsp rsp = new GetGachaDisplayRsp();
            rsp.set_retcode(0);
            rsp.set_hcoin_gacha_data(new HcoinGachaData());
            rsp.get_hcoin_gacha_data().set_common_data(new GachaDisplayCommonData());
            rsp.set_friends_point_gacha_data(new FriendsPointGachaData());
            rsp.get_friends_point_gacha_data().set_friends_point_cost(1);
            rsp.get_friends_point_gacha_data().set_common_data(new GachaDisplayCommonData());
            return rsp;
        }

        public static GetMailDataRsp GetFakeMailDataRsp()
        {
            GetMailDataRsp rsp = new GetMailDataRsp();
            for (int i = 0; i < 20; i++)
            {
                Mail item = new Mail();
                item.set_id((uint) (i + 0x2710));
                item.set_type(3);
                item.set_title("This is Test mail with id: " + i);
                item.set_content("This is Test mail with id: " + i);
                item.set_sender("Tester");
                MailAttachment attachment = new MailAttachment();
                attachment.set_hcoin(10);
                item.set_attachment(attachment);
                rsp.get_mail_list().Add(item);
            }
            return rsp;
        }

        public static GetPlayerDetailDataRsp GetFakePlayerDetailDataRsp(uint targetID)
        {
            GetPlayerDetailDataRsp rsp = new GetPlayerDetailDataRsp();
            rsp.set_detail(new PlayerDetailData());
            rsp.get_detail().set_uid(targetID);
            rsp.get_detail().set_leader_avatar(new AvatarDetailData());
            rsp.get_detail().get_leader_avatar().set_avatar_id(0x65);
            WeaponDetailData data = new WeaponDetailData();
            data.set_id((uint) AvatarMetaDataReader.GetAvatarMetaDataByKey((int) rsp.get_detail().get_leader_avatar().get_avatar_id()).initialWeapon);
            data.set_level(1);
            rsp.get_detail().get_leader_avatar().set_weapon(data);
            return rsp;
        }

        public static GetRecommendFriendListRsp GetFakeRecommendFriendListRsp()
        {
            GetRecommendFriendListRsp rsp = new GetRecommendFriendListRsp();
            List<AvatarMetaData> itemList = AvatarMetaDataReader.GetItemList();
            for (int i = 0; i < 100; i++)
            {
                PlayerFriendBriefData item = new PlayerFriendBriefData();
                item.set_uid((uint) (i + 0x2710));
                item.set_nickname("friend_" + i);
                item.set_level((uint) UnityEngine.Random.Range(1, 100));
                item.set_avatar_combat((uint) UnityEngine.Random.Range(100, 0x2710));
                item.set_avatar_star((uint) UnityEngine.Random.Range(1, 5));
                item.set_avatar_id((uint) itemList[UnityEngine.Random.Range(0, itemList.Count)].avatarID);
                rsp.get_recommend_list().Add(item);
            }
            return rsp;
        }

        public static T GetFakeRsp<T>()
        {
            string classNameByType = GetClassNameByType(typeof(T));
            JSONNode json = inputDict[classNameByType];
            return (T) JsonDeserialize(json, typeof(T));
        }

        public static GetStageDataRsp GetFakeStageDataRsp()
        {
            <GetFakeStageDataRsp>c__AnonStoreyDA yda = new <GetFakeStageDataRsp>c__AnonStoreyDA();
            GetStageDataRsp rsp = new GetStageDataRsp();
            yda.containType = new List<StageType> { 1 };
            foreach (LevelMetaData data in LevelMetaDataReader.GetItemList().FindAll(new Predicate<LevelMetaData>(yda.<>m__F7)))
            {
                Stage item = new Stage();
                item.set_id((uint) data.levelId);
                rsp.get_stage_list().Add(item);
            }
            return rsp;
        }

        public static StageEndRsp GetFakeStageEndRsp()
        {
            StageEndRsp rsp = new StageEndRsp();
            rsp.set_retcode(0);
            return rsp;
        }

        private static object JsonDeserialize(JSONNode json, System.Type type)
        {
            object instance = Activator.CreateInstance(type);
            IEnumerator<string> enumerator = json.Keys.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    string current = enumerator.Current;
                    if (json[current] is JSONArray)
                    {
                        System.Type type3 = ReflectionUtil.GetValueType(instance, current).GetGenericArguments()[0];
                        IList list = ReflectionUtil.GetValue(instance, current) as IList;
                        IEnumerator enumerator2 = (json[current] as JSONArray).GetEnumerator();
                        try
                        {
                            while (enumerator2.MoveNext())
                            {
                                JSONNode node = (JSONNode) enumerator2.Current;
                                list.Add(JsonDeserialize(node, type3));
                            }
                        }
                        finally
                        {
                            IDisposable disposable = enumerator2 as IDisposable;
                            if (disposable == null)
                            {
                            }
                            disposable.Dispose();
                        }
                    }
                    else
                    {
                        System.Type valueType = ReflectionUtil.GetValueType(instance, current);
                        if (valueType == typeof(uint))
                        {
                            ReflectionUtil.SetValue(instance, current, (uint) json[current].AsInt);
                        }
                        else
                        {
                            if (valueType == typeof(string))
                            {
                                ReflectionUtil.SetValue(instance, current, json[current].Value);
                                continue;
                            }
                            if (valueType.IsEnum)
                            {
                                ReflectionUtil.SetValue(instance, current, json[current].AsInt);
                            }
                        }
                    }
                }
            }
            finally
            {
                if (enumerator == null)
                {
                }
                enumerator.Dispose();
            }
            return instance;
        }

        public static void LoadFromFile()
        {
            inputDict = JSON.Parse(Miscs.LoadTextFileToString("DataDev/FakePacketRsp.json"));
        }

        [CompilerGenerated]
        private sealed class <DoAutoTestForAndroid>c__Iterator4B : IEnumerator, IDisposable, IEnumerator<object>
        {
            internal object $current;
            internal int $PC;
            internal WaitForSeconds <WAIT_LONG_STEP>__1;
            internal WaitForSeconds <WAIT_STEP>__0;

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
                        this.<WAIT_STEP>__0 = new WaitForSeconds(1f);
                        this.<WAIT_LONG_STEP>__1 = new WaitForSeconds(5f);
                        this.$current = null;
                        this.$PC = 1;
                        goto Label_04C2;

                    case 1:
                        Singleton<NetworkManager>.Instance.LoginGameServer();
                        break;

                    case 2:
                        break;

                    case 3:
                        Singleton<MainUIManager>.Instance.ShowPage(new GachaMainPageContext(), UIType.Page);
                        goto Label_0125;

                    case 4:
                        goto Label_0125;

                    case 5:
                        goto Label_017F;

                    case 6:
                        Singleton<MainUIManager>.Instance.CurrentPageContext.BackToMainMenuPage();
                        goto Label_01D8;

                    case 7:
                        goto Label_01D8;

                    case 8:
                        goto Label_021A;

                    case 9:
                        GameObject.Find("StorageShowPage(Clone)").transform.Find("WeaponTab/ScrollView/Content/0").GetComponent<Button>().onClick.Invoke();
                        goto Label_028E;

                    case 10:
                        goto Label_028E;

                    case 11:
                        GameObject.Find("WeaponDetailPage(Clone)").transform.Find("ActionBtns/PowerUpBtn").GetComponent<Button>().onClick.Invoke();
                        goto Label_0302;

                    case 12:
                        goto Label_0302;

                    case 13:
                        goto Label_035D;

                    case 14:
                        GameObject.Find("StorageShowPage(Clone)").transform.Find("PowerUpPanel/OKBtn").GetComponent<Button>().onClick.Invoke();
                        goto Label_03F9;

                    case 15:
                        goto Label_03F9;

                    case 0x10:
                        Singleton<MainUIManager>.Instance.CurrentPageContext.BackToMainMenuPage();
                        goto Label_047C;

                    case 0x11:
                        goto Label_047C;

                    default:
                        goto Label_04C0;
                }
                if (GameObject.Find("MainPage(Clone)") == null)
                {
                    this.$current = this.<WAIT_STEP>__0;
                    this.$PC = 2;
                }
                else
                {
                    Singleton<MainUIManager>.Instance.ShowPage(new SupplyEntrancePageContext(), UIType.Page);
                    this.$current = this.<WAIT_STEP>__0;
                    this.$PC = 3;
                }
                goto Label_04C2;
            Label_0125:
                while (GameObject.Find("GachaMainPage(Clone)") == null)
                {
                    this.$current = this.<WAIT_STEP>__0;
                    this.$PC = 4;
                    goto Label_04C2;
                }
                GameObject.Find("GachaMainPage(Clone)").transform.Find("HCoinTab/ActBtns/Ten/Btn").GetComponent<Button>().onClick.Invoke();
            Label_017F:
                while (GameObject.Find("GachaResultPage(Clone)") == null)
                {
                    this.$current = this.<WAIT_STEP>__0;
                    this.$PC = 5;
                    goto Label_04C2;
                }
                this.$current = this.<WAIT_LONG_STEP>__1;
                this.$PC = 6;
                goto Label_04C2;
            Label_01D8:
                while (GameObject.Find("MainPage(Clone)") == null)
                {
                    this.$current = this.<WAIT_STEP>__0;
                    this.$PC = 7;
                    goto Label_04C2;
                }
                Singleton<MainUIManager>.Instance.ShowPage(new StorageShowPageContext(), UIType.Page);
            Label_021A:
                while (GameObject.Find("StorageShowPage(Clone)") == null)
                {
                    this.$current = this.<WAIT_STEP>__0;
                    this.$PC = 8;
                    goto Label_04C2;
                }
                this.$current = this.<WAIT_STEP>__0;
                this.$PC = 9;
                goto Label_04C2;
            Label_028E:
                while (GameObject.Find("WeaponDetailPage(Clone)") == null)
                {
                    this.$current = this.<WAIT_STEP>__0;
                    this.$PC = 10;
                    goto Label_04C2;
                }
                this.$current = this.<WAIT_LONG_STEP>__1;
                this.$PC = 11;
                goto Label_04C2;
            Label_0302:
                while (GameObject.Find("WeaponPowerUpPage(Clone)") == null)
                {
                    this.$current = this.<WAIT_STEP>__0;
                    this.$PC = 12;
                    goto Label_04C2;
                }
                GameObject.Find("WeaponPowerUpPage(Clone)").transform.Find("ResourceList/Content/1").GetComponent<Button>().onClick.Invoke();
            Label_035D:
                while (GameObject.Find("StorageShowPage(Clone)") == null)
                {
                    this.$current = this.<WAIT_STEP>__0;
                    this.$PC = 13;
                    goto Label_04C2;
                }
                GameObject.Find("StorageShowPage(Clone)").transform.Find("WeaponTab/ScrollView/Content/0").GetComponent<Button>().onClick.Invoke();
                this.$current = this.<WAIT_STEP>__0;
                this.$PC = 14;
                goto Label_04C2;
            Label_03F9:
                while (GameObject.Find("WeaponPowerUpPage(Clone)") == null)
                {
                    this.$current = this.<WAIT_STEP>__0;
                    this.$PC = 15;
                    goto Label_04C2;
                }
                GameObject.Find("WeaponPowerUpPage(Clone)").transform.Find("ActionBtns/OkBtn").GetComponent<Button>().onClick.Invoke();
                this.$current = this.<WAIT_LONG_STEP>__1;
                this.$PC = 0x10;
                goto Label_04C2;
            Label_047C:
                while (GameObject.Find("MainPage(Clone)") == null)
                {
                    this.$current = this.<WAIT_STEP>__0;
                    this.$PC = 0x11;
                    goto Label_04C2;
                }
                GameObject.Find("MainPage(Clone)").transform.Find("AutoBattlePanel/StartButton").GetComponent<Button>().onClick.Invoke();
                this.$PC = -1;
            Label_04C0:
                return false;
            Label_04C2:
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

        [CompilerGenerated]
        private sealed class <GetFakeStageDataRsp>c__AnonStoreyDA
        {
            internal List<StageType> containType;

            internal bool <>m__F7(LevelMetaData x)
            {
                return this.containType.Contains((StageType) x.type);
            }
        }
    }
}

