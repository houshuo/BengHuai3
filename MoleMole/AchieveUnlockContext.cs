namespace MoleMole
{
    using MoleMole.Config;
    using System;
    using UnityEngine;

    public class AchieveUnlockContext : BaseSequenceDialogContext
    {
        private SequenceAnimationManager _animationManager;
        private MissionDataItem _missionData;
        private CanvasTimer _timer;
        private const float dialogShowDelay = 0.8f;
        private const float TIMER_SPAN = 2f;

        public AchieveUnlockContext(int missionId)
        {
            ContextPattern pattern = new ContextPattern {
                contextName = "AchieveUnlockDialogContext",
                viewPrefabPath = "UI/Menus/Dialog/AchieveUnlockDialog"
            };
            base.config = pattern;
            this._missionData = Singleton<MissionModule>.Instance.GetMissionDataItem(missionId);
            this._timer = Singleton<MainUIManager>.Instance.SceneCanvas.CreateTimer(2f, 0f);
            this._timer.timeUpCallback = new Action(this.OnBGClick);
            this._timer.StopRun();
        }

        protected override void BindViewCallbacks()
        {
        }

        public override void Destroy()
        {
            this._timer.Destroy();
            base.Destroy();
        }

        private void OnBGClick()
        {
            this.Destroy();
        }

        protected override bool SetupView()
        {
            this._animationManager = new SequenceAnimationManager(new Action(this.StartTimer), null);
            Transform transform = base.view.transform.Find("Dialog/Content/AnimMoveIn3/AchieveName");
            if ((transform != null) && (this._missionData != null))
            {
                transform.GetComponent<Text>().text = LocalizationGeneralLogic.GetText(this._missionData.metaData.title, new object[0]);
            }
            this._animationManager.AddAllChildrenInTransform(base.view.transform.Find("Dialog/Content"));
            GameObject gameObject = base.view.transform.Find("Dialog/Content/AnimMoveIn1/AchieveIcon").gameObject;
            if (!string.IsNullOrEmpty(this._missionData.metaData.thumb) && (gameObject != null))
            {
                GameObject obj3 = Resources.Load<GameObject>(this._missionData.metaData.thumb);
                if (obj3 != null)
                {
                    gameObject.transform.Find("BG").GetComponent<Image>().sprite = obj3.transform.Find("BG").GetComponent<Image>().sprite;
                    gameObject.transform.Find("Icon").GetComponent<Image>().sprite = obj3.transform.Find("Icon").GetComponent<Image>().sprite;
                }
            }
            this._animationManager.StartPlay(0f, false);
            return false;
        }

        private void StartTimer()
        {
            this._timer.StartRun(true);
        }
    }
}

