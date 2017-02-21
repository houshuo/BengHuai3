namespace MoleMole
{
    using MoleMole.Config;
    using System;
    using UnityEngine.Events;

    public class AvatarLevelUpDialogContext : BaseSequenceDialogContext
    {
        private uint _level;
        private uint _levelBefore;
        private CanvasTimer _timer;
        private const float TIMER_SPAN = 2f;

        public AvatarLevelUpDialogContext(uint level, uint levelBefore)
        {
            ContextPattern pattern = new ContextPattern {
                contextName = "AvatarLevelUpDialogContext",
                viewPrefabPath = "UI/Menus/Dialog/AvatarLevelUpDialog"
            };
            base.config = pattern;
            this._timer = Singleton<MainUIManager>.Instance.SceneCanvas.CreateTimer(2f, 0f);
            this._timer.timeUpCallback = new Action(this.Destroy);
            this._timer.StopRun();
            this._level = level;
            this._levelBefore = levelBefore;
        }

        protected override void BindViewCallbacks()
        {
            base.BindViewCallback(base.view.transform.Find("Btn").GetComponent<Button>(), new UnityAction(this.OnBGClick));
        }

        public override void Destroy()
        {
            this._timer.Destroy();
            base.Destroy();
        }

        private void OnBGClick()
        {
        }

        protected override bool SetupView()
        {
            this._timer.StartRun(false);
            base.view.transform.Find("Dialog/Content/Level").GetComponent<Text>().text = string.Format("Lv.{0}", this._level);
            base.view.transform.Find("Dialog/Content/LevelPast").GetComponent<Text>().text = string.Format("Lv.{0}", this._levelBefore);
            return false;
        }
    }
}

