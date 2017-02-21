namespace MoleMole.Config
{
    using System;

    public class ConfigAvatarStateMachinePattern : ConfigEntityStateMachinePattern
    {
        public float IdleCD;
        [NonSerialized]
        public int SwitchInAnimatorStateHash;
        public string SwitchInAnimatorStateName = "SwitchInFast";
        [NonSerialized]
        public int SwitchOutAnimatorStateHash;
        public string SwitchOutAnimatorStateName = "SwitchOut";
    }
}

