namespace MoleMole
{
    using proto;
    using SimpleJSON;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Text;
    using UnityEngine;

    public class NetworkManager
    {
        private MiClient _client = new MiClient();
        private MonoClientPacketConsumer _clientPacketConsumer;
        private Dictionary<uint, bool> _CMD_SHOULD_ENQUEUE_MAP;
        private List<string> _globalDispatchUrlList;
        private Dictionary<int, DateTime> _lastRequestTimeDict;
        private Queue<NetPacketV1> _packetSendQueue;
        private Dictionary<int, float> _requestMinIntervalDict;
        private string _uuid;
        public bool alreadyLogin;
        public readonly ConfigChannel channelConfig;
        private const float DISPATCH_CONNECT_TIMEOUT_SECOND = 3f;
        public uint loginRandomNum;
        private const int QUEUE_CAPACITY = 20;
        public readonly ProtobufSerializer serializer;

        public NetworkManager()
        {
            Singleton<CommandMap>.Create();
            this._packetSendQueue = new Queue<NetPacketV1>();
            this.InitClientPacketConsumer();
            this.serializer = new ProtobufSerializer();
            this.alreadyLogin = false;
            this.channelConfig = ConfigUtil.LoadJSONConfig<ConfigChannel>("DataPersistent/BuildChannel/ChannelConfig");
            this._globalDispatchUrlList = this.channelConfig.DispatchUrls;
            GlobalVars.DataUseAssetBundle = this.channelConfig.DataUseAssetBundle;
            GlobalVars.ResourceUseAssetBundle = this.channelConfig.EventUseAssetBundle;
            this._lastRequestTimeDict = new Dictionary<int, DateTime>();
            this.SetupRequestMinIntervalDict();
        }

        private void BuildCmdShouldEnqueueMap()
        {
            Dictionary<uint, bool> dictionary = new Dictionary<uint, bool>();
            dictionary.Add(0xc9, true);
            dictionary.Add(0x3a, true);
            dictionary.Add(0x2b, true);
            dictionary.Add(0x48, true);
            dictionary.Add(0x2d, true);
            dictionary.Add(0x83, true);
            dictionary.Add(0x8d, true);
            dictionary.Add(0x8f, true);
            dictionary.Add(0x6a, true);
            dictionary.Add(0x62, true);
            dictionary.Add(100, true);
            dictionary.Add(0x70, true);
            dictionary.Add(0x75, true);
            dictionary.Add(0x81, true);
            dictionary.Add(0x1f, true);
            this._CMD_SHOULD_ENQUEUE_MAP = dictionary;
        }

        private static bool CheckDeviceUniqueIdentifier()
        {
            string deviceUniqueIdentifier = SystemInfo.deviceUniqueIdentifier;
            return (!string.IsNullOrEmpty(deviceUniqueIdentifier) && (deviceUniqueIdentifier != "00000000-0000-0000-0000-000000000000"));
        }

        private bool CheckRequestTimeValid(int cmdId)
        {
            if (!this._requestMinIntervalDict.ContainsKey(cmdId))
            {
                return true;
            }
            bool flag = false;
            if (this._lastRequestTimeDict.ContainsKey(cmdId))
            {
                DateTime time = this._lastRequestTimeDict[cmdId];
                if (time.AddSeconds((double) this._requestMinIntervalDict[cmdId]) < TimeUtil.Now)
                {
                    flag = true;
                }
            }
            else
            {
                flag = true;
            }
            if (flag)
            {
                this._lastRequestTimeDict[cmdId] = TimeUtil.Now;
            }
            return flag;
        }

        [DebuggerHidden]
        public IEnumerator ConnectDispatchServer(Action successCallback = null)
        {
            return new <ConnectDispatchServer>c__Iterator4E { successCallback = successCallback, <$>successCallback = successCallback, <>f__this = this };
        }

        private bool ConnectGameServer(string host, ushort port, int timeout_ms = 0xbb8)
        {
            if (this._client.isConnected())
            {
                if ((this._client.Host == host) && (this._client.Port == port))
                {
                    return true;
                }
                this._client.disconnect();
            }
            bool flag = this._client.connect(host, port, timeout_ms);
            if (flag)
            {
                NetPacketV1 packet = new NetPacketV1();
                packet.setCmdId(1);
                this._client.setKeepalive(0x2710, packet);
                this._client.setDisconnectCallback(new Action(this.UnexceptedDisconnectCallback));
                this._clientPacketConsumer.gameObject.SetActive(true);
            }
            return flag;
        }

        [DebuggerHidden]
        public IEnumerator ConnectGlobalDispatchServer(Action successCallback = null)
        {
            return new <ConnectGlobalDispatchServer>c__Iterator4D { successCallback = successCallback, <$>successCallback = successCallback, <>f__this = this };
        }

        public void Destroy()
        {
            this.DisConnect();
            Singleton<CommandMap>.Destroy();
            UnityEngine.Object.Destroy(this._clientPacketConsumer.gameObject);
        }

        public void DisConnect()
        {
            this._client.disconnect();
        }

        [DebuggerHidden]
        private IEnumerator DoSendFakePacket<T>(T data)
        {
            return new <DoSendFakePacket>c__Iterator4C<T> { data = data, <$>data = data };
        }

        private uint GeneralLoginRandomNum()
        {
            return (uint) (DateTime.Now.Ticks >> 13);
        }

        public string GetGameVersion()
        {
            return ("0.9.9_" + this.channelConfig.ChannelName);
        }

        private static string GetPersistentUUID()
        {
            return SystemInfo.deviceUniqueIdentifier;
        }

        private GetPlayerTokenReq GetTestPlayerTokenReq()
        {
            string persistentUUID = GetPersistentUUID();
            GetPlayerTokenReq req = new GetPlayerTokenReq();
            req.set_account_type(0);
            req.set_account_uid(string.Empty);
            req.set_account_token(string.Empty);
            req.set_account_ext(string.Empty);
            req.set_token(persistentUUID);
            return req;
        }

        private void InitClientPacketConsumer()
        {
            if (this._clientPacketConsumer == null)
            {
                this._clientPacketConsumer = new GameObject { name = "NetPacketConsumer" }.AddComponent<MonoClientPacketConsumer>();
            }
            this._clientPacketConsumer.Init(this._client);
            this._clientPacketConsumer.gameObject.SetActive(false);
        }

        public void LoginGameServer()
        {
            if (GlobalVars.DISABLE_NETWORK_DEBUG)
            {
                this.RequestPlayerToken();
            }
            else if (this.ConnectGameServer(this.DispatchSeverData.host, this.DispatchSeverData.port, 0xbb8))
            {
                this.RequestPlayerToken();
            }
        }

        public void NotifySendFriendChatMsg(int targetUid, string message)
        {
            SendFriendChatMsgNotify data = new SendFriendChatMsgNotify();
            data.set_msg(message);
            data.set_target_uid((uint) targetUid);
            this.SendPacket<SendFriendChatMsgNotify>(data);
        }

        public void NotifySendGuildChatMsg(string message)
        {
        }

        public void NotifySendWorldChatMsg(string message)
        {
            SendWorldChatMsgNotify data = new SendWorldChatMsgNotify();
            data.set_msg(message);
            this.SendPacket<SendWorldChatMsgNotify>(data);
        }

        public void NotifyUpdateAvatarTeam(StageType levelType)
        {
            UpdateAvatarTeamNotify data = new UpdateAvatarTeamNotify();
            data.set_team(new AvatarTeam());
            data.get_team().set_stage_type(levelType);
            foreach (int num in Singleton<PlayerModule>.Instance.playerData.GetMemberList(levelType))
            {
                data.get_team().get_avatar_id_list().Add((uint) num);
            }
            this.SendPacket<UpdateAvatarTeamNotify>(data);
        }

