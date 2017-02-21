namespace MoleMole
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using UnityEngine;

    [DisallowMultipleComponent]
    public class BaseStepController : MonoBehaviour
    {
        private Queue<Param> _leftStepParamQueue = new Queue<Param>();
        private BaseSingleStepParser _leftStepParser;
        private Queue<Param> _rightStepParamQueue = new Queue<Param>();
        private BaseSingleStepParser _rightStepParser;
        [Header("The threshold of angle speed to trigger event")]
        public float angleSpeedThreshold = 360f;
        [Header("When the distance to ground is less than this thickness, consider it to be in contact")]
        public float contactThickness = 0.1f;
        public float footLength;
        [Header("Foot width and height in world space")]
        public float footWidth;
        [Header("The ration of distance between foot anchor and front point to that of back point")]
        public float frontBackRatio = 1f;
        [Header("The threshold of horizontal speed to trigger movement")]
        public float horizontalSpeedThreshold = 5f;
        public Transform leftFootAnchor;
        [Header("The max number of state to hold in queue")]
        public int maxStateToHold = 10;
        public Action<Param> onStepEvent;
        public Transform rightFootAnchor;
        public float slideRatioThreshold = 5f;
        [Header("Span between two events")]
        public float spanBetweenEvents = 0.5f;
        [Header("The threshold of angle speed to trigger static")]
        public float staticAngleSpeedThreshold = 90f;
        [Header("The threshold of speed to trigger static")]
        public float staticSpeedThreshold = 1f;
        [Header("How much smooth the smoothed velocity be?"), Range(0f, 1f)]
        public float vectorSmoothFactor;
        [Header("The threshold of vertical speed to trigger movement")]
        public float verticleSpeedThreshold = 2f;

        protected virtual void Awake()
        {
            BaseMonoAvatar component = base.GetComponent<BaseMonoAvatar>();
            this.GetFootAnchorFromAvatar(ref this.leftFootAnchor, component, "LeftFoot");
            this.GetFootAnchorFromAvatar(ref this.rightFootAnchor, component, "RightFoot");
            this._leftStepParser = new BaseSingleStepParser(this, this.leftFootAnchor);
            this._rightStepParser = new BaseSingleStepParser(this, this.rightFootAnchor);
        }

        private void CleanParamsOutOfDate()
        {
            while (this._leftStepParamQueue.Count > this.maxStateToHold)
            {
                this._leftStepParamQueue.Dequeue();
            }
            while (this._rightStepParamQueue.Count > this.maxStateToHold)
            {
                this._rightStepParamQueue.Dequeue();
            }
        }

        protected virtual void FixedUpdate()
        {
            this._leftStepParser.Parse();
            this._rightStepParser.Parse();
            if (this._leftStepParser.hasUpdatedThisFrame)
            {
                this._leftStepParamQueue.Enqueue(this._leftStepParser.param);
            }
            if (this._rightStepParser.hasUpdatedThisFrame)
            {
                this._rightStepParamQueue.Enqueue(this._rightStepParser.param);
            }
        }

        private void GetFootAnchorFromAvatar(ref Transform anchor, BaseMonoAvatar avatar, string name)
        {
            if ((anchor == null) && (avatar != null))
            {
                anchor = avatar.GetAttachPoint(name);
            }
            if (anchor == null)
            {
                base.enabled = false;
            }
        }

        protected virtual void Update()
        {
            this.CleanParamsOutOfDate();
        }

        public Param currentLeftStepParam
        {
            get
            {
                return this._leftStepParser.param;
            }
        }

        public Param currentRightStepParam
        {
            get
            {
                return this._rightStepParser.param;
            }
        }

        public Param leftStepParam
        {
            get
            {
                return this._leftStepParser.param;
            }
        }

        public Queue<Param> leftStepParamQueue
        {
            get
            {
                return this._leftStepParamQueue;
            }
        }

        public Param rightStepParam
        {
            get
            {
                return this._rightStepParser.param;
            }
        }

        public Queue<Param> rightStepParamQueue
        {
            get
            {
                return this._rightStepParamQueue;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Param
        {
            public BaseStepController.Pattern pattern;
            public Vector3 position;
            public Vector3 velocityXZ;
            public Vector3 velocityXZCorrectSmooth;
            public Vector3 velocityInCam;
            public Vector3 velocityInCamSmooth;
            public Vector3 toeForwardXZ;
            public float acceleration;
            public float anglularSpeed;
            public float cameraPitchAngle;
            public override string ToString()
            {
                return this.pattern.ToString();
            }
        }

        public enum Pattern
        {
            Void,
            Static,
            Down,
            Up,
            Slide
        }
    }
}

