namespace MoleMole
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;
    using UnityEngine.UI;

    [RequireComponent(typeof(Animation)), ExecuteInEditMode]
    public class MonoSimpleAnimation : MonoAuxObject
    {
        private Animation _animation;
        private AnimationState _animationState;
        private Vector3 _circleLookAtPos;
        private float _circleStartTime;
        private bool _hasSetCircleStartTime;
        private float _lastTimeScale;
        private int _levelEntityTimescalIx = -1;
        private float _sampleTimer;
        [CompilerGenerated]
        private static Action <>f__am$cache1E;
        [CompilerGenerated]
        private static Action <>f__am$cache1F;
        [Header("circle track parameter")]
        public CircleTrack circleTrack;
        [Header("Animated UI Graphics")]
        public Graphic[] graphics;
        [HideInInspector]
        public bool hasPushedLevelTimeScale;
        [Header("Init Near Z Plane, only positive value works")]
        public float initClipZNear = -1f;
        [Header("Init FOV, only positive value works")]
        public float initFOV = -1f;
        [Header("Use animation to key this to control directional light forward")]
        public Vector3 keyedDirectionalLightRotation;
        [Header("Use animation to key this instead of maincamera fov")]
        public float keyedFOV;
        [Header("Use animation to key this instead of transform.rotation for working constant tangent")]
        public Vector3 keyedRotation;
        public Vector2 radialBlurCenter;
        public float radialBlurScatterScale;
        public float radialBlurStrenth;
        [Header("Use this Go postion and rotation")]
        public GameObject realAnimGameObject;
        [NonSerialized]
        public bool selfUpdateKeyedRotation = true;
        [Header("Ignore Time Scale")]
        public TimeScaleMode timeScaleMode;
        public bool useAnimRotation;
        [Header("Duplicate a fixed parent transform on start")]
        public bool useFixedParentAnchor;
        [Header("Use key Directional Light Forward")]
        public bool useKeyedDirectionalLightRotation;
        [Header("Set this to true and key")]
        public bool useKeyedFOV;
        [Header("Use keyed Radial Blur")]
        public bool useKeyedRadialBlur;
        [Header("Set this to true and key")]
        public bool useKeyedRotation;
        [Header("use circle track")]
        public bool UseSpecificCircleTrack;

        [AnimationCallback]
        private void ActCameraShake(float shakeTime)
        {
            Singleton<CameraManager>.Instance.GetMainCamera().ActShakeEffect(shakeTime, 0.36f, 90f, 2, false, false);
        }

        private void Awake()
        {
            this._animation = base.GetComponent<Animation>();
            if (this.timeScaleMode > TimeScaleMode.DoNothing)
            {
                this._lastTimeScale = Singleton<LevelManager>.Instance.levelEntity.TimeScale;
                this._sampleTimer = 0f;
                this._animationState = this._animation[this._animation.clip.name];
            }
        }

        [AnimationCallback]
        private void DebugBreak()
        {
            Debug.Break();
        }

        [AnimationCallback]
        private void EndLevel(string winMsg)
        {
        }

        [AnimationCallback]
        private void FadeIn(float fadeDuration)
        {
            if (<>f__am$cache1E == null)
            {
                <>f__am$cache1E = () => Singleton<EventManager>.Instance.FireEvent(new EvtLevelState(EvtLevelState.State.ExitTransition, EvtLevelState.LevelEndReason.EndUncertainReason, 0), MPEventDispatchMode.Normal);
            }
            Action fadeEndCallback = <>f__am$cache1E;
            Singleton<MainUIManager>.Instance.GetInLevelUICanvas().FadeInStageTransitPanel(fadeDuration, false, null, fadeEndCallback);
        }

        [AnimationCallback]
        private void FadeOut(float fadeDuration)
        {
            if (<>f__am$cache1F == null)
            {
                <>f__am$cache1F = () => Singleton<EventManager>.Instance.FireEvent(new EvtLevelState(EvtLevelState.State.EnterTransition, EvtLevelState.LevelEndReason.EndUncertainReason, 0), MPEventDispatchMode.Normal);
            }
            Action fadeStartCallback = <>f__am$cache1F;
            Singleton<MainUIManager>.Instance.GetInLevelUICanvas().FadeOutStageTransitPanel(fadeDuration, false, fadeStartCallback, null);
        }

        public Quaternion GetLightRotation()
        {
            if (this.ownedParent == null)
            {
                return Quaternion.Euler(this.keyedDirectionalLightRotation);
            }
            return (this.ownedParent.transform.rotation * Quaternion.Euler(this.keyedDirectionalLightRotation));
        }

        [AnimationCallback]
        private void HideCloseUpPanel()
        {
            Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.MonsterCloseUpEnd, null));
        }

        [AnimationCallback]
        private void HideGameObject(string childPath)
        {
            if (string.IsNullOrEmpty(childPath))
            {
                base.gameObject.SetActive(false);
            }
            else
            {
                base.transform.Find(childPath).gameObject.SetActive(false);
            }
        }

        [AnimationCallback]
        private void MainCanvasFadeIn()
        {
            Singleton<MainUIManager>.Instance.GetInLevelUICanvas().FadeInStageTransitPanel(0.18f, false, null, null);
        }

        [AnimationCallback]
        private void MainCanvasFadeOut()
        {
            Singleton<MainUIManager>.Instance.GetInLevelUICanvas().FadeOutStageTransitPanel(0.18f, false, null, null);
        }

        private void OnDestroy()
        {
            if (Application.isPlaying && (this.ownedParent != null))
            {
                UnityEngine.Object.Destroy(this.ownedParent.gameObject);
            }
        }

        [AnimationCallback]
        private void PopLevelTimeScale()
        {
            this.hasPushedLevelTimeScale = false;
            Singleton<LevelManager>.Instance.levelEntity.timeScaleStack.Pop(this._levelEntityTimescalIx);
        }

        [AnimationCallback]
        private void PushLevelTimeScale(float timescale)
        {
            this.hasPushedLevelTimeScale = true;
            this._levelEntityTimescalIx = Singleton<LevelManager>.Instance.levelEntity.timeScaleStack.Push(timescale, false);
        }

        [AnimationCallback]
        private void SetFloorReflectionHeight(float reflectionHeight)
        {
            Singleton<StageManager>.Instance.GetPerpStage().GetComponent<FloorReflection>().floorHeight = reflectionHeight;
        }

        public void SetInLevelUIActive()
        {
            MonoInLevelUICanvas inLevelUICanvas = Singleton<MainUIManager>.Instance.GetInLevelUICanvas();
            if (inLevelUICanvas != null)
            {
                inLevelUICanvas.SetInLevelUIActive(true);
            }
        }

        [AnimationCallback]
        public void SetInLevelUIActiveInstant()
        {
            BasePageContext currentPageContext = Singleton<MainUIManager>.Instance.CurrentPageContext;
            if (currentPageContext != null)
            {
                if (currentPageContext is InLevelMainPageContext)
                {
                    (currentPageContext as InLevelMainPageContext).SetInLevelMainPageActive(true, true, false);
                }
                else
                {
                    Singleton<MainUIManager>.Instance.CurrentPageContext.SetActive(true);
                }
            }
        }

        [AnimationCallback]
        public void SetInLevelUIDeactive()
        {
            MonoInLevelUICanvas inLevelUICanvas = Singleton<MainUIManager>.Instance.GetInLevelUICanvas();
            if (inLevelUICanvas != null)
            {
                inLevelUICanvas.SetInLevelUIActive(false);
            }
        }

        [AnimationCallback]
        private void SetInLevelUIDeactiveInstant()
        {
            BasePageContext currentPageContext = Singleton<MainUIManager>.Instance.CurrentPageContext;
            if (currentPageContext != null)
            {
                if (currentPageContext is InLevelMainPageContext)
                {
                    (currentPageContext as InLevelMainPageContext).SetInLevelMainPageActive(false, true, false);
                }
                else
                {
                    Singleton<MainUIManager>.Instance.CurrentPageContext.SetActive(false);
                }
            }
        }

        public void SetOwnedParent(Transform ownedParent)
        {
            this.ownedParent = ownedParent;
            base.transform.parent = this.ownedParent;
            base.transform.localPosition = Vector3.zero;
        }

        [AnimationCallback]
        private void ShowCloseUpPanel(string monsterName)
        {
            Singleton<MainUIManager>.Instance.ShowPage(new MonsterCloseUpPageContext(monsterName), UIType.Page);
        }

        private void Start()
        {
            if (this.useFixedParentAnchor)
            {
                if ((!Application.isPlaying || (this.ownedParent != null)) || (base.transform.parent == null))
                {
                    return;
                }
                GameObject obj2 = new GameObject {
                    name = "FixedCameraAnchor"
                };
                this.ownedParent = obj2.transform;
                this.ownedParent.position = base.transform.parent.transform.position;
                Vector3 forward = base.transform.parent.transform.forward;
                forward.y = 0f;
                this.ownedParent.forward = forward;
                base.transform.parent = this.ownedParent;
                base.transform.localPosition = Vector3.zero;
            }
            if (this.UseSpecificCircleTrack && (this.circleTrack != null))
            {
                base.transform.localRotation = this.circleTrack.centerPos.transform.localRotation;
                this._circleLookAtPos = this.circleTrack.centerPos.transform.position;
            }
        }

        public void SyncRadialBlur()
        {
            if (this.useKeyedRadialBlur)
            {
                this.radialBlurCenter.x = Mathf.Clamp(this.radialBlurCenter.x, 0f, 1f);
                this.radialBlurCenter.y = Mathf.Clamp(this.radialBlurCenter.y, 0f, 1f);
                this.radialBlurStrenth = Mathf.Clamp(this.radialBlurStrenth, 0f, 10f);
                this.radialBlurScatterScale = Mathf.Clamp(this.radialBlurScatterScale, 0f, 2f);
            }
        }

        public void SyncRotation()
        {
            if (this.useKeyedRotation)
            {
                base.transform.localRotation = Quaternion.Euler(this.keyedRotation);
            }
        }

        [AnimationCallback]
        private void TimeSlow(float duration)
        {
            Singleton<LevelManager>.Instance.levelActor.TimeSlow(duration);
        }

        private void Update()
        {
            if (this.selfUpdateKeyedRotation)
            {
                this.SyncRotation();
            }
            if (this.useKeyedRadialBlur)
            {
                this.SyncRadialBlur();
            }
            if ((this.UseSpecificCircleTrack && (this.circleTrack != null)) && ((Singleton<LevelManager>.Instance != null) && (Singleton<LevelManager>.Instance.levelEntity != null)))
            {
                Vector3 vector;
                if (!this._hasSetCircleStartTime)
                {
                    this._circleStartTime = Time.time;
                    this._hasSetCircleStartTime = true;
                }
                float num = this.circleTrack.startAngle + ((this.circleTrack.angularSpeed * (Time.time - this._circleStartTime)) * Singleton<LevelManager>.Instance.levelEntity.TimeScale);
                if (this.circleTrack.isAntiClockwise)
                {
                    num *= -1f;
                }
                vector.z = (this.circleTrack.radius * Mathf.Cos(num * 0.01745329f)) * Mathf.Cos(this.circleTrack.elevation * 0.01745329f);
                vector.x = (this.circleTrack.radius * Mathf.Sin(num * 0.01745329f)) * Mathf.Cos(this.circleTrack.elevation * 0.01745329f);
                vector.y = this.circleTrack.radius * Mathf.Sin(this.circleTrack.elevation * 0.01745329f);
                base.transform.localPosition = this.circleTrack.centerPos.transform.localPosition + vector;
                base.transform.LookAt(this._circleLookAtPos);
            }
            if (this.timeScaleMode == TimeScaleMode.IgnoreTimeScale)
            {
                this._sampleTimer += Time.unscaledDeltaTime * Singleton<LevelManager>.Instance.levelEntity.TimeScale;
                this._animationState.time = this._sampleTimer;
                this._animation.Sample();
            }
            else if (this.timeScaleMode == TimeScaleMode.UseLevelTimeScale)
            {
                float timeScale = Singleton<LevelManager>.Instance.levelEntity.TimeScale;
                if (this._lastTimeScale != timeScale)
                {
                    this._animationState.speed = timeScale;
                }
                this._lastTimeScale = timeScale;
            }
            if ((this.graphics.Length > 0) && this._animation.isPlaying)
            {
                for (int i = 0; i < this.graphics.Length; i++)
                {
                    this.graphics[i].SetAllDirty();
                }
            }
        }

        public Transform ownedParent { get; private set; }

        [Serializable]
        public class CircleTrack
        {
            public float angularSpeed;
            public GameObject centerPos;
            public float elevation;
            public bool isAntiClockwise;
            public float radius;
            public float startAngle;
        }

        public enum TimeScaleMode
        {
            DoNothing,
            IgnoreTimeScale,
            UseLevelTimeScale
        }
    }
}