        private bool OnConnectDispatchServer(string retJsonString, Action successCallback = null)
        {
            GeneralDialogContext context;
            string errorMsg = string.Empty;
            int retCode = 0;
            JSONNode retJson = null;
            if (!this.TryGetRetCodeFromJsonString(retJsonString, out retJson, out errorMsg, out retCode))
            {
                if (!this.alreadyLogin)
                {
                    context = new GeneralDialogContext {
                        type = GeneralDialogContext.ButtonType.SingleButton,
                        title = LocalizationGeneralLogic.GetText("Menu_NetError", new object[0]),
                        desc = LocalizationGeneralLogic.GetText("Menu_Desc_ConnectDispatchErr", new object[0]),
                        notDestroyAfterTouchBG = true,
                        hideCloseBtn = true,
                        buttonCallBack = delegate (bool confirmed) {
                            this.TryReconnectDispatch();
                        }
                    };
                    Singleton<MainUIManager>.Instance.ShowDialog(context, UIType.Any);
                }
                if (!string.IsNullOrEmpty(retJsonString))
                {
                    SuperDebug.VeryImportantError("Connect Dispatch Error msg=" + errorMsg + " retJsonString=" + retJsonString);
                }
                return false;
            }
            retCode = retJson["retcode"].AsInt;
            switch (retCode)
            {
                case 4:
                {
                    <OnConnectDispatchServer>c__AnonStoreyDB ydb = new <OnConnectDispatchServer>c__AnonStoreyDB {
                        forceUupdateUrl = OpeUtil.ConvertEventUrl((string) retJson["force_update_url"])
                    };
                    context = new GeneralDialogContext {
                        type = GeneralDialogContext.ButtonType.SingleButton,
                        title = LocalizationGeneralLogic.GetText("Menu_Title_NewVersion", new object[0]),
                        desc = (string) retJson["msg"],
                        notDestroyAfterTouchBG = true,
                        hideCloseBtn = true,
                        buttonCallBack = new Action<bool>(ydb.<>m__FC)
                    };
                    Singleton<MainUIManager>.Instance.ShowDialog(context, UIType.Any);
                    break;
                }
                case 2:
                {
                    DateTime dateTimeFromTimeStamp = Miscs.GetDateTimeFromTimeStamp((uint) retJson["stop_begin_time"].AsInt);
                    DateTime time2 = Miscs.GetDateTimeFromTimeStamp((uint) retJson["stop_end_time"].AsInt);
                    object[] replaceParams = new object[] { dateTimeFromTimeStamp.ToString("MM-dd HH:mm"), time2.ToString("MM-dd HH:mm") };
                    string str2 = ((string) retJson["msg"]) + Environment.NewLine + LocalizationGeneralLogic.GetText("Menu_ServerStopTime", replaceParams);
                    context = new GeneralDialogContext {
                        type = GeneralDialogContext.ButtonType.SingleButton,
                        title = LocalizationGeneralLogic.GetText("Menu_Title_ServerStop", new object[0]),
                        desc = str2,
                        notDestroyAfterTouchBG = true,
                        hideCloseBtn = true,
                        buttonCallBack = delegate (bool confirmed) {
                            this.TryReconnectDispatch();
                        }
                    };
                    Singleton<MainUIManager>.Instance.ShowDialog(context, UIType.Any);
                    break;
                }
                case 0:
                    this.DispatchSeverData = new DispatchServerDataItem(retJson);
                    Singleton<AssetBundleManager>.Instance.remoteAssetBundleUrl = this.DispatchSeverData.assetBundleUrl;
                    if (this.DispatchSeverData.dataUseAssetBundleUseSever)
                    {
                        GlobalVars.DataUseAssetBundle = this.DispatchSeverData.dataUseAssetBundle;
                    }
                    if (this.DispatchSeverData.resUseAssetBundleUseSever)
                    {
                        GlobalVars.ResourceUseAssetBundle = this.DispatchSeverData.resUseAssetBundle;
                    }
                    if (successCallback != null)
                    {
                        successCallback();
                    }
                    return true;

                default:
                    context = new GeneralDialogContext {
                        type = GeneralDialogContext.ButtonType.SingleButton,
                        title = LocalizationGeneralLogic.GetText("Menu_Tittle_DispatchUnknownErr", new object[0]),
                        desc = retJson["msg"] + " retcode=" + retCode,
                        notDestroyAfterTouchBG = true,
                        hideCloseBtn = true,
                        buttonCallBack = delegate (bool confirmed) {
                            this.TryReconnectDispatch();
                        }
                    };
                    Singleton<MainUIManager>.Instance.ShowDialog(context, UIType.Any);
                    break;
            }
            return false;
        }

        private bool OnConnectGlobalDispatch(string retJsonString, Action successCallback = null)
        {
            bool flag = false;
            bool flag2 = false;
            string errorMsg = string.Empty;
            int retCode = 0;
            JSONNode retJson = null;
            string text = LocalizationGeneralLogic.GetText("Menu_Desc_ConnectGlobalDispatchErr", new object[0]);
            flag2 = this.TryGetRetCodeFromJsonString(retJsonString, out retJson, out errorMsg, out retCode);
            flag = flag2;
            if (flag2)
            {
                if (retCode == 0)
                {
                    this.GlobalDispatchData = new GlobalDispatchDataItem(retJson);
                    if (this.GlobalDispatchData.regionList.Count <= 0)
                    {
                        flag = false;
                        errorMsg = "Error: GlobalDispatchData.regionList.Count <= 0";
                    }
                    else
                    {
                        flag = true;
                    }
                }
                else
                {
                    flag = false;
                    errorMsg = retJson["msg"] + " retcode=" + retCode;
                    text = errorMsg;
                }
            }
            if (flag)
            {
                if (successCallback != null)
                {
                    successCallback();
                }
                return flag;
            }
            GeneralDialogContext dialogContext = new GeneralDialogContext {
                type = GeneralDialogContext.ButtonType.SingleButton,
                title = LocalizationGeneralLogic.GetText("Menu_Tittle_GlobalDispatchUnknownErr", new object[0]),
                desc = text,
                notDestroyAfterTouchBG = true,
                hideCloseBtn = true,
                buttonCallBack = delegate (bool confirmed) {
                    this.TryReconnectGlobalDispatch();
                }
            };
            Singleton<MainUIManager>.Instance.ShowDialog(dialogContext, UIType.Any);
            if (!string.IsNullOrEmpty(retJsonString))
            {
                SuperDebug.VeryImportantError("Connect Global Dispatch Error msg=" + errorMsg + " retJsonString=" + retJsonString);
            }
            return flag;
        }

        public void ProcessWaitStopAnotherLogin()
        {
            float duration = 2f;
            if (this.alreadyLogin)
            {
                Singleton<ApplicationManager>.Instance.Invoke(duration, new Action(this.RequestPlayerLogin));
            }
            else
            {
                LoadingWheelWidgetContext widget = new LoadingWheelWidgetContext();
                widget.SetMaxWaitTime(duration);
                widget.timeOutCallBack = new Action(this.RequestPlayerLogin);
                Singleton<MainUIManager>.Instance.ShowWidget(widget, UIType.Any);
            }
        }

        public void QuickLogin()
        {
            if (GlobalVars.DISABLE_NETWORK_DEBUG)
            {
                this.RequestPlayerToken();
            }
            else
            {
                Singleton<ApplicationManager>.Instance.StartCoroutine(this.ConnectDispatchServer(delegate {
                    if (this.ConnectGameServer(this.DispatchSeverData.host, this.DispatchSeverData.port, 0xbb8))
                    {
                        this.RequestPlayerToken();
                    }
                }));
            }
        }

        public void RequestAddAvatarExpByMaterial(int avatarID, int materialID, int num)
        {
            AddAvatarExpByMaterialReq data = new AddAvatarExpByMaterialReq();
            data.set_avatar_id((uint) avatarID);
            data.set_material_id((uint) materialID);
            data.set_material_num((uint) num);
            this.SendPacket<AddAvatarExpByMaterialReq>(data);
        }

        public void RequestAddCabinTech(CabinType cabinType, int x, int y)
        {
            AddCabinTechReq data = new AddCabinTechReq();
            data.set_cabin_type(cabinType);
            data.set_pos_x(x);
            data.set_pos_y(y);
            this.SendPacket<AddCabinTechReq>(data);
        }

        public void RequestAddFriend(int targetUid)
        {
            AddFriendReq data = new AddFriendReq();
            data.set_action(1);
            data.set_target_uid((uint) targetUid);
            this.SendPacket<AddFriendReq>(data);
        }

        public void RequestAgreeFriend(int targetUid)
        {
            AddFriendReq data = new AddFriendReq();
            data.set_action(2);
            data.set_target_uid((uint) targetUid);
            this.SendPacket<AddFriendReq>(data);
        }

        public void RequestAntiCheatSDKReport()
        {
            AntiCheatSDKReportReq data = new AntiCheatSDKReportReq();
            this.SendPacket<AntiCheatSDKReportReq>(data);
        }

        public void RequestAskAddFriendList()
        {
            if (GlobalVars.DISABLE_NETWORK_DEBUG)
            {
                this.SendFakePacket<GetAskAddFriendListRsp>(FakePacketHelper.GetFakeAskAddFriendListRsp());
            }
            else
            {
                GetAskAddFriendListReq data = new GetAskAddFriendListReq();
                this.SendPacket<GetAskAddFriendListReq>(data);
            }
        }

        public void RequestAvatarRevive(bool is_retry = false)
        {
            if (GlobalVars.DISABLE_NETWORK_DEBUG)
            {
                this.SendFakePacket<AvatarReviveRsp>(FakePacketHelper.GetAvatarReviveRsp());
            }
            else
            {
                AvatarReviveReq data = new AvatarReviveReq();
                data.set_is_retry(is_retry);
                this.SendPacket<AvatarReviveReq>(data);
            }
        }

        public void RequestAvatarStarUp(int m_avatarId)
        {
            if (GlobalVars.DISABLE_NETWORK_DEBUG)
            {
                this.SendFakePacket<GetAvatarDataRsp>();
                this.SendFakePacket<AvatarStarUpRsp>();
            }
            else
            {
                AvatarStarUpReq data = new AvatarStarUpReq();
                data.set_avatar_id((uint) m_avatarId);
                this.SendPacket<AvatarStarUpReq>(data);
            }
        }

