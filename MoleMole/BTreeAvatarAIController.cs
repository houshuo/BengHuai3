namespace MoleMole
{
    using BehaviorDesigner.Runtime;
    using BehaviorDesigner.Runtime.Tasks;
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public class BTreeAvatarAIController : BaseAvatarAIController
    {
        private BaseMonoAvatar _avatar;
        private BehaviorDesigner.Runtime.BehaviorTree _btree;
        private bool _hasOverridenBehavior;
        public const string AUTO_BATTLE_AI_RES_PATH = "AI/Avatar/AvatarAutoBattleBehavior_Alt";
        public const string AUTO_MOVE_AI_RES_PATH = "AI/Avatar/AvatarAutoMoveBehavior";
        public ExternalBehaviorTree autoBattleBehavior;
        public ExternalBehaviorTree autoMoveBehvior;
        public const string SUPPORTER_AI_RES_PATH = "AI/Avatar/";
        public ExternalBehaviorTree supporterBehavior;

        public BTreeAvatarAIController(BaseMonoEntity avatar) : base(avatar)
        {
            this._avatar = (BaseMonoAvatar) avatar;
            string path = "AI/Avatar/AvatarAutoBattleBehavior_Alt";
            this.autoBattleBehavior = Miscs.LoadResource<ExternalBehaviorTree>(path, BundleType.RESOURCE_FILE);
            this.autoMoveBehvior = Miscs.LoadResource<ExternalBehaviorTree>("AI/Avatar/AvatarAutoMoveBehavior", BundleType.RESOURCE_FILE);
            string supporterAI = "AvatarSupporterBehavior_Alt";
            if (this._avatar.config.AIArguments.SupporterAI != string.Empty)
            {
                supporterAI = this._avatar.config.AIArguments.SupporterAI;
            }
            this.supporterBehavior = Miscs.LoadResource<ExternalBehaviorTree>("AI/Avatar/" + supporterAI, BundleType.RESOURCE_FILE);
            this._btree = this._avatar.gameObject.AddComponent<BehaviorDesigner.Runtime.BehaviorTree>();
            this._btree.RestartWhenComplete = true;
            this._btree.StartWhenEnabled = false;
            this._btree.DisableBehavior();
            this._btree.UpdateInterval = UpdateIntervalType.EveryFrame;
        }

        public void ChangeBehavior(string AIName)
        {
            ExternalBehaviorTree tree = Miscs.LoadResource<ExternalBehaviorTree>("AI/Avatar/" + AIName, BundleType.RESOURCE_FILE);
            this._btree.ExternalBehavior = tree;
            this._hasOverridenBehavior = true;
        }

        public void ChangeToAutoBattleBehavior()
        {
            this._btree.ExternalBehavior = this.autoBattleBehavior;
            this._btree.EnableBehavior();
        }

        public void ChangeToMoveBehavior(Vector3 pointPos)
        {
            this._btree.ExternalBehavior = this.autoMoveBehvior;
            this._hasOverridenBehavior = true;
            this._btree.SetVariableValue("TargetPosition", pointPos);
            this._btree.EnableBehavior();
        }

        public void ChangeToSupporterBehavior()
        {
            this._btree.ExternalBehavior = this.supporterBehavior;
            this._btree.EnableBehavior();
        }

        public override void Core()
        {
        }

        public void DisableBehavior()
        {
            this._btree.DisableBehavior();
        }

        private void ResetWhenRefesh()
        {
            if (this._avatar.IsActive())
            {
                this._avatar.OrderMove = false;
                this._avatar.ClearAttackTriggers();
            }
            base.controlData.FrameReset();
        }

        public override void SetActive(bool isActive)
        {
            if ((base.active != isActive) || (!isActive != (this._btree.ExecutionStatus == TaskStatus.Inactive)))
            {
                this.ResetWhenRefesh();
                base.SetActive(isActive);
                if (!this._hasOverridenBehavior)
                {
                    if (((Singleton<LevelManager>.Instance.levelActor.levelMode == LevelActor.Mode.Multi) && !Singleton<AvatarManager>.Instance.IsLocalAvatar(this._avatar.GetRuntimeID())) && Singleton<AvatarManager>.Instance.IsPlayerAvatar(this._avatar))
                    {
                        this._btree.ExternalBehavior = this.supporterBehavior;
                    }
                    else
                    {
                        this._btree.ExternalBehavior = this.supporterBehavior;
                    }
                }
                if (!isActive)
                {
                    this._btree.UpdateInterval = UpdateIntervalType.Manual;
                    base.avatar.SetMuteAnimRetarget(false);
                }
                else
                {
                    SharedFloat variable = (SharedFloat) this._btree.GetVariable("AttackDistance");
                    variable.Value = base.avatar.config.AIArguments.AttackDistance;
                    this._btree.EnableBehavior();
                    this._btree.UpdateInterval = UpdateIntervalType.EveryFrame;
                }
            }
        }

        public void SetBehaviorVariable(string variableName, object variableValue)
        {
            List<SharedVariable> allVariables = this._btree.GetAllVariables();
            for (int i = 0; i < allVariables.Count; i++)
            {
                if (allVariables[i].Name == variableName)
                {
                    allVariables[i].SetValue(variableValue);
                    break;
                }
            }
        }
    }
}

