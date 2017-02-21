namespace MoleMole
{
    using System;
    using UnityEngine;

    public class BaseSingleStepParser
    {
        private BaseStepController.Param _currentParam;
        private float _lastEventTimer;
        private Vector3 _lastForward;
        private Vector3 _lastPosition;
        private Transform anchor;
        private BaseStepController controler;
        public bool hasUpdatedThisFrame;

        public BaseSingleStepParser(BaseStepController controler, Transform anchor)
        {
            this.controler = controler;
            this.anchor = anchor;
            this._currentParam = new BaseStepController.Param();
            this._lastPosition = anchor.position;
            this._lastForward = anchor.forward;
            this._lastEventTimer = 0f;
        }

        public virtual BaseStepController.Param Parse()
        {
            this.hasUpdatedThisFrame = false;
            this._lastEventTimer += Time.deltaTime;
            Matrix4x4 worldToCameraMatrix = Camera.main.worldToCameraMatrix;
            Matrix4x4 matrixx2 = Camera.main.worldToCameraMatrix;
            Vector3 zero = Vector3.zero;
            Vector3 v = Vector3.zero;
            Vector3 rhs = Vector3.zero;
            if (Time.deltaTime > 0f)
            {
                zero = (Vector3) ((this.anchor.position - this._lastPosition) / Time.deltaTime);
                Vector3 vector4 = worldToCameraMatrix.MultiplyPoint(this.anchor.position);
                Vector3 vector5 = worldToCameraMatrix.MultiplyPoint(this._lastPosition);
                v = (Vector3) ((vector4 - vector5) / Time.deltaTime);
                rhs = matrixx2.MultiplyVector(v);
                rhs.y = 0f;
            }
            if (this._lastEventTimer > this.controler.spanBetweenEvents)
            {
                BaseStepController.Param param = new BaseStepController.Param();
                Vector3 vector6 = zero;
                vector6.y = 0f;
                float num = 0f;
                if (Time.deltaTime > 0f)
                {
                    num = Vector3.Angle(this._lastForward, this.anchor.forward) / Time.deltaTime;
                }
                if (this.anchor.position.y < this.controler.contactThickness)
                {
                    if ((this._currentParam.pattern == BaseStepController.Pattern.Static) || (this._currentParam.pattern == BaseStepController.Pattern.Void))
                    {
                        if (vector6.magnitude > this.controler.horizontalSpeedThreshold)
                        {
                            param.pattern = BaseStepController.Pattern.Slide;
                        }
                        else if (zero.y > this.controler.verticleSpeedThreshold)
                        {
                            param.pattern = BaseStepController.Pattern.Up;
                        }
                        else
                        {
                            param.pattern = BaseStepController.Pattern.Static;
                        }
                    }
                    else if (this._currentParam.pattern == BaseStepController.Pattern.Slide)
                    {
                        if (vector6.magnitude < this.controler.staticSpeedThreshold)
                        {
                            param.pattern = BaseStepController.Pattern.Static;
                        }
                        else
                        {
                            param.pattern = BaseStepController.Pattern.Slide;
                        }
                    }
                    else if (this._currentParam.pattern == BaseStepController.Pattern.Up)
                    {
                        if (zero.y < this.controler.staticSpeedThreshold)
                        {
                            param.pattern = BaseStepController.Pattern.Down;
                        }
                        else
                        {
                            param.pattern = BaseStepController.Pattern.Up;
                        }
                    }
                    else if (this._currentParam.pattern == BaseStepController.Pattern.Down)
                    {
                        if (zero.y > this.controler.verticleSpeedThreshold)
                        {
                            param.pattern = BaseStepController.Pattern.Up;
                        }
                        else if (zero.y > -this.controler.staticSpeedThreshold)
                        {
                            param.pattern = BaseStepController.Pattern.Static;
                        }
                        else
                        {
                            param.pattern = BaseStepController.Pattern.Down;
                        }
                    }
                }
                else if ((this._currentParam.pattern == BaseStepController.Pattern.Static) || (this._currentParam.pattern == BaseStepController.Pattern.Void))
                {
                    if (zero.y > this.controler.verticleSpeedThreshold)
                    {
                        param.pattern = BaseStepController.Pattern.Up;
                    }
                    else
                    {
                        param.pattern = BaseStepController.Pattern.Static;
                    }
                }
                else if (this._currentParam.pattern == BaseStepController.Pattern.Slide)
                {
                    param.pattern = BaseStepController.Pattern.Up;
                }
                else if (this._currentParam.pattern == BaseStepController.Pattern.Up)
                {
                    if (zero.y < -this.controler.verticleSpeedThreshold)
                    {
                        param.pattern = BaseStepController.Pattern.Down;
                    }
                    else
                    {
                        param.pattern = BaseStepController.Pattern.Up;
                    }
                }
                else if (this._currentParam.pattern == BaseStepController.Pattern.Down)
                {
                    if (zero.y > this.controler.verticleSpeedThreshold)
                    {
                        param.pattern = BaseStepController.Pattern.Up;
                    }
                    else
                    {
                        param.pattern = BaseStepController.Pattern.Down;
                    }
                }
                if (this._currentParam.pattern != param.pattern)
                {
                    this.hasUpdatedThisFrame = true;
                }
                param.position = this.anchor.position;
                param.velocityXZ = vector6;
                param.velocityInCam = v;
                float magnitude = this._currentParam.velocityXZCorrectSmooth.magnitude;
                if (Vector3.Dot(this._currentParam.velocityXZCorrectSmooth, rhs) > 0f)
                {
                    float num3 = Mathf.Min(this.controler.vectorSmoothFactor, (magnitude * 2f) / (magnitude + rhs.magnitude));
                    param.velocityXZCorrectSmooth = (Vector3) ((this._currentParam.velocityXZCorrectSmooth * num3) + (rhs * (1f - num3)));
                }
                else
                {
                    param.velocityXZCorrectSmooth = rhs;
                }
                float num4 = this._currentParam.velocityInCamSmooth.magnitude;
                float num5 = Mathf.Min(this.controler.vectorSmoothFactor, (num4 * 2f) / (num4 + v.magnitude));
                param.velocityInCamSmooth = (Vector3) ((this._currentParam.velocityInCamSmooth * num5) + (v * (1f - num5)));
                param.anglularSpeed = num;
                param.toeForwardXZ = -this.anchor.right;
                param.toeForwardXZ.y = 0f;
                this._currentParam = param;
            }
            this._lastPosition = this.anchor.position;
            this._lastForward = this.anchor.forward;
            return this._currentParam;
        }

        public BaseStepController.Param param
        {
            get
            {
                return this._currentParam;
            }
        }
    }
}

