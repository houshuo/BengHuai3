namespace MoleMole
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public class HintArrowManager
    {
        private MonoHintArrow _hintArrowForPath;
        private List<MonoHintArrow> _hintArrowLs;
        private GameObject _hintRing;
        private Animation _hintRingAnim;
        private AnimationState _hintRingOutAnimState;
        private bool _hintRingVisible;
        private MonoSpawnPoint _pathSpawn;
        public const string AVATAR_HINT_PATH = "UI/HintArrowAlt/HintArrowAvatarAlt";
        private const float CHANGE_DIR_LERP_RATIO = 10f;
        public const string EXIT_HINT_PATH = "UI/HintArrowAlt/HintArrowExitAlt";
        private const string HINT_RING_PATH = "UI/HintArrowAlt/RingAlt";
        public const string MONSTER_HINT_PATH = "UI/HintArrowAlt/HintArrowMonsterAlt";
        private const string RING_IN_ANIM = "RingAltIn";
        private const string RING_OUT_ANIM = "RingAltOut";

        public void AddHintArrow(uint listenRuntimeID)
        {
            BaseMonoEntity listenEntity = Singleton<EventManager>.Instance.GetEntity(listenRuntimeID);
            string path = string.Empty;
            if (Singleton<RuntimeIDManager>.Instance.ParseCategory(listenRuntimeID) == 4)
            {
                path = "UI/HintArrowAlt/HintArrowMonsterAlt";
            }
            else if (Singleton<RuntimeIDManager>.Instance.ParseCategory(listenRuntimeID) == 3)
            {
                path = "UI/HintArrowAlt/HintArrowAvatarAlt";
            }
            else
            {
                if (!(listenEntity is BaseMonoDynamicObject) || (Singleton<EventManager>.Instance.GetActor(listenEntity.GetRuntimeID()) == null))
                {
                    throw new Exception("Invalid Type or State!");
                }
                path = "UI/HintArrowAlt/HintArrowExitAlt";
            }
            MonoHintArrow component = UnityEngine.Object.Instantiate<GameObject>(Miscs.LoadResource<GameObject>(path, BundleType.RESOURCE_FILE)).GetComponent<MonoHintArrow>();
            component.Init(listenRuntimeID, listenEntity);
            component.transform.SetParent(this._hintRing.transform, false);
            this._hintArrowLs.Add(component);
        }

        public void AddHintArrowForPath(MonoSpawnPoint spawn)
        {
            if (this._hintArrowForPath != null)
            {
                this.RemoveHintArrowForPath();
            }
            string path = "UI/HintArrowAlt/HintArrowExitAlt";
            MonoHintArrow component = UnityEngine.Object.Instantiate<GameObject>(Miscs.LoadResource<GameObject>(path, BundleType.RESOURCE_FILE)).GetComponent<MonoHintArrow>();
            component.Init(0, null);
            component.transform.SetParent(this._hintRing.transform, false);
            this._pathSpawn = spawn;
            this._hintArrowForPath = component;
            this._hintArrowForPath.SetVisible(true);
            this.UpdateHintArrow(component.transform, spawn.XZPosition, false);
        }

        public void Core()
        {
            this.UpdateAllHintArrow();
        }

        public MonoSpawnPoint GetSpawnPoint()
        {
            return this._pathSpawn;
        }

        public void InitAtStart()
        {
            this._hintRing = UnityEngine.Object.Instantiate<GameObject>(Miscs.LoadResource<GameObject>("UI/HintArrowAlt/RingAlt", BundleType.RESOURCE_FILE));
            this._hintRing.name = "_HintRing";
            this._hintRing.SetActive(false);
            this._hintRingVisible = false;
            this._hintRingAnim = this._hintRing.GetComponent<Animation>();
            this._hintRingOutAnimState = this._hintRingAnim["RingAltOut"];
            this._hintArrowLs = new List<MonoHintArrow>();
        }

        public void RemoveHintArrowForPath()
        {
            if (this._hintArrowForPath != null)
            {
                this._hintArrowForPath.SetVisible(false);
                this._hintArrowForPath.SetDestroyUponFadeOut();
            }
        }

        private void SetHintArrowByScreenPos(MonoHintArrow arrow, BaseMonoEntity entity)
        {
            bool flag = (((entity != null) && entity.IsActive()) && !Singleton<AvatarManager>.Instance.IsLocalAvatar(entity.GetRuntimeID())) && !Singleton<CameraManager>.Instance.GetMainCamera().IsEntityVisible(entity);
            bool isArrowVisibleBefore = (arrow.state == MonoHintArrow.State.Visible) || (arrow.state == MonoHintArrow.State.FadingIn);
            bool flag3 = (entity == null) || entity.IsToBeRemove();
            if ((arrow.state == MonoHintArrow.State.Hidden) && flag3)
            {
                UnityEngine.Object.Destroy(arrow.gameObject);
            }
            else if (isArrowVisibleBefore && !flag)
            {
                arrow.SetVisible(false);
                if (flag3)
                {
                    arrow.SetDestroyUponFadeOut();
                }
            }
            else if (!isArrowVisibleBefore && flag)
            {
                arrow.SetVisible(true);
            }
            if (flag)
            {
                this.UpdateHintArrow(arrow.transform, entity.XZPosition, isArrowVisibleBefore);
            }
        }

        private void SetHintRingPosition()
        {
            BaseMonoAvatar avatar = Singleton<AvatarManager>.Instance.TryGetLocalAvatar();
            if (avatar != null)
            {
                Vector3 position = avatar.transform.position;
                position.y = this._hintRing.transform.position.y;
                this._hintRing.transform.position = position;
            }
        }

        public void SetHintRingVisible(bool visible)
        {
            if (visible)
            {
                if (!this._hintRing.gameObject.activeSelf)
                {
                    this._hintRing.gameObject.SetActive(true);
                }
                this._hintRingAnim.Play("RingAltIn");
            }
            else
            {
                this._hintRingAnim.Play("RingAltOut");
            }
            this._hintRingVisible = visible;
        }

        public void TriggerHintArrowEffect(uint runtimeID, MonoHintArrow.EffectType effectType)
        {
            for (int i = 0; i < this._hintArrowLs.Count; i++)
            {
                if ((this._hintArrowLs[i] != null) && (this._hintArrowLs[i].listenRuntimID == runtimeID))
                {
                    this._hintArrowLs[i].TriggerEffect(effectType);
                    return;
                }
            }
        }

        private void UpdateAllHintArrow()
        {
            this.SetHintRingPosition();
            bool visible = false;
            for (int i = 0; i < this._hintArrowLs.Count; i++)
            {
                if (this._hintArrowLs[i] != null)
                {
                    MonoHintArrow arrow = this._hintArrowLs[i];
                    this.SetHintArrowByScreenPos(arrow, arrow.listenEntity);
                    if (arrow.state != MonoHintArrow.State.Hidden)
                    {
                        visible = true;
                    }
                }
            }
            this.UpdateHintArrowForPath();
            visible |= this._hintArrowForPath != null;
            if (visible != this._hintRingVisible)
            {
                this.SetHintRingVisible(visible);
            }
            if (!this._hintRingVisible && (this._hintRingOutAnimState.normalizedTime > 1f))
            {
                this._hintRing.SetActive(false);
            }
        }

        private void UpdateHintArrow(Transform hintArrowTrans, Vector3 targetXZPosition, bool isArrowVisibleBefore)
        {
            Vector3 forward = hintArrowTrans.forward;
            Vector3 b = targetXZPosition - Singleton<AvatarManager>.Instance.GetLocalAvatar().XZPosition;
            b.y = 0f;
            b.Normalize();
            if (isArrowVisibleBefore)
            {
                hintArrowTrans.forward = Vector3.Lerp(forward, b, (10f * Singleton<LevelManager>.Instance.levelEntity.TimeScale) * Time.deltaTime);
            }
            else
            {
                hintArrowTrans.forward = b;
            }
        }

        private void UpdateHintArrowForPath()
        {
            if ((this._hintArrowForPath != null) && (this._pathSpawn != null))
            {
                this.UpdateHintArrow(this._hintArrowForPath.transform, this._pathSpawn.XZPosition, true);
            }
        }
    }
}

