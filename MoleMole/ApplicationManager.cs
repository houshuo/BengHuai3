namespace MoleMole
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using UniRx;
    using UnityEngine;

    public class ApplicationManager
    {
        private MonoApplicationBehaviour _applicationBehaviour;
        private List<Coroutine> _coroutines;
        private GameObject _go;
        private int _manualCoroutineID;
        private List<Tuple<int, IEnumerator>> _manualCoroutines;
        [CompilerGenerated]
        private static Action<bool> <>f__am$cache5;

        public ApplicationManager()
        {
            if (this._go != null)
            {
                UnityEngine.Object.Destroy(this._go);
            }
            this._go = new GameObject("ApplicationManagerGO");
            UnityEngine.Object.DontDestroyOnLoad(this._go);
            this._applicationBehaviour = this._go.AddComponent<MonoApplicationBehaviour>();
            this._applicationBehaviour.Init(this);
            this._go.AddComponent<MonoTalkingData>();
            this._go.AddComponent<MonoNotificationServices>();
            this._go.AddComponent<AntiCheatPlugin>();
            this._coroutines = new List<Coroutine>();
            this._manualCoroutines = new List<Tuple<int, IEnumerator>>();
        }

        public void AntiCheatQuit(string title, string text)
        {
            try
            {
                GeneralDialogContext dialogContext = new GeneralDialogContext {
                    type = GeneralDialogContext.ButtonType.SingleButton,
                    title = LocalizationGeneralLogic.GetText(title, new object[0]),
                    desc = LocalizationGeneralLogic.GetText(text, new object[0]),
                    notDestroyAfterTouchBG = true,
                    hideCloseBtn = true
                };
                if (<>f__am$cache5 == null)
                {
                    <>f__am$cache5 = ok => Quit();
                }
                dialogContext.buttonCallBack = <>f__am$cache5;
                Singleton<MainUIManager>.Instance.ShowDialog(dialogContext, UIType.Any);
            }
            catch (Exception exception)
            {
                SuperDebug.VeryImportantError("Exception: " + exception.ToString());
                Quit();
            }
        }

        public void ClearFinishCoroutines()
        {
            for (int i = 0; i < this._manualCoroutines.Count; i++)
            {
                if (this._manualCoroutines[i] == null)
                {
                    this._manualCoroutines.RemoveAt(i);
                    i--;
                }
            }
        }

        public void Destroy()
        {
            if (this._go != null)
            {
                UnityEngine.Object.Destroy(this._go);
            }
            this.StopAllCoroutines();
        }

        public void DetectCheat()
        {
            if (AntiCheatPlugin.Detect())
            {
                try
                {
                    Singleton<NetworkManager>.Instance.RequestAntiCheatSDKReport();
                    this.ReportProcList();
                }
                catch (Exception exception)
                {
                    SuperDebug.VeryImportantError("Exception: " + exception.ToString());
                    this.AntiCheatQuit("Menu_Title_AntiCheatQuit", "Menu_Desc_AntiCheatQuit");
                }
            }
        }

        public void DetectEmulator()
        {
            if (AntiEmulatorPlugin.Detect())
            {
                try
                {
                    Singleton<NetworkManager>.Instance.RequestAntiCheatSDKReport();
                    byte[] fileContent = AntiEmulatorPlugin.GetFileContent();
                    Singleton<QAManager>.Instance.SendFileToServer(MiscData.Config.DumpFileUploadUrl, "anti-cheat", fileContent, rspText => this.AntiCheatQuit("Menu_Title_AntiCheatQuit", "Menu_Desc_AntiCheatQuit"), () => this.AntiCheatQuit("Menu_Title_AntiCheatQuit", "Menu_Desc_AntiCheatQuit"), 10f, "Emulator", true);
                }
                catch (Exception exception)
                {
                    SuperDebug.VeryImportantError("Exception: " + exception.ToString());
                    this.AntiCheatQuit("Menu_Title_AntiCheatQuit", "Menu_Desc_AntiCheatQuit");
                }
            }
        }

        private int GenerateManualCoroutineID()
        {
            return this._manualCoroutineID++;
        }

        public void Invoke(float duration, Action callback)
        {
            if (this._applicationBehaviour != null)
            {
                this._applicationBehaviour.StartCoroutine(this.InvokeCoroutine(duration, callback));
            }
        }

        public void InvokeAfterFrames(int frames, Action callback)
        {
            if (this._applicationBehaviour != null)
            {
                this._applicationBehaviour.StartCoroutine(this.InvokeAfterFramesCoroutine(frames, callback));
            }
        }

        [DebuggerHidden]
        private IEnumerator InvokeAfterFramesCoroutine(int frames, Action callback)
        {
            return new <InvokeAfterFramesCoroutine>c__Iterator34 { frames = frames, callback = callback, <$>frames = frames, <$>callback = callback };
        }

        [DebuggerHidden]
        private IEnumerator InvokeCoroutine(float duration, Action callback)
        {
            return new <InvokeCoroutine>c__Iterator32 { duration = duration, callback = callback, <$>duration = duration, <$>callback = callback };
        }

        public void InvokeNextFrame(Action callback)
        {
            if (this._applicationBehaviour != null)
            {
                this._applicationBehaviour.StartCoroutine(this.InvokeNextFrameCoroutine(callback));
            }
        }

        [DebuggerHidden]
        private IEnumerator InvokeNextFrameCoroutine(Action callback)
        {
            return new <InvokeNextFrameCoroutine>c__Iterator33 { callback = callback, <$>callback = callback };
        }

        private void ManualCoroutineMoveNext(int manualCoroutineID)
        {
            for (int i = 0; i < this._manualCoroutines.Count; i++)
            {
                Tuple<int, IEnumerator> tuple = this._manualCoroutines[i];
                if ((tuple != null) && (tuple.Item1 == manualCoroutineID))
                {
                    bool flag = true;
                    if (tuple.Item2 != null)
                    {
                        flag = tuple.Item2.MoveNext();
                    }
                    if (!flag)
                    {
                        this._manualCoroutines[i] = null;
                    }
                    break;
                }
            }
        }

        public void ManualCoroutinesMoveNext()
        {
            for (int i = 0; i < this._manualCoroutines.Count; i++)
            {
                Tuple<int, IEnumerator> tuple = this._manualCoroutines[i];
                if (tuple != null)
                {
                    bool flag = true;
                    if (tuple.Item2 != null)
                    {
                        flag = tuple.Item2.MoveNext();
                    }
                    if (!flag)
                    {
                        this._manualCoroutines[i] = null;
                    }
                }
            }
        }

        public static void Quit()
        {
            Application.Quit();
        }

        public void ReportProcList()
        {
            byte[] buf = AntiCheatPlugin.ReadProcList();
            Singleton<QAManager>.Instance.SendFileToServer(MiscData.Config.DumpFileUploadUrl, "anti-cheat", buf, rspText => this.AntiCheatQuit("Menu_Title_AntiCheatQuit", "Menu_Desc_AntiCheatQuit"), () => this.AntiCheatQuit("Menu_Title_AntiCheatQuit", "Menu_Desc_AntiCheatQuit"), 10f, null, true);
        }

        public Coroutine StartCoroutine(IEnumerator routine)
        {
            Coroutine item = this._applicationBehaviour.StartCoroutine(routine);
            this._coroutines.Add(item);
            return item;
        }

        public int StartCoroutineManual(IEnumerator routine)
        {
            int num = this.GenerateManualCoroutineID();
            this._manualCoroutines.Add(new Tuple<int, IEnumerator>(num, routine));
            this.ManualCoroutineMoveNext(num);
            return num;
        }

        public void StopAllCoroutines()
        {
            foreach (Coroutine coroutine in this._coroutines)
            {
                if ((coroutine != null) && (this._applicationBehaviour != null))
                {
                    this._applicationBehaviour.StopCoroutine(coroutine);
                }
            }
            this._coroutines.Clear();
            this._manualCoroutines.Clear();
        }

        public void StopCoroutine(Coroutine routine)
        {
            if ((routine != null) && this._coroutines.Contains(routine))
            {
                if (this._applicationBehaviour != null)
                {
                    this._applicationBehaviour.StopCoroutine(routine);
                }
                this._coroutines.Remove(routine);
            }
        }

        public void StopCoroutineManual(int manualCoroutineID)
        {
            for (int i = 0; i < this._manualCoroutines.Count; i++)
            {
                Tuple<int, IEnumerator> tuple = this._manualCoroutines[i];
                if ((tuple != null) && (tuple.Item1 == manualCoroutineID))
                {
                    this._manualCoroutines[i] = null;
                    break;
                }
            }
        }

        public MonoApplicationBehaviour applicationBehaviour
        {
            get
            {
                return this._applicationBehaviour;
            }
        }

        [CompilerGenerated]
        private sealed class <InvokeAfterFramesCoroutine>c__Iterator34 : IEnumerator, IDisposable, IEnumerator<object>
        {
            internal object $current;
            internal int $PC;
            internal Action <$>callback;
            internal int <$>frames;
            internal int <i>__0;
            internal Action callback;
            internal int frames;

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
                        this.<i>__0 = 0;
                        break;

                    case 1:
                        this.<i>__0++;
                        break;

                    default:
                        goto Label_007C;
                }
                if (this.<i>__0 < this.frames)
                {
                    this.$current = null;
                    this.$PC = 1;
                    return true;
                }
                if (this.callback != null)
                {
                    this.callback();
                }
                this.$PC = -1;
            Label_007C:
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
        private sealed class <InvokeCoroutine>c__Iterator32 : IEnumerator, IDisposable, IEnumerator<object>
        {
            internal object $current;
            internal int $PC;
            internal Action <$>callback;
            internal float <$>duration;
            internal Action callback;
            internal float duration;

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
                        this.$current = new WaitForSeconds(this.duration);
                        this.$PC = 1;
                        return true;

                    case 1:
                        if (this.callback != null)
                        {
                            this.callback();
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
        private sealed class <InvokeNextFrameCoroutine>c__Iterator33 : IEnumerator, IDisposable, IEnumerator<object>
        {
            internal object $current;
            internal int $PC;
            internal Action <$>callback;
            internal Action callback;

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
                        this.$current = null;
                        this.$PC = 1;
                        return true;

                    case 1:
                        if (this.callback != null)
                        {
                            this.callback();
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
    }
}

