namespace MoleMole
{
    using FlatBuffers;
    using MoleMole.MPProtocol;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    public class MonoMPDevLevel : MonoMPLevelV1
    {
        private Popup[] _avatarPops;
        private MonoDebugMP _debugMP;
        private bool _established;
        private Popup _modePop;
        private MPMode _mpMode;
        private GUIContent[] _mpModes;
        private int _playerCount;
        private Popup _stagePop;
        private GUIStyle _style;
        private GUIContent[] _whiteListAvatars;
        private GUIContent[] _whiteListStages;
        [CompilerGenerated]
        private static Func<string, GUIContent> <>f__am$cache10;
        [CompilerGenerated]
        private static Func<string, GUIContent> <>f__am$cacheF;
        public MoleMole.MPAvatarDataItem[] avatarDataLs;
        private const int MAX_PLAYER_COUNT = 4;
        public MPMode mpMode;
        public MoleMole.MPStageData stageData;
        public bool usePresetData;

        private void Awake()
        {
            Screen.sleepTimeout = -1;
            GlobalVars.DISABLE_NETWORK_DEBUG = true;
            MainUIData.USE_VIEW_CACHING = false;
            GeneralLogicManager.InitAll();
            GlobalDataManager.Refresh();
            Singleton<LevelScoreManager>.Create();
            Singleton<LevelScoreManager>.Instance.luaFile = "Lua/Levels/Common/Level 0.lua";
            UnityEngine.Object.FindObjectOfType<MonoDebugMP>().onPeerReady = new Action<MPPeer>(this.OnPeerReady);
            base.Awake();
            Singleton<LevelManager>.Instance.levelActor.AddPlugin(new MPDevLevelActorPlugin(this));
            this._established = false;
            this.InitGUI();
        }

        [DebuggerHidden]
        private IEnumerator EndOfFrameHideUI()
        {
            return new <EndOfFrameHideUI>c__Iterator4A();
        }

        private void InitGUI()
        {
            this._style = new GUIStyle();
            this._style.normal.textColor = Color.white;
            this._style.hover.textColor = Color.white;
            this._style.focused.textColor = Color.gray;
            this._style.active.textColor = Color.white;
            this._style.onNormal.textColor = Color.white;
            this._style.onHover.textColor = Color.white;
            this._style.onFocused.textColor = Color.gray;
            this._style.onActive.textColor = Color.white;
            string[] source = new string[] { "Kiana_C2_PT", "Kiana_C1_FX", "Mei_C2_CK" };
            if (<>f__am$cacheF == null)
            {
                <>f__am$cacheF = x => new GUIContent(x);
            }
            this._whiteListAvatars = source.Select<string, GUIContent>(<>f__am$cacheF).ToArray<GUIContent>();
            string[] textArray2 = new string[] { "St_Freyja_05" };
            if (<>f__am$cache10 == null)
            {
                <>f__am$cache10 = x => new GUIContent(x);
            }
            this._whiteListStages = textArray2.Select<string, GUIContent>(<>f__am$cache10).ToArray<GUIContent>();
            this._mpModes = new GUIContent[] { new GUIContent(MPMode.Normal.ToString()), new GUIContent(MPMode.PvP_SendNoReceive.ToString()), new GUIContent(MPMode.PvP_ReceiveNoSend.ToString()) };
            this._stagePop = new Popup();
            this._avatarPops = new Popup[4];
            for (int i = 0; i < 4; i++)
            {
                this._avatarPops[i] = new Popup();
            }
            this._modePop = new Popup();
            this._debugMP = UnityEngine.Object.FindObjectOfType<MonoDebugMP>();
            this._playerCount = this._debugMP.WaitForPlayerCount;
        }

        public void OnDestroy()
        {
            Singleton<LevelManager>.Instance.Destroy();
            Singleton<LevelManager>.Destroy();
        }

        private void OnGUI()
        {
            if (!this._established)
            {
                this.PreEstablishedGUIUpdate();
            }
        }

        private void OnPeerReady(MPPeer peer)
        {
            this._established = true;
            if (!this.usePresetData)
            {
                this.SyncSelectionToData();
            }
            Singleton<MPManager>.Instance.Setup(peer);
            Singleton<MainUIManager>.Instance.GetInLevelUICanvas().mainPageContext.SetInLevelMainPageActive(true, false, false);
            LevelIdentity identity = Singleton<MPManager>.Instance.GetIdentity<LevelIdentity>(0x21800001);
            if (identity.isAuthority)
            {
                MPSendPacketContainer pc = Singleton<MPManager>.Instance.CreateSendPacket<Packet_Level_CreateStageFullData>();
                Offset<MoleMole.MPProtocol.MPStageData> stageDataOffset = MPMappings.Serialize(pc.builder, this.stageData);
                Offset<MoleMole.MPProtocol.MPAvatarDataItem>[] data = new Offset<MoleMole.MPProtocol.MPAvatarDataItem>[this.avatarDataLs.Length];
                for (int i = 0; i < data.Length; i++)
                {
                    data[i] = MPMappings.Serialize(pc.builder, this.avatarDataLs[i]);
                }
                VectorOffset avatarsOffset = Packet_Level_CreateStageFullData.CreateAvatarsVector(pc.builder, data);
                Packet_Level_CreateStageFullData.StartPacket_Level_CreateStageFullData(pc.builder);
                Packet_Level_CreateStageFullData.AddStageData(pc.builder, stageDataOffset);
                Packet_Level_CreateStageFullData.AddAvatars(pc.builder, avatarsOffset);
                Packet_Level_CreateStageFullData.AddMpMode(pc.builder, this.mpMode);
                pc.Finish<Packet_Level_CreateStageFullData>(Packet_Level_CreateStageFullData.EndPacket_Level_CreateStageFullData(pc.builder));
                identity.DebugCreateStageWithFullDataSync(pc);
            }
        }

        private void PreEstablishedGUIUpdate()
        {
            if (this.usePresetData)
            {
                if (GUI.Button(new Rect(50f, 50f, 200f, 100f), "Override Preset Data"))
                {
                    this.usePresetData = false;
                }
            }
            else
            {
                GUILayout.BeginArea(new Rect(50f, 50f, 300f, 400f));
                if (GUILayout.Button("Player Count: " + this._playerCount, new GUILayoutOption[0]))
                {
                    int num = Mathf.Clamp((this._playerCount + 1) % 4, 2, 4);
                    if (num != this._playerCount)
                    {
                        this._debugMP.WaitForPlayerCount = num;
                    }
                    this._playerCount = num;
                }
                this._stagePop.List(GUILayoutUtility.GetRect((float) 20f, (float) (this._style.lineHeight * 1.5f)), this._whiteListStages, this._style, this._style, 1);
                for (int i = 0; i < this._playerCount; i++)
                {
                    GUILayout.BeginHorizontal(new GUILayoutOption[0]);
                    GUILayoutOption[] optionArray1 = new GUILayoutOption[] { GUILayout.ExpandWidth(false) };
                    GUILayout.Label("p" + (i + 1), optionArray1);
                    this._avatarPops[i].List(GUILayoutUtility.GetRect((float) 20f, (float) (this._style.lineHeight * 1.5f)), this._whiteListAvatars, this._style, this._style, 1);
                    GUILayout.EndHorizontal();
                }
                GUILayout.BeginHorizontal(new GUILayoutOption[0]);
                GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.ExpandWidth(false) };
                GUILayout.Label("Mode", options);
                this._modePop.List(GUILayoutUtility.GetRect((float) 20f, (float) (this._style.lineHeight * 1.5f)), this._mpModes, this._style, this._style, 1);
                GUILayout.EndHorizontal();
                GUILayout.EndArea();
            }
        }

