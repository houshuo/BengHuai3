namespace MoleMole
{
    using System;
    using UnityEngine;
    using UnityEngine.EventSystems;

    [RequireComponent(typeof(Camera))]
    public class MainMenuCameraNavigater : MonoBehaviour
    {
        private float _time;
        public float amplitudeX = 1f;
        public float amplitudeY = 1f;
        public Vector2 angleBounds = new Vector2(5f, 5f);
        protected Quaternion baseAttitude;
        public float Distance = 4f;
        private Vector2 euler;
        public AnimationCurve GyroSensitivityCurve;
        public float headSpeed = 250f;
        private const float lowPassFilterFactor = 0.2f;
        private Vector2 origEuler;
        private Quaternion origRot;
        public float ParallexBoundHardness = 0.5f;
        public float ParallexRange = 2.5f;
        public float ParallexSensitivity = 0.05f;
        public Rect paramInputBounds = new Rect(0f, 0f, 1f, 1f);
        public Transform pilot1;
        public Transform pilot2;
        public Transform pilot3;
        private Vector3 pilotOffset1 = new Vector3(0f, 0f, 0f);
        private Vector3 pilotOffset2 = new Vector3(0f, 0f, 0f);
        private Vector3 pilotOffset3 = new Vector3(0f, 0f, 0f);
        private Vector3 pilotPosition1 = new Vector3(0f, 0f, 0f);
        private Vector3 pilotPosition2 = new Vector3(0f, 0f, 0f);
        private Vector3 pilotPosition3 = new Vector3(0f, 0f, 0f);
        public float pilotShakeSpeed = 0.02f;
        public AnimationCurve PilotShakeX = AnimationCurve.Linear(0f, 0f, 1f, 1f);
        public AnimationCurve PilotShakeY = AnimationCurve.Linear(0f, 0f, 1f, 1f);
        public float pitchSpeed = 120f;
        private Quaternion referanceRotation = Quaternion.identity;
        private Quaternion referenceAttitude = Quaternion.Euler(2f, 180f, 0f);
        private const float RESET_TIME = 0.2f;
        private float resetTimer;
        public float restoreSmoothing = 0.2f;
        public float rotateSmoothing = 0.5f;
        public State state;
        public Transform target;
        private Quaternion targetRot;

        private float ClampAngle(float angle, float min, float max)
        {
            if (angle < -360f)
            {
                angle += 360f;
            }
            if (angle > 360f)
            {
                angle -= 360f;
            }
            return Mathf.Clamp(angle, min, max);
        }

        private static Quaternion ConvertRotation(Quaternion q)
        {
            return new Quaternion(q.x, q.y, -q.z, -q.w);
        }

        private void FixedUpdate()
        {
            MonoGameEntry sceneCanvas = Singleton<MainUIManager>.Instance.SceneCanvas as MonoGameEntry;
            if (sceneCanvas == null)
            {
                this.ParallexSensitivity = this.GyroSensitivityCurve.Evaluate(base.transform.position.z);
                base.transform.rotation = Quaternion.Slerp(base.transform.rotation, this.referenceAttitude * ConvertRotation((Quaternion.Inverse(this.baseAttitude) * this.referanceRotation) * Input.gyro.attitude), this.ParallexSensitivity);
                Vector3 eulerAngles = base.transform.rotation.eulerAngles;
                eulerAngles.z = 0f;
                base.transform.rotation = Quaternion.Euler(eulerAngles);
                if (Quaternion.Angle(Input.gyro.attitude, this.baseAttitude) > this.ParallexRange)
                {
                    this.baseAttitude = Quaternion.Slerp(this.baseAttitude, Input.gyro.attitude, this.ParallexBoundHardness);
                }
                if (this.target != null)
                {
                    Vector3 b = this.target.position - ((Vector3) (base.transform.forward * this.Distance));
                    base.transform.position = Vector3.Lerp(base.transform.position, b, Time.deltaTime * 5f);
                }
            }
        }

        public void OnDrag(BaseEventData evt)
        {
            if (evt is PointerEventData)
            {
                Vector2 delta = (evt as PointerEventData).delta;
                delta.x *= this.headSpeed * 0.02f;
                delta.y *= this.pitchSpeed * 0.02f;
                this.euler.x += delta.x;
                this.euler.y -= delta.y;
                this.euler.x = this.ClampAngle(this.euler.x, this.origEuler.x - this.angleBounds.x, this.origEuler.x + this.angleBounds.x);
                this.euler.y = this.ClampAngle(this.euler.y, this.origEuler.y - this.angleBounds.y, this.origEuler.y + this.angleBounds.y);
                this.targetRot = Quaternion.Euler(this.euler.y, this.euler.x, 0f);
                this.state = State.Drag;
            }
        }

        public void OnDragEnd(BaseEventData evt)
        {
            if (evt is PointerEventData)
            {
                this.state = State.Reset;
                this.resetTimer = 0f;
            }
        }

        private void Start()
        {
            Vector3 eulerAngles = base.transform.eulerAngles;
            this.euler.x = eulerAngles.y;
            this.euler.y = eulerAngles.x;
            this.euler.y = Mathf.Repeat(this.euler.y + 180f, 360f) - 180f;
            this.origEuler = this.euler;
            this.origRot = this.targetRot = base.transform.rotation;
            if (this.target != null)
            {
                base.transform.position = this.target.position - ((Vector3) (base.transform.forward * this.Distance));
            }
            this.state = State.Idle;
            GameObject obj2 = GameObject.Find("MainMenu_SpaceShip");
            if ((this.pilot1 == null) && (obj2 != null))
            {
                this.pilot1 = obj2.transform.Find("Warship/Warship_ControlDesk01");
            }
            if ((this.pilot2 == null) && (obj2 != null))
            {
                this.pilot2 = obj2.transform.Find("Warship/Warship_ControlDesk02");
            }
            if ((this.pilot3 == null) && (obj2 != null))
            {
                this.pilot3 = obj2.transform.Find("Warship/Warship_ControlDesk03");
            }
            if (this.pilot1 != null)
            {
                this.pilotPosition1 = this.pilot1.transform.position;
            }
            if (this.pilot2 != null)
            {
                this.pilotPosition2 = this.pilot2.transform.position;
            }
            if (this.pilot3 != null)
            {
                this.pilotPosition3 = this.pilot3.transform.position;
            }
            this.PilotShakeX.preWrapMode = WrapMode.Loop;
            this.PilotShakeX.postWrapMode = WrapMode.Loop;
            this.PilotShakeY.preWrapMode = WrapMode.Loop;
            this.PilotShakeY.postWrapMode = WrapMode.Loop;
            Input.gyro.enabled = GraphicsSettingData.IsEnableGyroscope();
            this.baseAttitude = Input.gyro.attitude;
        }

        private void Update()
        {
            if (((this.pilot1 != null) && (this.pilot2 != null)) && (this.pilot3 != null))
            {
                this._time += Time.deltaTime * this.pilotShakeSpeed;
                this.pilotOffset1.y = this.PilotShakeY.Evaluate(0.7f + this._time) * this.amplitudeY;
                this.pilotOffset2.y = this.PilotShakeY.Evaluate(this._time * 0.994f) * this.amplitudeY;
                this.pilotOffset3.y = this.PilotShakeY.Evaluate(0.1f + (this._time * 1.031f)) * this.amplitudeY;
                this.pilotOffset1.x = this.PilotShakeX.Evaluate(0.7f + (this._time * 0.997f)) * this.amplitudeX;
                this.pilotOffset2.x = this.PilotShakeX.Evaluate(this._time * 0.995f) * this.amplitudeX;
                this.pilotOffset3.x = this.PilotShakeX.Evaluate(0.1f + (this._time * 1.03f)) * this.amplitudeX;
                this.pilot1.transform.position = this.pilotPosition1 + this.pilotOffset1;
                this.pilot2.transform.position = this.pilotPosition2 + this.pilotOffset2;
                this.pilot3.transform.position = this.pilotPosition3 + this.pilotOffset3;
            }
        }

        public enum State
        {
            Idle,
            Drag,
            Reset
        }
    }
}

