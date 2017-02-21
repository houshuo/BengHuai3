namespace MoleMole
{
    using System;
    using UnityEngine;

    public class MonoTriggerUnactive : MonoBehaviour
    {
        public void TriggerUnactive()
        {
            base.gameObject.SetActive(false);
        }
    }
}

