namespace MoleMole
{
    using System;
    using UnityEngine;

    public class DetourElement
    {
        public Vector3[] corners;
        public float disReachCornerThreshold;
        public uint id;
        public bool isCompletePath;
        public float lastGetPathTime;
        public uint targetCornerIndex;
        public Vector3 targetPosition;
    }
}

