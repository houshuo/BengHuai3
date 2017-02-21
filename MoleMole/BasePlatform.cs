namespace MoleMole
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    public abstract class BasePlatform
    {
        protected Transform _platformTrans;

        public BasePlatform(MonoBasePerpStage stageOwner)
        {
            this.StageOwner = stageOwner;
        }

        public virtual void Core()
        {
        }

        public abstract Vector3 GetARandomPlace();

        public MonoBasePerpStage StageOwner { get; private set; }
    }
}

