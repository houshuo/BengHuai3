namespace MoleMole
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    public class MonoFireTriggerProp : MonoTriggerUnitFieldProp
    {
        private string _animEventIDForAttack = "ATK01";
        private float _attackInterval = 0.5f;
        private float _CD;
        private float _defaultCD = 2f;
        private float _defaultEffectDuration = 2f;
        private float _effectDuration;
        private ParticleSystem[] _fireEffect;
        private bool _inForceDisable;
        private Coroutine _waitTriggerHitCoroutine;

        private void DisableFire()
        {
            if (base._triggerCollider.enabled)
            {
                if (this._waitTriggerHitCoroutine != null)
                {
                    base.StopCoroutine(this._waitTriggerHitCoroutine);
                    this._waitTriggerHitCoroutine = null;
                }
                base.ClearInsideColliders();
                base._triggerCollider.enabled = false;
                this.StopEffect();
                base.StartCoroutine(this.WaitEnableFire(this._CD));
            }
        }

        public void EnableFire(float effectDuration, float CD)
        {
            if (!base._triggerCollider.enabled)
            {
                this._inForceDisable = false;
                this._effectDuration = (effectDuration <= 0f) ? this._defaultEffectDuration : effectDuration;
                this._CD = (CD <= 0f) ? this._defaultCD : CD;
                base.ClearInsideColliders();
                base._triggerCollider.enabled = true;
                this.StartEffect();
                this._waitTriggerHitCoroutine = base.StartCoroutine(this.WaitTriggerFireHit());
                base.StartCoroutine(this.WaitDisableFire(this._effectDuration));
            }
        }

        public void ForceDisableFire()
        {
            this._inForceDisable = true;
            base.ClearInsideColliders();
            base._triggerCollider.enabled = false;
            this.StopEffect();
            base.StopAllCoroutines();
        }

        public override void InitUnitFieldPropRange(int numberX, int numberZ)
        {
            base.InitUnitFieldPropRange(numberX, numberZ);
            Transform child = base.gameObject.transform.GetChild(1);
            float length = base.config.PropArguments.Length;
            Vector3 forward = Vector3.forward;
            Vector3 right = Vector3.right;
            for (int i = 0; i < numberX; i++)
            {
                for (int j = 0; j < numberZ; j++)
                {
                    if ((i != 0) || (j != 0))
                    {
                        Transform transform2 = UnityEngine.Object.Instantiate<Transform>(child);
                        transform2.SetParent(base.gameObject.transform);
                        transform2.localPosition = (Vector3) ((child.localPosition + ((right * length) * i)) + ((forward * length) * j));
                    }
                }
            }
            this._fireEffect = base.GetComponentsInChildren<ParticleSystem>();
            this.StopEffect();
        }

        private void StartEffect()
        {
            if (this._fireEffect.Length != 0)
            {
                for (int i = 0; i < this._fireEffect.Length; i++)
                {
                    this._fireEffect[i].Play();
                }
            }
        }

        private void StopEffect()
        {
            if (this._fireEffect.Length != 0)
            {
                for (int i = 0; i < this._fireEffect.Length; i++)
                {
                    this._fireEffect[i].Stop();
                }
            }
        }

        private void TriggerFireHit()
        {
            Singleton<EventManager>.Instance.FireEvent(new EvtFieldHit(base._runtimeID, this._animEventIDForAttack), MPEventDispatchMode.Normal);
        }

        [DebuggerHidden]
        private IEnumerator WaitDisableFire(float effectDuration)
        {
            return new <WaitDisableFire>c__Iterator2A { effectDuration = effectDuration, <$>effectDuration = effectDuration, <>f__this = this };
        }

        [DebuggerHidden]
        private IEnumerator WaitEnableFire(float CD)
        {
            return new <WaitEnableFire>c__Iterator29 { CD = CD, <$>CD = CD, <>f__this = this };
        }

        [DebuggerHidden]
        private IEnumerator WaitTriggerFireHit()
        {
            return new <WaitTriggerFireHit>c__Iterator2B { <>f__this = this };
        }

        [CompilerGenerated]
        private sealed class <WaitDisableFire>c__Iterator2A : IEnumerator, IDisposable, IEnumerator<object>
        {
            internal object $current;
            internal int $PC;
            internal float <$>effectDuration;
            internal MonoFireTriggerProp <>f__this;
            internal float effectDuration;

            [DebuggerHidden]
            public void Dispose()
            {
                this.$PC = -1;
            }

            public bool MoveNext()
            {
                uint num = (uint) this.$PC;
                this.$PC = -1;
                switch (num)
                {
                    case 0:
                        this.$current = new WaitForSeconds(this.effectDuration);
                        this.$PC = 1;
                        return true;

                    case 1:
                        this.<>f__this.DisableFire();
                        this.$PC = -1;
                        break;
                }
                return false;
            }

            [DebuggerHidden]
            public void Reset()
            {
                throw new NotSupportedException();
            }

            object IEnumerator<object>.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }

            object IEnumerator.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }
        }

        [CompilerGenerated]
        private sealed class <WaitEnableFire>c__Iterator29 : IEnumerator, IDisposable, IEnumerator<object>
        {
            internal object $current;
            internal int $PC;
            internal float <$>CD;
            internal MonoFireTriggerProp <>f__this;
            internal float CD;

            [DebuggerHidden]
            public void Dispose()
            {
                this.$PC = -1;
            }

            public bool MoveNext()
            {
                uint num = (uint) this.$PC;
                this.$PC = -1;
                switch (num)
                {
                    case 0:
                        this.$current = new WaitForSeconds(this.CD);
                        this.$PC = 1;
                        return true;

                    case 1:
                        if (!this.<>f__this._inForceDisable)
                        {
                            this.<>f__this.EnableFire(this.<>f__this._effectDuration, this.<>f__this._CD);
                        }
                        this.$PC = -1;
                        break;
                }
                return false;
            }

            [DebuggerHidden]
            public void Reset()
            {
                throw new NotSupportedException();
            }

            object IEnumerator<object>.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }

            object IEnumerator.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }
        }

        [CompilerGenerated]
        private sealed class <WaitTriggerFireHit>c__Iterator2B : IEnumerator, IDisposable, IEnumerator<object>
        {
            internal object $current;
            internal int $PC;
            internal MonoFireTriggerProp <>f__this;

            [DebuggerHidden]
            public void Dispose()
            {
                this.$PC = -1;
            }

            public bool MoveNext()
            {
                uint num = (uint) this.$PC;
                this.$PC = -1;
                switch (num)
                {
                    case 0:
                        this.$current = new WaitForSeconds(this.<>f__this._attackInterval);
                        this.$PC = 1;
                        return true;

                    case 1:
                        this.<>f__this.TriggerFireHit();
                        this.<>f__this._waitTriggerHitCoroutine = this.<>f__this.StartCoroutine(this.<>f__this.WaitTriggerFireHit());
                        this.$PC = -1;
                        break;
                }
                return false;
            }

            [DebuggerHidden]
            public void Reset()
            {
                throw new NotSupportedException();
            }

            object IEnumerator<object>.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }

            object IEnumerator.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }
        }
    }
}

