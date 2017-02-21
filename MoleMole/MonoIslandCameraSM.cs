namespace MoleMole
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public class MonoIslandCameraSM : MonoBehaviour
    {
        public float _backto_base_speed;
        public float _backto_landed_speed;
        private IslandCameraBackToBaseState _backToBaseState;
        private IslandCameraBackToLandedState _backToLandedState;
        [HideInInspector]
        public MonoIslandBuildingsUtil _buildingsUtil;
        private Transform _camera;
        [SerializeField]
        private Transform _cameraBasePos;
        [SerializeField]
        private AnimationCurve _cameraMoveNormalizedCurve;
        private Transform _cover;
        [SerializeField]
        private float _coverLerpDuration;
        private E_AlphaLerpDir _coverLerpingDir;
        private IslandCameraBaseState _currentState;
        private IslandCameraDampingState _dampingState;
        [SerializeField]
        private Transform _debugLine_bottom;
        [SerializeField]
        private Transform _debugLine_left;
        [SerializeField]
        private Transform _debugLine_right;
        [SerializeField]
        private Transform _debugLine_up;
        private Vector3 _deltaPos;
        [SerializeField]
        private float _down_bound;
        [SerializeField]
        private float _down_bound_inner;
        public float _dragBack_speed;
        private DragBackPoint _dragBackPoint;
        private IslandCameraDragBackState _dragBackState;
        private MonoIslandBuilding _engine;
        private FingerUtil _fingerUtil;
        private IslandCameraFocusingState _focusingState;
        [SerializeField]
        private float _grayCoverAlpha;
        private IslandCameraNavigatorNew _gyroManager;
        private IslandCameraLandingState _landingState;
        [SerializeField]
        private float _left_bound;
        [SerializeField]
        private float _left_bound_inner;
        private Vector3 _lookAtDir;
        private float _lookAtDist = 150f;
        private float _lookAtPitch;
        private Vector3 _lookAtPos;
        private Transform _mainCameraTran;
        private AlphaLerpMaterialPropetyBlock _mpb;
        private int _next_frame;
        public float _pre_focus_time;
        public float _pre_landed_time;
        private System.Random _random = new System.Random();
        [SerializeField]
        private float _right_bound;
        [SerializeField]
        private float _right_bound_inner;
        private float _startTimeCoverAlpha;
        private Dictionary<E_IslandCameraState, IslandCameraBaseState> _stateDict;
        public float _swipe_damping_ratio = 10f;
        [SerializeField]
        private float _swipe_lerp_ratio = 0.08f;
        [SerializeField]
        private float _swipe_to_world_speed_ratio;
        public E_SwipeNextState _swipeNextState;
        private Vector2 _swipeSpeed;
        private IslandCameraSwipeState _swipeState;
        public E_SwipeType _swipeType;
        private Vector3 _targetPivot;
        public float _tension_speed_ratio;
        public float _to_focus_speed;
        public float _to_landed_speed;
        private IslandCameraToFocusState _toFocusState;
        private IslandCameraToLandedState _toLandedState;
        [SerializeField]
        private float _up_bound;
        [SerializeField]
        private float _up_bound_inner;
        private Vector3 _vPivot;

        private void Awake()
        {
            this._camera = base.transform;
            this._mainCameraTran = base.transform.Find("MainCamera");
            this._buildingsUtil = GameObject.Find("IslandWorld").GetComponent<MonoIslandBuildingsUtil>();
            this._cover = base.transform.Find("MainCamera/Cover");
            this._cameraBasePos.position = base.transform.position;
            this._cameraBasePos.rotation = base.transform.rotation;
            this._gyroManager = base.GetComponent<IslandCameraNavigatorNew>();
            this._engine = GameObject.Find("IslandWorld/Engine").GetComponent<MonoIslandBuilding>();
        }

        public void BackToBase()
        {
            MonoIslandBuilding param = null;
            if (this._currentState == this._focusingState)
            {
                param = this._focusingState.GetBuilding();
            }
            else if (this._currentState == this._landingState)
            {
                param = this._landingState.GetBuilding();
            }
            else
            {
                return;
            }
            this.GotoState(E_IslandCameraState.BackToBase, param);
        }

        public void CameraToBasePos()
        {
            this._camera.position = this.GetCameraBasePos().position;
            this._camera.rotation = this.GetCameraBasePos().rotation;
        }

        public Vector3 CheckSwipeCameraPos(Vector3 cameraPos)
        {
            cameraPos.y = this.GetSwipeLayerY();
            float max = this._cameraBasePos.position.z + this._up_bound;
            float min = this._cameraBasePos.position.z - this._down_bound;
            float num3 = this._cameraBasePos.position.x - this._left_bound;
            float num4 = this._cameraBasePos.position.x + this._right_bound;
            cameraPos.x = Mathf.Clamp(cameraPos.x, num3, num4);
            cameraPos.z = Mathf.Clamp(cameraPos.z, min, max);
            return cameraPos;
        }

        private void Debug_DrawBorderInfo()
        {
            int cameraOutInfo = this.GetCameraOutInfo(this._camera.position);
            this._debugLine_right.gameObject.SetActive((cameraOutInfo & 1) != 0);
            this._debugLine_left.gameObject.SetActive((cameraOutInfo & 4) != 0);
            this._debugLine_up.gameObject.SetActive((cameraOutInfo & 8) != 0);
            this._debugLine_bottom.gameObject.SetActive((cameraOutInfo & 2) != 0);
        }

        public void ExitFocusing()
        {
            if (this._currentState is IslandCameraFocusingState)
            {
                this.GotoState(E_IslandCameraState.BackToLanded, (this._currentState as IslandCameraFocusingState).GetBuilding());
            }
        }

        public float GetBackToBaseSpeed(float dist)
        {
            Vector3 vector = this._cameraBasePos.position - this._engine.GetFocusPos();
            float magnitude = vector.magnitude;
            float max = magnitude * 2f;
            float b = 2f;
            dist = Mathf.Clamp(dist, magnitude, max);
            float t = (dist - magnitude) / (max - magnitude);
            t = Mathf.Lerp(1f, b, t);
            return (this._backto_base_speed * t);
        }

        public Transform GetCamera()
        {
            return this._camera;
        }

        public Transform GetCameraBasePos()
        {
            return this._cameraBasePos;
        }

        public int GetCameraOutInfo(Vector3 cameraPos)
        {
            int num = 0;
            float num2 = this._cameraBasePos.position.z + this._up_bound_inner;
            float num3 = this._cameraBasePos.position.z - this._down_bound_inner;
            float num4 = this._cameraBasePos.position.x - this._left_bound_inner;
            float num5 = this._cameraBasePos.position.x + this._right_bound_inner;
            if (cameraPos.x < num4)
            {
                num |= 1;
            }
            if (cameraPos.x > num5)
            {
                num |= 4;
            }
            if (cameraPos.z < num3)
            {
                num |= 8;
            }
            if (cameraPos.z > num2)
            {
                num |= 2;
            }
            return num;
        }

        public DragBackPoint GetDragBackPoint()
        {
            return this._dragBackPoint;
        }

        public IslandCameraNavigatorNew GetGyroManager()
        {
            return this._gyroManager;
        }

        private Vector3 GetGyroOffset()
        {
            return this._gyroManager.GetOffset();
        }

        public float GetLandedSpeedFinal(float dist)
        {
            Vector3 vector = this._cameraBasePos.position - this._engine.GetLandedPos();
            float magnitude = vector.magnitude;
            float max = magnitude * 2.5f;
            float b = 2f;
            dist = Mathf.Clamp(dist, magnitude, max);
            float t = (dist - magnitude) / (max - magnitude);
            t = Mathf.Lerp(1f, b, t);
            return (this._to_landed_speed * t);
        }

        public Vector3 GetLookAtDir()
        {
            this._lookAtDir = (Vector3) (Quaternion.Euler(this._lookAtPitch, 0f, 0f) * Vector3.forward);
            return this._lookAtDir;
        }

        public float GetLookAtPitch()
        {
            return this._lookAtPitch;
        }

        private Vector3 GetLookAtPos()
        {
            this._lookAtPos = this.GetPivot() + ((Vector3) (this.GetLookAtDir() * this._lookAtDist));
            return this._lookAtPos;
        }

        public float GetNomalizedCurvePos(float t)
        {
            return this._cameraMoveNormalizedCurve.Evaluate(t);
        }

        public float GetOutRotio(E_IslandCameraOut type, Vector3 cameraPos)
        {
            float num = this._cameraBasePos.position.z + this._up_bound;
            float num2 = this._cameraBasePos.position.z - this._down_bound;
            float num3 = this._cameraBasePos.position.x - this._left_bound;
            float num4 = this._cameraBasePos.position.x + this._right_bound;
            float num5 = this._cameraBasePos.position.z + this._up_bound_inner;
            float num6 = this._cameraBasePos.position.z - this._down_bound_inner;
            float num7 = this._cameraBasePos.position.x - this._left_bound_inner;
            float num8 = this._cameraBasePos.position.x + this._right_bound_inner;
            float num9 = 0f;
            if (type == E_IslandCameraOut.Left)
            {
                if (cameraPos.x < num7)
                {
                    num9 = (num7 - cameraPos.x) / (num7 - num3);
                    num9 = Mathf.Clamp01(num9);
                }
                return num9;
            }
            if (type == E_IslandCameraOut.Right)
            {
                if (cameraPos.x > num8)
                {
                    num9 = (cameraPos.x - num8) / (num4 - num8);
                    num9 = Mathf.Clamp01(num9);
                }
                return num9;
            }
            if (type == E_IslandCameraOut.Bottom)
            {
                if (cameraPos.z < num6)
                {
                    num9 = (num6 - cameraPos.z) / (num6 - num2);
                    num9 = Mathf.Clamp01(num9);
                }
                return num9;
            }
            if ((type == E_IslandCameraOut.Top) && (cameraPos.z > num5))
            {
                num9 = (cameraPos.z - num5) / (num - num5);
                num9 = Mathf.Clamp01(num9);
            }
            return num9;
        }

        public Vector3 GetPivot()
        {
            return this._vPivot;
        }

        private float GetRatioCurve(float ratio)
        {
            return ((1f - ratio) * (1f - ratio));
        }

        private IslandCameraBaseState GetState(E_IslandCameraState stateName)
        {
            return this._stateDict[stateName];
        }

        public float GetSwipeLayerY()
        {
            return this._cameraBasePos.position.y;
        }

        public void GotoState(E_IslandCameraState newStateName, object param = null)
        {
            IslandCameraBaseState nextState = this.GetState(newStateName);
            if (this._currentState != null)
            {
                this._currentState.Exit(nextState);
            }
            nextState.Enter(this._currentState, param);
            this._currentState = nextState;
        }

        private void LerpPivot(Vector3 targetPos, Vector3 delta)
        {
            float num = delta.magnitude / Time.deltaTime;
            float num2 = (Time.deltaTime * this._swipe_lerp_ratio) * (1f + num);
            num2 = Mathf.Clamp01(num2);
            this._vPivot = Vector3.Lerp(this._vPivot, targetPos, num2);
            this._targetPivot = targetPos;
        }

        public void OnDestroy()
        {
            this.UnsubscribeEvent();
        }

        public void OnDisable()
        {
            this.UnsubscribeEvent();
        }

        private void OnDrag(Gesture gesture)
        {
            if ((this._fingerUtil.GetFingerNum() == 1) && this._fingerUtil.ContainFinger(gesture.fingerIndex))
            {
                this._currentState.OnDrag(gesture);
            }
        }

        private void OnDragEnd(Gesture gesture)
        {
            if ((this._fingerUtil.GetFingerNum() == 1) && this._fingerUtil.ContainFinger(gesture.fingerIndex))
            {
                this._currentState.OnDragEnd(gesture);
            }
        }

        private void OnDragStart(Gesture gesture)
        {
            if ((this._fingerUtil.GetFingerNum() == 1) && this._fingerUtil.ContainFinger(gesture.fingerIndex))
            {
                this._currentState.OnDragStart(gesture);
            }
        }

        public void OnEnable()
        {
            this.SubscribeEvent();
        }

        private void OnSwipe(Gesture gesture)
        {
            if ((this._fingerUtil.GetFingerNum() == 1) && this._fingerUtil.ContainFinger(gesture.fingerIndex))
            {
                this._currentState.OnSwipe(gesture);
            }
        }

        private void OnSwipeEnd(Gesture gesture)
        {
            if ((this._fingerUtil.GetFingerNum() == 1) && this._fingerUtil.ContainFinger(gesture.fingerIndex))
            {
                this._currentState.OnSwipeEnd(gesture);
            }
        }

        private void OnSwipeStart(Gesture gesture)
        {
        }

        private void OnTouchStart(Gesture gesture)
        {
            bool flag = this._fingerUtil.AddFinger(gesture.fingerIndex);
            this._currentState.OnTouchStart(gesture);
        }

        private void OnTouchUp(Gesture gesture)
        {
            bool flag = this._fingerUtil.RemoveFinger(gesture.fingerIndex);
            this._currentState.OnTouchUp(gesture);
        }

        public void SetLookAtPitch(float pitch)
        {
            this._lookAtPitch = pitch;
        }

        public void SetPivot(Vector3 pos)
        {
            this._vPivot = pos;
        }

        public void SetSwipeSpeed(Vector2 _speed)
        {
            this._swipeSpeed = _speed;
        }

        private void Start()
        {
            this._fingerUtil = new FingerUtil();
            this._swipeState = new IslandCameraSwipeState(this);
            this._dampingState = new IslandCameraDampingState(this);
            this._toLandedState = new IslandCameraToLandedState(this);
            this._toFocusState = new IslandCameraToFocusState(this);
            this._backToLandedState = new IslandCameraBackToLandedState(this);
            this._focusingState = new IslandCameraFocusingState(this);
            this._backToBaseState = new IslandCameraBackToBaseState(this);
            this._dragBackState = new IslandCameraDragBackState(this);
            this._landingState = new IslandCameraLandingState(this);
            Dictionary<E_IslandCameraState, IslandCameraBaseState> dictionary = new Dictionary<E_IslandCameraState, IslandCameraBaseState>();
            dictionary.Add(E_IslandCameraState.Swipe, this._swipeState);
            dictionary.Add(E_IslandCameraState.Damping, this._dampingState);
            dictionary.Add(E_IslandCameraState.ToLanded, this._toLandedState);
            dictionary.Add(E_IslandCameraState.ToFocus, this._toFocusState);
            dictionary.Add(E_IslandCameraState.BackToLanded, this._backToLandedState);
            dictionary.Add(E_IslandCameraState.Focusing, this._focusingState);
            dictionary.Add(E_IslandCameraState.BackToBase, this._backToBaseState);
            dictionary.Add(E_IslandCameraState.DragBack, this._dragBackState);
            dictionary.Add(E_IslandCameraState.Landing, this._landingState);
            this._stateDict = dictionary;
            this._mpb = new AlphaLerpMaterialPropetyBlock(this._cover.GetComponent<MeshRenderer>(), "_Color", 0f, this._grayCoverAlpha);
            this._mpb.SetAlpha(0f);
            this._currentState = null;
            this.GotoState(E_IslandCameraState.Swipe, null);
            this.SetPivot(this._cameraBasePos.position);
            this._targetPivot = this._cameraBasePos.position;
            this._lookAtPitch = this._cameraBasePos.eulerAngles.x;
            this._lookAtDir = this._cameraBasePos.forward;
            this._dragBackPoint = new DragBackPoint(this._cameraBasePos.position);
        }

        public void SubscribeEvent()
        {
            EasyTouch.On_TouchStart += new EasyTouch.TouchStartHandler(this.OnTouchStart);
            EasyTouch.On_TouchUp += new EasyTouch.TouchUpHandler(this.OnTouchUp);
            EasyTouch.On_SwipeStart += new EasyTouch.SwipeStartHandler(this.OnSwipeStart);
            EasyTouch.On_SwipeEnd += new EasyTouch.SwipeEndHandler(this.OnSwipeEnd);
            EasyTouch.On_Swipe += new EasyTouch.SwipeHandler(this.OnSwipe);
            EasyTouch.On_Drag += new EasyTouch.DragHandler(this.OnDrag);
            EasyTouch.On_DragStart += new EasyTouch.DragStartHandler(this.OnDragStart);
            EasyTouch.On_DragEnd += new EasyTouch.DragEndHandler(this.OnDragEnd);
        }

        public Vector2 SwipeMoveHandler()
        {
            this._deltaPos.x = this._swipeSpeed.x;
            this._deltaPos.y = 0f;
            this._deltaPos.z = this._swipeSpeed.y;
            if (this._swipeType == E_SwipeType.Normal)
            {
                Vector3 cameraPos = this.GetPivot() + this._deltaPos;
                cameraPos = this.CheckSwipeCameraPos(cameraPos);
                this.SetPivot(cameraPos);
            }
            else
            {
                Vector3 vector2 = this._targetPivot + this._deltaPos;
                vector2 = this.CheckSwipeCameraPos(vector2);
                this.LerpPivot(vector2, this._deltaPos);
            }
            return this._swipeSpeed;
        }

        public void SwipeToWorldSpeed(Vector2 deltaPosition)
        {
            float num = this._swipe_to_world_speed_ratio;
            float num2 = this._swipe_to_world_speed_ratio;
            int cameraOutInfo = this.GetCameraOutInfo(this._camera.position);
            if (((cameraOutInfo & 1) != 0) && (deltaPosition.x > 0f))
            {
                float outRotio = this.GetOutRotio(E_IslandCameraOut.Left, this._camera.position);
                num = this._swipe_to_world_speed_ratio * this.GetRatioCurve(outRotio);
            }
            if (((cameraOutInfo & 4) != 0) && (deltaPosition.x < 0f))
            {
                float ratio = this.GetOutRotio(E_IslandCameraOut.Right, this._camera.position);
                num = this._swipe_to_world_speed_ratio * this.GetRatioCurve(ratio);
            }
            if (((cameraOutInfo & 8) != 0) && (deltaPosition.y > 0f))
            {
                float num6 = this.GetOutRotio(E_IslandCameraOut.Bottom, this._camera.position);
                num2 = this._swipe_to_world_speed_ratio * this.GetRatioCurve(num6);
            }
            if (((cameraOutInfo & 2) != 0) && (deltaPosition.y < 0f))
            {
                float num7 = this.GetOutRotio(E_IslandCameraOut.Top, this._camera.position);
                num2 = this._swipe_to_world_speed_ratio * this.GetRatioCurve(num7);
            }
            this._swipeSpeed.x = (-deltaPosition.x / ((Screen.width <= Screen.height) ? ((float) Screen.height) : ((float) Screen.width))) * num;
            this._swipeSpeed.y = (-deltaPosition.y / ((Screen.width <= Screen.height) ? ((float) Screen.height) : ((float) Screen.width))) * num2;
            this._swipeSpeed = (Vector2) ((this._swipeSpeed * Time.smoothDeltaTime) / Time.deltaTime);
        }

        public void ToDampingSpeed()
        {
            int cameraOutInfo = this.GetCameraOutInfo(this._camera.position);
            if (cameraOutInfo > 0)
            {
                this._swipeSpeed = Vector2.zero;
                if ((cameraOutInfo & 1) != 0)
                {
                    float outRotio = this.GetOutRotio(E_IslandCameraOut.Left, this._camera.position);
                    float num3 = this._tension_speed_ratio * outRotio;
                    this._swipeSpeed.x = num3;
                }
                if ((cameraOutInfo & 4) != 0)
                {
                    float num4 = this.GetOutRotio(E_IslandCameraOut.Right, this._camera.position);
                    float num5 = -this._tension_speed_ratio * num4;
                    this._swipeSpeed.x = num5;
                }
                if ((cameraOutInfo & 8) != 0)
                {
                    float num6 = this.GetOutRotio(E_IslandCameraOut.Bottom, this._camera.position);
                    float num7 = this._tension_speed_ratio * num6;
                    this._swipeSpeed.y = num7;
                }
                if ((cameraOutInfo & 2) != 0)
                {
                    float num8 = this.GetOutRotio(E_IslandCameraOut.Top, this._camera.position);
                    float num9 = -this._tension_speed_ratio * num8;
                    this._swipeSpeed.y = num9;
                }
            }
            else
            {
                this._swipeSpeed = Vector2.Lerp(this._swipeSpeed, Vector2.zero, Time.deltaTime * this._swipe_damping_ratio);
            }
        }

        public void ToFocusing()
        {
            if (this._currentState == this._landingState)
            {
                this.GotoState(E_IslandCameraState.ToFocus, this._landingState.GetBuilding());
            }
        }

        public void TriggerCameraObj(bool enable)
        {
            this._mainCameraTran.gameObject.SetActive(enable);
        }

        public void TriggerCover(E_AlphaLerpDir dir)
        {
            this._coverLerpingDir = dir;
            this._mpb.SetDir(dir);
            this._startTimeCoverAlpha = Time.time;
        }

        public void UnsubscribeEvent()
        {
            EasyTouch.On_TouchStart -= new EasyTouch.TouchStartHandler(this.OnTouchStart);
            EasyTouch.On_TouchUp -= new EasyTouch.TouchUpHandler(this.OnTouchUp);
            EasyTouch.On_SwipeStart -= new EasyTouch.SwipeStartHandler(this.OnSwipeStart);
            EasyTouch.On_SwipeEnd -= new EasyTouch.SwipeEndHandler(this.OnSwipeEnd);
            EasyTouch.On_Swipe -= new EasyTouch.SwipeHandler(this.OnSwipe);
            EasyTouch.On_Drag -= new EasyTouch.DragHandler(this.OnDrag);
            EasyTouch.On_DragStart -= new EasyTouch.DragStartHandler(this.OnDragStart);
            EasyTouch.On_DragEnd -= new EasyTouch.DragEndHandler(this.OnDragEnd);
        }

        public void Update()
        {
            this._currentState.Update();
            this.UpdateCamera();
            this.UpdateCoverLerping();
        }

        private void UpdateCamera()
        {
            this._camera.position = this.GetPivot() + this.GetGyroOffset();
            this._camera.LookAt(this.GetLookAtPos());
        }

        private void UpdateCoverLerping()
        {
            if (this._coverLerpingDir != E_AlphaLerpDir.None)
            {
                if ((this._coverLerpingDir == E_AlphaLerpDir.ToLarge) && !this._cover.gameObject.activeSelf)
                {
                    this._cover.gameObject.SetActive(true);
                }
                float t = (Time.time - this._startTimeCoverAlpha) / this._coverLerpDuration;
                if (t > 1f)
                {
                    if (this._coverLerpingDir == E_AlphaLerpDir.ToLittle)
                    {
                        this._cover.gameObject.SetActive(false);
                    }
                    this.TriggerCover(E_AlphaLerpDir.None);
                }
                else
                {
                    this._mpb.LerpAlpha(t);
                }
            }
        }

        private void UpdateLowFPS()
        {
            if (this._next_frame <= 0)
            {
                this._next_frame = Time.frameCount;
            }
            if (this._next_frame == Time.frameCount)
            {
                for (int i = 0; i < 0x493e0; i++)
                {
                    RaycastHit hit;
                    Physics.Raycast(Vector3.zero, Vector3.up * this._next_frame, out hit, 10f, ((int) 1) << InLevelData.PROP_LAYER);
                }
                this._next_frame += this._random.Next(10, 15);
            }
        }

        public enum E_SwipeType
        {
            Normal,
            Lerp
        }

        public class FingerUtil
        {
            private List<int> _fingerIDList = new List<int>();

            public bool AddFinger(int id)
            {
                if (this.ContainFinger(id))
                {
                    return false;
                }
                this._fingerIDList.Add(id);
                return true;
            }

            public bool ContainFinger(int id)
            {
                for (int i = 0; i < this._fingerIDList.Count; i++)
                {
                    if (this._fingerIDList[i] == id)
                    {
                        return true;
                    }
                }
                return false;
            }

            public int GetFingerNum()
            {
                return this._fingerIDList.Count;
            }

            public bool RemoveFinger(int id)
            {
                if (!this.ContainFinger(id))
                {
                    return false;
                }
                this._fingerIDList.Remove(id);
                return true;
            }
        }
    }
}

