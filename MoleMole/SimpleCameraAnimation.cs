namespace MoleMole
{
    using BehaviorDesigner.Runtime;
    using System;
    using UnityEngine;

    [RequireComponent(typeof(Camera))]
    public class SimpleCameraAnimation : MonoBehaviour
    {
        private Transform _avatarTransform;
        private BehaviorDesigner.Runtime.BehaviorTree _behaviorTree;
        private Camera _camera;
        private PostFX _postFX;
        [SerializeField]
        private float _timer;
        public float attackDelay;
        public AnimationCurve distance = AnimationCurve.Linear(0f, 5f, X_AXIS_LEN, 5f);
        [Range(1f, 3600f)]
        public float duration = 1f;
        public AnimationCurve fov = AnimationCurve.Linear(0f, 50f, X_AXIS_LEN, 50f);
        public bool isPlaying;
        public float lookAtHeight = 1.7f;
        public AnimationCurve pitchAngle = AnimationCurve.Linear(0f, 0f, X_AXIS_LEN, 80f);
        public bool showEffect = true;
        public bool slowMode;
        private static readonly float X_AXIS_LEN = 1000f;
        public AnimationCurve yawAngle = AnimationCurve.Linear(0f, 0f, X_AXIS_LEN, 360f);

        private void Awake()
        {
            this._camera = base.GetComponent<Camera>();
        }

        private void EndPlay()
        {
            if (this._behaviorTree != null)
            {
                this._behaviorTree.SetVariableValue("DoAttack", false);
            }
            if (this.slowMode)
            {
                Time.timeScale = 1f;
            }
            if (!this.showEffect)
            {
                this._postFX.UseDistortion = true;
            }
            this.isPlaying = false;
        }

        private void InitPlay()
        {
            BaseMonoAvatar avatar = Singleton<AvatarManager>.Instance.TryGetLocalAvatar();
            this._avatarTransform = avatar.transform;
            this._behaviorTree = avatar.GetComponent<BehaviorDesigner.Runtime.BehaviorTree>();
            if (this.slowMode)
            {
                Time.timeScale = 0.25f;
            }
            if (!this.showEffect)
            {
                this._postFX = this._camera.GetComponent<PostFX>();
                this._postFX.UseDistortion = false;
            }
            this._timer = 0f;
            this.isPlaying = true;
        }

        private void Play(float time)
        {
            this._timer += time;
            float num = (this._timer / this.duration) * X_AXIS_LEN;
            if ((this._timer > this.attackDelay) && (this._behaviorTree != null))
            {
                this._behaviorTree.SetVariableValue("DoAttack", true);
            }
            base.transform.forward = this._avatarTransform.forward;
            Vector3 eulerAngles = base.transform.eulerAngles;
            eulerAngles.x += this.pitchAngle.Evaluate(num);
            eulerAngles.y += this.yawAngle.Evaluate(num);
            base.transform.eulerAngles = eulerAngles;
            base.transform.position = (Vector3) ((this._avatarTransform.position + (Vector3.up * this.lookAtHeight)) - (base.transform.forward * this.distance.Evaluate(num)));
            this._camera.fieldOfView = this.fov.Evaluate(num);
            if (num > X_AXIS_LEN)
            {
                this.EndPlay();
            }
        }

        private void Start()
        {
        }

        private void Update()
        {
            if (this.isPlaying)
            {
                this.Play(Time.deltaTime);
                if (Input.GetKeyDown(KeyCode.F))
                {
                    this.isPlaying = false;
                    this.EndPlay();
                }
            }
            else if (Input.GetKeyDown(KeyCode.F))
            {
                this.InitPlay();
            }
        }
    }
}