        public void RequestAvatarSubSkillLevelUp(int avatarID, int skillID, int subSkillID)
        {
            AvatarSubSkillLevelUpReq data = new AvatarSubSkillLevelUpReq();
            data.set_avatar_id((uint) avatarID);
            data.set_skill_id((uint) skillID);
            data.set_sub_skill_id((uint) subSkillID);
            this.SendPacket<AvatarSubSkillLevelUpReq>(data);
        }

        public void RequestBindAccount()
        {
            AccountType accountType = Singleton<AccountManager>.Instance.manager.AccountType;
            string accountUid = Singleton<AccountManager>.Instance.manager.AccountUid;
            string accountToken = Singleton<AccountManager>.Instance.manager.AccountToken;
            string accountExt = Singleton<AccountManager>.Instance.manager.AccountExt;
            if (!string.IsNullOrEmpty(accountUid) && !string.IsNullOrEmpty(accountToken))
            {
                BindAccountReq req2 = new BindAccountReq();
                req2.set_account_type(accountType);
                req2.set_account_uid(accountUid);
                req2.set_account_token(accountToken);
                req2.set_account_ext(accountExt);
                BindAccountReq data = req2;
                this.SendPacket<BindAccountReq>(data);
            }
        }

        public void RequestBuyGachaTicket(int ticketID, int num)
        {
            BuyGachaTicketReq data = new BuyGachaTicketReq();
            data.set_material_id((uint) ticketID);
            data.set_num((uint) num);
            this.SendPacket<BuyGachaTicketReq>(data);
        }

        public void RequestBuyGoods(int shopID, int goodsID)
        {
            BuyGoodsReq data = new BuyGoodsReq();
            data.set_shop_id((uint) shopID);
            data.set_goods_id((uint) goodsID);
            this.SendPacket<BuyGoodsReq>(data);
        }

        public void RequestCabinLevelUp(CabinType cabinType)
        {
            LevelUpCabinReq data = new LevelUpCabinReq();
            data.set_cabin_type(cabinType);
            this.SendPacket<LevelUpCabinReq>(data);
        }

        public void RequestCancelDispatchIslandVenture(int ventureId)
        {
            CancelDispatchIslandVentureReq data = new CancelDispatchIslandVentureReq();
            data.set_venture_id((uint) ventureId);
            this.SendPacket<CancelDispatchIslandVentureReq>(data);
        }

        public void RequestChangeEquipmentProtectdStatus(StorageDataItemBase storageDataItem)
        {
            if (!(storageDataItem is MaterialDataItem))
            {
                List<WeaponDataItem> weaponList = new List<WeaponDataItem>();
                List<StigmataDataItem> stigmataList = new List<StigmataDataItem>();
                if (storageDataItem is WeaponDataItem)
                {
                    weaponList.Add(storageDataItem as WeaponDataItem);
                }
                else
                {
                    stigmataList.Add(storageDataItem as StigmataDataItem);
                }
                this.RequestUpdateEquipmentProtectdStatus(weaponList, stigmataList, !storageDataItem.isProtected);
            }
        }

        public void RequestChapterDropList(ChapterDataItem chapter)
        {
            GetStageDropDisplayReq data = new GetStageDropDisplayReq();
            foreach (LevelDataItem item in chapter.GetAllLevelList())
            {
                data.get_stage_id_list().Add((uint) item.levelId);
            }
            this.SendPacket<GetStageDropDisplayReq>(data);
        }

        public void RequestCheckAppleReceipt(string receipt)
        {
            VerifyItunesOrderNotify data = new VerifyItunesOrderNotify();
            data.set_receipt_data(receipt);
            this.SendPacket<VerifyItunesOrderNotify>(data);
        }

        public void RequestCommentReport(CommentType comment, uint times)
        {
            CommentReportReq data = new CommentReportReq();
            data.set_comment(comment);
            data.set_times(times);
            this.SendPacket<CommentReportReq>(data);
        }

        public void RequestConfigData()
        {
            GetConfigReq data = new GetConfigReq();
            this.SendPacket<GetConfigReq>(data);
        }

        public void RequestCreateWeiXinOrder(string productID, string productName, float productPrice)
        {
            CreateWeiXinOrderReq data = new CreateWeiXinOrderReq();
            data.set_body(productName);
            data.set_attach(Singleton<PlayerModule>.Instance.playerData.userId.ToString() + "|" + productID);
            data.set_total_fee(((int) (productPrice * 100f)).ToString());
            data.set_notify_url(Singleton<NetworkManager>.Instance.DispatchSeverData.oaServerUrl + "/callback/weixin");
            this.SendPacket<CreateWeiXinOrderReq>(data);
        }

        public void RequestDelFriend(int targetUid)
        {
            DelFriendReq data = new DelFriendReq();
            data.set_target_uid((uint) targetUid);
            this.SendPacket<DelFriendReq>(data);
        }

        public void RequestDispatchIslandVenture(int ventureId, List<int> avatarIdList)
        {
            DispatchIslandVentureReq data = new DispatchIslandVentureReq();
            data.set_venture_id((uint) ventureId);
            foreach (int num in avatarIdList)
            {
                data.get_avatar_id_list().Add((uint) num);
            }
            this.SendPacket<DispatchIslandVentureReq>(data);
        }

        public void RequestDressEquipmentReq(int avatarID, StorageDataItemBase dataItem, EquipmentSlot slot)
        {
            DressEquipmentReq data = new DressEquipmentReq();
            data.set_avatar_id((uint) avatarID);
            data.set_slot(slot);
            data.set_unique_id((dataItem != null) ? ((uint) dataItem.uid) : 0);
            this.SendPacket<DressEquipmentReq>(data);
        }

        public void RequestEndlessAvatarHp()
        {
            GetEndlessAvatarHpReq data = new GetEndlessAvatarHpReq();
            this.SendPacket<GetEndlessAvatarHpReq>(data);
        }

        public void RequestEndlessData()
        {
            GetEndlessDataReq data = new GetEndlessDataReq();
            this.SendPacket<GetEndlessDataReq>(data);
        }

        public void RequestEndlessFloorEndReq(StageEndStatus status, List<DropItem> drops, List<EndlessAvatarHp> avatarHPList, bool hashChanged)
        {
            EndlessStageEndReq data = new EndlessStageEndReq();
            data.set_end_status(status);
            data.get_drop_item_list().AddRange(drops);
            data.get_avatar_hp_list().AddRange(avatarHPList);
            data.set_is_hash_changed(hashChanged);
            this.SendPacket<EndlessStageEndReq>(data);
        }

        public void RequestEndlessLevelBeginReq()
        {
            EndlessStageBeginReq data = new EndlessStageBeginReq();
            foreach (int num in Singleton<PlayerModule>.Instance.playerData.GetMemberList(4))
            {
                data.get_avatar_id_list().Add((uint) num);
            }
            this.SendPacket<EndlessStageBeginReq>(data);
        }

        public void RequestEnterWorldChatroom(int chatRoomId = 0)
        {
            EnterWorldChatroomReq data = new EnterWorldChatroomReq();
            data.set_chatroom_id((uint) chatRoomId);
            this.SendPacket<EnterWorldChatroomReq>(data);
        }

        public void RequestEquipmentEvo(List<StorageDataItemBase> resourceList, StorageDataItemBase mainItem)
        {
            EquipmentEvoReq data = new EquipmentEvoReq();
            EquipmentItem equipmentItem = new EquipmentItem();
            this.SetEquipmentItemByStorageDataItem(equipmentItem, mainItem);
            data.set_main_item(equipmentItem);
            EquipmentItemList list = new EquipmentItemList();
            foreach (StorageDataItemBase base2 in resourceList)
            {
                EquipmentItem item2 = new EquipmentItem();
                this.SetEquipmentItemByStorageDataItem(item2, base2);
                list.get_item_list().Add(item2);
            }
            data.set_consume_item_list(list);
            this.SendPacket<EquipmentEvoReq>(data);
        }

        public void RequestEquipmentPowerUp(StorageDataItemBase mainItem, List<StorageDataItemBase> consumeItemList)
        {
            if (GlobalVars.DISABLE_NETWORK_DEBUG)
            {
                this.SendFakePacket<EquipmentPowerUpRsp>();
            }
            else
            {
                EquipmentPowerUpReq data = new EquipmentPowerUpReq();
                EquipmentItem equipmentItem = new EquipmentItem();
                this.SetEquipmentItemByStorageDataItem(equipmentItem, mainItem);
                data.set_main_item(equipmentItem);
                EquipmentItemList list = new EquipmentItemList();
                foreach (StorageDataItemBase base2 in consumeItemList)
                {
                    EquipmentItem item2 = new EquipmentItem();
                    this.SetEquipmentItemByStorageDataItem(item2, base2);
                    list.get_item_list().Add(item2);
                }
                data.set_consume_item_list(list);
                this.SendPacket<EquipmentPowerUpReq>(data);
            }
        }

