namespace MoleMole
{
    using System;

    public sealed class MonoInLevelUICamera : BaseMonoCamera
    {
        public void Init(uint runtimeID)
        {
            base.Init(2, runtimeID);
        }

        public override void Update()
        {
            base.Update();
        }
    }
}

