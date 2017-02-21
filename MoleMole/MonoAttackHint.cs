namespace MoleMole
{
    using MoleMole.Config;
    using System;
    using UnityEngine;

    public class MonoAttackHint : MonoAuxObject
    {
        private ConfigMonsterAttackHint _config;
        private float _innerMaxScale;
        private float _innerMinScale;
        private InnerState _innerState;
        private MaterialPropertyBlockFader _mpb_flagFader;
        private MaterialPropertyBlockFader _mpb_innerFader;
        private MaterialPropertyBlockFader _mpb_outerFader;
        private OuterState _outerState;
        private BaseMonoAnimatorEntity _owner;
        private Material _sharedFlagMaterial;
        private float _timer;
        public AnimationCurve fadeOutCurve;
        public float fadeOutTime = 0.3f;
        public Transform flag;
        public float flagBorderOffset = 0.1f;
        public float flagGap = 0.8f;
        public AnimationCurve inflateCurve;
        public Transform innerHint;
        private const int MAX_FLAG_COUNT = 6;
        private const float OUTER_FADE_IN_DURATION = 0.2f;
        public AnimationCurve outerFadeCurve;
        public Transform outerHint;

        public MonoAttackHint()
        {
            Keyframe[] keys = new Keyframe[] { new Keyframe(0f, 0f), new Keyframe(1f, 1f) };
            this.inflateCurve = new AnimationCurve(keys);
            Keyframe[] keyframeArray2 = new Keyframe[] { new Keyframe(0f, 0f), new Keyframe(1f, 1f) };
            this.outerFadeCurve = new AnimationCurve(keyframeArray2);
            Keyframe[] keyframeArray3 = new Keyframe[] { new Keyframe(0f, 0f), new Keyframe(1f, 1f) };
            this.fadeOutCurve = new AnimationCurve(keyframeArray3);
        }

        private void Awake()
        {
            this._sharedFlagMaterial = this.flag.GetComponent<Renderer>().sharedMaterial;
            this.flag.gameObject.SetActive(false);
            this.innerHint.SetLocalPositionY(0.0001f);
            this._mpb_flagFader = new MaterialPropertyBlockFader(this.flag.GetComponent<Renderer>(), "_TintColor");
            this._mpb_outerFader = new MaterialPropertyBlockFader(this.outerHint.GetComponent<Renderer>(), "_TintColor");
            this._mpb_innerFader = new MaterialPropertyBlockFader(this.innerHint.GetComponent<Renderer>(), "_TintColor");
            this.outerHint.SetLocalPositionY(0.02f);
            this.innerHint.SetLocalPositionY(0.06f);
        }

        public void Init(BaseMonoAnimatorEntity owner, BaseMonoAnimatorEntity target, ConfigMonsterAttackHint config)
        {
            this._timer = 0f;
            this._owner = owner;
            this._config = config;
            this._outerState = OuterState.Glow;
            this._innerState = InnerState.Hide;
            if (config is RectAttackHint)
            {
                RectAttackHint hint = (RectAttackHint) config;
                if ((config.OffsetBase == HintOffsetBase.Target) && (target != null))
                {
                    base.transform.position = target.XZPosition + ((Vector3) (target.FaceDirection * (hint.OffsetZ + (0.5f * hint.Distance))));
                }
                else if (config.OffsetBase == HintOffsetBase.Owner)
                {
                    base.transform.position = owner.XZPosition + ((Vector3) (owner.FaceDirection * (hint.OffsetZ + (0.5f * hint.Distance))));
                }
                base.transform.forward = owner.FaceDirection;
                base.transform.SetLocalScaleX(hint.Width);
                base.transform.SetLocalScaleZ(hint.Distance);
                this._innerMinScale = 0f;
                this._innerMaxScale = 1f;
            }
            else
            {
                Vector3 vector5;
                if (config is CircleAttackHint)
                {
                    CircleAttackHint hint2 = (CircleAttackHint) config;
                    if ((config.OffsetBase == HintOffsetBase.Target) && (target != null))
                    {
                        base.transform.position = target.XZPosition + ((Vector3) (target.FaceDirection * hint2.OffsetZ));
                    }
                    else if (config.OffsetBase == HintOffsetBase.Owner)
                    {
                        base.transform.position = owner.XZPosition + ((Vector3) (owner.FaceDirection * hint2.OffsetZ));
                    }
                    base.transform.forward = owner.FaceDirection;
                    vector5 = (Vector3) (Vector3.one * hint2.Radius);
                    this.outerHint.localScale = vector5;
                    this.innerHint.localScale = vector5;
                    this._innerMinScale = 0f;
                    this._innerMaxScale = hint2.Radius;
                    this.outerHint.GetComponent<MeshRenderer>().material.SetFloat("_FanAngle", 6.283185f);
                    this.innerHint.GetComponent<MeshRenderer>().material.SetFloat("_FanAngle", 6.283185f);
                    GameObject obj2 = new GameObject("Flags");
                    obj2.transform.SetParent(base.transform);
                    Vector3 zero = Vector3.zero;
                    zero.y = 0.04f;
                    obj2.transform.localPosition = zero;
                    int num = Mathf.Min(Mathf.FloorToInt((hint2.Radius * 7f) / this.flagGap), 6);
                    float num2 = (6.283185f / ((float) num)) * 57.29578f;
                    for (int i = 0; i < num; i++)
                    {
                        Transform transform = UnityEngine.Object.Instantiate<Transform>(this.flag);
                        transform.SetParent(obj2.transform, false);
                        transform.gameObject.SetActive(true);
                        transform.localRotation = Quaternion.AngleAxis(num2 * i, Vector3.up);
                        Vector3 vector2 = (Vector3) (transform.localRotation * (Vector3.forward * (hint2.Radius - this.flagBorderOffset)));
                        transform.localPosition = vector2;
                        transform.GetComponent<Renderer>().sharedMaterial = this._sharedFlagMaterial;
                    }
                }
                else if (config is SectorAttackHint)
                {
                    SectorAttackHint hint3 = (SectorAttackHint) config;
                    if ((config.OffsetBase == HintOffsetBase.Target) && (target != null))
                    {
                        base.transform.position = target.XZPosition + ((Vector3) (target.FaceDirection * hint3.OffsetZ));
                    }
                    else if (config.OffsetBase == HintOffsetBase.Owner)
                    {
                        base.transform.position = owner.XZPosition + ((Vector3) (owner.FaceDirection * hint3.OffsetZ));
                    }
                    base.transform.forward = owner.FaceDirection;
                    vector5 = (Vector3) (Vector3.one * hint3.Radius);
                    this.outerHint.localScale = vector5;
                    this.innerHint.localScale = vector5;
                    this._innerMinScale = 0f;
                    this._innerMaxScale = hint3.Radius;
                    this.outerHint.GetComponent<MeshRenderer>().material.SetFloat("_FanAngle", 0.01745329f * hint3.Angle);
                    this.innerHint.GetComponent<MeshRenderer>().material.SetFloat("_FanAngle", 0.01745329f * hint3.Angle);
                    GameObject obj3 = new GameObject("Flags");
                    obj3.transform.SetParent(base.transform);
                    Vector3 vector3 = Vector3.zero;
                    vector3.y = 0.04f;
                    obj3.transform.localPosition = vector3;
                    obj3.transform.forward = owner.FaceDirection;
                    int num4 = Mathf.Min(Mathf.FloorToInt((((((hint3.Radius * 7f) * 0.01745329f) * hint3.Angle) / 3.141593f) / 2f) / this.flagGap), 6);
                    float num5 = ((0.01745329f * hint3.Angle) / ((float) num4)) * 57.29578f;
                    bool flag = (num4 % 2) == 0;
                    for (int j = 0; j < num4; j++)
                    {
                        Transform transform2 = UnityEngine.Object.Instantiate<Transform>(this.flag);
                        transform2.SetParent(obj3.transform, false);
                        transform2.gameObject.SetActive(true);
                        if (flag)
                        {
                            transform2.localRotation = ((j % 2) != 0) ? Quaternion.AngleAxis(-num5 * ((j / 2) + 0.5f), Vector3.up) : Quaternion.AngleAxis(num5 * ((j / 2) + 0.5f), Vector3.up);
                        }
                        else if (j == 0)
                        {
                            transform2.localRotation = Quaternion.AngleAxis(0f, Vector3.up);
                        }
                        else
                        {
                            transform2.localRotation = ((j % 2) != 0) ? Quaternion.AngleAxis(-num5 * j, Vector3.up) : Quaternion.AngleAxis(num5 * j, Vector3.up);
                        }
                        Vector3 vector4 = (Vector3) (transform2.localRotation * (Vector3.forward * (hint3.Radius - this.flagBorderOffset)));
                        transform2.localPosition = vector4;
                        transform2.GetComponent<Renderer>().sharedMaterial = this._sharedFlagMaterial;
                    }
                }
            }
        }

        private void Start()
        {
            this.innerHint.localScale = Vector3.zero;
            this._mpb_flagFader.LerpAlpha(0f);
            this._mpb_outerFader.LerpAlpha(0f);
        }

        private void Update()
        {
            if (this._outerState == OuterState.Glow)
            {
                float t = this._timer / 0.2f;
                this._mpb_flagFader.LerpAlpha(t);
                this._mpb_outerFader.LerpAlpha(t);
                if (this._timer > 0.2f)
                {
                    this._outerState = OuterState.Idle;
                }
            }
            else if (this._outerState == OuterState.Idle)
            {
            }
            if (this._innerState == InnerState.Hide)
            {
                if (this._timer > this._config.InnerStartDelay)
                {
                    this._innerState = InnerState.Inflate;
                }
            }
            else if (this._innerState == InnerState.Inflate)
            {
                float time = (this._timer - this._config.InnerStartDelay) / this._config.InnerInflateDuration;
                if (time > 1f)
                {
                    this.innerHint.localScale = (Vector3) (Vector3.one * this._innerMaxScale);
                    this._innerState = InnerState.FadeOut;
                }
                else
                {
                    this._mpb_flagFader.LerpAlpha(this.outerFadeCurve.Evaluate(time));
                    this._mpb_outerFader.LerpAlpha(this.outerFadeCurve.Evaluate(time));
                    this.innerHint.localScale = (Vector3) (Vector3.one * Mathf.Lerp(this._innerMinScale, this._innerMaxScale, this.inflateCurve.Evaluate(time)));
                }
            }
            else if (this._innerState == InnerState.FadeOut)
            {
                float num3 = ((this._timer - this._config.InnerStartDelay) - this._config.InnerInflateDuration) / this.fadeOutTime;
                if (num3 > 1f)
                {
                    base.SetDestroy();
                }
                else
                {
                    this._mpb_innerFader.LerpAlpha(this.fadeOutCurve.Evaluate(num3));
                }
            }
            this._timer += Time.deltaTime * this._owner.TimeScale;
        }

        private enum InnerState
        {
            Hide,
            Inflate,
            FadeOut
        }

        private enum OuterState
        {
            Glow,
            Idle
        }
    }
}

