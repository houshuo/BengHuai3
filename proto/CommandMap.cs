namespace proto
{
    using System;
    using System.Collections.Generic;

    public class CommandMap
    {
        private Dictionary<ushort, Type> _cmdIDMap;
        private Dictionary<Type, ushort> _typeMap;

        private CommandMap()
        {
            this._cmdIDMap = this.MakeCmdIDMap();
            this._typeMap = this.GetReverseMap(this._cmdIDMap);
        }

        public ushort GetCmdIDByType(Type type)
        {
            ushort num;
            if (!this._typeMap.TryGetValue(type, out num))
            {
            }
            return num;
        }

        private Dictionary<Type, ushort> GetReverseMap(Dictionary<ushort, Type> orgMap)
        {
            Dictionary<Type, ushort> dictionary = new Dictionary<Type, ushort>();
            foreach (KeyValuePair<ushort, Type> pair in orgMap)
            {
                dictionary.Add(pair.Value, pair.Key);
            }
            return dictionary;
        }

        public Type GetTypeByCmdID(ushort cmdID)
        {
            Type type;
            if (!this._cmdIDMap.TryGetValue(cmdID, out type))
            {
            }
            return type;
        }

        private Dictionary<ushort, Type> MakeCmdIDMap()
        {
            Dictionary<ushort, Type> dictionary = new Dictionary<ushort, Type>();
            dictionary.Add(1, typeof(KeepAliveNotify));
            dictionary.Add(2, typeof(GetGameserverReq));
            dictionary.Add(3, typeof(GetGameserverRsp));
            dictionary.Add(4, typeof(GetPlayerTokenReq));
            dictionary.Add(5, typeof(GetPlayerTokenRsp));
            dictionary.Add(6, typeof(PlayerLoginReq));
            dictionary.Add(7, typeof(PlayerLoginRsp));
            dictionary.Add(8, typeof(PlayerLogoutReq));
            dictionary.Add(10, typeof(GetMainDataReq));
            dictionary.Add(11, typeof(GetMainDataRsp));
            dictionary.Add(12, typeof(GetScoinExchangeInfoReq));
            dictionary.Add(13, typeof(GetScoinExchangeInfoRsp));
            dictionary.Add(14, typeof(ScoinExchangeReq));
            dictionary.Add(15, typeof(ScoinExchangeRsp));
            dictionary.Add(0x10, typeof(GetStaminaExchangeInfoReq));
            dictionary.Add(0x11, typeof(GetStaminaExchangeInfoRsp));
            dictionary.Add(0x12, typeof(StaminaExchangeReq));
            dictionary.Add(0x13, typeof(StaminaExchangeRsp));
            dictionary.Add(20, typeof(NicknameModifyReq));
            dictionary.Add(0x15, typeof(NicknameModifyRsp));
            dictionary.Add(0x16, typeof(GmTalkReq));
            dictionary.Add(0x17, typeof(GmTalkRsp));
            dictionary.Add(0x18, typeof(GetAvatarDataReq));
            dictionary.Add(0x19, typeof(GetAvatarDataRsp));
            dictionary.Add(0x1a, typeof(GetEquipmentDataReq));
            dictionary.Add(0x1b, typeof(GetEquipmentDataRsp));
            dictionary.Add(0x1c, typeof(DelEquipmentNotify));
            dictionary.Add(0x1d, typeof(AvatarStarUpReq));
            dictionary.Add(30, typeof(AvatarStarUpRsp));
            dictionary.Add(0x1f, typeof(EquipmentPowerUpReq));
            dictionary.Add(0x20, typeof(EquipmentPowerUpRsp));
            dictionary.Add(0x21, typeof(EquipmentSellReq));
            dictionary.Add(0x22, typeof(EquipmentSellRsp));
            dictionary.Add(0x23, typeof(AddAvatarExpByMaterialReq));
            dictionary.Add(0x24, typeof(AddAvatarExpByMaterialRsp));
            dictionary.Add(0x25, typeof(EquipmentEvoReq));
            dictionary.Add(0x26, typeof(EquipmentEvoRsp));
            dictionary.Add(0x27, typeof(DressEquipmentReq));
            dictionary.Add(40, typeof(DressEquipmentRsp));
            dictionary.Add(0x29, typeof(GetStageDataReq));
            dictionary.Add(0x2a, typeof(GetStageDataRsp));
            dictionary.Add(0x2b, typeof(StageBeginReq));
            dictionary.Add(0x2c, typeof(StageBeginRsp));
            dictionary.Add(0x2d, typeof(StageEndReq));
            dictionary.Add(0x2e, typeof(StageEndRsp));
            dictionary.Add(0x2f, typeof(GetAvatarTeamDataReq));
            dictionary.Add(0x30, typeof(GetAvatarTeamDataRsp));
            dictionary.Add(0x31, typeof(UpdateAvatarTeamNotify));
            dictionary.Add(50, typeof(AvatarSubSkillLevelUpReq));
            dictionary.Add(0x33, typeof(AvatarSubSkillLevelUpRsp));
            dictionary.Add(0x34, typeof(GetSkillPointExchangeInfoReq));
            dictionary.Add(0x35, typeof(GetSkillPointExchangeInfoRsp));
            dictionary.Add(0x36, typeof(SkillPointExchangeReq));
            dictionary.Add(0x37, typeof(SkillPointExchangeRsp));
            dictionary.Add(0x38, typeof(MaterialEvoReq));
            dictionary.Add(0x39, typeof(MaterialEvoRsp));
            dictionary.Add(0x3a, typeof(GachaReq));
            dictionary.Add(0x3b, typeof(GachaRsp));
            dictionary.Add(60, typeof(GetStageDropDisplayReq));
            dictionary.Add(0x3d, typeof(GetStageDropDisplayRsp));
            dictionary.Add(0x3e, typeof(GetGachaDisplayReq));
            dictionary.Add(0x3f, typeof(GetGachaDisplayRsp));
            dictionary.Add(0x40, typeof(GetFriendListReq));
            dictionary.Add(0x41, typeof(GetFriendListRsp));
            dictionary.Add(0x42, typeof(AddFriendReq));
            dictionary.Add(0x43, typeof(AddFriendRsp));
            dictionary.Add(0x44, typeof(DelFriendReq));
            dictionary.Add(0x45, typeof(DelFriendRsp));
            dictionary.Add(70, typeof(GetAskAddFriendListReq));
            dictionary.Add(0x47, typeof(GetAskAddFriendListRsp));
            dictionary.Add(0x48, typeof(GetPlayerDetailDataReq));
            dictionary.Add(0x49, typeof(GetPlayerDetailDataRsp));
            dictionary.Add(0x4a, typeof(UpdateEquipmentProtectedStatusReq));
            dictionary.Add(0x4b, typeof(UpdateEquipmentProtectedStatusRsp));
            dictionary.Add(0x4c, typeof(GetRecommendFriendListReq));
            dictionary.Add(0x4d, typeof(GetRecommendFriendListRsp));
            dictionary.Add(0x4e, typeof(SetSelfDescReq));
            dictionary.Add(0x4f, typeof(SetSelfDescRsp));
            dictionary.Add(80, typeof(DelFriendNotify));
            dictionary.Add(0x51, typeof(GetOfflineFriendsPointNotify));
            dictionary.Add(0x52, typeof(VerifyItunesOrderNotify));
            dictionary.Add(0x53, typeof(RechargeFinishNotify));
            dictionary.Add(0x54, typeof(GetMailDataReq));
            dictionary.Add(0x55, typeof(GetMailDataRsp));
            dictionary.Add(0x56, typeof(GetMailAttachmentReq));
            dictionary.Add(0x57, typeof(GetMailAttachmentRsp));
            dictionary.Add(0x58, typeof(EnterWorldChatroomReq));
            dictionary.Add(0x59, typeof(EnterWorldChatroomRsp));
            dictionary.Add(90, typeof(SendWorldChatMsgNotify));
            dictionary.Add(0x5b, typeof(RecvWorldChatMsgNotify));
            dictionary.Add(0x5c, typeof(SendFriendChatMsgNotify));
            dictionary.Add(0x5d, typeof(RecvFriendChatMsgNotify));
            dictionary.Add(0x5e, typeof(RecvFriendOfflineChatMsgNotify));
            dictionary.Add(0x5f, typeof(LeaveChatroomNotify));
            dictionary.Add(0x60, typeof(SendSystemChatMsgNotify));
            dictionary.Add(0x61, typeof(RecvSystemChatMsgNotify));
            dictionary.Add(0x62, typeof(GetProductListReq));
            dictionary.Add(0x63, typeof(GetProductListRsp));
            dictionary.Add(100, typeof(GetAssistantFrozenListReq));
            dictionary.Add(0x65, typeof(GetAssistantFrozenListRsp));
            dictionary.Add(0x66, typeof(SellAvatarFragmentReq));
            dictionary.Add(0x67, typeof(SellAvatarFragmentRsp));
            dictionary.Add(0x68, typeof(GetHasGotItemIdListReq));
            dictionary.Add(0x69, typeof(GetHasGotItemIdListRsp));
            dictionary.Add(0x6a, typeof(AvatarReviveReq));
            dictionary.Add(0x6b, typeof(AvatarReviveRsp));
            dictionary.Add(0x6c, typeof(ResetStageEnterTimesReq));
            dictionary.Add(0x6d, typeof(ResetStageEnterTimesRsp));
            dictionary.Add(110, typeof(GetConfigReq));
            dictionary.Add(0x6f, typeof(GetConfigRsp));
            dictionary.Add(0x70, typeof(GetMissionDataReq));
            dictionary.Add(0x71, typeof(GetMissionDataRsp));
            dictionary.Add(0x72, typeof(GetMissionRewardReq));
            dictionary.Add(0x73, typeof(GetMissionRewardRsp));
            dictionary.Add(0x74, typeof(DelMissionNotify));
            dictionary.Add(0x75, typeof(UpdateMissionProgressReq));
            dictionary.Add(0x76, typeof(UpdateMissionProgressRsp));
            dictionary.Add(0x77, typeof(BindAccountReq));
            dictionary.Add(120, typeof(BindAccountRsp));
            dictionary.Add(0x79, typeof(GetSignInRewardStatusReq));
            dictionary.Add(0x7a, typeof(GetSignInRewardStatusRsp));
            dictionary.Add(0x7b, typeof(GetSignInRewardReq));
            dictionary.Add(0x7c, typeof(GetSignInRewardRsp));
            dictionary.Add(0x7d, typeof(GetWeekDayActivityDataReq));
            dictionary.Add(0x7e, typeof(GetWeekDayActivityDataRsp));
            dictionary.Add(0x7f, typeof(GetFinishGuideDataReq));
            dictionary.Add(0x80, typeof(GetFinishGuideDataRsp));
            dictionary.Add(0x81, typeof(FinishGuideReportReq));
            dictionary.Add(130, typeof(FinishGuideReportRsp));
            dictionary.Add(0x83, typeof(StageInnerDataReportReq));
            dictionary.Add(0x84, typeof(StageInnerDataReportRsp));
            dictionary.Add(0x85, typeof(GetDispatchReq));
            dictionary.Add(0x86, typeof(GetDispatchRsp));
            dictionary.Add(0x87, typeof(ExchangeAvatarWeaponReq));
            dictionary.Add(0x88, typeof(ExchangeAvatarWeaponRsp));
            dictionary.Add(0x89, typeof(GetBulletinReq));
            dictionary.Add(0x8a, typeof(GetBulletinRsp));
            dictionary.Add(0x8b, typeof(GetEndlessDataReq));
            dictionary.Add(140, typeof(GetEndlessDataRsp));
            dictionary.Add(0x8d, typeof(EndlessStageBeginReq));
            dictionary.Add(0x8e, typeof(EndlessStageBeginRsp));
            dictionary.Add(0x8f, typeof(EndlessStageEndReq));
            dictionary.Add(0x90, typeof(EndlessStageEndRsp));
            dictionary.Add(0x91, typeof(GetLastEndlessRewardDataReq));
            dictionary.Add(0x92, typeof(GetLastEndlessRewardDataRsp));
            dictionary.Add(0x93, typeof(UseEndlessItemReq));
            dictionary.Add(0x94, typeof(UseEndlessItemRsp));
            dictionary.Add(0x95, typeof(GetEndlessAvatarHpReq));
            dictionary.Add(150, typeof(GetEndlessAvatarHpRsp));
            dictionary.Add(0x97, typeof(EndlessPlayerDataUpdateNotify));
            dictionary.Add(0x98, typeof(EndlessItemDataUpdateNotify));
            dictionary.Add(0x99, typeof(EndlessWarInfoNotify));
            dictionary.Add(0x9a, typeof(AddGoodfeelReq));
            dictionary.Add(0x9b, typeof(AddGoodfeelRsp));
            dictionary.Add(0x9c, typeof(GetIslandReq));
            dictionary.Add(0x9d, typeof(GetIslandRsp));
            dictionary.Add(0x9e, typeof(LevelUpCabinReq));
            dictionary.Add(0x9f, typeof(LevelUpCabinRsp));
            dictionary.Add(160, typeof(ExtendCabinReq));
            dictionary.Add(0xa1, typeof(ExtendCabinRsp));
            dictionary.Add(0xa2, typeof(FinishCabinLevelUpReq));
            dictionary.Add(0xa3, typeof(FinishCabinLevelUpRsp));
            dictionary.Add(0xa4, typeof(AddCabinTechReq));
            dictionary.Add(0xa5, typeof(AddCabinTechRsp));
            dictionary.Add(0xa6, typeof(ResetCabinTechReq));
            dictionary.Add(0xa7, typeof(ResetCabinTechRsp));
            dictionary.Add(0xa8, typeof(GetIslandVentureReq));
            dictionary.Add(0xa9, typeof(GetIslandVentureRsp));
            dictionary.Add(170, typeof(DelIslandVentureNotify));
            dictionary.Add(0xab, typeof(RefreshIslandVentureReq));
            dictionary.Add(0xac, typeof(RefreshIslandVentureRsp));
            dictionary.Add(0xad, typeof(DispatchIslandVentureReq));
            dictionary.Add(0xae, typeof(DispatchIslandVentureRsp));
            dictionary.Add(0xaf, typeof(GetIslandVentureRewardReq));
            dictionary.Add(0xb0, typeof(GetIslandVentureRewardRsp));
            dictionary.Add(0xb1, typeof(CancelDispatchIslandVentureReq));
            dictionary.Add(0xb2, typeof(CancelDispatchIslandVentureRsp));
            dictionary.Add(0xb3, typeof(IslandDisjoinEquipmentReq));
            dictionary.Add(180, typeof(IslandDisjoinEquipmentRsp));
            dictionary.Add(0xb5, typeof(IslandCollectReq));
            dictionary.Add(0xb6, typeof(IslandCollectRsp));
            dictionary.Add(0xb7, typeof(GetCollectCabinReq));
            dictionary.Add(0xb8, typeof(GetCollectCabinRsp));
            dictionary.Add(0xb9, typeof(GetGuideRewardReq));
            dictionary.Add(0xba, typeof(GetGuideRewardRsp));
            dictionary.Add(0xbb, typeof(UrgencyMsgNotify));
            dictionary.Add(0xbf, typeof(IdentifyStigmataAffixReq));
            dictionary.Add(0xc0, typeof(IdentifyStigmataAffixRsp));
            dictionary.Add(0xc1, typeof(FeedStigmataAffixReq));
            dictionary.Add(0xc2, typeof(FeedStigmataAffixRsp));
            dictionary.Add(0xc3, typeof(SelectNewStigmataAffixReq));
            dictionary.Add(0xc4, typeof(SelectNewStigmataAffixRsp));
            dictionary.Add(0xc5, typeof(GetVipRewardDataReq));
            dictionary.Add(0xc6, typeof(GetVipRewardDataRsp));
            dictionary.Add(0xc7, typeof(GetVipRewardReq));
            dictionary.Add(200, typeof(GetVipRewardRsp));
            dictionary.Add(0xc9, typeof(GetShopListReq));
            dictionary.Add(0xca, typeof(GetShopListRsp));
            dictionary.Add(0xcb, typeof(BuyGoodsReq));
            dictionary.Add(0xcc, typeof(BuyGoodsRsp));
            dictionary.Add(0xcd, typeof(ManualRefreshShopReq));
            dictionary.Add(0xce, typeof(ManualRefreshShopRsp));
            dictionary.Add(0xcf, typeof(CreateWeiXinOrderReq));
            dictionary.Add(0xd0, typeof(CreateWeiXinOrderRsp));
            dictionary.Add(0xd1, typeof(SpeedUpIslandVentureReq));
            dictionary.Add(210, typeof(SpeedUpIslandVentureRsp));
            dictionary.Add(0xd3, typeof(GetRedeemCodeInfoReq));
            dictionary.Add(0xd4, typeof(GetRedeemCodeInfoRsp));
            dictionary.Add(0xd5, typeof(ExchangeRedeemCodeReq));
            dictionary.Add(0xd6, typeof(ExchangeRedeemCodeRsp));
            dictionary.Add(0xd7, typeof(BuyGachaTicketReq));
            dictionary.Add(0xd8, typeof(BuyGachaTicketRsp));
            dictionary.Add(0xd9, typeof(AntiCheatSDKReportReq));
            dictionary.Add(0xda, typeof(AntiCheatSDKReportRsp));
            dictionary.Add(0xdb, typeof(GetEndlessTopGroupReq));
            dictionary.Add(220, typeof(GetEndlessTopGroupRsp));
            dictionary.Add(0xdd, typeof(GetRefreshIslandVentureInfoReq));
            dictionary.Add(0xde, typeof(GetRefreshIslandVentureInfoRsp));
            dictionary.Add(0x12d, typeof(EnterLobbyReq));
            dictionary.Add(0x12e, typeof(EnterLobbyRsp));
            dictionary.Add(0x12f, typeof(LeaveLobbyReq));
            dictionary.Add(0x130, typeof(LeaveLobbyRsp));
            dictionary.Add(0x131, typeof(ExchangeLobbyAvatarReq));
            dictionary.Add(0x132, typeof(ExchangeLobbyAvatarRsp));
            dictionary.Add(0x133, typeof(SwitchMemberStatusReq));
            dictionary.Add(0x134, typeof(SwitchMemberStatusRsp));
            dictionary.Add(0x135, typeof(SwitchLeaderReq));
            dictionary.Add(310, typeof(SwitchLeaderRsp));
            dictionary.Add(0x137, typeof(LobbyFightStartReq));
            dictionary.Add(0x138, typeof(LobbyFightStartRsp));
            dictionary.Add(0x139, typeof(LobbyFightEndReq));
            dictionary.Add(0x13a, typeof(LobbyFightEndRsp));
            dictionary.Add(0x13b, typeof(GetLobbyDataReq));
            dictionary.Add(0x13c, typeof(GetLobbyDataRsp));
            dictionary.Add(0xdf, typeof(GetInviteFriendReq));
            dictionary.Add(0xe0, typeof(GetInviteFriendRsp));
            dictionary.Add(0xe1, typeof(GetInviteeFriendReq));
            dictionary.Add(0xe2, typeof(GetInviteeFriendRsp));
            dictionary.Add(0xe3, typeof(AcceptFriendInviteReq));
            dictionary.Add(0xe4, typeof(AcceptFriendInviteRsp));
            dictionary.Add(0xe5, typeof(CommentReportReq));
            dictionary.Add(230, typeof(CommentReportRsp));
            dictionary.Add(0xe7, typeof(GetExtraStoryDataReq));
            dictionary.Add(0xe8, typeof(GetExtraStoryDataRsp));
            dictionary.Add(0xe9, typeof(GetExtraStoryActivityActReq));
            dictionary.Add(0xea, typeof(GetExtraStoryActivityActRsp));
            return dictionary;
        }
    }
}

