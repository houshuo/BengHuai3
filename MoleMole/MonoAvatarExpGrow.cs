namespace MoleMole
{
    using System;
    using UnityEngine;

    public class MonoAvatarExpGrow : MonoBehaviour
    {
        public MonoMaskSliderGrow[] sliders;

        public void PlayAvatarExpSliderGrow()
        {
            LevelScoreManager instance = Singleton<LevelScoreManager>.Instance;
            for (int i = 0; i < instance.memberList.Count; i++)
            {
                AvatarDataItem avatarData = instance.memberList[i];
                AvatarDataItem avatarByID = Singleton<AvatarModule>.Instance.GetAvatarByID(avatarData.avatarID);
                MonoMaskSliderGrow grow = this.sliders[i];
                int level = avatarData.level;
                int exp = avatarData.exp;
                int toLevel = avatarByID.level;
                int num5 = avatarByID.exp;
                grow.Play((float) exp, (float) num5, UIUtil.GetAvatarMaxExpList(avatarData, level, toLevel), new Action<Transform>(this.ShowAvatarLevelUpHint), new Action<Transform>(this.ShowCanUnlockSkillDialog));
            }
        }

        private void ShowAvatarLevelUpHint(Transform sliderTrans)
        {
            sliderTrans.Find("LevelUpHint").GetComponent<Animation>().Play();
        }

        private void ShowCanUnlockSkillDialog(Transform sliderTrans)
        {
            Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.AvatarLevelUp, null));
        }
    }
}

