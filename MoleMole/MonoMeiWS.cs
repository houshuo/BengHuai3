namespace MoleMole
{
    using System;
    using UnityEngine;

    public class MonoMeiWS : MonoMei
    {
        private string _attackRunStopSkillID = "ATK02_02_RUN";
        private string _attackRunTriggerID = "TriggerRunToAtk";
        private bool _isInRun;

        protected override void OnCollisionEnter(Collision collision)
        {
            base.OnCollisionEnter(collision);
            if ((InLevelData.MONSTER_LAYER == collision.gameObject.layer) && this._isInRun)
            {
                this.SetTrigger(this._attackRunTriggerID);
                this._isInRun = false;
            }
        }

        protected override void PostInit()
        {
            base.PostInit();
            base.onCurrentSkillIDChanged = (Action<string, string>) Delegate.Combine(base.onCurrentSkillIDChanged, new Action<string, string>(this.SkillIDChangedCallback));
        }

        private void SkillIDChangedCallback(string from, string to)
        {
            if (to == this._attackRunStopSkillID)
            {
                if (CollisionDetectPattern.CircleCollisionDetectBySphere(base.RootNodePosition, 0f, base.FaceDirection, base.config.CommonArguments.CollisionRadius * 3f, ((int) 1) << InLevelData.MONSTER_LAYER).Count > 0)
                {
                    this.SetTrigger(this._attackRunTriggerID);
                    this._isInRun = false;
                }
                else
                {
                    this._isInRun = true;
                }
            }
            else
            {
                this.ResetTrigger(this._attackRunTriggerID);
                this._isInRun = false;
            }
        }
    }
}

