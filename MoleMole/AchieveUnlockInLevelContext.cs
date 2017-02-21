namespace MoleMole
{
    using MoleMole.Config;
    using System;
    using UnityEngine;

    public class AchieveUnlockInLevelContext : BaseWidgetContext
    {
        private MissionDataItem _missionData;
        private int _missionId;
        private CanvasTimer _timer;
        private const float TIMER_SPAN = 2.5f;

        public AchieveUnlockInLevelContext()
        {
            ContextPattern pattern = new ContextPattern {
                contextName = "AchieveUnlockInLevelDialogContext",
                viewPrefabPath = "UI/Menus/Dialog/AchieveUnlockInLevelDialog"
            };
            base.config = pattern;
            base.uiType = UIType.SuspendBar;
            this._timer = Singleton<MainUIManager>.Instance.SceneCanvas.CreateTimer(2.5f, 0f);
            this._timer.timeUpCallback = new Action(this.OnTimeout);
            this._timer.StopRun();
        }

        public override void Destroy()
        {
            this._timer.Destroy();
            base.Destroy();
        }

        public bool OnMissionUpdated(uint id)
        {
            if (this.SetupByMissionInfo((int) id))
            {
                base.view.SetActive(true);
                this._timer.StartRun(true);
            }
            return false;
        }

        public override bool OnNotify(Notify ntf)
        {
            return ((ntf.type == NotifyTypes.MissionUpdated) && this.OnMissionUpdated((uint) ntf.body));
        }

        private void OnTimeout()
        {
            base.view.SetActive(false);
            this._timer.StopRun();
        }

        private bool SetupByMissionInfo(int missionId)
        {
            this._missionId = missionId;
            this._missionData = Singleton<MissionModule>.Instance.GetMissionDataItem(this._missionId);
            if ((this._missionData == null) || (this._missionData.status != 3))
            {
                return false;
            }
            LinearMissionData data = LinearMissionDataReader.TryGetLinearMissionDataByKey(missionId);
            if ((data == null) || (data.IsAchievement == 0))
            {
                return false;
            }
            Transform transform = base.view.transform.Find("Dialog/AchieveName");
            if (transform != null)
            {
                transform.GetComponent<Text>().text = LocalizationGeneralLogic.GetText(this._missionData.metaData.title, new object[0]);
            }
            return true;
        }
    }
}