        public void RequestEquipmentSell(List<StorageDataItemBase> storageItemList)
        {
            EquipmentItemList list = new EquipmentItemList();
            List<AvatarFragment> collection = new List<AvatarFragment>();
            foreach (StorageDataItemBase base2 in storageItemList)
            {
                if (base2 is AvatarFragmentDataItem)
                {
                    AvatarFragment fragment = new AvatarFragment();
                    fragment.set_fragment_id((uint) base2.ID);
                    fragment.set_num((uint) base2.number);
                    collection.Add(fragment);
                }
                else
                {
                    EquipmentItem equipmentItem = new EquipmentItem();
                    this.SetEquipmentItemByStorageDataItem(equipmentItem, base2);
                    list.get_item_list().Add(equipmentItem);
                }
            }
            if (list.get_item_list().Count > 0)
            {
                EquipmentSellReq data = new EquipmentSellReq();
                data.set_sell_item_list(list);
                this.SendPacket<EquipmentSellReq>(data);
            }
            if (collection.Count > 0)
            {
                SellAvatarFragmentReq req2 = new SellAvatarFragmentReq();
                req2.get_fragment_list().AddRange(collection);
                this.SendPacket<SellAvatarFragmentReq>(req2);
            }
        }

        public void RequestExchangeAvatarWeapon(int avatarID1, int avatarID2)
        {
            ExchangeAvatarWeaponReq data = new ExchangeAvatarWeaponReq();
            data.set_avatar_id_1((uint) avatarID1);
            data.set_avatar_id_2((uint) avatarID2);
            this.SendPacket<ExchangeAvatarWeaponReq>(data);
        }

        public void RequestExchangeRedeemCode(string redeemCode)
        {
            ExchangeRedeemCodeReq data = new ExchangeRedeemCodeReq();
            data.set_redeem_code(redeemCode);
            this.SendPacket<ExchangeRedeemCodeReq>(data);
        }

        public void RequestExtendCabin(CabinType cabinType)
        {
            ExtendCabinReq data = new ExtendCabinReq();
            data.set_cabin_type(cabinType);
            this.SendPacket<ExtendCabinReq>(data);
        }

        public void RequestFeedStigmataAffix(int mainUID, int consumeUID)
        {
            FeedStigmataAffixReq data = new FeedStigmataAffixReq();
            data.set_main_unique_id((uint) mainUID);
            data.set_consume_unique_id((uint) consumeUID);
            this.SendPacket<FeedStigmataAffixReq>(data);
        }

        public void RequestFinishCabinLevelUp(CabinType cabinType)
        {
            FinishCabinLevelUpReq data = new FinishCabinLevelUpReq();
            data.set_cabin_type(cabinType);
            this.SendPacket<FinishCabinLevelUpReq>(data);
        }

        public void RequestFinishGuideReport(List<int> guideIDList, bool isForceFinish)
        {
            FinishGuideReportReq data = new FinishGuideReportReq();
            List<uint> collection = new List<uint>();
            foreach (int num in guideIDList)
            {
                collection.Add((uint) num);
            }
            data.get_guide_id_list().Clear();
            data.get_guide_id_list().AddRange(collection);
            data.set_is_force_finish(isForceFinish);
            this.SendPacket<FinishGuideReportReq>(data);
        }

        public void RequestFinishGuideReport(uint guideID, bool isForceFinish)
        {
            FinishGuideReportReq data = new FinishGuideReportReq();
            List<uint> collection = new List<uint> {
                guideID
            };
            data.get_guide_id_list().Clear();
            data.get_guide_id_list().AddRange(collection);
            data.set_is_force_finish(isForceFinish);
            this.SendPacket<FinishGuideReportReq>(data);
        }

        public void RequestFriendDetailInfo(int uid)
        {
            if (GlobalVars.DISABLE_NETWORK_DEBUG)
            {
                this.SendFakePacket<GetPlayerDetailDataRsp>(FakePacketHelper.GetFakePlayerDetailDataRsp((uint) uid));
            }
            else
            {
                GetPlayerDetailDataReq data = new GetPlayerDetailDataReq();
                data.set_target_uid((uint) uid);
                this.SendPacket<GetPlayerDetailDataReq>(data);
            }
        }

        public void RequestFriendList()
        {
            if (GlobalVars.DISABLE_NETWORK_DEBUG)
            {
                this.SendFakePacket<GetFriendListRsp>(FakePacketHelper.GetFakeFriendListRsp());
            }
            else
            {
                GetFriendListReq data = new GetFriendListReq();
                this.SendPacket<GetFriendListReq>(data);
            }
        }

        public void RequestGacha(GachaType type, int num)
        {
            if (GlobalVars.DISABLE_NETWORK_DEBUG)
            {
                this.SendFakePacket<GachaRsp>();
            }
            else
            {
                GachaReq data = new GachaReq();
                data.set_type(type);
                data.set_num((uint) num);
                this.SendPacket<GachaReq>(data);
            }
        }

        public void RequestGachaDisplayInfo()
        {
            if (GlobalVars.DISABLE_NETWORK_DEBUG)
            {
                this.SendFakePacket<GetGachaDisplayRsp>(FakePacketHelper.GetFakeGetGachaDisplayRsp());
            }
            else
            {
                GetGachaDisplayReq data = new GetGachaDisplayReq();
                this.SendPacket<GetGachaDisplayReq>(data);
            }
        }

        public void RequestGalAddGoodFeel(int avatarId, int amount, uint type)
        {
            AddGoodfeelReq data = new AddGoodfeelReq();
            data.set_avatar_id((uint) avatarId);
            data.set_add_goodfeel(amount);
            data.set_add_goodfeel_type(type);
            this.SendPacket<AddGoodfeelReq>(data);
        }

        public void RequestGetAcceptFriendInvite(string inviteCode)
        {
            AcceptFriendInviteReq data = new AcceptFriendInviteReq();
            data.set_invite_code(inviteCode);
            this.SendPacket<AcceptFriendInviteReq>(data);
        }

        public void RequestGetAllAvatarData()
        {
            if (GlobalVars.DISABLE_NETWORK_DEBUG)
            {
                this.SendFakePacket<GetAvatarDataRsp>(FakePacketHelper.GetFakeAvatarDataRsp());
            }
            else
            {
                GetAvatarDataReq data = new GetAvatarDataReq();
                data.get_avatar_id_list().Add(0);
                this.SendPacket<GetAvatarDataReq>(data);
            }
        }

        public void RequestGetAllEquipmentData()
        {
            if (GlobalVars.DISABLE_NETWORK_DEBUG)
            {
                this.SendFakePacket<GetEquipmentDataRsp>(FakePacketHelper.GetFakeEquipmentDataRsp());
            }
            else
            {
                GetEquipmentDataReq data = new GetEquipmentDataReq();
                data.get_weapon_unique_id_list().Add(0);
                data.get_stigmata_unique_id_list().Add(0);
                data.get_material_id_list().Add(0);
                this.SendPacket<GetEquipmentDataReq>(data);
            }
        }

        public void RequestGetAllLevelData()
        {
            if (GlobalVars.DISABLE_NETWORK_DEBUG)
            {
                this.SendFakePacket<GetStageDataRsp>(FakePacketHelper.GetFakeStageDataRsp());
            }
            else
            {
                GetStageDataReq data = new GetStageDataReq();
                data.get_stage_id_list().Add(0);
                this.SendPacket<GetStageDataReq>(data);
            }
        }

        public void RequestGetAllMailAttachment()
        {
            GetMailAttachmentReq data = new GetMailAttachmentReq();
            this.SendPacket<GetMailAttachmentReq>(data);
        }

        public void RequestGetAllMainData()
        {
            if (GlobalVars.DISABLE_NETWORK_DEBUG)
            {
                this.SendFakePacket<GetMainDataRsp>();
            }
            else
            {
                GetMainDataReq data = new GetMainDataReq();
                data.get_type_list().Add(0);
                this.SendPacket<GetMainDataReq>(data);
            }
        }

        public void RequestGetAllWeekDayActivityData()
        {
            GetWeekDayActivityDataReq data = new GetWeekDayActivityDataReq();
            this.SendPacket<GetWeekDayActivityDataReq>(data);
        }

        public void RequestGetAssistantFrozenList()
        {
            GetAssistantFrozenListReq data = new GetAssistantFrozenListReq();
            this.SendPacket<GetAssistantFrozenListReq>(data);
        }

        public void RequestGetAvatarTeamData()
        {
            if (GlobalVars.DISABLE_NETWORK_DEBUG)
            {
                this.SendFakePacket<GetAvatarTeamDataRsp>(FakePacketHelper.GetFakeGetAvatarTeamDataRsp());
            }
            else
            {
                this.SendPacket<GetAvatarTeamDataReq>(new GetAvatarTeamDataReq());
            }
        }

