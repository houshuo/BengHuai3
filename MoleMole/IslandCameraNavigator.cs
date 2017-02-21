namespace MoleMole
{
    using System;
    using UnityEngine;

    [RequireComponent(typeof(Camera))]
    public class IslandCameraNavigator : MonoBehaviour
    {
        protected Quaternion baseAttitude;
        public float ParallexBoundHardness = 0.5f;
        public float ParallexRange = 2.5f;
        public float ParallexSensitivity = 0.05f;
        private Quaternion referanceRotation = Quaternion.identity;
        private Quaternion referenceAttitude = Quaternion.identity;

        private static Quaternion ConvertRotation(Quaternion q)
        {
            return new Quaternion(q.x, q.y, -q.z, -q.w);
        }

        private void FixedUpdate()
        {
            base.transform.rotation = Quaternion.Slerp(base.transform.rotation, this.referenceAttitude * ConvertRotation((Quaternion.Inverse(this.baseAttitude) * this.referanceRotation) * Input.gyro.attitude), this.ParallexSensitivity);
            Vector3 eulerAngles = base.transform.rotation.eulerAngles;
            eulerAngles.z = 0f;
            base.transform.rotation = Quaternion.Euler(eulerAngles);
            if (Quaternion.Angle(Input.gyro.attitude, this.baseAttitude) > this.ParallexRange)
            {
                this.baseAttitude = Quaternion.Slerp(this.baseAttitude, Input.gyro.attitude, this.ParallexBoundHardness);
            }
        }

        private void Start()
        {
            Input.gyro.enabled = GraphicsSettingData.IsEnableGyroscope();
            this.baseAttitude = Input.gyro.attitude;
            this.referenceAttitude = Quaternion.Euler(GameObject.Find("IslandCameraGroup").transform.eulerAngles.x, 0f, 0f);
        }
    }
}