        private void Start()
        {
            Singleton<LevelManager>.Instance.InitAtStart();
            base.StartCoroutine(this.EndOfFrameHideUI());
        }

        private void SyncSelectionToData()
        {
            this.mpMode = (MPMode) ((byte) this._modePop.GetSelectedItemIndex());
            this.stageData = new MoleMole.MPStageData();
            this.stageData.stageName = this._whiteListStages[this._stagePop.GetSelectedItemIndex()].text;
            this.avatarDataLs = new MoleMole.MPAvatarDataItem[this._playerCount];
            for (int i = 0; i < this._playerCount; i++)
            {
                AvatarMetaData avatarMetaDataByRegistryKey = Singleton<AvatarModule>.Instance.GetAvatarMetaDataByRegistryKey(this._whiteListAvatars[this._avatarPops[i].GetSelectedItemIndex()].text);
                MoleMole.MPAvatarDataItem item = new MoleMole.MPAvatarDataItem();
                this.avatarDataLs[i] = item;
                item.avatarID = avatarMetaDataByRegistryKey.avatarID;
                item.level = 10;
            }
        }

        [CompilerGenerated]
        private sealed class <EndOfFrameHideUI>c__Iterator4A : IEnumerator, IDisposable, IEnumerator<object>
        {
            internal object $current;
            internal int $PC;

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
                        this.$current = new WaitForEndOfFrame();
                        this.$PC = 1;
                        return true;

                    case 1:
                        Singleton<MainUIManager>.Instance.GetInLevelUICanvas().mainPageContext.SetInLevelMainPageActive(false, false, false);
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

