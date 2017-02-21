namespace MoleMole
{
    using MoleMole.Config;
    using System;
    using UnityEngine;

    public class AvatarIntroPageContext : BasePageContext
    {
        private MonoAvatarRotatePanel _avatarRotatePanel;
        private const string AVATAR_MODEL_PARA_KEY = "LvUpTab";
        public readonly AvatarDataItem avatarData;

        public AvatarIntroPageContext(AvatarDataItem avatarData)
        {
            ContextPattern pattern = new ContextPattern {
                contextName = "AvatarIntroPageContext",
                viewPrefabPath = "UI/Menus/Page/AvatarIntroPage"
            };
            base.config = pattern;
            base.showSpaceShip = true;
            this.avatarData = avatarData;
        }

        private void SetupAvatarInfo()
        {
            base.view.transform.Find("InfoPanel/Attributes").Find("BasicStatus").GetComponent<MonoAttributeDisplay>().SetupView(this.avatarData);
            Transform transform2 = base.view.transform.Find("InfoPanel/Skills/Info");
            this.SetupSkillIcon(this.avatarData.GetUltraSkill(), transform2.Find("UltraSkill"));
            this.SetupSkillIcon(this.avatarData.GetLeaderSkill(), transform2.Find("LeaderSkill"));
        }

        private void SetupAvatarProfile()
        {
            Transform transform = base.view.transform.Find("AvatarDetailProfile");
            this.SetupClassName(transform.Find("ClassName"), this.avatarData);
            transform.Find("AvatarStar").GetComponent<MonoAvatarStar>().SetupView(this.avatarData.star);
            transform.Find("Desc").GetComponent<Text>().text = this.avatarData.Desc;
        }

        private void SetupClassName(Transform parent, AvatarDataItem avatarSelected)
        {
            parent.Find("FirstName").GetComponent<Text>().text = avatarSelected.ClassFirstName;
            parent.Find("FirstName/EnText").GetComponent<Text>().text = avatarSelected.ClassEnFirstName;
            parent.Find("LastName").GetComponent<Text>().text = avatarSelected.ClassLastName;
            parent.Find("LastName/EnText").GetComponent<Text>().text = avatarSelected.ClassEnLastName;
        }

        private void SetupSkillIcon(AvatarSkillDataItem skillData, Transform skillTrans)
        {
            skillTrans.Find("Info/NameText").GetComponent<Text>().text = skillData.SkillName;
            skillTrans.Find("Info/Desc").GetComponent<Text>().text = skillData.SkillInfo;
            Sprite spriteByPrefab = Miscs.GetSpriteByPrefab(skillData.IconPath);
            skillTrans.Find("SkillIcon/Icon/Image").GetComponent<Image>().sprite = spriteByPrefab;
            string key = "SkillBtnBlue";
            skillTrans.Find("SkillIcon/Icon/Image").GetComponent<Image>().material = null;
            skillTrans.Find("SkillIcon/Icon/BG").GetComponent<Image>().color = MiscData.GetColor(key);
        }

        protected override bool SetupView()
        {
            base.view.transform.Find("AvatarFigurePanel/Image").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(Singleton<StorageModule>.Instance.GetDummyStorageDataItem(AvatarMetaDataReaderExtend.GetAvatarIDsByKey(this.avatarData.avatarID).avatarCardID, 1).GetImagePath());
            this.SetupAvatarProfile();
            this.SetupAvatarInfo();
            if (base.view.GetComponent<MonoFadeInAnimManager>() != null)
            {
                base.view.GetComponent<MonoFadeInAnimManager>().Play("PageFadeIn", false, null);
            }
            return false;
        }
    }
}

