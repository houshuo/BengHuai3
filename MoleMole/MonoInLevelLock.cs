namespace MoleMole
{
    using System;
    using UnityEngine;

    public class MonoInLevelLock : MonoBehaviour
    {
        private BaseMonoEntity _lockFollowTarget;
        private Status _status;
        private float _timer;
        private bool _triggerBegin;
        private bool _triggerEnd;
        private const float BEGIN_EFFECT_PLAY_TIME = 0.2f;
        public ParticleSystem beginEffect;
        public ParticleSystem endEffect;
        public ParticleSystem loopEffect;

        public void Awake()
        {
            this.ClearAllEffect();
        }

        private void ClearAllEffect()
        {
            this.beginEffect.Clear();
            this.loopEffect.Clear();
            this.endEffect.Clear();
            this.beginEffect.gameObject.SetActive(false);
            this.loopEffect.gameObject.SetActive(false);
            this.endEffect.gameObject.SetActive(false);
            this._timer = 0f;
        }

        public void Core()
        {
            if (!GlobalVars.muteInlevelLock)
            {
                if (this._lockFollowTarget != null)
                {
                    if (this._lockFollowTarget.IsActive())
                    {
                        Vector3 position = this._lockFollowTarget.GetAttachPoint("RootNode").position;
                        Camera cameraComponent = Singleton<CameraManager>.Instance.GetMainCamera().cameraComponent;
                        Vector3 vector2 = cameraComponent.WorldToViewportPoint(position);
                        vector2.z = 10f;
                        base.transform.position = cameraComponent.ViewportToWorldPoint(vector2);
                    }
                    else
                    {
                        this.Reset();
                    }
                }
                if (this._status == Status.Begin)
                {
                    if (this._triggerBegin)
                    {
                        base.gameObject.SetActive(true);
                        this.ClearAllEffect();
                        this.beginEffect.gameObject.SetActive(true);
                        this._triggerBegin = false;
                    }
                    if (this._timer <= 0.2f)
                    {
                        this._timer += Time.unscaledDeltaTime;
                    }
                    else
                    {
                        this._status = Status.Loop;
                        this.loopEffect.gameObject.SetActive(true);
                    }
                }
                else if (this._status == Status.End)
                {
                    if (this._triggerEnd)
                    {
                        this.loopEffect.gameObject.SetActive(false);
                        this.endEffect.gameObject.SetActive(true);
                        this._triggerEnd = false;
                    }
                    if (!this.endEffect.IsAlive(false))
                    {
                        this.Reset();
                    }
                }
            }
        }

        private void Reset()
        {
            this._status = Status.None;
            base.gameObject.SetActive(false);
            this._lockFollowTarget = null;
        }

        public void SetLockFollowTarget(BaseMonoEntity lockFollowTarget)
        {
            BaseMonoEntity entity = (this._status != Status.End) ? this._lockFollowTarget : null;
            if ((entity != null) && (lockFollowTarget == null))
            {
                this._status = Status.End;
                this._triggerEnd = true;
            }
            else if (entity != lockFollowTarget)
            {
                this._lockFollowTarget = lockFollowTarget;
                this._status = Status.Begin;
                this._triggerBegin = true;
            }
        }

        public enum Status
        {
            None,
            Begin,
            Loop,
            End
        }
    }
}

