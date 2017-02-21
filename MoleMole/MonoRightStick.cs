namespace MoleMole
{
    using System;
    using UnityEngine;

    public class MonoRightStick : MonoBehaviour
    {
        public bool isRotating;
        private const float ROTATION_DELTA_X = 5f;
        private const float ROTATION_DELTA_Y = 3f;

        private void Start()
        {
            this.isRotating = false;
        }
    }
}

