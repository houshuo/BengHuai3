namespace MoleMole
{
    using System;
    using UnityEngine;

    public class MonoKianaWithWing : MonoKiana_C5
    {
        public Renderer wing;

        public override void Awake()
        {
            base.Awake();
        }

        private void EnableWing()
        {
            this.wing.enabled = true;
        }

        public override void Revive(Vector3 revivePosition)
        {
            base.Revive(revivePosition);
            this.EnableWing();
        }

        [AnimationCallback]
        private void TriggerEnableWing(int enable)
        {
            this.wing.enabled = enable != 0;
        }
    }
}

