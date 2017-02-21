namespace MoleMole
{
    using proto;
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public class MonoLevelPanel : MonoBehaviour
    {
        public GameObject level;

        public void SetupView(List<LevelDataItem> levels, LevelBtnClickCallBack OnLevelClick, Dictionary<LevelDataItem, Transform> levelTransDict, WeekDayActivityDataItem activityData = null, int totalFinishChallengeNum = 0)
        {
            if ((activityData == null) || (activityData.GetStatus() == ActivityDataItemBase.Status.InProgress))
            {
                foreach (LevelDataItem item in levels)
                {
                    Transform parent = base.transform.Find(item.SectionId.ToString());
                    Transform transform = UnityEngine.Object.Instantiate<GameObject>(this.level).transform;
                    transform.SetParent(parent, false);
                    int num = 1;
                    StageType levelType = item.LevelType;
                    if (((levelType == 2) || (levelType == 3)) && (activityData != null))
                    {
                        num = activityData.maxEnterTimes - activityData.enterTimes;
                    }
                    transform.GetComponent<MonoLevelView>().SetupView(item, num > 0, OnLevelClick, totalFinishChallengeNum);
                    if (item.LevelType == 1)
                    {
                        levelTransDict.Add(item, transform);
                    }
                }
            }
            else
            {
                base.transform.Find("Icon/Desc").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(MiscData.Config.ActivityStatusImgPath[(int) activityData.GetStatus()]);
            }
        }
    }
}

