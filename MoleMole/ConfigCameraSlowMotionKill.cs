namespace MoleMole
{
    using System;
    using UnityEngine;

    public class ConfigCameraSlowMotionKill : ScriptableObject
    {
        public float CameraDistanceThreshold;
        public AnimationCurve CameraRadiusOffsetCurve;
        public AnimationCurve CameraSpeedCurve;
        public Vector4 DistanceAttenuationFactorsForElevationOffset;
        public Vector4 DistanceAttenuationFactorsForRotateAngle;
        public float DistanceThreshold;
        public float Duration;
        public float MaxCameraElevationOffset;
        public float MaxCameraRadiusOffset;
        public float MaxCameraRotateAngle;
        public float MinCameraElevationOffset;
        public float MinCameraRadiusOffset;
        public float MinCameraRotateAngle;
        public AnimationCurve TimeScaleCurve;
    }
}