        public void RequestGetBulletin()
        {
            this.SendPacket<GetBulletinReq>(new GetBulletinReq());
            Singleton<BulletinModule>.Instance.LastCheckBulletinTime = TimeUtil.Now;
        }

        public void RequestGetCollectCabin()
        {
            GetCollectCabinReq data = new GetCollectCabinReq();
            this.SendPacket<GetCollectCabinReq>(data);
        }

        public void RequestGetEndlessTopGroup()
        {
            GetEndlessTopGroupReq data = new GetEndlessTopGroupReq();
            this.SendPacket<GetEndlessTopGroupReq>(data);
        }

        public void RequestGetFinishGuideData()
        {
            if (GlobalVars.DISABLE_NETWORK_DEBUG)
            {
                this.SendFakePacket<GetFinishGuideDataRsp>(FakePacketHelper.GetFakeFinishGuideDataRsp());
            }
            else
            {
                GetFinishGuideDataReq data = new GetFinishGuideDataReq();
                this.SendPacket<GetFinishGuideDataReq>(data);
            }
        }

        public void RequestGetInviteeFriend()
        {
            GetInviteeFriendReq data = new GetInviteeFriendReq();
            this.SendPacket<GetInviteeFriendReq>(data);
        }

        public void RequestGetInviteFriend()
        {
            GetInviteFriendReq data = new GetInviteFriendReq();
            this.SendPacket<GetInviteFriendReq>(data);
        }

        public void RequestGetIsland()
        {
            GetIslandReq data = new GetIslandReq();
            this.SendPacket<GetIslandReq>(data);
        }

        public void RequestGetIslandVentureReward(int ventureId)
        {
            GetIslandVentureRewardReq data = new GetIslandVentureRewardReq();
            data.set_venture_id((uint) ventureId);
            this.SendPacket<GetIslandVentureRewardReq>(data);
        }

        public void RequestGetMailData()
        {
            if (GlobalVars.DISABLE_NETWORK_DEBUG)
            {
                this.SendFakePacket<GetMailDataRsp>(FakePacketHelper.GetFakeMailDataRsp());
            }
            else
            {
                GetMailDataReq data = new GetMailDataReq();
                this.SendPacket<GetMailDataReq>(data);
            }
        }

        public void RequestGetMissionData()
        {
            GetMissionDataReq data = new GetMissionDataReq();
            this.SendPacket<GetMissionDataReq>(data);
        }

        public void RequestGetMissionReward(uint missionId)
        {
            GetMissionRewardReq data = new GetMissionRewardReq();
            data.set_mission_id(missionId);
            this.SendPacket<GetMissionRewardReq>(data);
        }

        public void RequestGetOneMailAttachment(MailDataItem mailData)
        {
            MailKey item = new MailKey();
            item.set_id((uint) mailData.ID);
            item.set_type(mailData.type);
            GetMailAttachmentReq data = new GetMailAttachmentReq();
            data.get_mail_key_list().Add(item);
            this.SendPacket<GetMailAttachmentReq>(data);
        }

        public void RequestGetRedeemCodeInfo(string redeemCode)
        {
            GetRedeemCodeInfoReq data = new GetRedeemCodeInfoReq();
            data.set_redeem_code(redeemCode);
            this.SendPacket<GetRedeemCodeInfoReq>(data);
        }

        public void RequestGetRefreshIslandVentureInfo()
        {
            GetRefreshIslandVentureInfoReq data = new GetRefreshIslandVentureInfoReq();
            this.SendPacket<GetRefreshIslandVentureInfoReq>(data);
        }

        public void RequestGetScoinExchangeInfo()
        {
            if (GlobalVars.DISABLE_NETWORK_DEBUG)
            {
                this.SendFakePacket<GetScoinExchangeInfoRsp>();
            }
            else
            {
                this.SendPacket<GetScoinExchangeInfoReq>(new GetScoinExchangeInfoReq());
            }
        }

        public void RequestGetShopList()
        {
            GetShopListReq data = new GetShopListReq();
            this.SendPacket<GetShopListReq>(data);
        }

        public void RequestGetSignInReward()
        {
            GetSignInRewardReq data = new GetSignInRewardReq();
            this.SendPacket<GetSignInRewardReq>(data);
        }

        public void RequestGetSignInRewardStatus()
        {
            GetSignInRewardStatusReq data = new GetSignInRewardStatusReq();
            this.SendPacket<GetSignInRewardStatusReq>(data);
        }

        public void RequestGetSkillPointExchangeInfo()
        {
            if (GlobalVars.DISABLE_NETWORK_DEBUG)
            {
                this.SendFakePacket<GetSkillPointExchangeInfoRsp>();
            }
            else
            {
                this.SendPacket<GetSkillPointExchangeInfoReq>(new GetSkillPointExchangeInfoReq());
            }
        }

        public void RequestGetSkillPointRecoverLeftTime()
        {
            if (GlobalVars.DISABLE_NETWORK_DEBUG)
            {
                this.SendFakePacket<GetMainDataRsp>();
            }
            else
            {
                GetMainDataReq data = new GetMainDataReq();
                data.get_type_list().Add(10);
                this.SendPacket<GetMainDataReq>(data);
            }
        }

        public void RequestGetSpecifiedEquipmentData(List<uint> weaponUIDList, List<uint> stigmataUIDList, List<uint> materialIDList)
        {
            GetEquipmentDataReq data = new GetEquipmentDataReq();
            foreach (uint num in weaponUIDList)
            {
                data.get_weapon_unique_id_list().Add(num);
            }
            foreach (uint num2 in stigmataUIDList)
            {
                data.get_stigmata_unique_id_list().Add(num2);
            }
            foreach (uint num3 in materialIDList)
            {
                data.get_material_id_list().Add(num3);
            }
            this.SendPacket<GetEquipmentDataReq>(data);
        }

        public void RequestGetStaminaExchangeInfo()
        {
            if (GlobalVars.DISABLE_NETWORK_DEBUG)
            {
                this.SendFakePacket<GetStaminaExchangeInfoRsp>();
            }
            else
            {
                this.SendPacket<GetStaminaExchangeInfoReq>(new GetStaminaExchangeInfoReq());
            }
        }

        public void RequestGetStaminaRecoverLeftTime()
        {
            if (GlobalVars.DISABLE_NETWORK_DEBUG)
            {
                this.SendFakePacket<GetMainDataRsp>();
            }
            else
            {
                GetMainDataReq data = new GetMainDataReq();
                data.get_type_list().Add(7);
                this.SendPacket<GetMainDataReq>(data);
            }
        }

        public void RequestGetVipReward(int VipLevel)
        {
            GetVipRewardReq data = new GetVipRewardReq();
            data.set_vip_level((uint) VipLevel);
            this.SendPacket<GetVipRewardReq>(data);
        }

        public void RequestGetVipRewardData()
        {
            GetVipRewardDataReq data = new GetVipRewardDataReq();
            this.SendPacket<GetVipRewardDataReq>(data);
        }

        public void RequestGuideReward(int rewardID)
        {
            GetGuideRewardReq data = new GetGuideRewardReq();
            data.set_guide_id((uint) rewardID);
            this.SendPacket<GetGuideRewardReq>(data);
        }

        public void RequestHasGotItemIdList()
        {
            GetHasGotItemIdListReq data = new GetHasGotItemIdListReq();
            this.SendPacket<GetHasGotItemIdListReq>(data);
        }

        public void RequestIdentifyStigmataAffix(StorageDataItemBase storageItem)
        {
            IdentifyStigmataAffixReq data = new IdentifyStigmataAffixReq();
            data.set_unique_id((uint) storageItem.uid);
            this.SendPacket<IdentifyStigmataAffixReq>(data);
        }

        public void RequestIslandCollect()
        {
            IslandCollectReq data = new IslandCollectReq();
            this.SendPacket<IslandCollectReq>(data);
        }

        public void RequestIslandDisjoinEquipment(EquipmentType type, uint id)
        {
            IslandDisjoinEquipmentReq data = new IslandDisjoinEquipmentReq();
            data.set_type(type);
            data.set_unique_id(id);
            this.SendPacket<IslandDisjoinEquipmentReq>(data);
        }

        public void RequestIslandVenture()
        {
            GetIslandVentureReq data = new GetIslandVentureReq();
            this.SendPacket<GetIslandVentureReq>(data);
        }

        public void RequestLevelBeginReq(LevelDataItem level, int helperUid = 0)
        {
            if (GlobalVars.DISABLE_NETWORK_DEBUG)
            {
                this.SendFakePacket<StageBeginRsp>();
            }
            else
            {
                StageBeginReq data = new StageBeginReq();
                data.set_stage_id((uint) level.levelId);
                foreach (int num in Singleton<PlayerModule>.Instance.playerData.GetMemberList(level.LevelType))
                {
                    data.get_avatar_id_list().Add((uint) num);
                }
                if (helperUid != 0)
                {
                    data.set_assistant_uid((uint) helperUid);
                }
                this.SendPacket<StageBeginReq>(data);
            }
        }

