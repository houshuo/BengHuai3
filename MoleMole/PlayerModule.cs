namespace MoleMole
{
    using proto;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    public class PlayerModule : BaseModule
    {
        private PlayerModule()
        {
            Singleton<NotifyManager>.Instance.RegisterModule(this);
            this.playerData = new PlayerDataItem(1);
        }

        public bool IsBehaviourDone(string key)
        {
            return Singleton<MiHoYoGameData>.Instance.LocalData.DoneBehaviourList.Contains(key);
        }

        private bool OnAntiCheatSDKReportRsp(AntiCheatSDKReportRsp rsp)
        {
            return false;
        }

        private bool OnBindAccountRsp(BindAccountRsp rsp)
        {
            if (rsp.get_retcode() == null)
            {
                this.SaveLastLoginAccountInfo();
            }
            return false;
        }

        private bool OnGetAvatarTeamDataRsp(GetAvatarTeamDataRsp rsp)
        {
            if (rsp.get_retcode() == null)
            {
                foreach (AvatarTeam team in rsp.get_avatar_team_list())
                {
                    StageType key = (StageType) team.get_stage_type();
                    if (!this.playerData.teamDict.ContainsKey(key))
                    {
                        this.playerData.teamDict.Add(key, new List<int>());
                    }
                    this.playerData.teamDict[key] = base.ConvertList(team.get_avatar_id_list());
                }
            }
            return false;
        }

        private bool OnGetConfigDataRsp(GetConfigRsp rsp)
        {
            base.UpdateField<int>(ref this.playerData.staminaRecoverConfigTime, (int) rsp.get_stamina_recover_config_time(), null);
            base.UpdateField<int>(ref this.playerData.skillPointRecoverConfigTime, (int) rsp.get_skill_point_recover_config_time(), null);
            base.UpdateField<int>(ref this.playerData.reviveHcoinCost, (int) rsp.get_avatar_revive_hcoin_cost(), null);
            base.UpdateField<int>(ref this.playerData.sameTypePowerUpRataInt, (int) rsp.get_same_type_power_up_rate(), null);
            base.UpdateField<int>(ref this.playerData.powerUpScoinCostRate, (int) rsp.get_power_up_scoin_cost_rate(), null);
            base.UpdateField<int>(ref this.playerData.maxFriend, (int) rsp.get_max_friend_num(), null);
            base.UpdateField<int>(ref this.playerData.endlessMinPlayerLevel, (int) rsp.get_endless_min_player_level(), null);
            base.UpdateField<int>(ref this.playerData.endlessMaxProgress, (int) rsp.get_endless_max_progress(), null);
            base.UpdateField<int>(rsp.get_endless_use_item_cd_timeSpecified(), ref this.playerData.endlessUseItemCDTime, (int) rsp.get_endless_use_item_cd_time(), null);
            base.UpdateField<int>(ref this.playerData.disjoin_equipment_back_exp_percent, (int) rsp.get_disjoin_equipment_back_exp_percent(), null);
            base.UpdateField<int>(ref this.playerData.avatarCombatBaseWeight, (int) rsp.get_avatar_combat_base_weight(), null);
            base.UpdateField<int>(ref this.playerData.avatarCombatBaseStarRate, (int) rsp.get_avatar_combat_base_star_rate(), null);
            base.UpdateField<int>(ref this.playerData.avatarCombatBaseLevelRate, (int) rsp.get_avatar_combat_base_level_rate(), null);
            base.UpdateField<int>(ref this.playerData.avatarCombatBaseUnlockStarRate, (int) rsp.get_avatar_combat_base_unlock_star_rate(), null);
            base.UpdateField<int>(ref this.playerData.avatarCombatSkillWeight, (int) rsp.get_avatar_combat_skill_weight(), null);
            base.UpdateField<int>(ref this.playerData.avatarCombatIslandWeight, (int) rsp.get_avatar_combat_island_weight(), null);
            base.UpdateField<int>(ref this.playerData.avatarCombatWeaponWeight, (int) rsp.get_avatar_combat_weapon_weight(), null);
            base.UpdateField<int>(ref this.playerData.avatarCombatWeaponRarityRate, (int) rsp.get_avatar_combat_weapon_rarity_rate(), null);
            base.UpdateField<int>(ref this.playerData.avatarCombatWeaponSubRarityRate, (int) rsp.get_avatar_combat_weapon_sub_rarity_rate(), null);
            base.UpdateField<int>(ref this.playerData.avatarCombatWeaponLevelRate, (int) rsp.get_avatar_combat_weapon_level_rate(), null);
            base.UpdateField<int>(ref this.playerData.avatarCombatStigmataWeight, (int) rsp.get_avatar_combat_stigmata_weight(), null);
            base.UpdateField<int>(ref this.playerData.avatarCombatStigmataRarityRate, (int) rsp.get_avatar_combat_stigmata_rarity_rate(), null);
            base.UpdateField<int>(ref this.playerData.avatarCombatStigmataSubRarityRate, (int) rsp.get_avatar_combat_stigmata_sub_rarity_rate(), null);
            base.UpdateField<int>(ref this.playerData.avatarCombatStigmataLevelRate, (int) rsp.get_avatar_combat_stigmata_level_rate(), null);
            base.UpdateField<int>(ref this.playerData.avatarCombatStigmataSuitNumRate, (int) rsp.get_avatar_combat_stigmata_suit_num_rate(), null);
            base.UpdateField<int>(ref this.playerData.minLevelToGenerateInviteCode, (int) rsp.get_min_invite_level(), null);
            base.UpdateField<int>(ref this.playerData.maxLevelToAcceptInvite, (int) rsp.get_max_accept_invitee_level(), null);
            foreach (AvatarCostPlusConfig config in rsp.get_avatar_cost_plus_config_list())
            {
                this.playerData.costAddByAvatarStar[(int) config.get_star()] = (int) config.get_cost_plus();
            }
            foreach (GetConfigRsp.GachaTicket ticket in rsp.get_gacha_ticket_list())
            {
                this.playerData.gachaTicketPriceDict[(int) ticket.get_material_id()] = (int) ticket.get_hcoin_cost();
                this.playerData.gachaTicketPriceDict[(int) (ticket.get_material_id() * 10)] = (int) (ticket.get_hcoin_cost() * 10);
            }
            TimeUtil.SetServerCurTime(rsp.get_server_cur_time());
            TimeUtil.SetDayTimeOffset((int) rsp.get_day_time_offset());
            if (rsp.get_region_nameSpecified() && (Singleton<AccountManager>.Instance.manager != null))
            {
                Singleton<AccountManager>.Instance.manager.ChannelRegion = rsp.get_region_name();
            }
            return false;
        }

        private bool OnGetLastEndlessRewardDataRsp(GetLastEndlessRewardDataRsp rsp)
        {
            if ((rsp.get_retcode() == null) && (rsp.get_reward_list().Count > 0))
            {
                Singleton<MiHoYoGameData>.Instance.LocalData.LastRewardData = rsp;
                Singleton<MiHoYoGameData>.Instance.Save();
            }
            return false;
        }

        private bool OnGetMainDataRsp(GetMainDataRsp rsp)
        {
            if (rsp.get_retcode() == null)
            {
                this.playerData.initByGetMainDataRsp = true;
                base.UpdateField<string>(rsp.get_nicknameSpecified(), ref this.playerData.nickname, rsp.get_nickname(), null);
                base.UpdateField<int>(rsp.get_levelSpecified(), ref this.playerData.teamLevel, (int) rsp.get_level(), new Action<int, int>(this.playerData.OnLevelChange));
                base.UpdateField<int>(rsp.get_expSpecified(), ref this.playerData.teamExp, (int) rsp.get_exp(), null);
                base.UpdateField<int>(rsp.get_hcoinSpecified(), ref this.playerData.hcoin, (int) rsp.get_hcoin(), new Action<int, int>(this.playerData.OnCoinChange));
                base.UpdateField<int>(rsp.get_scoinSpecified(), ref this.playerData.scoin, (int) rsp.get_scoin(), new Action<int, int>(this.playerData.OnCoinChange));
                base.UpdateField<int>(rsp.get_staminaSpecified(), ref this.playerData.stamina, (int) rsp.get_stamina(), null);
                base.UpdateField<int>(rsp.get_stamina_recover_left_timeSpecified(), ref this.playerData.staminaRecoverLeftTime, (int) rsp.get_stamina_recover_left_time(), new Action<int, int>(this.playerData.OnStaminaRecoverTimeChange));
                base.UpdateField<int>(rsp.get_stamina_recover_config_timeSpecified(), ref this.playerData.staminaRecoverConfigTime, (int) rsp.get_stamina_recover_config_time(), new Action<int, int>(this.playerData.OnStaminaRecoverTimeChange));
                base.UpdateField<int>(rsp.get_skill_pointSpecified(), ref this.playerData.skillPoint, (int) rsp.get_skill_point(), null);
                base.UpdateField<int>(rsp.get_skill_point_recover_left_timeSpecified(), ref this.playerData.skillPointRecoverLeftTime, (int) rsp.get_skill_point_recover_left_time(), new Action<int, int>(this.playerData.OnSkillPointRecoverTimeChange));
                base.UpdateField<int>(rsp.get_skill_point_recover_config_timeSpecified(), ref this.playerData.skillPointRecoverConfigTime, (int) rsp.get_skill_point_recover_config_time(), new Action<int, int>(this.playerData.OnSkillPointRecoverTimeChange));
                base.UpdateField<int>(rsp.get_skill_point_limitSpecified(), ref this.playerData.skillPointLimit, (int) rsp.get_skill_point_limit(), null);
                base.UpdateField<int>(rsp.get_equipment_size_limitSpecified(), ref this.playerData.equipmentSizeLimit, (int) rsp.get_equipment_size_limit(), null);
                base.UpdateField<int>(rsp.get_friends_pointSpecified(), ref this.playerData.friendsPoint, (int) rsp.get_friends_point(), null);
                base.UpdateField<string>(rsp.get_self_descSpecified(), ref this.playerData.selfDesc, rsp.get_self_desc(), null);
            }
            return false;
        }

        private bool OnGetOfflineFriendsPointNotify(GetOfflineFriendsPointNotify rsp)
        {
            this.playerData.offlineFriendsPoint = (int) rsp.get_friends_point();
            return false;
        }

        private bool OnGetPlayerTokenRsp(GetPlayerTokenRsp rsp)
        {
            if (rsp.get_retcode() == null)
            {
                this.playerData.userId = (int) rsp.get_uid();
                this.SaveLastLoginAccountInfo();
                Singleton<NetworkManager>.Instance.RequestPlayerLogin();
            }
            return false;
        }

        private bool OnGetScoinExchangeInfoRsp(GetScoinExchangeInfoRsp rsp)
        {
            bool flag = this.playerData.scoinExchangeCache.Value == null;
            PlayerScoinExchangeInfo info = new PlayerScoinExchangeInfo {
                usableTimes = (int) rsp.get_usable_times(),
                totalTimes = (int) rsp.get_total_times(),
                hcoinCost = (int) rsp.get_hcoin_cost(),
                scoinGet = (int) rsp.get_scoin_get()
            };
            this.playerData.scoinExchangeCache.Value = info;
            if (flag)
            {
                Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.ShowScoinExchangeInfo, rsp));
            }
            return false;
        }

        private bool OnGetSignInRewardStatusRsp(GetSignInRewardStatusRsp rsp)
        {
            this.playerData.signInStatus = rsp;
            return false;
        }

        private bool OnGetSkillPointExchangeInfoRsp(GetSkillPointExchangeInfoRsp rsp)
        {
            PlayerSkillPointExchangeInfo info = new PlayerSkillPointExchangeInfo {
                usableTimes = (int) rsp.get_usable_times(),
                totalTimes = (int) rsp.get_total_times(),
                hcoinCost = (int) rsp.get_hcoin_cost(),
                skillPointGet = (int) rsp.get_skill_point_get()
            };
            this.playerData.skillPointExchangeCache.Value = info;
            return false;
        }

        private bool OnGetStaminaExchangeInfoRsp(GetStaminaExchangeInfoRsp rsp)
        {
            PlayerStaminaExchangeInfo info = new PlayerStaminaExchangeInfo {
                usableTimes = (int) rsp.get_usable_times(),
                totalTimes = (int) rsp.get_total_times(),
                hcoinCost = (int) rsp.get_hcoin_cost(),
                staminaGet = (int) rsp.get_stamina_get()
            };
            this.playerData.staminaExchangeCache.Value = info;
            this.playerData._cacheDataUtil.OnGetRsp(0x11);
            return false;
        }

        public override bool OnPacket(NetPacketV1 pkt)
        {
            switch (pkt.getCmdId())
            {
                case 5:
                    return this.OnGetPlayerTokenRsp(pkt.getData<GetPlayerTokenRsp>());

                case 7:
                    return this.OnPlayerLoginRsp(pkt.getData<PlayerLoginRsp>());

                case 11:
                    return this.OnGetMainDataRsp(pkt.getData<GetMainDataRsp>());

                case 0x6f:
                    return this.OnGetConfigDataRsp(pkt.getData<GetConfigRsp>());

                case 13:
                    return this.OnGetScoinExchangeInfoRsp(pkt.getData<GetScoinExchangeInfoRsp>());

                case 0x11:
                    return this.OnGetStaminaExchangeInfoRsp(pkt.getData<GetStaminaExchangeInfoRsp>());

                case 0x30:
                    return this.OnGetAvatarTeamDataRsp(pkt.getData<GetAvatarTeamDataRsp>());

                case 0x35:
                    return this.OnGetSkillPointExchangeInfoRsp(pkt.getData<GetSkillPointExchangeInfoRsp>());

                case 0x51:
                    return this.OnGetOfflineFriendsPointNotify(pkt.getData<GetOfflineFriendsPointNotify>());

                case 120:
                    return this.OnBindAccountRsp(pkt.getData<BindAccountRsp>());

                case 0x7a:
                    return this.OnGetSignInRewardStatusRsp(pkt.getData<GetSignInRewardStatusRsp>());

                case 0x92:
                    return this.OnGetLastEndlessRewardDataRsp(pkt.getData<GetLastEndlessRewardDataRsp>());

                case 0xda:
                    return this.OnAntiCheatSDKReportRsp(pkt.getData<AntiCheatSDKReportRsp>());
            }
            return false;
        }

        private bool OnPlayerLoginRsp(PlayerLoginRsp rsp)
        {
            if (rsp.get_retcode() == null)
            {
                uint serverProcessedPacketId = !rsp.get_last_client_packet_idSpecified() ? 0 : rsp.get_last_client_packet_id();
                Singleton<NetworkManager>.Instance.SendPacketsOnLoginSuccess(false, serverProcessedPacketId);
                Singleton<NetworkManager>.Instance.alreadyLogin = true;
            }
            else if (rsp.get_retcode() == 4)
            {
                Singleton<NetworkManager>.Instance.ProcessWaitStopAnotherLogin();
            }
            else
            {
                Singleton<NetworkManager>.Instance.DisConnect();
                if ((MiscData.Config.BasicConfig.IsBlockUserWhenRepeatLogin && (rsp.get_retcode() == 2)) && Singleton<NetworkManager>.Instance.alreadyLogin)
                {
                    Singleton<NetworkManager>.Instance.SetRepeatLogin();
                }
            }
            return false;
        }

        private void SaveLastLoginAccountInfo()
        {
            Singleton<MiHoYoGameData>.Instance.GeneralLocalData.LastLoginUserId = this.playerData.userId;
            Singleton<AccountManager>.Instance.manager.SaveAccountToLocal();
        }

        public void SetBehaviourDone(string key)
        {
            if (!this.IsBehaviourDone(key))
            {
                Singleton<MiHoYoGameData>.Instance.LocalData.DoneBehaviourList.Add(key);
                Singleton<MiHoYoGameData>.Instance.Save();
            }
        }

        public PlayerDataItem playerData { get; private set; }
    }
}

