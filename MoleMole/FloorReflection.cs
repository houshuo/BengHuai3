namespace MoleMole
{
    using System;
    using UnityEngine;

    [DisallowMultipleComponent]
    public class FloorReflection : ReflectionBase
    {
        public float floorHeight;

        protected override void Awake()
        {
            base.Awake();
            base._reflectionPlanePosition = (Vector3) (Vector3.up * this.floorHeight);
        }

        protected override void Update()
        {
            base._reflectionPlanePosition = (Vector3) (Vector3.up * this.floorHeight);
        }
    }
}