        public void RequestLevelDropList(List<uint> levelIDList)
        {
            GetStageDropDisplayReq data = new GetStageDropDisplayReq();
            data.get_stage_id_list().AddRange(levelIDList);
            this.SendPacket<GetStageDropDisplayReq>(data);
        }

        public void RequestLevelEndReq(int levelId, StageEndStatus status, int scoinReward, int avatarExpReward, List<int> challengeIndexList, List<DropItem> drops, List<StageCheatData> cheatDataList, bool hashChanged, string signKey)
        {
            if (GlobalVars.DISABLE_NETWORK_DEBUG)
            {
                this.SendFakePacket<StageEndRsp>(FakePacketHelper.GetFakeStageEndRsp());
            }
            else
            {
                StageEndReqBody body = new StageEndReqBody();
                body.set_stage_id((uint) levelId);
                body.set_end_status(status);
                body.set_scoin_reward((uint) scoinReward);
                body.set_avatar_exp_reward((uint) avatarExpReward);
                foreach (int num in challengeIndexList)
                {
                    body.get_challenge_index_list().Add((uint) num);
                }
                body.get_drop_item_list().AddRange(drops);
                if (cheatDataList != null)
                {
                    body.get_cheat_data_list().AddRange(cheatDataList);
                }
                body.set_is_hash_changed(hashChanged);
                MemoryStream dest = new MemoryStream();
                dest.SetLength(0L);
                dest.Position = 0L;
                Singleton<NetworkManager>.Instance.serializer.Serialize(dest, body);
                byte[] buffer = dest.ToArray();
                byte[] bytes = Encoding.Default.GetBytes(signKey);
                dest.Write(bytes, 0, bytes.Length);
                byte[] buffer3 = dest.ToArray();
                StageEndReq data = new StageEndReq();
                data.set_body(buffer);
                data.set_sign(SecurityUtil.SHA256(buffer3));
                this.SendPacket<StageEndReq>(data);
                Singleton<LevelModule>.Instance.SaveLevelEndReqInfo(data);
            }
        }

        public void RequestManualRefreshShop(int shopID)
        {
            ManualRefreshShopReq data = new ManualRefreshShopReq();
            data.set_shop_id((uint) shopID);
            this.SendPacket<ManualRefreshShopReq>(data);
        }

        public void RequestNicknameChange(string newName)
        {
            NicknameModifyReq data = new NicknameModifyReq();
            data.set_nickname(newName);
            this.SendPacket<NicknameModifyReq>(data);
        }

        public void RequestPlayerLogin()
        {
            if (GlobalVars.DISABLE_NETWORK_DEBUG)
            {
                this.SendFakePacket<PlayerLoginRsp>();
            }
            else
            {
                this.loginRandomNum = Singleton<MiHoYoGameData>.Instance.LocalData.LoginRandomNum;
                if (this.loginRandomNum == 0)
                {
                    this.loginRandomNum = this.GeneralLoginRandomNum();
                    Singleton<MiHoYoGameData>.Instance.LocalData.LoginRandomNum = this.loginRandomNum;
                    Singleton<MiHoYoGameData>.Instance.Save();
                }
                PlayerLoginReq data = new PlayerLoginReq();
                data.set_login_random_num(this.loginRandomNum);
                data.set_last_server_packet_id(this._clientPacketConsumer.lastServerPacketId);
                if (Singleton<AccountManager>.Instance.apkCommentInfo != null)
                {
                    string cps = Singleton<AccountManager>.Instance.apkCommentInfo.cps;
                    if (!string.IsNullOrEmpty(cps))
                    {
                        data.set_cps(cps);
                    }
                    string checksum = Singleton<AccountManager>.Instance.apkCommentInfo.checksum;
                    if (!string.IsNullOrEmpty(checksum))
                    {
                        data.set_check_sum(checksum);
                    }
                }
                if (string.IsNullOrEmpty(this._uuid))
                {
                    this._uuid = GetPersistentUUID();
                }
                data.set_device_uuid(this._uuid);
                data.set_android_signatures(Singleton<AccountManager>.Instance.apkSignature);
                this.SendPacket<PlayerLoginReq>(data);
            }
        }

        public void RequestPlayerToken()
        {
            if (GlobalVars.DISABLE_NETWORK_DEBUG)
            {
                this.SendFakePacket<GetPlayerTokenRsp>();
            }
            else
            {
                GetPlayerTokenReq testPlayerTokenReq;
                AccountType accountType = Singleton<AccountManager>.Instance.manager.AccountType;
                string accountUid = Singleton<AccountManager>.Instance.manager.AccountUid;
                string accountToken = Singleton<AccountManager>.Instance.manager.AccountToken;
                string accountExt = Singleton<AccountManager>.Instance.manager.AccountExt;
                if (!string.IsNullOrEmpty(accountUid))
                {
                    GetPlayerTokenReq req2 = new GetPlayerTokenReq();
                    req2.set_account_type(accountType);
                    req2.set_account_uid(accountUid);
                    req2.set_account_token(accountToken);
                    req2.set_account_ext(accountExt);
                    req2.set_token(string.Empty);
                    testPlayerTokenReq = req2;
                }
                else
                {
                    testPlayerTokenReq = this.GetTestPlayerTokenReq();
                }
                testPlayerTokenReq.set_version(this.GetGameVersion());
                testPlayerTokenReq.set_device(SystemInfo.deviceModel);
                testPlayerTokenReq.set_system_info(SystemInfo.operatingSystem);
                this.SendPacket<GetPlayerTokenReq>(testPlayerTokenReq);
            }
        }

        public void RequestProductList()
        {
            GetProductListReq data = new GetProductListReq();
            this.SendPacket<GetProductListReq>(data);
        }

        public void RequestRecommandFriendList()
        {
            if (GlobalVars.DISABLE_NETWORK_DEBUG)
            {
                this.SendFakePacket<GetRecommendFriendListRsp>(FakePacketHelper.GetFakeRecommendFriendListRsp());
            }
            else
            {
                GetRecommendFriendListReq data = new GetRecommendFriendListReq();
                this.SendPacket<GetRecommendFriendListReq>(data);
            }
        }

        public void RequestRefreshIslandVenture()
        {
            RefreshIslandVentureReq data = new RefreshIslandVentureReq();
            this.SendPacket<RefreshIslandVentureReq>(data);
        }

        public void RequestRejectFriend(int targetUid)
        {
            AddFriendReq data = new AddFriendReq();
            data.set_action(3);
            data.set_target_uid((uint) targetUid);
            this.SendPacket<AddFriendReq>(data);
        }

        public void RequestResetCabinTech(CabinType cabinType)
        {
            ResetCabinTechReq data = new ResetCabinTechReq();
            data.set_cabin_type(cabinType);
            this.SendPacket<ResetCabinTechReq>(data);
        }

        public void RequestScoinExchange()
        {
            if (GlobalVars.DISABLE_NETWORK_DEBUG)
            {
                this.SendFakePacket<ScoinExchangeRsp>();
            }
            else
            {
                this.SendPacket<ScoinExchangeReq>(new ScoinExchangeReq());
            }
        }

        public void RequestSelectNewStigmataAffix()
        {
            SelectNewStigmataAffixReq data = new SelectNewStigmataAffixReq();
            this.SendPacket<SelectNewStigmataAffixReq>(data);
        }

        public void RequestSelfDescChange(string desc)
        {
            SetSelfDescReq data = new SetSelfDescReq();
            data.set_self_desc(desc);
            this.SendPacket<SetSelfDescReq>(data);
        }

        public void RequestSetFriendDesc(string desc)
        {
            SetSelfDescReq data = new SetSelfDescReq();
            data.set_self_desc(desc);
            this.SendPacket<SetSelfDescReq>(data);
        }

        public void RequestSkillPointExchange()
        {
            if (GlobalVars.DISABLE_NETWORK_DEBUG)
            {
                this.SendFakePacket<SkillPointExchangeRsp>();
            }
            else
            {
                this.SendPacket<SkillPointExchangeReq>(new SkillPointExchangeReq());
            }
        }

        public void RequestSpeedUpIslandVenture(int ventureID, int materialID, int num)
        {
            SpeedUpIslandVentureReq data = new SpeedUpIslandVentureReq();
            data.set_venture_id((uint) ventureID);
            data.set_material_id((uint) materialID);
            data.set_num((uint) num);
            this.SendPacket<SpeedUpIslandVentureReq>(data);
        }

        public void RequestStageEnterTimes(uint _stageId)
        {
            ResetStageEnterTimesReq data = new ResetStageEnterTimesReq();
            data.set_stage_id(_stageId);
            this.SendPacket<ResetStageEnterTimesReq>(data);
        }

