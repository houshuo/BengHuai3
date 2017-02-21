namespace MoleMole
{
    using System;
    using UnityEngine;

    public class TestNagivation : MonoBehaviour
    {
        public Transform goal;

        private void Start()
        {
            base.GetComponent<NavMeshAgent>().destination = this.goal.position;
        }

        private void Update()
        {
        }
    }
}

