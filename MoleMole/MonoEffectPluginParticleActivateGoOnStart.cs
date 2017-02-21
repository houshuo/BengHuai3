namespace MoleMole
{
    using System;
    using UnityEngine;

    public class MonoEffectPluginParticleActivateGoOnStart : BaseMonoEffectPlugin
    {
        private bool _hasStarted;
        [Header("Target Game Objects that needs to be set active on Start()")]
        public GameObject[] targetGOs;

        protected override void Awake()
        {
            for (int i = 0; i < this.targetGOs.Length; i++)
            {
                this.targetGOs[i].gameObject.SetActive(false);
            }
        }

        public override bool IsToBeRemove()
        {
            return false;
        }

        private void OnDisable()
        {
            for (int i = 0; i < this.targetGOs.Length; i++)
            {
                this.targetGOs[i].gameObject.SetActive(false);
            }
        }

        public override void SetDestroy()
        {
        }

        public override void Setup()
        {
            if (this._hasStarted)
            {
                for (int i = 0; i < this.targetGOs.Length; i++)
                {
                    this.targetGOs[i].gameObject.SetActive(true);
                }
            }
        }

        private void Start()
        {
            for (int i = 0; i < this.targetGOs.Length; i++)
            {
                this.targetGOs[i].gameObject.SetActive(true);
            }
            this._hasStarted = true;
        }
    }
}

