namespace MoleMole
{
    using System;
    using UnityEngine;

    public class MonoStageObject : MonoBehaviour
    {
        public void TriggerDestroy()
        {
            UnityEngine.Object.Destroy(base.gameObject);
        }

        public void TriggerLevelState(string state)
        {
        }

        public void TriggerStageTransitFinish()
        {
            base.gameObject.SetActive(false);
        }
    }
}

