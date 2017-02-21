namespace MoleMole
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public class RewardUIData
    {
        public string descTextID;
        public string iconPath;
        public string imagePath;
        public static string ITEM_ICON_PREFAB_PATH = "ItemIconPrefabPath";
        public static string ITEM_ICON_TEXT_ID = "ItemIconTextID";
        public int itemID;
        public int level;
        public string nameTextID;
        public ResourceType rewardType;
        public int value;
        public string valueLabelTextID;

        public RewardUIData(ResourceType type, int value, string textID, string descTextID, int itemID = 0, int level = 0)
        {
            this.rewardType = type;
            this.value = value;
            this.iconPath = UIUtil.GetResourceIconPath(type, itemID);
            this.valueLabelTextID = textID;
            this.descTextID = descTextID;
            this.itemID = itemID;
            this.level = level;
        }

        public static RewardUIData GetFriendPointData(int value = 0)
        {
            return new RewardUIData(ResourceType.FriendPoint, value, "RewardName_FriendPoint", "MaterialDetail_Fpoint", 0, 0) { nameTextID = "Menu_FriendPoint", imagePath = "SpriteOutput/MaterialFigures/IconFP" };
        }

        public static RewardUIData GetHcoinData(int value = 0)
        {
            return new RewardUIData(ResourceType.Hcoin, value, "RewardName_Hcoin", "MaterialDetail_Hcoin", 0, 0) { nameTextID = "Menu_Hcoin", imagePath = "SpriteOutput/MaterialFigures/IconHC" };
        }

        public Sprite GetIconSprite()
        {
            if (!string.IsNullOrEmpty(this.iconPath))
            {
                return Miscs.GetSpriteByPrefab(this.iconPath);
            }
            return null;
        }

        public Sprite GetImageSprite()
        {
            if (!string.IsNullOrEmpty(this.imagePath))
            {
                return Miscs.GetSpriteByPrefab(this.imagePath);
            }
            return null;
        }

        public static RewardUIData GetPlayerExpData(int value = 0)
        {
            return new RewardUIData(ResourceType.PlayerExp, value, "RewardName_Exp", "MaterialDetail_Exp", 0, 0) { nameTextID = "Menu_Exp", imagePath = "SpriteOutput/MaterialFigures/IconExp" };
        }

        public static RewardUIData GetScoinData(int value = 0)
        {
            return new RewardUIData(ResourceType.Scoin, value, "RewardName_Scoin", "MaterialDetail_Scoin", 0, 0) { nameTextID = "Menu_Scoin", imagePath = "SpriteOutput/MaterialFigures/IconSC" };
        }

        public static RewardUIData GetSkillPointData(int value = 0)
        {
            return new RewardUIData(ResourceType.SkillPoint, value, "RewardName_SkillPoint", string.Empty, 0, 0) { nameTextID = "Menu_SkillPtNum", imagePath = "SpriteOutput/MaterialFigures/IconST" };
        }

        public static RewardUIData GetStaminaData(int value = 0)
        {
            return new RewardUIData(ResourceType.Stamina, value, "RewardName_Stamina", "MaterialDetail_Stamina", 0, 0) { nameTextID = "Menu_Stamina", imagePath = "SpriteOutput/MaterialFigures/IconST" };
        }
    }
}