        public void RequestStageInnerDataReport(List<AvatarStastics> avatarInfoList, List<MonsterStastics> monsterInfoList, PlayerStastics playerData)
        {
            StageInnerDataReportReq req = new StageInnerDataReportReq();
            foreach (AvatarStastics stastics in avatarInfoList)
            {
                StageInnerAvatarData item = new StageInnerAvatarData();
                item.set_avatar_id((uint) stastics.avatarID);
                item.set_stage_id((uint) stastics.stageID);
                item.set_avatar_level((uint) stastics.avatarLevel);
                item.set_avatar_star((uint) stastics.avatarStar);
                item.set_total_output((uint) Mathf.FloorToInt((float) stastics.avatarDamage));
                item.set_no_restrict_output((uint) Mathf.FloorToInt((float) stastics.normalDamage));
                item.set_do_restrict_output((uint) Mathf.FloorToInt((float) stastics.restrictionDamage));
                item.set_be_restrict_output((uint) Mathf.FloorToInt((float) stastics.beRestrictedDamage));
                item.set_total_input((uint) Mathf.FloorToInt((float) stastics.avatarBeDamaged));
                item.set_battle_time((uint) Mathf.FloorToInt((float) stastics.battleTime));
                item.set_total_time((uint) Mathf.FloorToInt((float) stastics.onStageTime));
                item.set_enter_times((uint) stastics.swapInTimes);
                item.set_leave_times((uint) stastics.swapOutTimes);
                item.set_do_break_times((uint) stastics.avatarBreakTimes);
                item.set_be_break_times((uint) stastics.avatarBeingBreakTimes);
                item.set_do_hit_times((uint) stastics.avatarHitTimes);
                item.set_be_hit_times((uint) stastics.avatarBeingHitTimes);
                item.set_exskill_times((uint) stastics.avatarSkill02Times);
                item.set_evade_times((uint) stastics.avatarEvadeTimes);
                item.set_evade_success_times((uint) stastics.avatarEvadeSuccessTimes);
                item.set_evade_effect_times((uint) stastics.avatarEvadeEffectTimes);
                item.set_attack_sp_recover((uint) Mathf.FloorToInt((float) stastics.selfSPRecover));
                item.set_total_sp_recover((uint) Mathf.FloorToInt((float) stastics.SpRecover));
                item.set_dps((uint) Mathf.FloorToInt((float) stastics.dps));
                item.set_special_attack_times((uint) Mathf.FloorToInt((float) stastics.avatarSpecialAttackTimes));
                item.set_weapon_active_skill((uint) Mathf.FloorToInt((float) stastics.avatarActiveWeaponSkillDamage));
                req.get_avatar_list().Add(item);
            }
            foreach (MonsterStastics stastics2 in monsterInfoList)
            {
                StageInnerMonsterData data2 = new StageInnerMonsterData();
                data2.set_monster_name(stastics2.key.monsterName);
                data2.set_monster_type(stastics2.key.configType);
                data2.set_monster_level((uint) stastics2.key.level);
                data2.set_monster_num((uint) stastics2.monsterCount);
                data2.set_avg_output((uint) Mathf.FloorToInt((float) stastics2.damage));
                data2.set_avg_live_time((uint) Mathf.FloorToInt((float) stastics2.aliveTime));
                data2.set_dps((uint) Mathf.FloorToInt((float) stastics2.dps));
                data2.set_hit_avatar_times((uint) stastics2.hitAvatarTimes);
                data2.set_break_avatar_times((uint) stastics2.breakAvatarTimes);
                req.get_monster_list().Add(data2);
            }
            req.set_rotate_camera_times((uint) playerData.screenRotateTimes);
            req.set_stage_time((uint) UIUtil.FloorToIntCustom((float) playerData.stageTime));
            req.set_max_combo_num((uint) Singleton<LevelScoreManager>.Instance.maxComboNum);
            this.SendPacket<StageInnerDataReportReq>(req);
        }

        public void RequestStaminaExchange()
        {
            if (GlobalVars.DISABLE_NETWORK_DEBUG)
            {
                this.SendFakePacket<StaminaExchangeRsp>();
            }
            else
            {
                this.SendPacket<StaminaExchangeReq>(new StaminaExchangeReq());
            }
        }

        public void RequestUpdateEquipmentProtectdStatus(List<WeaponDataItem> weaponList, List<StigmataDataItem> stigmataList, bool isProtected)
        {
            UpdateEquipmentProtectedStatusReq data = new UpdateEquipmentProtectedStatusReq();
            if (weaponList != null)
            {
                foreach (WeaponDataItem item in weaponList)
                {
                    data.get_weapon_unique_id_list().Add((uint) item.uid);
                }
            }
            if (stigmataList != null)
            {
                foreach (StigmataDataItem item2 in stigmataList)
                {
                    data.get_stigmata_unique_id_list().Add((uint) item2.uid);
                }
            }
            data.set_is_protected(isProtected);
            this.SendPacket<UpdateEquipmentProtectedStatusReq>(data);
        }

        public void RequestUpdateMissionProgress(MissionFinishWay finishWay, uint finishParaInt, string finishParaStr, uint progressAdd)
        {
            UpdateMissionProgressReq data = new UpdateMissionProgressReq();
            data.set_finish_way(finishWay);
            data.set_finish_para(finishParaInt);
            data.set_finish_para_str(finishParaStr);
            data.set_progress_add(progressAdd);
            this.SendPacket<UpdateMissionProgressReq>(data);
        }

        public void RequestUseEndlessItem(uint itemId, int targetUid = -1)
        {
            UseEndlessItemReq data = new UseEndlessItemReq();
            data.set_item_id(itemId);
            if (targetUid > 0)
            {
                data.set_target_uid((uint) targetUid);
            }
            this.SendPacket<UseEndlessItemReq>(data);
        }

        public void SendFakePacket<T>()
        {
            T fakeRsp = FakePacketHelper.GetFakeRsp<T>();
            this.SendFakePacket<T>(fakeRsp);
        }

        public void SendFakePacket<T>(T data)
        {
            Singleton<ApplicationManager>.Instance.StartCoroutine(this.DoSendFakePacket<T>(data));
        }

        public void SendPacket<T>(T data)
        {
            if (!GlobalVars.DISABLE_NETWORK_DEBUG)
            {
                ushort cmdIDByType = Singleton<CommandMap>.Instance.GetCmdIDByType(typeof(T));
                if (this.CheckRequestTimeValid(cmdIDByType))
                {
                    NetPacketV1 reqPack = new NetPacketV1();
                    reqPack.setUserId((uint) Singleton<PlayerModule>.Instance.playerData.userId);
                    reqPack.setCmdId(cmdIDByType);
                    reqPack.setData<T>(data);
                    if ((cmdIDByType != 4) && (cmdIDByType != 6))
                    {
                        reqPack.setTime(++this._clientPacketConsumer.clientPacketId);
                    }
                    System.Type typeByCmdID = Singleton<CommandMap>.Instance.GetTypeByCmdID(cmdIDByType);
                    this.TryCacheSendPacket(reqPack);
                    this._client.send(reqPack);
                }
            }
        }

        public void SendPacketsOnLoginSuccess(bool forceAll = false, uint serverProcessedPacketId = 0)
        {
            if (forceAll || !this.alreadyLogin)
            {
                this.RequestConfigData();
                this.RequestGetAllEquipmentData();
                this.RequestGetAllAvatarData();
                this.RequestGetAvatarTeamData();
                this.RequestRecommandFriendList();
                this.RequestHasGotItemIdList();
                this.RequestGetBulletin();
                this.RequestIslandVenture();
                this.RequestGetIsland();
                this.RequestGetCollectCabin();
                this.RequestGetFinishGuideData();
                this.RequestGetAllLevelData();
                this.RequestGetVipRewardData();
                this.RequestGetShopList();
                this.RequestGachaDisplayInfo();
                this.RequestGetMailData();
            }
            this.RequestGetAllMainData();
            this.RequestGetAllWeekDayActivityData();
            this.RequestGetAssistantFrozenList();
            this.RequestEnterWorldChatroom(0);
            this.RequestFriendList();
            this.RequestAskAddFriendList();
            this.RequestRecommandFriendList();
            this.RequestGetMissionData();
            this.RequestGetInviteeFriend();
            this.RequestGetInviteFriend();
            if (this.alreadyLogin && (serverProcessedPacketId > 0))
            {
                this.SendQueuePacketWhenReconnected(serverProcessedPacketId);
            }
        }

        private void SendQueuePacketWhenReconnected(uint serverProcessedPacketId)
        {
            foreach (NetPacketV1 tv in this._packetSendQueue)
            {
                if (tv.getTime() > serverProcessedPacketId)
                {
                    System.Type typeByCmdID = Singleton<CommandMap>.Instance.GetTypeByCmdID(tv.getCmdId());
                    this._client.send(tv);
                }
            }
        }

        private void SetEquipmentItemByStorageDataItem(EquipmentItem equipmentItem, StorageDataItemBase storageDataItem)
        {
            if (storageDataItem.GetType() == typeof(WeaponDataItem))
            {
                equipmentItem.set_type(3);
                equipmentItem.set_id_or_unique_id((uint) storageDataItem.uid);
            }
            if (storageDataItem.GetType() == typeof(StigmataDataItem))
            {
                equipmentItem.set_type(4);
                equipmentItem.set_id_or_unique_id((uint) storageDataItem.uid);
            }
            if (storageDataItem.GetType() == typeof(MaterialDataItem))
            {
                equipmentItem.set_type(1);
                equipmentItem.set_id_or_unique_id((uint) storageDataItem.ID);
                equipmentItem.set_num((uint) storageDataItem.number);
            }
        }

