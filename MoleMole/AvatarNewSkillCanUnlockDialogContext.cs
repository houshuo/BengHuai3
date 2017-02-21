namespace MoleMole
{
    using MoleMole.Config;
    using System;
    using UnityEngine.Events;

    public class AvatarNewSkillCanUnlockDialogContext : BaseSequenceDialogContext
    {
        private CanvasTimer _timer;
        public readonly string avatarFullName;
        private const string CAN_UNLOCK_NEW_SUB_SKILL_TEXT_MAP_ID = "Menu_Desc_CanUnlockNewSubSkill";
        public readonly bool isSubSkill;
        public readonly string skillName;
        private const float TIMER_SPAN = 2f;
        private const string UNLOCK_NEW_SKILL_TEXT_MAP_ID = "Menu_Desc_UnlockNewSkill";

        public AvatarNewSkillCanUnlockDialogContext(string avatarFullName, string skillName, bool isSubSkill)
        {
            ContextPattern pattern = new ContextPattern {
                contextName = "AvatarNewSkillCanUnlockDialogContext",
                viewPrefabPath = "UI/Menus/Dialog/AvatarNewSkillCanUnlockDialog"
            };
            base.config = pattern;
            this.avatarFullName = avatarFullName;
            this.skillName = skillName;
            this.isSubSkill = isSubSkill;
            this._timer = Singleton<MainUIManager>.Instance.SceneCanvas.CreateTimer(2f, 0f);
            this._timer.timeUpCallback = new Action(this.OnTimerUp);
            this._timer.StopRun();
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
            this.Destroy();
        }

        private void OnDialogBGGrowEnd()
        {
            base.view.transform.Find("Dialog/Content/NewSkill").GetComponent<Animation>().Play();
            base.view.transform.Find("Btn").gameObject.SetActive(true);
            this.Startimer();
        }

        private void OnTimerUp()
        {
            this.Destroy();
        }

        protected override bool SetupView()
        {
            base.view.transform.Find("Btn").gameObject.SetActive(false);
            base.view.transform.Find("Dialog/Content/NewSkill/AvatarName").GetComponent<Text>().text = this.avatarFullName;
            base.view.transform.Find("Dialog/Content/NewSkill/SkillName").GetComponent<Text>().text = this.skillName;
            base.view.transform.Find("Dialog/Content/NewSkill/CanUnlockLabel").GetComponent<Text>().text = LocalizationGeneralLogic.GetText(!this.isSubSkill ? "Menu_Desc_UnlockNewSkill" : "Menu_Desc_CanUnlockNewSubSkill", new object[0]);
            base.view.transform.Find("Dialog").GetComponent<MonoDialogHeightGrow>().PlayGrow(new Action(this.OnDialogBGGrowEnd));
            return false;
        }

        private void Startimer()
        {
            this._timer.StartRun(false);
        }
    }
}

