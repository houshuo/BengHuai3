namespace MoleMole.Config
{
    using System;

    public class MonsterSuicideAttackMixinArgument : IMixinArgument
    {
        public float BeapInterval = 1f;
        public string OnTouchTriggerID;
        public float SuicideCountDown;
        public bool SuicideOnTouch;
    }
}