        public void SetRepeatLogin()
        {
            this._clientPacketConsumer.SetRepeatLogin();
        }

        private void SetupRequestMinIntervalDict()
        {
            Dictionary<int, float> dictionary = new Dictionary<int, float>();
            dictionary.Add(14, 1f);
            dictionary.Add(0x12, 1f);
            dictionary.Add(0x36, 1f);
            dictionary.Add(0x1f, 3f);
            dictionary.Add(0x21, 3f);
            dictionary.Add(0x66, 3f);
            dictionary.Add(0x1d, 3f);
            dictionary.Add(0x25, 3f);
            dictionary.Add(0x23, 3f);
            dictionary.Add(50, 1f);
            dictionary.Add(0x56, 1f);
            dictionary.Add(0x6a, 1f);
            dictionary.Add(0x87, 1f);
            dictionary.Add(0x93, 1f);
            dictionary.Add(0x9e, 3f);
            dictionary.Add(160, 3f);
            dictionary.Add(0xa2, 1f);
            dictionary.Add(0xc1, 3f);
            dictionary.Add(0xc3, 3f);
            dictionary.Add(0xad, 3f);
            dictionary.Add(0xab, 3f);
            dictionary.Add(0xaf, 1f);
            dictionary.Add(0xcb, 1f);
            dictionary.Add(0xcd, 3f);
            dictionary.Add(0xb3, 3f);
            this._requestMinIntervalDict = dictionary;
        }

        private void TryCacheSendPacket(NetPacketV1 reqPack)
        {
            if (this._CMD_SHOULD_ENQUEUE_MAP == null)
            {
                this.BuildCmdShouldEnqueueMap();
            }
            if (this._CMD_SHOULD_ENQUEUE_MAP.ContainsKey(reqPack.getCmdId()))
            {
                if (this._packetSendQueue.Count >= 20)
                {
                    this._packetSendQueue.Dequeue();
                }
                this._packetSendQueue.Enqueue(reqPack);
            }
        }

        private bool TryGetRetCodeFromJsonString(string jsonString, out JSONNode retJson, out string errorMsg, out int retCode)
        {
            bool flag = false;
            errorMsg = string.Empty;
            retCode = 0;
            retJson = null;
            if (string.IsNullOrEmpty(jsonString))
            {
                flag = false;
                errorMsg = "Error: retJsonString IsNullOrEmpty!";
                return flag;
            }
            retJson = JSON.Parse(jsonString);
            if ((retJson == null) || string.IsNullOrEmpty((string) retJson["retcode"]))
            {
                flag = false;
                errorMsg = "Error: JSON.Parse null!";
                return flag;
            }
            if (!int.TryParse(retJson["retcode"].Value, out retCode))
            {
                flag = false;
                errorMsg = "Error: retcode is not integer!";
                return flag;
            }
            return true;
        }

        private void TryReconnectDispatch()
        {
            if (this.alreadyLogin)
            {
                this.QuickLogin();
            }
            else
            {
                MonoGameEntry sceneCanvas = Singleton<MainUIManager>.Instance.SceneCanvas as MonoGameEntry;
                if (sceneCanvas != null)
                {
                    sceneCanvas.ConnectDispatch();
                }
            }
        }

        private void TryReconnectGlobalDispatch()
        {
            if (!this.alreadyLogin)
            {
                MonoGameEntry sceneCanvas = Singleton<MainUIManager>.Instance.SceneCanvas as MonoGameEntry;
                if (sceneCanvas != null)
                {
                    sceneCanvas.ConnentGlobalDispatch();
                }
            }
        }

        public void UnexceptedDisconnectCallback()
        {
        }

        public DispatchServerDataItem DispatchSeverData { get; set; }

        public GlobalDispatchDataItem GlobalDispatchData { get; private set; }

        [CompilerGenerated]
        private sealed class <ConnectDispatchServer>c__Iterator4E : IEnumerator, IDisposable, IEnumerator<object>
        {
            internal object $current;
            internal int $PC;
            internal Action <$>successCallback;
            internal NetworkManager <>f__this;
            internal string <dispatchUrl>__1;
            internal int <lastLoginUserId>__0;
            internal string <retString>__3;
            internal WWW <www>__2;
            internal Action successCallback;

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
                    {
                        this.<lastLoginUserId>__0 = Singleton<MiHoYoGameData>.Instance.GeneralLocalData.LastLoginUserId;
                        object[] objArray1 = new object[] { this.<>f__this.GlobalDispatchData.regionList[0].dispatchUrl, "?version=", this.<>f__this.GetGameVersion(), "&uid=", this.<lastLoginUserId>__0 };
                        this.<dispatchUrl>__1 = string.Concat(objArray1);
                        this.<www>__2 = new WWW(this.<dispatchUrl>__1);
                        this.$current = this.<www>__2;
                        this.$PC = 1;
                        return true;
                    }
                    case 1:
                        this.<retString>__3 = string.Empty;
                        if (string.IsNullOrEmpty(this.<www>__2.error))
                        {
                            this.<retString>__3 = this.<www>__2.text;
                            break;
                        }
                        break;

                    default:
                        goto Label_011A;
                }
                this.<>f__this.OnConnectDispatchServer(this.<retString>__3, this.successCallback);
                this.<www>__2.Dispose();
                this.$PC = -1;
            Label_011A:
                return false;
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
        private sealed class <ConnectGlobalDispatchServer>c__Iterator4D : IEnumerator, IDisposable, IEnumerator<object>
        {
            internal object $current;
            internal int $PC;
            internal Action <$>successCallback;
            internal NetworkManager <>f__this;
            internal int <count>__1;
            internal string <globalDispatchUrl>__3;
            internal string <retJsonString>__0;
            internal bool <timeout>__4;
            internal float <timer>__2;
            internal WWW <www>__5;
            internal Action successCallback;

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
                        this.<retJsonString>__0 = string.Empty;
                        this.<count>__1 = 0;
                        goto Label_0136;

                    case 1:
                        break;

                    default:
                        goto Label_0170;
                }
            Label_00D2:
                while (!this.<www>__5.isDone)
                {
                    if (this.<timer>__2 > 3f)
                    {
                        this.<timeout>__4 = true;
                        break;
                    }
                    this.<timer>__2 += Time.deltaTime;
                    this.$current = null;
                    this.$PC = 1;
                    return true;
                }
                if (!string.IsNullOrEmpty(this.<www>__5.error) || this.<timeout>__4)
                {
                    this.<www>__5.Dispose();
                    this.<count>__1++;
                }
                else
                {
                    this.<retJsonString>__0 = this.<www>__5.text;
                    goto Label_0151;
                }
            Label_0136:
                if (this.<count>__1 < this.<>f__this._globalDispatchUrlList.Count)
                {
                    this.<timer>__2 = 0f;
                    this.<globalDispatchUrl>__3 = this.<>f__this._globalDispatchUrlList[this.<count>__1] + "?version=" + this.<>f__this.GetGameVersion();
                    this.<timeout>__4 = false;
                    this.<www>__5 = new WWW(this.<globalDispatchUrl>__3);
                    goto Label_00D2;
                }
            Label_0151:
                this.<>f__this.OnConnectGlobalDispatch(this.<retJsonString>__0, this.successCallback);
                this.$PC = -1;
            Label_0170:
                return false;
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
        private sealed class <DoSendFakePacket>c__Iterator4C<T> : IEnumerator, IDisposable, IEnumerator<object>
        {
            internal object $current;
            internal int $PC;
            internal T <$>data;
            internal ushort <cmdID>__0;
            internal System.Type <cmdType>__2;
            internal NetPacketV1 <req_pack>__1;
            internal T data;

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
                        this.$current = null;
                        this.$PC = 1;
                        return true;

                    case 1:
                        this.<cmdID>__0 = Singleton<CommandMap>.Instance.GetCmdIDByType(typeof(T));
                        this.<req_pack>__1 = new NetPacketV1();
                        this.<req_pack>__1.setUserId((uint) Singleton<PlayerModule>.Instance.playerData.userId);
                        this.<req_pack>__1.setCmdId(this.<cmdID>__0);
                        this.<req_pack>__1.setData<T>(this.data);
                        this.<cmdType>__2 = Singleton<CommandMap>.Instance.GetTypeByCmdID(this.<cmdID>__0);
                        Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.NetwrokPacket, this.<req_pack>__1));
                        this.$PC = -1;
                        break;
                }
                return false;
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
        private sealed class <OnConnectDispatchServer>c__AnonStoreyDB
        {
            internal string forceUupdateUrl;

            internal void <>m__FC(bool confirmed)
            {
                Application.OpenURL(this.forceUupdateUrl);
            }
        }
    }
}

