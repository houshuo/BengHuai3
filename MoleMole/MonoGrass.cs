namespace MoleMole
{
    using System;
    using UnityEngine;

    [RequireComponent(typeof(Renderer)), RequireComponent(typeof(MeshFilter))]
    public class MonoGrass : MonoBehaviour
    {
        private BaseMonoAvatar __avatar;
        private Transform _avatarAnchor;
        private float _avatarHeight;
        private float _avatarLandingTimer;
        private Vector3 _avatarPosition;
        private AvatarState _avatarState;
        private MaterialPropertyBlock _mpb;
        private Renderer _renderer;
        public float avatarFloatHeight = 1f;
        public AnimationCurve avatarJumpBlendCurve;
        public float avatarJumpImpulseDuration = 1f;
        public float avatarJumpImpulseStength = 1f;
        [Range(0f, 1f)]
        public float avatarPositionSmooth = 0.1f;
        public float avatarWalkImpulseStength = 0.3f;

        public MonoGrass()
        {
            Keyframe[] keys = new Keyframe[] { new Keyframe(0f, 0f), new Keyframe(1f, 1f) };
            this.avatarJumpBlendCurve = new AnimationCurve(keys);
        }

        private void GetAvatarAnchor()
        {
            if (this.__avatar != null)
            {
                this._avatarPosition = this.__avatar.XZPosition;
                this._avatarAnchor = this.__avatar.GetAttachPoint("LeftFoot");
                if (this._avatarAnchor == null)
                {
                    this._avatarAnchor = this.__avatar.transform;
                }
                this._avatarHeight = this._avatarAnchor.position.y;
            }
        }

        private void OnLocalAvatarChanged(BaseMonoAvatar from, BaseMonoAvatar to)
        {
            this._avatar = to;
        }

        private void SetAvatarImpulse(string propName)
        {
            if (this._avatar != null)
            {
                Vector4 vector;
                this._avatarPosition = Vector3.Lerp(this._avatar.XZPosition, this._avatarPosition, this.avatarPositionSmooth);
                this._avatarHeight = Mathf.Lerp(this._avatarAnchor.position.y, this._avatarHeight, this.avatarPositionSmooth);
                if (this._avatarState == AvatarState.Walk)
                {
                    Vector3 vector2 = this._avatarPosition;
                    vector = new Vector4(vector2.x, 0f, vector2.z, this.avatarWalkImpulseStength);
                    if (this._avatarAnchor.position.y > this.avatarFloatHeight)
                    {
                        this._avatarState = AvatarState.Float;
                    }
                }
                else if (this._avatarState == AvatarState.Landing)
                {
                    this._avatarLandingTimer += Time.deltaTime;
                    float num = this._avatarLandingTimer / this.avatarJumpImpulseDuration;
                    if (num > 1f)
                    {
                        this._avatarState = AvatarState.Walk;
                    }
                    num = Mathf.Clamp01(num);
                    num = this.avatarJumpBlendCurve.Evaluate(num);
                    float w = Mathf.Lerp(Mathf.Lerp(this.avatarWalkImpulseStength, this.avatarJumpImpulseStength, num), 0f, Mathf.Clamp01(this._avatarHeight / this.avatarFloatHeight));
                    Vector3 vector3 = this._avatarPosition;
                    vector = new Vector4(vector3.x, 0f, vector3.z, w);
                }
                else
                {
                    Vector3 vector4 = this._avatarPosition;
                    vector = new Vector4(vector4.x, 0f, vector4.z, this.avatarWalkImpulseStength);
                    if (this._avatarAnchor.position.y < this.avatarFloatHeight)
                    {
                        this._avatarState = AvatarState.Landing;
                        this._avatarLandingTimer = 0f;
                    }
                }
                this._mpb.SetVector(propName, vector);
                this._renderer.SetPropertyBlock(this._mpb);
            }
        }

        private void Start()
        {
            this._renderer = base.GetComponent<Renderer>();
            this._mpb = new MaterialPropertyBlock();
            this._renderer.GetPropertyBlock(this._mpb);
            AvatarManager instance = Singleton<AvatarManager>.Instance;
            instance.onLocalAvatarChanged = (Action<BaseMonoAvatar, BaseMonoAvatar>) Delegate.Combine(instance.onLocalAvatarChanged, new Action<BaseMonoAvatar, BaseMonoAvatar>(this.OnLocalAvatarChanged));
        }

        private void Update()
        {
            this.SetAvatarImpulse("_RadialWind1");
        }

        private BaseMonoAvatar _avatar
        {
            get
            {
                if (this.__avatar == null)
                {
                    this.__avatar = Singleton<AvatarManager>.Instance.TryGetLocalAvatar();
                    this.GetAvatarAnchor();
                }
                return this.__avatar;
            }
            set
            {
                this.__avatar = value;
                this.GetAvatarAnchor();
            }
        }

        private enum AvatarState
        {
            Walk,
            Float,
            Landing
        }
    }
}

