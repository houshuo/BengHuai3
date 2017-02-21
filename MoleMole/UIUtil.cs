namespace MoleMole
{
    using MoleMole.MainMenu;
    using proto;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.UI;

    public class UIUtil
    {
        [CompilerGenerated]
        private static Action<bool> <>f__am$cache0;
        [CompilerGenerated]
        private static Action<bool> <>f__am$cache1;

        public static void CalCulateExpFromItems(out float scoinNeed, out float expGet, List<StorageDataItemBase> dogFoodList, StorageDataItemBase equipToPowerUp)
        {
            float f = 0f;
            foreach (StorageDataItemBase base2 in dogFoodList)
            {
                float gearExp = base2.GetGearExp();
                MaterialExpBonusMetaData data = MaterialExpBonusMetaDataReader.TryGetMaterialExpBonusMetaDataByKey(base2.ID);
                if (data != null)
                {
                    if (equipToPowerUp is WeaponDataItem)
                    {
                        gearExp *= data.weaponExpBonus / 100f;
                    }
                    else if (equipToPowerUp is StigmataDataItem)
                    {
                        gearExp *= data.stigmataExpBonus / 100f;
                    }
                }
                if (base2.GetType() == equipToPowerUp.GetType())
                {
                    gearExp *= ((float) Singleton<PlayerModule>.Instance.playerData.sameTypePowerUpRataInt) / 100f;
                }
                f += gearExp;
            }
            expGet = Mathf.FloorToInt(f);
            scoinNeed = (expGet * Singleton<PlayerModule>.Instance.playerData.powerUpScoinCostRate) / 100f;
        }

        public static int CalculateLvWithExp(float exp, StorageDataItemBase equipToPowerUp)
        {
            List<EquipmentLevelMetaData> itemList = EquipmentLevelMetaDataReader.GetItemList();
            int expType = equipToPowerUp.GetExpType();
            int maxLevel = equipToPowerUp.GetMaxLevel();
            float num3 = exp + equipToPowerUp.exp;
            int level = equipToPowerUp.level;
            while ((num3 > 0f) && (level < maxLevel))
            {
                int num5 = itemList[level - 1].expList[expType];
                if (num5 > num3)
                {
                    return level;
                }
                num3 -= num5;
                level++;
            }
            return level;
        }

        public static bool ContainsUIAvatar(int avatarID)
        {
            BaseMonoCanvas sceneCanvas = Singleton<MainUIManager>.Instance.SceneCanvas;
            if (sceneCanvas is MonoMainCanvas)
            {
                return ((MonoMainCanvas) sceneCanvas).avatar3dModelContext.ContainUIAvatar(avatarID);
            }
            if (sceneCanvas is MonoTestUI)
            {
                return ((MonoTestUI) sceneCanvas).avatar3dModelContext.ContainUIAvatar(avatarID);
            }
            return ((MonoGameEntry) sceneCanvas).avatar3dModelContext.ContainUIAvatar(avatarID);
        }

        public static void Create3DAvatarByPage(AvatarDataItem avatar, MiscData.PageInfoKey pageKey, string tabName = "Default")
        {
            ConfigPageAvatarShowInfo pageAvatarShowInfo = MiscData.GetPageAvatarShowInfo(pageKey);
            ConfigAvatarShowInfo info2 = GetAvatarShowInfo(avatar, pageKey, tabName);
            List<Avatar3dModelDataItem> body = new List<Avatar3dModelDataItem> {
                new Avatar3dModelDataItem(avatar, info2.Avatar.Position, info2.Avatar.EulerAngle, pageAvatarShowInfo.ShowLockViewIfLock)
            };
            Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.CreateAvatarUIModels, body));
            GameObject obj2 = GameObject.Find("LookAt");
            if (obj2 != null)
            {
                obj2.transform.position = info2.LookAt.Position;
                obj2.transform.eulerAngles = info2.LookAt.EulerAngle;
            }
        }

        public static int FloorToIntCustom(float num)
        {
            return (((0f >= num) || (num >= 1f)) ? Mathf.FloorToInt(num) : 1);
        }

        public static Sprite GetAvatarCardIcon(int avatarID)
        {
            return Miscs.GetSpriteByPrefab(AvatarCardMetaDataReader.GetAvatarCardMetaDataByKey(AvatarMetaDataReaderExtend.GetAvatarIDsByKey(avatarID).avatarCardID).iconPath);
        }

        public static List<float> GetAvatarMaxExpList(AvatarDataItem avatarData, int fromLevel, int toLevel)
        {
            List<float> list = new List<float>();
            List<AvatarLevelMetaData> itemList = AvatarLevelMetaDataReader.GetItemList();
            for (int i = fromLevel; i <= toLevel; i++)
            {
                list.Add((float) itemList[i - 1].exp);
            }
            return list;
        }

        public static ConfigAvatarShowInfo GetAvatarShowInfo(AvatarDataItem avatar, MiscData.PageInfoKey pageKey, string tabName)
        {
            ConfigTabAvatarTransformInfo info2;
            ConfigPageAvatarShowInfo pageAvatarShowInfo = MiscData.GetPageAvatarShowInfo(pageKey);
            string avatarRegistryKey = avatar.AvatarRegistryKey;
            char[] separator = new char[] { '_' };
            string key = avatarRegistryKey.Split(separator)[0];
            if (pageAvatarShowInfo.AvatarTabTransformInfos.ContainsKey(avatarRegistryKey))
            {
                info2 = pageAvatarShowInfo.AvatarTabTransformInfos[avatarRegistryKey];
            }
            else if (pageAvatarShowInfo.AvatarTabTransformInfos.ContainsKey(key))
            {
                info2 = pageAvatarShowInfo.AvatarTabTransformInfos[key];
            }
            else
            {
                info2 = pageAvatarShowInfo.AvatarTabTransformInfos["Default"];
            }
            string str3 = tabName + "_" + avatar.avatarID;
            if (info2.AvatarShowInfos.ContainsKey(str3))
            {
                return info2.AvatarShowInfos[str3];
            }
            if (info2.AvatarShowInfos.ContainsKey(tabName))
            {
                return info2.AvatarShowInfos[tabName];
            }
            return info2.AvatarShowInfos["Default"];
        }

        public static LevelDiffculty GetDifficultyFromMark(char mark)
        {
            LevelDiffculty normal = LevelDiffculty.Normal;
            switch (mark)
            {
                case 'H':
                    return LevelDiffculty.Hard;

                case 'S':
                    return LevelDiffculty.Hell;
            }
            return normal;
        }

        public static string GetDifficultyMark(LevelDiffculty difficulty)
        {
            string str = "N";
            LevelDiffculty diffculty = difficulty;
            if (diffculty != LevelDiffculty.Hard)
            {
                if (diffculty != LevelDiffculty.Hell)
                {
                    return str;
                }
            }
            else
            {
                return "H";
            }
            return "S";
        }

        public static List<float> GetEquipmentMaxExpList(StorageDataItemBase equipToPowerUp, int fromLevel, int toLevel)
        {
            List<float> list = new List<float>();
            List<EquipmentLevelMetaData> itemList = EquipmentLevelMetaDataReader.GetItemList();
            int expType = equipToPowerUp.GetExpType();
            for (int i = fromLevel; i <= toLevel; i++)
            {
                list.Add((float) itemList[i - 1].expList[expType]);
            }
            return list;
        }

        public static bool GetEquipmentSlot(StorageDataItemBase storageData, out EquipmentSlot slot)
        {
            if (storageData is WeaponDataItem)
            {
                slot = 1;
                return true;
            }
            if (storageData is StigmataDataItem)
            {
                switch ((storageData as StigmataDataItem).GetBaseType())
                {
                    case 1:
                        slot = 2;
                        return true;

                    case 2:
                        slot = 3;
                        return true;

                    case 3:
                        slot = 4;
                        return true;
                }
                slot = 1;
                return false;
            }
            slot = 1;
            return false;
        }

        public static List<int> GetGoodsOriginPrice(Goods goods)
        {
            ShopGoodsMetaData shopGoodsMetaDataByKey = ShopGoodsMetaDataReader.GetShopGoodsMetaDataByKey((int) goods.get_goods_id());
            List<int> list = new List<int>();
            if (shopGoodsMetaDataByKey.HCoinCost > 0)
            {
                list.Add(shopGoodsMetaDataByKey.HCoinCost);
            }
            if (shopGoodsMetaDataByKey.SCoinCost > 0)
            {
                list.Add(shopGoodsMetaDataByKey.SCoinCost);
            }
            if (shopGoodsMetaDataByKey.CostItemId > 0)
            {
                list.Add(shopGoodsMetaDataByKey.CostItemNum);
            }
            if (shopGoodsMetaDataByKey.CostItemId2 > 0)
            {
                list.Add(shopGoodsMetaDataByKey.CostItemNum2);
            }
            if (shopGoodsMetaDataByKey.CostItemId3 > 0)
            {
                list.Add(shopGoodsMetaDataByKey.CostItemNum3);
            }
            if (shopGoodsMetaDataByKey.CostItemId4 > 0)
            {
                list.Add(shopGoodsMetaDataByKey.CostItemNum4);
            }
            if (shopGoodsMetaDataByKey.CostItemId5 > 0)
            {
                list.Add(shopGoodsMetaDataByKey.CostItemNum5);
            }
            float num = 1f;
            int buyTimes = ((int) goods.get_buy_times()) + 1;
            ShopGoodsPriceRateMetaData shopGoodsPriceRateMetaDataByKey = ShopGoodsPriceRateMetaDataReader.GetShopGoodsPriceRateMetaDataByKey(buyTimes);
            if (shopGoodsMetaDataByKey.PriceRateID == 1)
            {
                num *= shopGoodsPriceRateMetaDataByKey.PriceType1;
            }
            else if (shopGoodsMetaDataByKey.PriceRateID == 2)
            {
                num *= shopGoodsPriceRateMetaDataByKey.PriceType2;
            }
            else if (shopGoodsMetaDataByKey.PriceRateID == 3)
            {
                num *= shopGoodsPriceRateMetaDataByKey.PriceType3;
            }
            else if (shopGoodsMetaDataByKey.PriceRateID == 4)
            {
                num *= shopGoodsPriceRateMetaDataByKey.PriceType4;
            }
            else if (shopGoodsMetaDataByKey.PriceRateID == 5)
            {
                num *= shopGoodsPriceRateMetaDataByKey.PriceType5;
            }
            else if (shopGoodsMetaDataByKey.PriceRateID == 6)
            {
                num *= shopGoodsPriceRateMetaDataByKey.PriceType6;
            }
            else if (shopGoodsMetaDataByKey.PriceRateID == 7)
            {
                num *= shopGoodsPriceRateMetaDataByKey.PriceType7;
            }
            else if (shopGoodsMetaDataByKey.PriceRateID == 8)
            {
                num *= shopGoodsPriceRateMetaDataByKey.PriceType8;
            }
            else if (shopGoodsMetaDataByKey.PriceRateID == 9)
            {
                num *= shopGoodsPriceRateMetaDataByKey.PriceType9;
            }
            else if (shopGoodsMetaDataByKey.PriceRateID == 10)
            {
                num *= shopGoodsPriceRateMetaDataByKey.PriceType10;
            }
            num /= 100f;
            for (int i = 0; i < list.Count; i++)
            {
                list[i] = Mathf.FloorToInt(((float) list[i]) * num);
            }
            return list;
        }

        public static List<int> GetGoodsRealPrice(Goods goods)
        {
            ShopGoodsMetaData shopGoodsMetaDataByKey = ShopGoodsMetaDataReader.GetShopGoodsMetaDataByKey((int) goods.get_goods_id());
            List<int> goodsOriginPrice = GetGoodsOriginPrice(goods);
            for (int i = 0; i < goodsOriginPrice.Count; i++)
            {
                goodsOriginPrice[i] = (int) Mathf.Floor((((float) goodsOriginPrice[i]) * shopGoodsMetaDataByKey.Discount) / 10000f);
            }
            return goodsOriginPrice;
        }

        public static List<float> GetPlayerMaxExpList(int fromLevel, int toLevel)
        {
            List<float> list = new List<float>();
            List<PlayerLevelMetaData> itemList = PlayerLevelMetaDataReader.GetItemList();
            for (int i = fromLevel; i <= toLevel; i++)
            {
                list.Add((float) itemList[i - 1].exp);
            }
            return list;
        }

        public static string GetPlayerNickname(PlayerFriendBriefData briefData)
        {
            if (briefData.get_nicknameSpecified() && !string.IsNullOrEmpty(briefData.get_nickname()))
            {
                return briefData.get_nickname();
            }
            object[] replaceParams = new object[] { briefData.get_uid() };
            return LocalizationGeneralLogic.GetText("Menu_DefaultNickname", replaceParams);
        }

        public static string GetResourceIconPath(ResourceType resourceType, int itemID = 0)
        {
            switch (resourceType)
            {
                case ResourceType.Hcoin:
                    return "SpriteOutput/GeneralIcon/IconHC";

                case ResourceType.Scoin:
                    return "SpriteOutput/GeneralIcon/IconSC";

                case ResourceType.PlayerExp:
                    return "SpriteOutput/GeneralIcon/IconEXP";

                case ResourceType.Stamina:
                    return "SpriteOutput/GeneralIcon/IconST";

                case ResourceType.SkillPoint:
                    return "SpriteOutput/GeneralIcon/IconSP";

                case ResourceType.FriendPoint:
                    return "SpriteOutput/GeneralIcon/IconFP";
            }
            if (itemID != 0)
            {
                StorageDataItemBase dummyStorageDataItem = Singleton<StorageModule>.Instance.GetDummyStorageDataItem(itemID, 1);
                if (dummyStorageDataItem != null)
                {
                    return dummyStorageDataItem.GetIconPath();
                }
            }
            return null;
        }

        public static Sprite GetResourceSprite(ResourceType resourceType, StorageDataItemBase itemData = null)
        {
            switch (resourceType)
            {
                case ResourceType.Hcoin:
                    return Miscs.GetSpriteByPrefab("SpriteOutput/GeneralIcon/IconHC");

                case ResourceType.Scoin:
                    return Miscs.GetSpriteByPrefab("SpriteOutput/GeneralIcon/IconSC");

                case ResourceType.PlayerExp:
                    return Miscs.GetSpriteByPrefab("SpriteOutput/GeneralIcon/IconEXP");

                case ResourceType.Stamina:
                    return Miscs.GetSpriteByPrefab("SpriteOutput/GeneralIcon/IconST");

                case ResourceType.SkillPoint:
                    return Miscs.GetSpriteByPrefab("SpriteOutput/GeneralIcon/IconSP");

                case ResourceType.FriendPoint:
                    return Miscs.GetSpriteByPrefab("SpriteOutput/GeneralIcon/IconFP");
            }
            if (itemData == null)
            {
                return null;
            }
            return Miscs.GetSpriteByPrefab(itemData.GetIconPath());
        }

        public static BaseMonoUIAvatar GetUIAvatar(int avatarID)
        {
            Transform avatarById;
            BaseMonoCanvas sceneCanvas = Singleton<MainUIManager>.Instance.SceneCanvas;
            if (sceneCanvas is MonoMainCanvas)
            {
                avatarById = ((MonoMainCanvas) sceneCanvas).avatar3dModelContext.GetAvatarById(avatarID);
            }
            else if (sceneCanvas is MonoTestUI)
            {
                avatarById = ((MonoTestUI) sceneCanvas).avatar3dModelContext.GetAvatarById(avatarID);
            }
            else
            {
                avatarById = ((MonoGameEntry) sceneCanvas).avatar3dModelContext.GetAvatarById(avatarID);
            }
            if (avatarById == null)
            {
                return null;
            }
            return avatarById.GetComponent<BaseMonoUIAvatar>();
        }

        public static Transform GetUIAvatarTattooByID(int avatarID, string attachmentName)
        {
            BaseMonoUIAvatar uIAvatar = GetUIAvatar(avatarID);
            if (uIAvatar == null)
            {
                return null;
            }
            if (!uIAvatar.HasAttachPoint(attachmentName))
            {
                return null;
            }
            return uIAvatar.GetAttachPoint(attachmentName);
        }

        public static string ProcessStrWithNewLine(string text)
        {
            if (!string.IsNullOrEmpty(text))
            {
                text = text.Replace(@"\n", Environment.NewLine);
                text = text.Replace("{{{", Environment.NewLine);
            }
            return text;
        }

        public static void SetAvatarTattooVisible(bool visible, AvatarDataItem avatarData)
        {
            BaseMonoUIAvatar uIAvatar = GetUIAvatar(avatarData.avatarID);
            if (uIAvatar != null)
            {
                uIAvatar.tattooVisible = visible;
                uIAvatar.avatarData = avatarData;
                Transform uIAvatarTattooByID = GetUIAvatarTattooByID(avatarData.avatarID, "Stigmata");
                if (uIAvatarTattooByID != null)
                {
                    uIAvatarTattooByID.gameObject.SetActive(visible);
                }
                if (visible)
                {
                    UpdateAvatarTattoo(avatarData);
                }
            }
        }

        public static void SetCameraLookAt(AvatarDataItem avatar, MiscData.PageInfoKey pageKey, string tabName)
        {
            ConfigAvatarShowInfo info = GetAvatarShowInfo(avatar, pageKey, tabName);
            Transform transform = GameObject.Find("LookAt").transform;
            transform.position = info.LookAt.Position;
            transform.eulerAngles = info.LookAt.EulerAngle;
        }

        public static Color SetupColor(string hexString)
        {
            Color white = Color.white;
            if (ColorUtility.TryParseHtmlString(hexString, out white))
            {
                return white;
            }
            return white;
        }

        public static void ShowAppStoreComment(int appCommentId)
        {
            if (!Singleton<CommonIDModule>.Instance.IsCommonFinished(appCommentId))
            {
                ShowAppstoreCommentDialog();
                Singleton<CommonIDModule>.Instance.MarkCommonIDFinish(appCommentId);
            }
        }

        public static void ShowAppstoreCommentDialog()
        {
            <ShowAppstoreCommentDialog>c__AnonStorey104 storey = new <ShowAppstoreCommentDialog>c__AnonStorey104();
            if (!Singleton<NetworkManager>.Instance.DispatchSeverData.isReview)
            {
                storey.times = 1;
                if (Singleton<CommonIDModule>.Instance.IsCommonFinished(CommonIDModule.APP_STORE_COMMENT_ID_1))
                {
                    storey.times++;
                }
                if (Singleton<CommonIDModule>.Instance.IsCommonFinished(CommonIDModule.APP_STORE_COMMENT_ID_2))
                {
                    storey.times++;
                }
                storey.times = Mathf.Clamp(storey.times, 1, 2);
                GeneralDialogContext dialogContext = new GeneralDialogContext {
                    type = GeneralDialogContext.ButtonType.DoubleButton,
                    title = LocalizationGeneralLogic.GetText("Menu_Title_AppstoreComment", new object[0]),
                    desc = LocalizationGeneralLogic.GetText("Menu_Desc_AppstoreComment", new object[0]),
                    okBtnText = LocalizationGeneralLogic.GetText("Menu_OkBtn_AppstoreComment", new object[0]),
                    cancelBtnText = LocalizationGeneralLogic.GetText("Menu_CancelBtn_AppstoreComment", new object[0]),
                    notDestroyAfterTouchBG = true,
                    buttonCallBack = new Action<bool>(storey.<>m__1B2),
                    destroyCallBack = new Action(storey.<>m__1B3)
                };
                Singleton<MainUIManager>.Instance.ShowDialog(dialogContext, UIType.Any);
            }
        }

        public static void ShowBindWarningDialog()
        {
            GeneralDialogContext context2 = new GeneralDialogContext {
                type = GeneralDialogContext.ButtonType.DoubleButton,
                title = LocalizationGeneralLogic.GetText("Menu_Title_BindAccount", new object[0]),
                desc = LocalizationGeneralLogic.GetText("Menu_Desc_BindAccount", new object[0]),
                okBtnText = LocalizationGeneralLogic.GetText("Menu_Action_DoBindAccount", new object[0]),
                cancelBtnText = LocalizationGeneralLogic.GetText("Menu_Action_CancelBindAccount", new object[0])
            };
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = delegate (bool confirmed) {
                    if (confirmed)
                    {
                        Singleton<AccountManager>.Instance.manager.BindUI();
                    }
                };
            }
            context2.buttonCallBack = <>f__am$cache0;
            GeneralDialogContext dialogContext = context2;
            Singleton<MainUIManager>.Instance.ShowDialog(dialogContext, UIType.Any);
        }

        public static bool ShowFriendDetailInfo(FriendDetailDataItem detailData, bool fromDialog = false, Transform dialogTrans = null)
        {
            RemoteAvatarDetailPageContext context = new RemoteAvatarDetailPageContext(detailData, fromDialog, dialogTrans);
            Singleton<MainUIManager>.Instance.ShowPage(context, UIType.Page);
            return false;
        }

        public static void ShowItemDetail(StorageDataItemBase item, bool hideActionBtns = false, bool unlock = true)
        {
            if ((item is WeaponDataItem) || (item is StigmataDataItem))
            {
                Singleton<MainUIManager>.Instance.ShowPage(new StorageItemDetailPageContext(item, hideActionBtns, unlock), UIType.Page);
            }
            else if (item is AvatarCardDataItem)
            {
                AvatarDataItem dummyAvatarDataItem = Singleton<AvatarModule>.Instance.GetDummyAvatarDataItem(AvatarMetaDataReaderExtend.GetAvatarIDsByKey(item.ID).avatarID);
                Singleton<MainUIManager>.Instance.ShowPage(new AvatarIntroPageContext(dummyAvatarDataItem), UIType.Page);
            }
            else
            {
                Singleton<MainUIManager>.Instance.ShowDialog(new ItemDetailDialogContext(item, hideActionBtns), UIType.Any);
            }
        }

        public static void ShowResourceDetail(RewardUIData resourceData)
        {
            if (resourceData.rewardType == ResourceType.Item)
            {
                StorageDataItemBase dummyStorageDataItem = Singleton<StorageModule>.Instance.GetDummyStorageDataItem(resourceData.itemID, resourceData.level);
                dummyStorageDataItem.number = resourceData.value;
                ShowItemDetail(dummyStorageDataItem, true, true);
            }
            else
            {
                Singleton<MainUIManager>.Instance.ShowDialog(new ResourceDetailDialogContext(resourceData), UIType.Any);
            }
        }

        public static void SpaceshipCheckWeather()
        {
            if (!SpaceshipCheckWeatherRealTime())
            {
                if (TimeUtil.Now < Singleton<MiHoYoGameData>.Instance.LocalData.EndThunderDateTime)
                {
                    Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.ShowThunderWeather, null));
                }
                else if (!Singleton<MainUIManager>.Instance.spaceShipVisibleOnPreviesPage)
                {
                    if (TimeUtil.Now > Singleton<MiHoYoGameData>.Instance.LocalData.NextRandomDateTime)
                    {
                        Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.ShowRandomWeather, null));
                        Singleton<MiHoYoGameData>.Instance.LocalData.NextRandomDateTime = TimeUtil.Now.AddHours(2.0);
                    }
                    else
                    {
                        Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.ShowDefaultWeather, null));
                    }
                }
            }
        }

        private static bool SpaceshipCheckWeatherRealTime()
        {
            RealTimeWeatherManager instance = Singleton<RealTimeWeatherManager>.Instance;
            if ((instance == null) || !instance.Available)
            {
                return false;
            }
            WeatherInfo weatherInfo = instance.GetWeatherInfo();
            if ((weatherInfo == null) || (weatherInfo.weatherType == WeatherType.None))
            {
                return false;
            }
            string configName = null;
            int sceneId = -1;
            instance.GetWeatherConfig(weatherInfo.weatherType, out configName, out sceneId);
            if ((configName == null) || (sceneId == -1))
            {
                return false;
            }
            MainMenuStage stage = UnityEngine.Object.FindObjectOfType<MainMenuStage>();
            if (stage == null)
            {
                return false;
            }
            ConfigAtmosphereSeries config = ConfigAtmosphereSeries.LoadFromFileAndDetach(string.Format("Rendering/MainMenuAtmosphereConfig/{0}", configName));
            if (config == null)
            {
                return false;
            }
            stage.ChooseCloudScene(config, sceneId);
            return true;
        }

        public static void TryParseHexString(string str, out Color color)
        {
            ColorUtility.TryParseHtmlString(str, out color);
        }

        public static bool TrySetupEventSprite(Image img, string path)
        {
            if (!string.IsNullOrEmpty(path))
            {
                Sprite sprite = Miscs.LoadResource<Sprite>(path, BundleType.RESOURCE_FILE);
                if (sprite != null)
                {
                    img.GetComponent<Image>().sprite = sprite;
                    return true;
                }
                if (GlobalVars.ResourceUseAssetBundle)
                {
                    if (<>f__am$cache1 == null)
                    {
                        <>f__am$cache1 = delegate (bool downloadSucc) {
                            if (downloadSucc)
                            {
                                Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.DownloadResAssetSucc, null));
                            }
                        };
                    }
                    Singleton<AssetBundleManager>.Instance.Loader.TryStartDownloadOneAssetBundle(path, null, <>f__am$cache1);
                }
            }
            return false;
        }

        public static void UpdateAvatarSkillStatusInLocalData(AvatarDataItem avatarData)
        {
            Dictionary<int, SubSkillStatus> subSkillStatusDict = Singleton<MiHoYoGameData>.Instance.LocalData.SubSkillStatusDict;
            foreach (AvatarSkillDataItem item in avatarData.skillDataList)
            {
                foreach (AvatarSubSkillDataItem item2 in item.avatarSubSkillList)
                {
                    if (item2.ShouldShowHintPoint())
                    {
                        subSkillStatusDict[item2.subSkillID] = item2.Status;
                    }
                    else
                    {
                        subSkillStatusDict.Remove(item2.subSkillID);
                    }
                }
            }
            Singleton<MiHoYoGameData>.Instance.Save();
            Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.SubSkillStatusCacheUpdate, null));
        }

        public static void UpdateAvatarTattoo(AvatarDataItem avatarData)
        {
            if ((avatarData != null) && (GetUIAvatarTattooByID(avatarData.avatarID, "Stigmata") != null))
            {
                BaseMonoUIAvatar uIAvatar = GetUIAvatar(avatarData.avatarID);
                if (uIAvatar != null)
                {
                    foreach (KeyValuePair<EquipmentSlot, StigmataDataItem> pair in avatarData.GetStigmataDict())
                    {
                        Transform uIAvatarTattooByID = GetUIAvatarTattooByID(avatarData.avatarID, pair.Key.ToString());
                        if (uIAvatarTattooByID != null)
                        {
                            uIAvatarTattooByID.gameObject.SetActive(pair.Value != null);
                            if (pair.Value != null)
                            {
                                uIAvatarTattooByID.GetComponent<MeshRenderer>().material.SetTexture("_MainTex", Miscs.LoadResource<Texture>(pair.Value.GetTattooPath(), BundleType.RESOURCE_FILE));
                                uIAvatar.StigmataFadeIn(pair.Key);
                            }
                        }
                    }
                }
            }
        }

        [CompilerGenerated]
        private sealed class <ShowAppstoreCommentDialog>c__AnonStorey104
        {
            internal int times;

            internal void <>m__1B2(bool confirmed)
            {
                if (confirmed)
                {
                    Application.OpenURL(MiscData.Config.BasicConfig.AppstoreUrl);
                }
                Singleton<NetworkManager>.Instance.RequestCommentReport(!confirmed ? ((CommentType) 2) : ((CommentType) 1), (uint) this.times);
            }

            internal void <>m__1B3()
            {
                Singleton<NetworkManager>.Instance.RequestCommentReport(3, (uint) this.times);
            }
        }
    }
}

