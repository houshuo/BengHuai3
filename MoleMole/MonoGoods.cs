namespace MoleMole
{
    using FullInspector;
    using MoleMole.Config;
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public class MonoGoods : BaseMonoDynamicObject
    {
        private bool _collisionChecked;
        private bool _hasTriggerEntered;
        private bool _isToBeRemoved;
        private Rigidbody _rigidbody;
        private float _selfRotateAcceleration = 30f;
        private float _selfRotateSpeed = 1100f;
        [ShowInInspector]
        private GoodsState _state;
        public float acceleration = 10f;
        [NonSerialized]
        public bool actDropAnim = true;
        [Header("Attached Effect Pattern")]
        public string AttachEffectPattern;
        public float attractRadius;
        private EntityTimer attractTimer;
        private const string BLUE_TINT_1 = "#0041FF7F";
        private const string BLUE_TINT_2 = "#7081FF7F";
        public const float DEFAULT_HEIGHT = 0.3f;
        [NonSerialized]
        public bool dropAnimFinished;
        public int DropItemLevel;
        public int DropItemMetaID = -1;
        public int DropItemNum;
        public List<MonoEffect> effects;
        [NonSerialized]
        public bool forceFlyToAvatar;
        public const float GOODS_MIN_RADIUS = 0.25f;
        private const string GREEN_TINT_1 = "#00E48F7F";
        private const string GREEN_TINT_2 = "#95FF967F";
        [Header("Inside Effect Pattern")]
        public string InsideEffectPattern;
        private const float MAX_ATTRACT_TIME = 2.5f;
        [NonSerialized]
        public bool muteSound;
        private const string PURPLE_TINT_1 = "#3700FF7F";
        private const string PURPLE_TINT_2 = "#8E72FF7F";
        public int reboundTimes;
        private const float ROTATE_SPEED_ON_GROUND = 100f;
        public float speed;

        protected void Awake()
        {
            this._rigidbody = base.GetComponent<Rigidbody>();
            this._rigidbody.useGravity = false;
            this._hasTriggerEntered = false;
            this.attractTimer = new EntityTimer(2.5f);
            this.attractTimer.SetActive(false);
        }

        private void CheckBarrierCollider()
        {
            if ((this.actDropAnim && (!this.actDropAnim || !this.dropAnimFinished)) && !this._collisionChecked)
            {
                BaseMonoAvatar localAvatar = Singleton<AvatarManager>.Instance.GetLocalAvatar();
                if (localAvatar != null)
                {
                    Vector3 start = new Vector3(localAvatar.transform.position.x, 0.01f, localAvatar.transform.position.z);
                    Vector3 vector2 = new Vector3(this._rigidbody.velocity.x, 0f, this._rigidbody.velocity.z);
                    Vector3 vector3 = new Vector3(base.transform.position.x, 0.01f, base.transform.position.z);
                    Vector3 end = vector3 + ((Vector3) ((vector2 * Time.deltaTime) * this.TimeScale));
                    if (Physics.Linecast(start, end, (int) ((((int) 1) << InLevelData.OBSTACLE_COLLIDER_LAYER) | (((int) 1) << InLevelData.STAGE_COLLIDER_LAYER))))
                    {
                        this._rigidbody.velocity = new Vector3(0f, this._rigidbody.velocity.y, 0f);
                        this._collisionChecked = true;
                    }
                }
            }
        }

        public override bool IsActive()
        {
            return !this._isToBeRemoved;
        }

        public override bool IsToBeRemove()
        {
            return this._isToBeRemoved;
        }

        public void OnTriggerEnter(Collider other)
        {
            if (!this._hasTriggerEntered)
            {
                BaseMonoEntity componentInParent = other.GetComponentInParent<BaseMonoEntity>();
                if ((componentInParent != null) && Singleton<AvatarManager>.Instance.IsLocalAvatar(componentInParent.GetRuntimeID()))
                {
                    Singleton<EventManager>.Instance.FireEvent(new EvtFieldEnter(base._runtimeID, componentInParent.GetRuntimeID()), MPEventDispatchMode.Normal);
                    this._hasTriggerEntered = true;
                }
            }
        }

        public void SetAttractTimerActive(bool isActive)
        {
            this.attractTimer.SetActive(isActive);
        }

        public override void SetDied()
        {
            base.SetDied();
            this._isToBeRemoved = true;
            Singleton<EffectManager>.Instance.ClearEffectsByOwner(base._runtimeID);
        }

        public void SetOutsideParticleColorByRarity(GameObject obj, int rarity)
        {
            ParticleSystem[] componentsInChildren = obj.GetComponentsInChildren<ParticleSystem>();
            int index = 0;
            int length = componentsInChildren.Length;
            while (index < length)
            {
                Renderer component = componentsInChildren[index].GetComponent<Renderer>();
                if ((component != null) && (component.material.shader.name.IndexOf("Channel Mix") != -1))
                {
                    Color black = Color.black;
                    Color color = Color.black;
                    if ((rarity == 1) || (rarity == 2))
                    {
                        black = Miscs.ParseColor("#00E48F7F");
                        color = Miscs.ParseColor("#95FF967F");
                    }
                    else if (rarity == 3)
                    {
                        black = Miscs.ParseColor("#0041FF7F");
                        color = Miscs.ParseColor("#7081FF7F");
                    }
                    else if (((rarity == 4) || (rarity == 5)) || (rarity == 6))
                    {
                        black = Miscs.ParseColor("#3700FF7F");
                        color = Miscs.ParseColor("#8E72FF7F");
                    }
                    component.material.SetColor("_TintColor1", black);
                    component.material.SetColor("_TintColor2", color);
                }
                index++;
            }
        }

        protected override void Start()
        {
            base.Start();
            if (this.actDropAnim)
            {
                this._rigidbody.useGravity = true;
                this._rigidbody.velocity = new Vector3(UnityEngine.Random.Range((float) -1f, (float) 1f) * 2f, UnityEngine.Random.Range((float) 1.5f, (float) 2f) * 2f, UnityEngine.Random.Range((float) -1f, (float) 1f) * 2f);
                this._rigidbody.AddForce((Vector3) ((Vector3.down * 3f) * this._rigidbody.mass));
                this.dropAnimFinished = false;
                this.reboundTimes = 2;
            }
            else
            {
                this.dropAnimFinished = true;
                this.reboundTimes = -1;
                UnityEngine.Object.Destroy(this._rigidbody);
                this._rigidbody = null;
                base.transform.SetLocalPositionY(0.3f);
            }
        }

        protected override void Update()
        {
            float num;
            if (!this.actDropAnim)
            {
                num = (Time.deltaTime * 100f) * this.TimeScale;
            }
            else if (this.actDropAnim && !this.dropAnimFinished)
            {
                num = (Time.deltaTime * this._selfRotateSpeed) * this.TimeScale;
                this.CheckBarrierCollider();
            }
            else
            {
                this._selfRotateAcceleration = (this._selfRotateAcceleration >= 10f) ? (this._selfRotateAcceleration - 3f) : 10f;
                this._selfRotateSpeed = (this._selfRotateSpeed >= 100f) ? (this._selfRotateSpeed - this._selfRotateAcceleration) : 100f;
                num = (Time.deltaTime * this._selfRotateSpeed) * this.TimeScale;
            }
            base.transform.Rotate(base.transform.up, num);
            if (this._state == GoodsState.Appear)
            {
                this._state = GoodsState.Idle;
            }
            else if (((this._state == GoodsState.Idle) && (base.transform.position.y <= 0.3f)) && ((this._rigidbody != null) && (this._rigidbody.velocity.y <= 0f)))
            {
                if (this.reboundTimes > 0)
                {
                    this._rigidbody.velocity = new Vector3(this._rigidbody.velocity.x, -0.8f * this._rigidbody.velocity.y, this._rigidbody.velocity.z);
                    this.reboundTimes--;
                }
                else if (!this.dropAnimFinished)
                {
                    this.dropAnimFinished = true;
                    UnityEngine.Object.Destroy(this._rigidbody);
                    this._rigidbody = null;
                    base.transform.SetLocalPositionY(0.3f);
                    if (!string.IsNullOrEmpty(this.AttachEffectPattern) && (Singleton<EventManager>.Instance.GetActor<EquipItemActor>(base.GetRuntimeID()) != null))
                    {
                        bool flag = true;
                        switch (GraphicsSettingData.GetGraphicsRecommendGrade())
                        {
                            case GraphicsRecommendGrade.Off:
                            case GraphicsRecommendGrade.Low:
                            {
                                ConfigGraphicsPersonalSetting personalGraphicsSetting = Singleton<MiHoYoGameData>.Instance.GeneralLocalData.PersonalGraphicsSetting;
                                if (!personalGraphicsSetting.IsUserDefinedGrade)
                                {
                                    flag = false;
                                }
                                else if (personalGraphicsSetting.RecommendGrade != GraphicsRecommendGrade.High)
                                {
                                    flag = false;
                                }
                                break;
                            }
                        }
                        if (flag)
                        {
                            int rarity = Singleton<EventManager>.Instance.GetActor<EquipItemActor>(base.GetRuntimeID()).rarity;
                            this.effects = Singleton<EffectManager>.Instance.TriggerEntityEffectPatternReturnValue(this.AttachEffectPattern, this, true);
                            int num3 = 0;
                            int count = this.OutsideEffects.Count;
                            while (num3 < count)
                            {
                                this.SetOutsideParticleColorByRarity(this.OutsideEffects[num3].gameObject, rarity);
                                num3++;
                            }
                        }
                    }
                }
            }
            BaseMonoAvatar localAvatar = Singleton<AvatarManager>.Instance.GetLocalAvatar();
            if (this._state == GoodsState.Idle)
            {
                if (this.dropAnimFinished || (this.reboundTimes <= 0))
                {
                    float property = localAvatar.GetProperty("Actor_GoodsAttrackRadius");
                    property = Mathf.Clamp(property, 0f, property);
                    this.attractRadius = localAvatar.config.CommonArguments.GoodsAttractRadius * property;
                    if (Vector3.Distance(this.XZPosition, localAvatar.XZPosition) < this.attractRadius)
                    {
                        this._state = GoodsState.Attract;
                        this.attractTimer.SetActive(false);
                        if (!string.IsNullOrEmpty(this.AttachEffectPattern) && (Singleton<EventManager>.Instance.GetActor<EquipItemActor>(base.GetRuntimeID()) != null))
                        {
                            foreach (MonoEffect effect in this.effects)
                            {
                                if (effect != null)
                                {
                                    effect.SetDestroyImmediately();
                                }
                            }
                        }
                    }
                }
            }
            else if ((this._state == GoodsState.Attract) || this.forceFlyToAvatar)
            {
                this.speed += (this.acceleration * this.TimeScale) * Time.deltaTime;
                this.speed = (this.speed >= 20f) ? 20f : this.speed;
                Vector3 vector7 = localAvatar.RootNodePosition - base.transform.position;
                Vector3 vector = (Vector3) ((vector7.normalized * this.speed) * this.TimeScale);
                Transform transform = base.transform;
                transform.position += (Vector3) (vector * Time.deltaTime);
                this.attractTimer.Core(1f);
                if ((Vector3.Distance(this.XZPosition, localAvatar.XZPosition) < localAvatar.config.CommonArguments.CollisionRadius) || this.attractTimer.isTimeUp)
                {
                    this.OnTriggerEnter(localAvatar.hitbox);
                }
            }
            base.Update();
        }

        public List<MonoEffect> OutsideEffects
        {
            get
            {
                return this.effects;
            }
        }

        public GoodsState state
        {
            set
            {
                this._state = value;
            }
        }

        public enum GoodsState
        {
            Appear,
            Idle,
            Attract,
            Consumed
        }
    }
}

