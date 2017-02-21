namespace MoleMole
{
    using System;
    using UnityEngine;

    public sealed class Mono_UL_020 : BaseMonoUlysses
    {
        protected override void Update()
        {
            base.Update();
            base.hitbox.transform.eulerAngles = Vector3.zero;
        }
    }
}

