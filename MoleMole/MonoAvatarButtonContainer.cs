namespace MoleMole
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    public class MonoAvatarButtonContainer : MonoBehaviour
    {
        private Coroutine _buttonAnimateCoroutine;
        private FastForwardType _fastForwardType;
        private uint _localRuntimeIDAfter;
        private uint _localRuntimeIDBefore;
        public State _state;
        [CompilerGenerated]
        private static Comparison<MonoAvatarButton> <>f__am$cache6;
        private const string AVATAR_BUTTON_PREFAB_PATH = "UI/Menus/Widget/InLevel/AvatarButton";
        [NonSerialized]
        public List<MonoAvatarButton> avatarBtnList;
        private const float BUTTON_ANIM_DURATION = 0.25f;

        public void AddAvatarButton(uint runtimeID)
        {
            if (this.avatarBtnList == null)
            {
                this.avatarBtnList = new List<MonoAvatarButton>();
            }
            GameObject obj2 = UnityEngine.Object.Instantiate<GameObject>(Miscs.LoadResource<GameObject>("UI/Menus/Widget/InLevel/AvatarButton", BundleType.RESOURCE_FILE));
            obj2.transform.SetParent(base.transform, false);
            MonoAvatarButton component = obj2.GetComponent<MonoAvatarButton>();
            component.Init(runtimeID);
            this.avatarBtnList.Add(component);
            this.ResortAvatarBtns(Singleton<AvatarManager>.Instance.GetLocalAvatar().GetRuntimeID());
            if (Singleton<AvatarManager>.Instance.IsLocalAvatar(runtimeID))
            {
                base.StartCoroutine(this.HideFirstLocalAvatarButton(component));
            }
        }

        private void FastForwardBtnAnimation()
        {
            if (this._fastForwardType == FastForwardType.Btn_1_ClickWith_2_Avatar)
            {
                this.avatarBtnList[1].gameObject.SetActive(true);
                this.StopRewindAndClearAlpha(this.avatarBtnList[0]);
                this.StopRewindAndClearAlpha(this.avatarBtnList[1]);
            }
            else if (this._fastForwardType == FastForwardType.Btn_1_ClickWith_3_Avatar)
            {
                this.avatarBtnList[2].gameObject.SetActive(true);
                this.StopRewindAndClearAlpha(this.avatarBtnList[0]);
                this.StopRewindAndClearAlpha(this.avatarBtnList[1]);
                this.StopRewindAndClearAlpha(this.avatarBtnList[2]);
            }
            else if (this._fastForwardType == FastForwardType.Btn_2_ClickWith_3_Avatar)
            {
                this.avatarBtnList[2].gameObject.SetActive(true);
                this.StopRewindAndClearAlpha(this.avatarBtnList[1]);
                this.StopRewindAndClearAlpha(this.avatarBtnList[2]);
            }
            this.OnSwapAvatarAnimEnd();
            this.SetButtonAvailable(true);
        }

        public MonoAvatarButton GetAvatarButtonByRuntimeID(uint runtimeID)
        {
            <GetAvatarButtonByRuntimeID>c__AnonStorey106 storey = new <GetAvatarButtonByRuntimeID>c__AnonStorey106 {
                runtimeID = runtimeID
            };
            return this.avatarBtnList.Find(new Predicate<MonoAvatarButton>(storey.<>m__1B7));
        }

        [DebuggerHidden]
        private IEnumerator HideFirstLocalAvatarButton(MonoAvatarButton button)
        {
            return new <HideFirstLocalAvatarButton>c__Iterator6E { button = button, <$>button = button };
        }

        private void OnEnable()
        {
            if ((this._state == State.Animating) && (this._buttonAnimateCoroutine != null))
            {
                base.StopCoroutine(this._buttonAnimateCoroutine);
                this._buttonAnimateCoroutine = null;
                this.FastForwardBtnAnimation();
            }
        }

        private void OnSwapAvatarAnimEnd()
        {
            this.SetAvatarButtonActive(this._localRuntimeIDBefore, true);
            this.SetAvatarButtonActive(this._localRuntimeIDAfter, false);
            this.ResortAvatarBtns(this._localRuntimeIDAfter);
            this._localRuntimeIDBefore = 0;
            this._localRuntimeIDAfter = 0;
            base.GetComponent<GridLayoutGroup>().enabled = true;
            this._buttonAnimateCoroutine = null;
            this._state = State.Idle;
        }

        [DebuggerHidden]
        private IEnumerator PlayClickBtn1With2AvtarAnim()
        {
            return new <PlayClickBtn1With2AvtarAnim>c__Iterator6F { <>f__this = this };
        }

        [DebuggerHidden]
        private IEnumerator PlayClickBtn1With3AvatarAnim()
        {
            return new <PlayClickBtn1With3AvatarAnim>c__Iterator70 { <>f__this = this };
        }

        [DebuggerHidden]
        private IEnumerator PlayClickBtn2With3AvatarAnim()
        {
            return new <PlayClickBtn2With3AvatarAnim>c__Iterator71 { <>f__this = this };
        }

        public void PlaySwapAvatarAnim(uint localRuntimeIDBefore, uint localRuntimeIDAfter)
        {
            if (localRuntimeIDBefore != localRuntimeIDAfter)
            {
                if (this._state == State.Animating)
                {
                    base.StopCoroutine(this._buttonAnimateCoroutine);
                    this._buttonAnimateCoroutine = null;
                    this.FastForwardBtnAnimation();
                }
                this._state = State.Animating;
                this._localRuntimeIDBefore = localRuntimeIDBefore;
                this._localRuntimeIDAfter = localRuntimeIDAfter;
                MonoAvatarButton button = this.avatarBtnList.Find(x => x.avatarRuntimeID == this._localRuntimeIDAfter);
                if (!button.gameObject.activeInHierarchy)
                {
                    this.OnSwapAvatarAnimEnd();
                }
                else
                {
                    switch (button.index)
                    {
                        case 1:
                            if (this.avatarBtnList.Count == 2)
                            {
                                this._buttonAnimateCoroutine = base.StartCoroutine(this.PlayClickBtn1With2AvtarAnim());
                            }
                            else if (this.avatarBtnList.Count == 3)
                            {
                                this._buttonAnimateCoroutine = base.StartCoroutine(this.PlayClickBtn1With3AvatarAnim());
                            }
                            break;

                        case 2:
                            this._buttonAnimateCoroutine = base.StartCoroutine(this.PlayClickBtn2With3AvatarAnim());
                            break;
                    }
                }
            }
        }

        private void ResortAvatarBtns(uint localAvatarID)
        {
            int num = 1;
            foreach (MonoAvatarButton button in this.avatarBtnList)
            {
                int index = (button.avatarRuntimeID != localAvatarID) ? num++ : this.avatarBtnList.Count;
                button.transform.name = index.ToString();
                button.transform.SetSiblingIndex(index - 1);
                button.SetIndex(index);
            }
            if (<>f__am$cache6 == null)
            {
                <>f__am$cache6 = (left, right) => left.index - right.index;
            }
            this.avatarBtnList.Sort(<>f__am$cache6);
        }

        private void SetAvatarButtonActive(uint runtimeID, bool active)
        {
            <SetAvatarButtonActive>c__AnonStorey105 storey = new <SetAvatarButtonActive>c__AnonStorey105 {
                runtimeID = runtimeID
            };
            MonoAvatarButton button = this.avatarBtnList.Find(new Predicate<MonoAvatarButton>(storey.<>m__1B6));
            button.gameObject.SetActive(active);
            button.OnSetActive(active);
        }

        public void SetButtonAvailable(bool available)
        {
            int num = 0;
            int count = this.avatarBtnList.Count;
            while (num < count)
            {
                this.avatarBtnList[num].canChange = available;
                num++;
            }
        }

        private void StopRewindAndClearAlpha(MonoAvatarButton button)
        {
            Animation component = button.GetComponent<Animation>();
            component.Stop();
            component.Sample();
            button.GetComponent<CanvasGroup>().alpha = 1f;
        }

        [CompilerGenerated]
        private sealed class <GetAvatarButtonByRuntimeID>c__AnonStorey106
        {
            internal uint runtimeID;

            internal bool <>m__1B7(MonoAvatarButton x)
            {
                return (x.avatarRuntimeID == this.runtimeID);
            }
        }

        [CompilerGenerated]
        private sealed class <HideFirstLocalAvatarButton>c__Iterator6E : IEnumerator, IDisposable, IEnumerator<object>
        {
            internal object $current;
            internal int $PC;
            internal MonoAvatarButton <$>button;
            internal MonoAvatarButton button;

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
                        this.button.GetComponent<CanvasGroup>().alpha = 0f;
                        this.$current = null;
                        this.$PC = 1;
                        return true;

                    case 1:
                        this.button.GetComponent<CanvasGroup>().alpha = 1f;
                        this.button.gameObject.SetActive(false);
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
        private sealed class <PlayClickBtn1With2AvtarAnim>c__Iterator6F : IEnumerator, IDisposable, IEnumerator<object>
        {
            internal object $current;
            internal int $PC;
            internal MonoAvatarButtonContainer <>f__this;
            internal Animation <btn1Anim>__0;
            internal Animation <btn2Anim>__1;

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
                        this.<>f__this._fastForwardType = MonoAvatarButtonContainer.FastForwardType.Btn_1_ClickWith_2_Avatar;
                        this.<>f__this.GetComponent<GridLayoutGroup>().enabled = false;
                        this.$current = null;
                        this.$PC = 1;
                        goto Label_0112;

                    case 1:
                        this.<btn1Anim>__0 = this.<>f__this.avatarBtnList[0].GetComponent<Animation>();
                        this.<btn2Anim>__1 = this.<>f__this.avatarBtnList[1].GetComponent<Animation>();
                        this.<btn2Anim>__1.gameObject.SetActive(true);
                        this.<btn1Anim>__0.GetComponent<Animation>().Play("FadeOut");
                        this.<btn2Anim>__1.GetComponent<Animation>().Play("FadeInFrom_2");
                        this.<>f__this.SetButtonAvailable(false);
                        this.$current = new WaitForSeconds(0.25f);
                        this.$PC = 2;
                        goto Label_0112;

                    case 2:
                        this.<>f__this.OnSwapAvatarAnimEnd();
                        this.<>f__this.SetButtonAvailable(true);
                        this.$PC = -1;
                        break;
                }
                return false;
            Label_0112:
                return true;
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
        private sealed class <PlayClickBtn1With3AvatarAnim>c__Iterator70 : IEnumerator, IDisposable, IEnumerator<object>
        {
            internal object $current;
            internal int $PC;
            internal MonoAvatarButtonContainer <>f__this;
            internal Animation <btn1Anim>__0;
            internal Animation <btn2Anim>__1;
            internal Animation <btn3Anim>__2;

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
                        this.<>f__this._fastForwardType = MonoAvatarButtonContainer.FastForwardType.Btn_1_ClickWith_3_Avatar;
                        this.<>f__this.GetComponent<GridLayoutGroup>().enabled = false;
                        this.$current = null;
                        this.$PC = 1;
                        goto Label_0144;

                    case 1:
                        this.<btn1Anim>__0 = this.<>f__this.avatarBtnList[0].GetComponent<Animation>();
                        this.<btn2Anim>__1 = this.<>f__this.avatarBtnList[1].GetComponent<Animation>();
                        this.<btn3Anim>__2 = this.<>f__this.avatarBtnList[2].GetComponent<Animation>();
                        this.<btn3Anim>__2.gameObject.SetActive(true);
                        this.<btn1Anim>__0.GetComponent<Animation>().Play("FadeOut");
                        this.<btn2Anim>__1.GetComponent<Animation>().Play("MoveUpFrom_2");
                        this.<btn3Anim>__2.GetComponent<Animation>().Play("FadeInFrom_3");
                        this.<>f__this.SetButtonAvailable(false);
                        this.$current = new WaitForSeconds(0.25f);
                        this.$PC = 2;
                        goto Label_0144;

                    case 2:
                        this.<>f__this.OnSwapAvatarAnimEnd();
                        this.<>f__this.SetButtonAvailable(true);
                        this.$PC = -1;
                        break;
                }
                return false;
            Label_0144:
                return true;
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
        private sealed class <PlayClickBtn2With3AvatarAnim>c__Iterator71 : IEnumerator, IDisposable, IEnumerator<object>
        {
            internal object $current;
            internal int $PC;
            internal MonoAvatarButtonContainer <>f__this;
            internal Animation <btn2Anim>__0;
            internal Animation <btn3Anim>__1;

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
                        this.<>f__this._fastForwardType = MonoAvatarButtonContainer.FastForwardType.Btn_2_ClickWith_3_Avatar;
                        this.<>f__this.GetComponent<GridLayoutGroup>().enabled = false;
                        this.$current = null;
                        this.$PC = 1;
                        goto Label_0112;

                    case 1:
                        this.<btn2Anim>__0 = this.<>f__this.avatarBtnList[1].GetComponent<Animation>();
                        this.<btn3Anim>__1 = this.<>f__this.avatarBtnList[2].GetComponent<Animation>();
                        this.<btn3Anim>__1.gameObject.SetActive(true);
                        this.<btn2Anim>__0.GetComponent<Animation>().Play("FadeOut");
                        this.<btn3Anim>__1.GetComponent<Animation>().Play("FadeInFrom_3");
                        this.<>f__this.SetButtonAvailable(false);
                        this.$current = new WaitForSeconds(0.25f);
                        this.$PC = 2;
                        goto Label_0112;

                    case 2:
                        this.<>f__this.OnSwapAvatarAnimEnd();
                        this.<>f__this.SetButtonAvailable(true);
                        this.$PC = -1;
                        break;
                }
                return false;
            Label_0112:
                return true;
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
        private sealed class <SetAvatarButtonActive>c__AnonStorey105
        {
            internal uint runtimeID;

            internal bool <>m__1B6(MonoAvatarButton x)
            {
                return (x.avatarRuntimeID == this.runtimeID);
            }
        }

        public enum FastForwardType
        {
            Btn_1_ClickWith_2_Avatar,
            Btn_1_ClickWith_3_Avatar,
            Btn_2_ClickWith_3_Avatar
        }

        public enum State
        {
            Idle,
            Animating
        }
    }
}

