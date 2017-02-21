namespace MoleMole
{
    using BehaviorDesigner.Runtime.Tasks;
    using System;
    using UnityEngine;

    [TaskCategory("Avatar")]
    public class DebugAvatarButtonHold : BaseAvatarAction
    {
        private bool _countBegin;
        private MonoSkillButton _skillButton;
        private float _timer;
        public string EndSkillID;
        public float HoldTime;
        public string SkillButtonID = "ATK";
        public string StartSkillID;

        public override void OnStart()
        {
            this._skillButton = Singleton<MainUIManager>.Instance.GetInLevelUICanvas().mainPageContext.GetSkillButtonBySkillID(this.SkillButtonID);
            this._skillButton.OnPointerDown(null);
            this._timer = this.HoldTime;
        }

        public override TaskStatus OnUpdate()
        {
            if (base._avatar.CurrentSkillID == this.StartSkillID)
            {
                this._countBegin = true;
            }
            if (this._countBegin)
            {
                this._timer -= Time.deltaTime;
                if (this._timer < 0f)
                {
                    this._skillButton.OnPointerUp(null);
                    this._countBegin = false;
                }
            }
            if (base._avatar.CurrentSkillID == this.EndSkillID)
            {
                return TaskStatus.Success;
            }
            return TaskStatus.Running;
        }
    }
}

