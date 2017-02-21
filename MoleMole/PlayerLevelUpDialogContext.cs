namespace MoleMole
{
    using MoleMole.Config;
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Events;

    public class PlayerLevelUpDialogContext : BaseSequenceDialogContext
    {
        private SequenceAnimationManager _animationManager;
        private AvatarCardDataItem _avatarData;
        private int _levelBefore_no_scoremanager;
        private CanvasTimer _timer;
        private const string NEW_FEATURE_PREFAB_PATH = "UI/Menus/Widget/Map/NewFeature";
        private const float TIMER_SPAN = 2f;

        public PlayerLevelUpDialogContext()
        {
            ContextPattern pattern = new ContextPattern {
                contextName = "PlayerLevelUpDialogContext",
                viewPrefabPath = "UI/Menus/Dialog/PlayerLevelUpDialog"
            };
            base.config = pattern;
            if (this._timer != null)
            {
                this._timer.Destroy();
            }
            this._timer = Singleton<MainUIManager>.Instance.SceneCanvas.CreateTimer(2f, 0f);
            this._timer.timeUpCallback = new Action(this.OnBGClick);
            this._timer.StopRun();
            this._avatarData = null;
        }

        protected override void BindViewCallbacks()
        {
            base.BindViewCallback(base.view.transform.Find("Btn").GetComponent<Button>(), new UnityAction(this.OnBGClick));
        }

        public override void Destroy()
        {
            if (this._avatarData != null)
            {
                Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.MissionRewardAvatarGot, this._avatarData));
            }
            this._timer.Destroy();
            base.Destroy();
        }

        private void OnBGClick()
        {
            this.Destroy();
        }

        private void OnDialogBGGrowEnd()
        {
            this._animationManager.StartPlay(0f, true);
            base.view.transform.Find("Btn").gameObject.SetActive(true);
        }

        public void SetLevelBeforeNoScoreManager(int level)
        {
            this._levelBefore_no_scoremanager = level;
        }

        public void SetNotifyWhenDestroy(AvatarCardDataItem avatarData)
        {
            this._avatarData = avatarData;
        }

        protected override bool SetupView()
        {
            base.view.transform.Find("Btn").gameObject.SetActive(false);
            this._animationManager = new SequenceAnimationManager(new Action(this.StartTimer), null);
            LevelScoreManager instance = Singleton<LevelScoreManager>.Instance;
            int level = (instance == null) ? this._levelBefore_no_scoremanager : instance.playerLevelBefore;
            int teamLevel = Singleton<PlayerModule>.Instance.playerData.teamLevel;
            PlayerLevelMetaData data = PlayerLevelMetaDataReader.TryGetPlayerLevelMetaDataByKey(level);
            SuperDebug.VeryImportantAssert(data != null, string.Format("Cannot get player level data for player level:{0}", level));
            if (data == null)
            {
                data = PlayerLevelMetaDataReader.TryGetPlayerLevelMetaDataByKey(1);
            }
            base.view.transform.Find("Dialog/Content/LevelInfo/LvBefore/Lv").GetComponent<Text>().text = level.ToString();
            base.view.transform.Find("Dialog/Content/LevelInfo/LvAfter/Lv").GetComponent<Text>().text = teamLevel.ToString();
            int maxStamina = Singleton<PlayerModule>.Instance.playerData.MaxStamina;
            int stamina = data.stamina;
            Transform transform = base.view.transform.Find("Dialog/Content/MaxStamina");
            if (maxStamina > stamina)
            {
                transform.gameObject.SetActive(true);
                transform.Find("Num").GetComponent<Text>().text = maxStamina.ToString();
                transform.Find("AddNum").GetComponent<Text>().text = "+" + ((maxStamina - stamina)).ToString();
            }
            else
            {
                transform.gameObject.SetActive(false);
            }
            int avatarLevelLimit = Singleton<PlayerModule>.Instance.playerData.AvatarLevelLimit;
            int num6 = data.avatarLevelLimit;
            Transform transform2 = base.view.transform.Find("Dialog/Content/MaxAvatarLevel");
            if (avatarLevelLimit > num6)
            {
                transform2.gameObject.SetActive(true);
                transform2.Find("Num").GetComponent<Text>().text = avatarLevelLimit.ToString();
                transform2.Find("AddNum").GetComponent<Text>().text = "+" + ((avatarLevelLimit - num6)).ToString();
            }
            else
            {
                transform2.gameObject.SetActive(false);
            }
            RectTransform component = base.view.transform.Find("Dialog").GetComponent<RectTransform>();
            List<string> newFeatures = MiscData.GetNewFeatures(level, teamLevel);
            for (int i = 0; i < newFeatures.Count; i++)
            {
                string text = LocalizationGeneralLogic.GetText(newFeatures[i], new object[0]);
                GameObject obj2 = UnityEngine.Object.Instantiate<GameObject>(Miscs.LoadResource<GameObject>("UI/Menus/Widget/Map/NewFeature", BundleType.RESOURCE_FILE));
                obj2.transform.SetParent(component.Find("Content"), false);
                obj2.transform.Find("FeatureName").GetComponent<Text>().text = text;
            }
            this._animationManager.AddAllChildrenInTransform(base.view.transform.Find("Dialog/Content"));
            base.view.transform.Find("Dialog").GetComponent<MonoDialogHeightGrow>().PlayGrow(new Action(this.OnDialogBGGrowEnd));
            return false;
        }

        private void StartTimer()
        {
            this._timer.StartRun(false);
        }
    }
}

