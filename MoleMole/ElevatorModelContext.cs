namespace MoleMole
{
    using MoleMole.Config;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    public class ElevatorModelContext : BaseWidgetContext
    {
        private MaterialPropertyBlock _block;
        private Coroutine _checkFloorPhase1AnimOverCoroutine;
        private Coroutine _checkFloorPhase2AnimOverCoroutine;
        private Renderer _renderer;
        private int _shaderMaintexID;
        private const string CONFIRMED_IMG_PATH = "GameEntry/Texture/Elevator_Display03";
        private const string IDENTIFY_IMG_PATH = "GameEntry/Texture/Elevator_Display02";
        private const string LOADING_IMG_PATH = "GameEntry/Texture/Elevator_Display01";

        public ElevatorModelContext(GameObject view)
        {
            ContextPattern pattern = new ContextPattern {
                contextName = "ElevatorModelContext",
                viewPrefabPath = "GameEntry/Elevator"
            };
            base.config = pattern;
            base.view = view;
        }

        [DebuggerHidden]
        private IEnumerator CheckFloorPhase1AnimOver(AnimationState floorState)
        {
            return new <CheckFloorPhase1AnimOver>c__Iterator69 { floorState = floorState, <$>floorState = floorState };
        }

        [DebuggerHidden]
        private IEnumerator CheckFloorPhase2AnimOver(AnimationState floorState)
        {
            return new <CheckFloorPhase2AnimOver>c__Iterator6A { floorState = floorState, <$>floorState = floorState };
        }

        public override void Destroy()
        {
            this.StopCheckFloorPhase1AnimOver();
            this.StopCheckFloorPhase2AnimOver();
            base.Destroy();
        }

        private void EnableBackgroundAnim()
        {
            MonoGameEntryElevator component = base.view.GetComponent<MonoGameEntryElevator>();
            if (component != null)
            {
                component.EnablePlayBackgroundAnim = true;
            }
        }

        public void HideSomeParts()
        {
            GameObject view = base.view;
            view.transform.Find("Elevator/Pillar").gameObject.SetActive(false);
            view.transform.Find("Elevator/InnerDoor/Shadow02").gameObject.SetActive(false);
            view.transform.Find("Elevator/Shadow_Left").gameObject.SetActive(false);
            view.transform.Find("Elevator/Shadow_Right").gameObject.SetActive(false);
            view.transform.Find("StartLoading_BG01").gameObject.SetActive(false);
            view.transform.Find("StartLoading_BG02_B").gameObject.SetActive(false);
            view.transform.Find("StartLoading_BG02_F").gameObject.SetActive(false);
        }

        public override bool OnNotify(Notify ntf)
        {
            return ((ntf.type == NotifyTypes.PlayAnimAfterLoad) && this.OnPlayAnimAfterLoad());
        }

        private bool OnPlayAnimAfterLoad()
        {
            this.PlayLoadingAnimation();
            return false;
        }

        public void PlayBackAnimation()
        {
            base.view.GetComponent<Animation>().Play("ElevatorBack", PlayMode.StopAll);
        }

        public void PlayDoorAnimation()
        {
            base.view.GetComponent<Animation>().Play("Door", PlayMode.StopAll);
        }

        public void PlayElevatorAnimation()
        {
            base.view.GetComponent<Animation>().Play("Elevator", PlayMode.StopAll);
        }

        public void PlayFloorPhase1Animation()
        {
            Animation component = base.view.GetComponent<Animation>();
            component.Blend("FloorPhase1");
            AnimationState floorState = null;
            IEnumerator enumerator = component.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    AnimationState current = (AnimationState) enumerator.Current;
                    if (current.name == "FloorPhase1")
                    {
                        floorState = current;
                        goto Label_0078;
                    }
                }
            }
            finally
            {
                IDisposable disposable = enumerator as IDisposable;
                if (disposable == null)
                {
                }
                disposable.Dispose();
            }
        Label_0078:
            this._checkFloorPhase1AnimOverCoroutine = Singleton<ApplicationManager>.Instance.StartCoroutine(this.CheckFloorPhase1AnimOver(floorState));
        }

        public void PlayFloorPhase2Animation()
        {
            Animation component = base.view.GetComponent<Animation>();
            component.Play("FloorPhase2", PlayMode.StopAll);
            AnimationState floorState = null;
            IEnumerator enumerator = component.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    AnimationState current = (AnimationState) enumerator.Current;
                    if (current.name == "FloorPhase2")
                    {
                        floorState = current;
                        goto Label_007A;
                    }
                }
            }
            finally
            {
                IDisposable disposable = enumerator as IDisposable;
                if (disposable == null)
                {
                }
                disposable.Dispose();
            }
        Label_007A:
            this._checkFloorPhase2AnimOverCoroutine = Singleton<ApplicationManager>.Instance.StartCoroutine(this.CheckFloorPhase2AnimOver(floorState));
        }

        public void PlayLoadingAnimation()
        {
            this.PlayElevatorAnimation();
            this.PlayPillarAnimation();
            foreach (MonoElevatorProjectiveLight light in base.view.GetComponentsInChildren<MonoElevatorProjectiveLight>())
            {
                light.enabled = true;
            }
            this.EnableBackgroundAnim();
        }

        public void PlayPillarAnimation()
        {
            base.view.GetComponent<Animation>().Blend("Pillar");
        }

        public void SetDescImage(DescImageType imageType)
        {
            if ((this._renderer != null) && (this._block != null))
            {
                if (imageType == DescImageType.Loading)
                {
                    this._block.SetTexture(this._shaderMaintexID, Miscs.LoadResource<Texture>("GameEntry/Texture/Elevator_Display01", BundleType.RESOURCE_FILE));
                    this._renderer.SetPropertyBlock(this._block);
                }
                else if (imageType == DescImageType.Identifying)
                {
                    this._block.SetTexture(this._shaderMaintexID, Miscs.LoadResource<Texture>("GameEntry/Texture/Elevator_Display02", BundleType.RESOURCE_FILE));
                    this._renderer.SetPropertyBlock(this._block);
                }
                else if (imageType == DescImageType.Confirmed)
                {
                    this._block.SetTexture(this._shaderMaintexID, Miscs.LoadResource<Texture>("GameEntry/Texture/Elevator_Display03", BundleType.RESOURCE_FILE));
                    this._renderer.SetPropertyBlock(this._block);
                }
            }
        }

        protected override bool SetupView()
        {
            this._renderer = base.view.transform.Find("Elevator/Elevator 1/StartLoading_Screen02").GetComponent<MeshRenderer>();
            this._block = new MaterialPropertyBlock();
            this._shaderMaintexID = Shader.PropertyToID("_MainTex");
            base.view.transform.Find("Elevator/OutsideDoor").localPosition = new Vector3(0f, 10f, 0f);
            this.PlayLoadingAnimation();
            return false;
        }

        private void StopCheckFloorPhase1AnimOver()
        {
            if (this._checkFloorPhase1AnimOverCoroutine != null)
            {
                Singleton<ApplicationManager>.Instance.StopCoroutine(this._checkFloorPhase1AnimOverCoroutine);
                this._checkFloorPhase1AnimOverCoroutine = null;
            }
        }

        private void StopCheckFloorPhase2AnimOver()
        {
            if (this._checkFloorPhase2AnimOverCoroutine != null)
            {
                Singleton<ApplicationManager>.Instance.StopCoroutine(this._checkFloorPhase2AnimOverCoroutine);
                this._checkFloorPhase2AnimOverCoroutine = null;
            }
        }

        [CompilerGenerated]
        private sealed class <CheckFloorPhase1AnimOver>c__Iterator69 : IEnumerator, IDisposable, IEnumerator<object>
        {
            internal object $current;
            internal int $PC;
            internal AnimationState <$>floorState;
            internal MonoGameEntry <gameEntry>__0;
            internal AnimationState floorState;

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
                    case 1:
                        if ((this.floorState == null) || (this.floorState.normalizedTime >= 1f))
                        {
                            if (Singleton<MainUIManager>.Instance.SceneCanvas != null)
                            {
                                this.<gameEntry>__0 = Singleton<MainUIManager>.Instance.SceneCanvas as MonoGameEntry;
                                if (this.<gameEntry>__0 != null)
                                {
                                    this.<gameEntry>__0.OnElevatorFloorPhase1AnimOver();
                                }
                            }
                            this.$PC = -1;
                            break;
                        }
                        this.$current = null;
                        this.$PC = 1;
                        return true;
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
        private sealed class <CheckFloorPhase2AnimOver>c__Iterator6A : IEnumerator, IDisposable, IEnumerator<object>
        {
            internal object $current;
            internal int $PC;
            internal AnimationState <$>floorState;
            internal MonoGameEntry <gameEntry>__0;
            internal AnimationState floorState;

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
                    case 1:
                        if ((this.floorState == null) || (this.floorState.normalizedTime >= 1f))
                        {
                            if (Singleton<MainUIManager>.Instance.SceneCanvas != null)
                            {
                                this.<gameEntry>__0 = Singleton<MainUIManager>.Instance.SceneCanvas as MonoGameEntry;
                                if (this.<gameEntry>__0 != null)
                                {
                                    this.<gameEntry>__0.OnElevatorFloorPhase2AnimOver();
                                }
                            }
                            this.$PC = -1;
                            break;
                        }
                        this.$current = null;
                        this.$PC = 1;
                        return true;
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

        public enum DescImageType
        {
            Loading,
            Identifying,
            Confirmed
        }
    }
}

