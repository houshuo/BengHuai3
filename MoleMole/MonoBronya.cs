namespace MoleMole
{
    using System;
    using UnityEngine;

    public class MonoBronya : BaseMonoAvatar
    {
        public override void Awake()
        {
            base.Awake();
        }

        public void SetMCVisible(bool visible)
        {
            foreach (Renderer renderer in base.renderers)
            {
                if (renderer.gameObject.name.StartsWith("MC"))
                {
                    renderer.enabled = visible;
                }
            }
        }
    }
}

