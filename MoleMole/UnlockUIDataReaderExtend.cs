namespace MoleMole
{
    using System;

    public class UnlockUIDataReaderExtend
    {
        public static void LoadFromFileAndBuildMap()
        {
            UnlockUIDataReader.LoadFromFile();
        }

        public static bool UnLockByMission(int id)
        {
            UnlockUIData unlockUIDataByKey = UnlockUIDataReader.GetUnlockUIDataByKey(id);
            if (unlockUIDataByKey.unlockByMission > 0)
            {
                MissionDataItem missionDataItem = Singleton<MissionModule>.Instance.GetMissionDataItem(unlockUIDataByKey.unlockByMission);
                if (missionDataItem == null)
                {
                    return false;
                }
                if ((((missionDataItem.status != 2) || (unlockUIDataByKey.OnDoing <= 0)) && ((missionDataItem.status != 3) || (unlockUIDataByKey.OnFinish <= 0))) && ((missionDataItem.status != 5) || (unlockUIDataByKey.OnClose <= 0)))
                {
                    return false;
                }
            }
            return true;
        }

        public static bool UnlockByTutorial(int id)
        {
            UnlockUIData unlockUIDataByKey = UnlockUIDataReader.GetUnlockUIDataByKey(id);
            return ((unlockUIDataByKey.unlockByTutorial <= 0) || Singleton<TutorialModule>.Instance.IsTutorialIDFinish(unlockUIDataByKey.unlockByTutorial));
        }
    }
}

