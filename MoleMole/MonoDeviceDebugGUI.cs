namespace MoleMole
{
    using BehaviorDesigner.Runtime;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    public class MonoDeviceDebugGUI : MonoBehaviour
    {
        private GUIStyle _bgStyle;
        private List<LogEntry> _logs;
        private int _queueIx;
        private GUIStyle _redStyle;
        private Vector2 _scrollPos;
        private DeviceLogState _state;
        private GUIStyle _whiteStyle;
        private GUIStyle _yellowStyle;
        private const int MAX_LOG_COUNT = 60;

        private void Awake()
        {
            this._logs = new List<LogEntry>();
            for (int i = 0; i < 60; i++)
            {
                this._logs.Add(new LogEntry());
            }
            this._queueIx = 0;
            this._redStyle = new GUIStyle();
            this._redStyle.normal.textColor = Color.red;
            this._yellowStyle = new GUIStyle();
            this._yellowStyle.normal.textColor = Color.yellow;
            this._whiteStyle = new GUIStyle();
            this._whiteStyle.normal.textColor = Color.white;
            this._bgStyle = new GUIStyle();
            Texture2D textured = new Texture2D(0x10, 0x10);
            Color[] colors = new Color[0x100];
            for (int j = 0; j < colors.Length; j++)
            {
                colors[j] = new Color(0f, 0f, 0f, 1f);
            }
            textured.SetPixels(colors);
            this._bgStyle.normal.background = textured;
            this._state = DeviceLogState.CollapsedWaitTouchDone;
            Application.logMessageReceived += new UnityEngine.Application.LogCallback(this.LogCallback);
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Time.timeScale = 0f;
            if (Singleton<EventManager>.Instance != null)
            {
                Singleton<EventManager>.Instance.DropEventsAndStop();
            }
            if (BehaviorManager.instance != null)
            {
                BehaviorManager.instance.gameObject.SetActive(false);
            }
        }

        private void LogCallback(string logString, string stackTrace, LogType type)
        {
            if (type == LogType.Exception)
            {
                this.OnException();
            }
            LogEntry entry = this._logs[this._queueIx++ % 60];
            entry.message = logString;
            entry.stackTrace = stackTrace;
            entry.type = type;
            if (type == LogType.Exception)
            {
                if (!Singleton<QAManager>.Instance.IsSentMessage(entry.message, entry.stackTrace))
                {
                    base.StartCoroutine(this.SendExceptionToSever(entry));
                }
                else
                {
                    this.OnExceptionContinue();
                }
            }
        }

        private void OnDestroy()
        {
            Application.logMessageReceived -= new UnityEngine.Application.LogCallback(this.LogCallback);
        }

        private void OnException()
        {
            this._state = DeviceLogState.Crashed;
            Application.logMessageReceived -= new UnityEngine.Application.LogCallback(this.LogCallback);
            if (Singleton<LevelManager>.Instance != null)
            {
                Singleton<LevelManager>.Instance.SetPause(true);
            }
        }

        private void OnExceptionConfirm(bool isOK)
        {
            if (isOK)
            {
                this.OnExceptionContinue();
            }
            else
            {
                this.OnExceptionQuit();
            }
        }

        private void OnExceptionContinue()
        {
            Application.logMessageReceived += new UnityEngine.Application.LogCallback(this.LogCallback);
            if (Singleton<LevelManager>.Instance != null)
            {
                Singleton<LevelManager>.Instance.SetPause(false);
            }
        }

        private void OnExceptionQuit()
        {
            if (Singleton<EventManager>.Instance != null)
            {
                Singleton<EventManager>.Instance.DropEventsAndStop();
            }
            if (BehaviorManager.instance != null)
            {
                BehaviorManager.instance.gameObject.SetActive(false);
            }
            Application.Quit();
        }

        [DebuggerHidden]
        private IEnumerator SendExceptionToSever(LogEntry entry)
        {
            return new <SendExceptionToSever>c__Iterator7E { entry = entry, <$>entry = entry, <>f__this = this };
        }

        private void ShowCrashReport()
        {
            CrashReport[] reports = CrashReport.reports;
            GUILayout.Label("Crash reports:", new GUILayoutOption[0]);
            foreach (CrashReport report in reports)
            {
                GUILayout.BeginHorizontal(new GUILayoutOption[0]);
                GUILayout.Label("Crash: " + report.time, new GUILayoutOption[0]);
                if (GUILayout.Button("Log", new GUILayoutOption[0]))
                {
                }
                if (GUILayout.Button("Remove", new GUILayoutOption[0]))
                {
                    report.Remove();
                }
                GUILayout.EndHorizontal();
            }
        }

        private void Update()
        {
            if (this._state == DeviceLogState.Collapsed)
            {
                if (Input.touchCount > 4)
                {
                    this._state = DeviceLogState.ExpanedWaitTouchDone;
                }
            }
            else if (this._state == DeviceLogState.ExpanedWaitTouchDone)
            {
                if (Input.touchCount < 5)
                {
                    this._state = DeviceLogState.Expanded;
                }
            }
            else if ((this._state == DeviceLogState.Expanded) || (this._state == DeviceLogState.Crashed))
            {
                if (Input.touchCount > 4)
                {
                    this._state = DeviceLogState.CollapsedWaitTouchDone;
                }
            }
            else if ((this._state == DeviceLogState.CollapsedWaitTouchDone) && (Input.touchCount < 5))
            {
                this._state = DeviceLogState.Collapsed;
            }
        }

        [CompilerGenerated]
        private sealed class <SendExceptionToSever>c__Iterator7E : IEnumerator, IDisposable, IEnumerator<object>
        {
            internal object $current;
            internal int $PC;
            internal MonoDeviceDebugGUI.LogEntry <$>entry;
            internal MonoDeviceDebugGUI <>f__this;
            internal string <errDesc>__3;
            internal string <nowStr>__2;
            internal string <uidStr>__0;
            internal string <vidStr>__1;
            internal MonoDeviceDebugGUI.LogEntry entry;

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
                        this.$current = Singleton<QAManager>.Instance.SendMessageToSeverSync(this.entry.message, this.entry.stackTrace);
                        this.$PC = 1;
                        goto Label_0222;

                    case 1:
                        this.$current = null;
                        this.$PC = 2;
                        goto Label_0222;

                    case 2:
                        this.$current = null;
                        this.$PC = 3;
                        goto Label_0222;

                    case 3:
                        this.$current = null;
                        this.$PC = 4;
                        goto Label_0222;

                    case 4:
                        this.$current = null;
                        this.$PC = 5;
                        goto Label_0222;

                    case 5:
                        this.$current = null;
                        this.$PC = 6;
                        goto Label_0222;

                    case 6:
                    {
                        if (Singleton<MainUIManager>.Instance == null)
                        {
                            Application.Quit();
                            break;
                        }
                        this.<uidStr>__0 = Singleton<PlayerModule>.Instance.playerData.userId.ToString();
                        this.<vidStr>__1 = Singleton<NetworkManager>.Instance.GetGameVersion();
                        this.<nowStr>__2 = TimeUtil.Now.ToString();
                        string[] textArray1 = new string[] { "time=", this.<nowStr>__2, "\nuid=", this.<uidStr>__0, "\nvid=", this.<vidStr>__1, "\nmsg=", this.entry.message };
                        this.<errDesc>__3 = string.Concat(textArray1);
                        if (!GlobalVars.ENABLE_EXCEPTION_CONTINUE)
                        {
                            Singleton<MainUIManager>.Instance.ShowDialog(new HintWithConfirmDialogContext(this.<errDesc>__3, LocalizationGeneralLogic.GetText("Menu_QuitGame", new object[0]), new Action(Application.Quit), LocalizationGeneralLogic.GetText("Menu_Title_FatalError", new object[0])), UIType.Any);
                            break;
                        }
                        Singleton<MainUIManager>.Instance.ShowDialog(new HintWithConfirmDialogContext(this.<errDesc>__3, LocalizationGeneralLogic.GetText("Menu_ContinueGame", new object[0]), LocalizationGeneralLogic.GetText("Menu_QuitGame", new object[0]), new Action<bool>(this.<>f__this.OnExceptionConfirm), LocalizationGeneralLogic.GetText("Menu_Title_FatalError", new object[0])), UIType.Any);
                        break;
                    }
                    default:
                        goto Label_0220;
                }
                this.$PC = -1;
            Label_0220:
                return false;
            Label_0222:
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

        private enum DeviceLogState
        {
            Collapsed,
            CollapsedWaitTouchDone,
            Expanded,
            ExpanedWaitTouchDone,
            Crashed
        }

        private class LogEntry
        {
            public string message;
            public string stackTrace;
            public LogType type;
        }
    }
}

