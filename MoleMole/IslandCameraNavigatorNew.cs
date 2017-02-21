namespace MoleMole
{
    using System;
    using UnityEngine;

    public class IslandCameraNavigatorNew : MonoBehaviour
    {
        private bool _bDir = true;
        private bool _bEnable;
        private bool _bIsLerpingOffset;
        private MonoIslandCameraSM _cameraSM;
        public float _gyroHorizonRadius = 10f;
        private Vector3 _gyroOffset = Vector3.zero;
        private Quaternion _gyroRot = Quaternion.identity;
        public float _gyroSpeed = 5f;
        public float _gyroVerticleRadius = 5f;
        public float _horizonAngleLimit = 30f;
        public float _lerpOffsetZoomInDuration = 0.5f;
        public float _lerpOffsetZoomOutDuration = 1f;
        private Vector3 _offsetWhenLerping;
        private Vector3 _orentation;
        private float _startTimeLerp;
        public float _verticleAngleLimit = 30f;
        private Vector3 _vLerpFrom;
        private Vector3 _vLerpTo;
        private float _xOffset;
        private float _yOffset;
        private Quaternion baseAttitude;
        public float ParallexBoundHardness = 0.5f;
        public float ParallexRange = 2.5f;
        public float ParallexSensitivity = 0.05f;
        private Quaternion referenceAttitude = Quaternion.identity;

        private Vector3 CheckAulerAngle(Vector3 vInput)
        {
            Vector3 vector = vInput;
            if (vector.x < 180f)
            {
                vector.x = Mathf.Clamp(vector.x, 0f, this._verticleAngleLimit);
            }
            else
            {
                vector.x = Mathf.Clamp(vector.x, 360f - this._verticleAngleLimit, 360f);
            }
            if (vector.y < 180f)
            {
                vector.y = Mathf.Clamp(vector.y, 0f, this._horizonAngleLimit);
                return vector;
            }
            vector.y = Mathf.Clamp(vector.y, 360f - this._horizonAngleLimit, 360f);
            return vector;
        }

        private static Quaternion ConvertRotation(Quaternion q)
        {
            return new Quaternion(q.x, q.y, -q.z, -q.w);
        }

        private void FixedUpdate()
        {
            if (this._bEnable)
            {
                Quaternion quaternion = this.referenceAttitude * ConvertRotation(Quaternion.Inverse(this.baseAttitude) * this.GetInputGyroAttitude());
                Quaternion quaternion2 = Quaternion.Inverse(this._gyroRot) * quaternion;
                this._orentation = this.CheckAulerAngle(quaternion2.eulerAngles);
                this._yOffset = Mathf.Sin(this._orentation.x * 0.01745329f) * this._gyroVerticleRadius;
                this._xOffset = Mathf.Sin(this._orentation.y * 0.01745329f) * this._gyroHorizonRadius;
                this._gyroOffset = (Vector3) (this._cameraSM.GetCameraBasePos().up * this._yOffset);
                this._gyroOffset += (Vector3) (this._cameraSM.GetCameraBasePos().right * this._xOffset);
                if (Quaternion.Angle(this.GetInputGyroAttitude(), this.baseAttitude) > this.ParallexRange)
                {
                    this.baseAttitude = Quaternion.Slerp(this.baseAttitude, this.GetInputGyroAttitude(), this.ParallexBoundHardness);
                }
            }
        }

        private Quaternion GetInputGyroAttitude()
        {
            return Input.gyro.attitude;
        }

        public Vector3 GetOffset()
        {
            if (!this._bIsLerpingOffset)
            {
                return this._gyroOffset;
            }
            float num = !this._bEnable ? this._lerpOffsetZoomInDuration : this._lerpOffsetZoomOutDuration;
            float num2 = (Time.time - this._startTimeLerp) / num;
            if (num2 >= 1f)
            {
                this._bIsLerpingOffset = false;
                if (this._bEnable)
                {
                    this._offsetWhenLerping = this._gyroOffset;
                }
                else
                {
                    this._offsetWhenLerping = this._vLerpTo;
                    this._gyroOffset = this._vLerpTo;
                }
            }
            else
            {
                float t = (2f * num2) - (num2 * num2);
                if (this._bEnable)
                {
                    this._offsetWhenLerping = Vector3.Lerp(Vector3.zero, this._gyroOffset, t);
                }
                else
                {
                    this._offsetWhenLerping = Vector3.Lerp(this._vLerpFrom, this._vLerpTo, t);
                }
            }
            return this._offsetWhenLerping;
        }

        public void SetEnable(bool enable)
        {
            this._bEnable = enable;
            this._bIsLerpingOffset = true;
            this._startTimeLerp = Time.time;
            if (!enable)
            {
                this._vLerpFrom = this._gyroOffset;
                this._vLerpTo = Vector3.zero;
            }
        }

        private void Simulate()
        {
            float num = !this._bDir ? -this._gyroSpeed : this._gyroSpeed;
            Vector3 vector = (Vector3) ((this._cameraSM.GetCameraBasePos().up * num) * Time.deltaTime);
            Vector3 vector2 = this._gyroOffset + vector;
            if (vector2.magnitude > this._gyroVerticleRadius)
            {
                this._bDir = !this._bDir;
            }
            else
            {
                this._gyroOffset += vector;
            }
        }

        private void Start()
        {
            Input.gyro.enabled = GraphicsSettingData.IsEnableGyroscope();
            this._bEnable = true;
            this._cameraSM = base.GetComponent<MonoIslandCameraSM>();
            this.baseAttitude = this.GetInputGyroAttitude();
            this.referenceAttitude = base.transform.rotation;
            this._gyroRot = base.transform.rotation;
        }
    }
}

