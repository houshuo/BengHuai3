namespace MoleMole
{
    using BehaviorDesigner.Runtime;
    using MoleMole.Config;
    using System;
    using System.Collections.Generic;
    using UniRx;
    using UnityEngine;

    public class BTreeMonsterAIController : BaseMonsterAIController
    {
        private string _AIName;
        private BehaviorDesigner.Runtime.BehaviorTree _btree;
        private List<Tuple<EntityTimer, ConfigDynamicArguments>> _delayedParameters;
        private bool _disableBehaviorWhenInit;
        private State _state;
        public Action<bool> OnAIActive;

        public BTreeMonsterAIController(BaseMonoMonster monster, string AIName, bool disableBehaviorWhenInit) : base(monster)
        {
            base._monster = monster;
            this._AIName = AIName;
            this._disableBehaviorWhenInit = disableBehaviorWhenInit;
            this.InitBTree(AIName);
            base.SetActive(true);
            this._state = State.WaitForStandby;
            monster.onAnimatorStateChanged = (Action<AnimatorStateInfo, AnimatorStateInfo>) Delegate.Combine(monster.onAnimatorStateChanged, new Action<AnimatorStateInfo, AnimatorStateInfo>(this.WaitFirstStandbyCallback));
        }

        public void AddBehaviorVariableFloat(string variableName, float variableValue)
        {
            List<SharedVariable> allVariables = this._btree.GetAllVariables();
            for (int i = 0; i < allVariables.Count; i++)
            {
                if (allVariables[i].Name == variableName)
                {
                    float num2 = ((float) allVariables[i].GetValue()) + variableValue;
                    allVariables[i].SetValue(num2);
                    break;
                }
            }
        }

        public void ChangeBehavior(string AIName)
        {
            ExternalBehaviorTree tree = Miscs.LoadResource<ExternalBehaviorTree>("AI/Monster/" + AIName, BundleType.RESOURCE_FILE);
            if (tree != null)
            {
                this._btree.ExternalBehavior = tree;
                this.EnableBehavior();
            }
        }

        public override void Core()
        {
            if (this._delayedParameters != null)
            {
                for (int i = 0; i < this._delayedParameters.Count; i++)
                {
                    if (this._delayedParameters[i] != null)
                    {
                        this._delayedParameters[i].Item1.Core(1f);
                        if (this._delayedParameters[i].Item1.isTimeUp)
                        {
                            AIData.SetSharedVariableCompat(this._btree, this._delayedParameters[i].Item2);
                            this._delayedParameters[i] = null;
                        }
                    }
                }
            }
        }

        public void DelayedSetParameter(float delay, ConfigDynamicArguments parameters)
        {
            if (this._delayedParameters == null)
            {
                this._delayedParameters = new List<Tuple<EntityTimer, ConfigDynamicArguments>>();
            }
            int num = this._delayedParameters.SeekAddPosition<Tuple<EntityTimer, ConfigDynamicArguments>>();
            this._delayedParameters[num] = Tuple.Create<EntityTimer, ConfigDynamicArguments>(new EntityTimer(delay, base._monster, true), parameters);
        }

        public void DisableBehavior()
        {
            this._btree.DisableBehavior();
        }

        public void EnableBehavior()
        {
            if (!BehaviorManager.instance.IsBehaviorEnabled(this._btree))
            {
                this._btree.EnableBehavior();
            }
            if (this.OnAIActive != null)
            {
                this.OnAIActive(true);
            }
        }

        private void InitBTree(string AIName)
        {
            this._btree = base._monster.gameObject.AddComponent<BehaviorDesigner.Runtime.BehaviorTree>();
            this._btree.RestartWhenComplete = true;
            this._btree.StartWhenEnabled = false;
            if (!this._disableBehaviorWhenInit)
            {
                this._btree.DisableBehaviorWhenMonoDisabled = false;
                this._btree.TryEnableBehaviorWhenMonoEnabled = false;
            }
            ExternalBehaviorTree tree = Miscs.LoadResource<ExternalBehaviorTree>("AI/Monster/" + AIName, BundleType.RESOURCE_FILE);
            this._btree.ExternalBehavior = tree;
            if (this._disableBehaviorWhenInit)
            {
                this._btree.CheckForSerialization();
                this._btree.DisableBehavior();
            }
            else
            {
                this._btree.UpdateInterval = UpdateIntervalType.Manual;
            }
        }

        public bool IsBehaviorRunning()
        {
            return (this._state == State.Running);
        }

        public void RefreshBehavior()
        {
            this.DisableBehavior();
            UnityEngine.Object.DestroyImmediate(this._btree);
            this._btree = null;
            this.InitBTree(this._AIName);
        }

        public override void SetActive(bool isActive)
        {
            if (base.active != isActive)
            {
                base.SetActive(isActive);
                if (this._state == State.Running)
                {
                    if (!isActive)
                    {
                        this._btree.UpdateInterval = UpdateIntervalType.Manual;
                    }
                    else
                    {
                        this.EnableBehavior();
                        this._btree.UpdateInterval = UpdateIntervalType.EveryFrame;
                    }
                }
            }
        }

        public void SetBehaviorVariable<T>(string variableName, T variableValue)
        {
            List<SharedVariable> allVariables = this._btree.GetAllVariables();
            for (int i = 0; i < allVariables.Count; i++)
            {
                if (allVariables[i].Name == variableName)
                {
                    ((SharedVariable<T>) allVariables[i]).SetValue(variableValue);
                    break;
                }
            }
        }

        private void WaitFirstStandbyCallback(AnimatorStateInfo fromState, AnimatorStateInfo toState)
        {
            if (toState.tagHash == MonsterData.MONSTER_IDLESUB_TAG)
            {
                this._state = State.Running;
                if (base.active)
                {
                    this.EnableBehavior();
                    this._btree.UpdateInterval = UpdateIntervalType.EveryFrame;
                }
                base._monster.onAnimatorStateChanged = (Action<AnimatorStateInfo, AnimatorStateInfo>) Delegate.Remove(base._monster.onAnimatorStateChanged, new Action<AnimatorStateInfo, AnimatorStateInfo>(this.WaitFirstStandbyCallback));
            }
        }

        public BehaviorDesigner.Runtime.BehaviorTree btree
        {
            get
            {
                return this._btree;
            }
        }

        private enum State
        {
            WaitForStandby,
            Running
        }
    }
}

