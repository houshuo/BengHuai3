namespace MoleMole
{
    using MoleMole.Config;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public class FaceAnimationEditor : MonoBehaviour
    {
        private int _action = 1;
        private Animator _animator;
        private string _animatorStateName = string.Empty;
        private Vector2 _blockEditViewScrollPos = Vector2.zero;
        public ConfigFaceAnimation _bronyaConfig;
        private bool _changed;
        private List<FaceAnimationFrameBlock> _clipBoard = new List<FaceAnimationFrameBlock>();
        private Vector2 _clipboardScrollPos = Vector2.zero;
        private Vector2 _contentViewScrollPos = Vector2.zero;
        private FaceAnimationEditAvatar _curAvatar;
        private int _currentAvatarIndex;
        private ConfigFaceAnimation _currentConfig;
        private FaceAnimationItem _currentFaceAnimationItem;
        private int _currentFaceAnimationItemIndex = -1;
        private FacePartType _currentPart;
        private int _eyeCopyDirection;
        private Vector2 _faceAnimationListScrollPos = Vector2.zero;
        private int _heartLevel = 1;
        public ConfigFaceAnimation _kianaConfig;
        public ConfigFaceAnimation _meiConfig;
        private float _normalizedTime;
        private bool _playing;
        private bool _useAnimator;
        public float angleOfPitch;
        public int[] avatarIds = new int[] { 0x65, 0x66, 0x67, 0x68, 0xc9, 0xca, 0xcb, 0xcc, 0x12d, 0x12e, 0x12f, 0x130 };
        public string[] avatarNames = new string[] { "Kiana_C1", "Kiana_C2", "Kiana_C3", "Kiana_C4", "Mei_C1", "Mei_C2", "Mei_C3", "Mei_C4", "Bronya_C1", "Bronya_C2", "Bronya_C3", "Bronya_C4" };
        public GameObject[] avatarObjects;
        public float blockEditPanelWidth = 100f;
        public AtlasMatInfoProvider bronyaEyeAtlas;
        public TestMatInfoProvider bronyaImageMap_L;
        public TestMatInfoProvider bronyaImageMap_M;
        public TestMatInfoProvider bronyaImageMap_R;
        public Material bronyaLeftEyeMaterial;
        public string bronyaLeftEyeOriginName;
        public AtlasMatInfoProvider bronyaMouthAtlas;
        public Material bronyaMouthMaterial;
        public string bronyaMouthOriginName;
        public Material bronyaRightEyeMaterial;
        public string bronyaRightEyeOriginName;
        public float camFactor;
        public float camFactorViewHeight = 100f;
        public Transform camFocTrans0;
        public Transform camFocTrans1;
        public Transform camPosTrans0;
        public Transform camPosTrans1;
        public float faceAnimationPanelHeight = 100f;
        public float frameButtonFactor = 0.8f;
        public float frameWidth = 30f;
        public float heightOffset;
        public AtlasMatInfoProvider kianaEyeAtlas;
        public TestMatInfoProvider kianaImageMap_L;
        public TestMatInfoProvider kianaImageMap_M;
        public TestMatInfoProvider kianaImageMap_R;
        public Material kianaLeftEyeMaterial;
        public string kianaLeftEyeOriginName;
        public AtlasMatInfoProvider kianaMouthAtlas;
        public Material kianaMouthMaterial;
        public string kianaMouthOriginName;
        public Material kianaRightEyeMaterial;
        public string kianaRightEyeOriginName;
        public AtlasMatInfoProvider meiEyeAtlas;
        public TestMatInfoProvider meiImageMap_L;
        public TestMatInfoProvider meiImageMap_M;
        public TestMatInfoProvider meiImageMap_R;
        public Material meiLeftEyeMaterial;
        public string meiLeftEyeOriginName;
        public AtlasMatInfoProvider meiMouthAtlas;
        public Material meiMouthMaterial;
        public string meiMouthOriginName;
        public Material meiRightEyeMaterial;
        public string meiRightEyeOriginName;
        public float rotation;
        private int selectCount;
        private int selectIndex;
        public float selectPanelWidth = 100f;
        public float timePerFrameMax = 5f;
        public float timePerFrameMin = 0.001f;

        public event ChangedHandler Changed;

        private void Awake()
        {
            GlobalDataManager.metaConfig = ConfigUtil.LoadConfig<ConfigMetaConfig>("Common/MetaConfig");
            TouchPatternData.ReloadFromFile();
            this.running = true;
            this.InitWwise();
        }

        private void Copy()
        {
            int selectIndex = this.selectIndex;
            FaceAnimationFrameBlock[] currentBlocks = this.GetCurrentBlocks();
            if (currentBlocks != null)
            {
                this._clipBoard.Clear();
                int num2 = (this.selectCount >= 1) ? this.selectCount : 1;
                while (((selectIndex < (this.selectIndex + num2)) && (selectIndex >= 0)) && (selectIndex < currentBlocks.Length))
                {
                    FaceAnimationFrameBlock item = new FaceAnimationFrameBlock {
                        frameLength = currentBlocks[selectIndex].frameLength,
                        frameKey = currentBlocks[selectIndex].frameKey
                    };
                    this._clipBoard.Add(item);
                    selectIndex++;
                }
            }
        }

        private FaceAnimationFrameBlock[] CreateInverseEyeBlocks(FaceAnimationFrameBlock[] src, char replaceFrom, char replaceTo)
        {
            FaceAnimationFrameBlock[] blockArray = new FaceAnimationFrameBlock[src.Length];
            int index = 0;
            int length = blockArray.Length;
            while (index < length)
            {
                int frameLength = src[index].frameLength;
                string str = src[index].frameKey.Replace(replaceFrom, replaceTo);
                blockArray[index] = new FaceAnimationFrameBlock { frameLength = frameLength, frameKey = str };
                index++;
            }
            return blockArray;
        }

        private void DrawAnimationBlocksView()
        {
            Color backgroundColor = GUI.backgroundColor;
            Color color = GUI.color;
            GUI.backgroundColor = Color.cyan;
            Rect position = new Rect(Screen.width - this.blockEditPanelWidth, 5f, this.blockEditPanelWidth - 5f, (Screen.height - this.faceAnimationPanelHeight) - 15f);
            GUI.Box(position, string.Empty);
            GUILayout.BeginArea(position);
            if (this._currentPart == FacePartType.LeftEye)
            {
                GUILayout.Label("<Left Eye>", new GUILayoutOption[0]);
            }
            else if (this._currentPart == FacePartType.RightEye)
            {
                GUILayout.Label("<Right Eye>", new GUILayoutOption[0]);
            }
            else if (this._currentPart == FacePartType.Mouth)
            {
                GUILayout.Label("<Mouth>", new GUILayoutOption[0]);
            }
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUILayout.Label("Length", new GUILayoutOption[0]);
            GUILayout.Label("Name", new GUILayoutOption[0]);
            GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.Width(30f) };
            GUILayout.Label(string.Empty, options);
            GUILayout.EndHorizontal();
            this._blockEditViewScrollPos = GUILayout.BeginScrollView(this._blockEditViewScrollPos, new GUILayoutOption[0]);
            if (this._currentFaceAnimationItemIndex >= 0)
            {
                FaceAnimationFrameBlock[] collection = null;
                if (this._currentPart == FacePartType.LeftEye)
                {
                    collection = this._currentConfig.items[this._currentFaceAnimationItemIndex].leftEyeBlocks;
                }
                else if (this._currentPart == FacePartType.RightEye)
                {
                    collection = this._currentConfig.items[this._currentFaceAnimationItemIndex].rightEyeBlocks;
                }
                else if (this._currentPart == FacePartType.Mouth)
                {
                    collection = this._currentConfig.items[this._currentFaceAnimationItemIndex].mouthBlocks;
                }
                if (collection != null)
                {
                    int index = -1;
                    int num2 = 1;
                    int num3 = 0;
                    int length = collection.Length;
                    while (num3 < length)
                    {
                        int frameLength = collection[num3].frameLength;
                        GUILayout.BeginHorizontal(new GUILayoutOption[0]);
                        Color color3 = GUI.color;
                        int num6 = (this.selectCount >= 1) ? this.selectCount : 1;
                        if ((num3 >= this.selectIndex) && (num3 < (this.selectIndex + num6)))
                        {
                            GUI.color = Color.red;
                        }
                        GUILayoutOption[] optionArray2 = new GUILayoutOption[] { GUILayout.Width(35f) };
                        if (GUILayout.Toggle(this.selectIndex == num3, num2.ToString(), optionArray2))
                        {
                            this.selectIndex = num3;
                        }
                        GUI.color = color3;
                        num2 += frameLength;
                        GUILayoutOption[] optionArray3 = new GUILayoutOption[] { GUILayout.Width(20f) };
                        string introduced26 = GUILayout.TextField(collection[num3].frameLength.ToString(), optionArray3);
                        int.TryParse(introduced26, out collection[num3].frameLength);
                        string frameKey = collection[num3].frameKey;
                        GUILayoutOption[] optionArray4 = new GUILayoutOption[] { GUILayout.Width(20f) };
                        if (GUILayout.Button("-", optionArray4))
                        {
                            int num7 = this.MapFromNameToIndex(collection[num3].frameKey);
                            int imageMapLength = this.GetImageMapLength();
                            num7 = Mathf.Clamp(num7 - 1, 0, imageMapLength - 1);
                            string str3 = this.MapFromIndexToName(num7);
                            if (str3 == null)
                            {
                                str3 = "<Unnamed>";
                            }
                            collection[num3].frameKey = str3;
                        }
                        collection[num3].frameKey = GUILayout.TextField(collection[num3].frameKey, new GUILayoutOption[0]);
                        GUILayoutOption[] optionArray5 = new GUILayoutOption[] { GUILayout.Width(20f) };
                        if (GUILayout.Button("+", optionArray5))
                        {
                            int num9 = this.MapFromNameToIndex(collection[num3].frameKey);
                            int num10 = this.GetImageMapLength();
                            num9 = Mathf.Clamp(num9 + 1, 0, num10 - 1);
                            string str4 = this.MapFromIndexToName(num9);
                            if (str4 == null)
                            {
                                str4 = "<Unnamed>";
                            }
                            collection[num3].frameKey = str4;
                        }
                        if ((frameLength != collection[num3].frameLength) || (frameKey != collection[num3].frameKey))
                        {
                            this._curAvatar.RebuildFaceAnimation();
                            this.changed = true;
                        }
                        GUILayoutOption[] optionArray6 = new GUILayoutOption[] { GUILayout.Width(30f) };
                        if (GUILayout.Button("X", optionArray6))
                        {
                            index = num3;
                        }
                        GUILayout.EndHorizontal();
                        num3++;
                    }
                    if (index >= 0)
                    {
                        List<FaceAnimationFrameBlock> list = new List<FaceAnimationFrameBlock>(collection);
                        list.RemoveAt(index);
                        if (this._currentPart == FacePartType.LeftEye)
                        {
                            this._currentConfig.items[this._currentFaceAnimationItemIndex].leftEyeBlocks = list.ToArray();
                        }
                        else if (this._currentPart == FacePartType.RightEye)
                        {
                            this._currentConfig.items[this._currentFaceAnimationItemIndex].rightEyeBlocks = list.ToArray();
                        }
                        else if (this._currentPart == FacePartType.Mouth)
                        {
                            this._currentConfig.items[this._currentFaceAnimationItemIndex].mouthBlocks = list.ToArray();
                        }
                        this._curAvatar.RebuildFaceAnimation();
                        this.changed = true;
                    }
                }
                GUILayout.EndScrollView();
                if (GUILayout.Button("Add", new GUILayoutOption[0]))
                {
                    List<FaceAnimationFrameBlock> list2 = new List<FaceAnimationFrameBlock>(collection);
                    FaceAnimationFrameBlock item = new FaceAnimationFrameBlock {
                        frameLength = 1,
                        frameKey = "origin"
                    };
                    list2.Add(item);
                    if (this._currentPart == FacePartType.LeftEye)
                    {
                        this._currentConfig.items[this._currentFaceAnimationItemIndex].leftEyeBlocks = list2.ToArray();
                    }
                    else if (this._currentPart == FacePartType.RightEye)
                    {
                        this._currentConfig.items[this._currentFaceAnimationItemIndex].rightEyeBlocks = list2.ToArray();
                    }
                    else if (this._currentPart == FacePartType.Mouth)
                    {
                        this._currentConfig.items[this._currentFaceAnimationItemIndex].mouthBlocks = list2.ToArray();
                    }
                    this._curAvatar.RebuildFaceAnimation();
                    this.changed = true;
                }
                GUILayout.BeginHorizontal(new GUILayoutOption[0]);
                GUILayout.Label("SelectCount", new GUILayoutOption[0]);
                string s = GUILayout.TextField(this.selectCount.ToString(), new GUILayoutOption[0]);
                int.TryParse(s, out this.selectCount);
                GUILayout.EndHorizontal();
                if (GUILayout.Button("Copy(Z+C)", new GUILayoutOption[0]))
                {
                    this.Copy();
                }
                GUILayout.BeginHorizontal(new GUILayoutOption[0]);
                if (GUILayout.Button("Paste(Z+V)", new GUILayoutOption[0]))
                {
                    this.PasteIndex();
                }
                if (GUILayout.Button("PasteTail(Z+B)", new GUILayoutOption[0]))
                {
                    this.PasteTail();
                }
                GUILayout.EndHorizontal();
                GUILayout.EndArea();
                GUI.backgroundColor = backgroundColor;
                GUI.color = color;
            }
        }

        private void DrawAnimationContentEditPanel()
        {
            this._contentViewScrollPos = GUILayout.BeginScrollView(this._contentViewScrollPos, new GUILayoutOption[0]);
            GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.Width(this._currentConfig.items[this._currentFaceAnimationItemIndex].length * this.frameWidth) };
            this._normalizedTime = GUILayout.HorizontalSlider(this._normalizedTime, 0f, 0.9999f, options);
            float num = this._currentConfig.items[this._currentFaceAnimationItemIndex].timePerFrame * this._currentConfig.items[this._currentFaceAnimationItemIndex].length;
            GUILayout.Label((this._normalizedTime * num).ToString(), new GUILayoutOption[0]);
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUILayoutOption[] optionArray2 = new GUILayoutOption[] { GUILayout.Width((this.frameWidth * this.frameButtonFactor) * 0.3f) };
            GUILayout.Label(string.Empty, optionArray2);
            int num2 = 0;
            int length = this._currentConfig.items[this._currentFaceAnimationItemIndex].length;
            while (num2 < length)
            {
                int num5 = num2 + 1;
                GUILayoutOption[] optionArray3 = new GUILayoutOption[] { GUILayout.Width(this.frameWidth * this.frameButtonFactor) };
                GUILayout.Label(num5.ToString(), optionArray3);
                num2++;
            }
            GUILayout.EndHorizontal();
            TestMatInfoProvider provider = this.kianaImageMap_L;
            TestMatInfoProvider provider2 = this.kianaImageMap_R;
            TestMatInfoProvider provider3 = this.kianaImageMap_M;
            if ((this._curAvatar.avatarId / 100) == 2)
            {
                provider = this.meiImageMap_L;
                provider2 = this.meiImageMap_R;
                provider3 = this.meiImageMap_M;
            }
            else if ((this._curAvatar.avatarId / 100) == 3)
            {
                provider = this.bronyaImageMap_L;
                provider2 = this.bronyaImageMap_R;
                provider3 = this.bronyaImageMap_M;
            }
            if (this.DrawBlockGraphArray(this._currentConfig.items[this._currentFaceAnimationItemIndex].leftEyeBlocks, provider, this._currentConfig.items[this._currentFaceAnimationItemIndex].length, ((Screen.width - this.selectPanelWidth) - 20f) - this.blockEditPanelWidth, Color.red, Color.yellow, this._currentPart != FacePartType.LeftEye))
            {
                this._currentPart = FacePartType.LeftEye;
            }
            if (this.DrawBlockGraphArray(this._currentConfig.items[this._currentFaceAnimationItemIndex].rightEyeBlocks, provider2, this._currentConfig.items[this._currentFaceAnimationItemIndex].length, ((Screen.width - this.selectPanelWidth) - 20f) - this.blockEditPanelWidth, Color.red, Color.yellow, this._currentPart != FacePartType.RightEye))
            {
                this._currentPart = FacePartType.RightEye;
            }
            if (this.DrawBlockGraphArray(this._currentConfig.items[this._currentFaceAnimationItemIndex].mouthBlocks, provider3, this._currentConfig.items[this._currentFaceAnimationItemIndex].length, ((Screen.width - this.selectPanelWidth) - 20f) - this.blockEditPanelWidth, Color.red, Color.yellow, this._currentPart != FacePartType.Mouth))
            {
                this._currentPart = FacePartType.Mouth;
            }
            GUILayout.EndScrollView();
        }

        private void DrawAnimationPanelGUI()
        {
            Color backgroundColor = GUI.backgroundColor;
            Color color = GUI.color;
            GUI.backgroundColor = Color.cyan;
            Rect position = new Rect(5f, (Screen.height - this.faceAnimationPanelHeight) - 5f, Screen.width - 10f, this.faceAnimationPanelHeight);
            GUI.Box(position, string.Empty);
            if (this._currentFaceAnimationItemIndex >= 0)
            {
                GUILayout.BeginArea(position);
                Rect screenRect = new Rect(5f, 5f, this.selectPanelWidth, this.faceAnimationPanelHeight);
                GUILayout.BeginArea(screenRect);
                GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.Width(this.selectPanelWidth - 10f) };
                GUILayout.BeginHorizontal(options);
                GUILayout.Label("Name:", new GUILayoutOption[0]);
                string name = GUILayout.TextField(this._currentConfig.items[this._currentFaceAnimationItemIndex].name, new GUILayoutOption[0]);
                if (name != this._currentConfig.items[this._currentFaceAnimationItemIndex].name)
                {
                    this._currentConfig.items[this._currentFaceAnimationItemIndex].name = name;
                    this._curAvatar.RebuildFaceAnimation();
                    this._curAvatar.PrepareFaceAnimation(name);
                    this.changed = true;
                }
                GUILayout.EndHorizontal();
                float timePerFrame = this._currentConfig.items[this._currentFaceAnimationItemIndex].timePerFrame;
                GUILayoutOption[] optionArray2 = new GUILayoutOption[] { GUILayout.Width(this.selectPanelWidth - 10f) };
                this._currentConfig.items[this._currentFaceAnimationItemIndex].timePerFrame = GUILayout.HorizontalSlider(this._currentConfig.items[this._currentFaceAnimationItemIndex].timePerFrame, this.timePerFrameMin, this.timePerFrameMax, optionArray2);
                float result = this._currentConfig.items[this._currentFaceAnimationItemIndex].timePerFrame * 60f;
                GUILayout.BeginHorizontal(new GUILayoutOption[0]);
                GUILayout.Label(string.Format("RF/AF", new object[0]), new GUILayoutOption[0]);
                string s = GUILayout.TextField(result.ToString(), new GUILayoutOption[0]);
                float.TryParse(s, out result);
                this._currentConfig.items[this._currentFaceAnimationItemIndex].timePerFrame = (result * 1f) / 60f;
                if (timePerFrame != this._currentConfig.items[this._currentFaceAnimationItemIndex].timePerFrame)
                {
                    this.changed = true;
                }
                GUILayout.EndHorizontal();
                this._curAvatar.SetAnimationTimePerFrame(this._currentConfig.items[this._currentFaceAnimationItemIndex].timePerFrame);
                GUILayoutOption[] optionArray3 = new GUILayoutOption[] { GUILayout.Width(this.selectPanelWidth - 10f) };
                GUILayout.BeginHorizontal(optionArray3);
                GUILayout.Label("Frames:", new GUILayoutOption[0]);
                int length = this._currentConfig.items[this._currentFaceAnimationItemIndex].length;
                string introduced16 = GUILayout.TextField(this._currentConfig.items[this._currentFaceAnimationItemIndex].length.ToString(), new GUILayoutOption[0]);
                if (int.TryParse(introduced16, out this._currentConfig.items[this._currentFaceAnimationItemIndex].length) && (length != this._currentConfig.items[this._currentFaceAnimationItemIndex].length))
                {
                    this._curAvatar.RebuildFaceAnimation();
                    this.changed = true;
                }
                GUILayout.EndHorizontal();
                if (this._eyeCopyDirection == 1)
                {
                    GUILayout.Label("L => R Ready", new GUILayoutOption[0]);
                }
                else if (this._eyeCopyDirection == 2)
                {
                    GUILayout.Label("R => L Ready", new GUILayoutOption[0]);
                }
                GUILayout.EndArea();
                Rect rect3 = new Rect(5f + this.selectPanelWidth, 5f, ((Screen.width - this.selectPanelWidth) - 20f) - this.blockEditPanelWidth, this.faceAnimationPanelHeight);
                GUILayout.BeginArea(rect3);
                this.DrawAnimationContentEditPanel();
                GUILayout.EndArea();
                Rect rect4 = new Rect((Screen.width - 5f) - this.blockEditPanelWidth, 5f, this.blockEditPanelWidth, this.faceAnimationPanelHeight);
                GUILayout.BeginArea(rect4);
                this._clipboardScrollPos = GUILayout.BeginScrollView(this._clipboardScrollPos, new GUILayoutOption[0]);
                this.DrawClipboard();
                GUILayout.EndScrollView();
                GUILayout.EndArea();
                GUILayout.EndArea();
            }
            GUI.backgroundColor = backgroundColor;
            GUI.color = color;
        }

        private void DrawAnimationSelectGUI()
        {
            Color backgroundColor = GUI.backgroundColor;
            Color color = GUI.color;
            GUI.backgroundColor = Color.cyan;
            Rect position = new Rect(5f, 5f, this.selectPanelWidth, (Screen.height - 15f) - this.faceAnimationPanelHeight);
            GUI.Box(position, string.Empty);
            GUILayout.BeginArea(position);
            int num = this._currentAvatarIndex;
            this._currentAvatarIndex = this.DrawSwitchView(this.avatarNames, this._currentAvatarIndex, 90f);
            if (num != this._currentAvatarIndex)
            {
                this.SwitchAvatarGameObject();
            }
            if (this._playing)
            {
                if (GUILayout.Button("Stop", new GUILayoutOption[0]) && (this._currentFaceAnimationItemIndex >= 0))
                {
                    if (this._animator != null)
                    {
                        this._animator.Play("StandBy");
                    }
                    this._playing = false;
                    this._normalizedTime = 0f;
                    Singleton<WwiseAudioManager>.Instance.StopAll();
                }
            }
            else if ((GUILayout.Button("Play", new GUILayoutOption[0]) && (this._currentFaceAnimationItemIndex >= 0)) && ((this._currentConfig.items[this._currentFaceAnimationItemIndex].length * this._currentConfig.items[this._currentFaceAnimationItemIndex].timePerFrame) > 0f))
            {
                this._playing = true;
                this._normalizedTime = 0f;
                if ((this._useAnimator && (this._animator != null)) && !string.IsNullOrEmpty(this._animatorStateName))
                {
                    this._animator.Play(this._animatorStateName);
                }
            }
            this._useAnimator = GUILayout.Toggle(this._useAnimator, "With Animation", new GUILayoutOption[0]);
            GUI.enabled = this._useAnimator;
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            if (GUILayout.Button("-", new GUILayoutOption[0]))
            {
                this._heartLevel = Mathf.Clamp(this._heartLevel - 1, 1, 4);
            }
            GUILayout.TextField(this._heartLevel.ToString(), new GUILayoutOption[0]);
            if (GUILayout.Button("+", new GUILayoutOption[0]))
            {
                this._heartLevel = Mathf.Clamp(this._heartLevel + 1, 1, 4);
            }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            if (GUILayout.Button("-", new GUILayoutOption[0]))
            {
                this._action = Mathf.Clamp(this._action - 1, 1, 9);
            }
            GUILayout.TextField(this._action.ToString(), new GUILayoutOption[0]);
            if (GUILayout.Button("+", new GUILayoutOption[0]))
            {
                this._action = Mathf.Clamp(this._action + 1, 1, 9);
            }
            GUILayout.EndHorizontal();
            GUILayout.EndHorizontal();
            string str = string.Empty;
            if ((this._curAvatar.avatarId / 100) == 1)
            {
                str = "Kia";
            }
            else if ((this._curAvatar.avatarId / 100) == 2)
            {
                str = "Mei";
            }
            else if ((this._curAvatar.avatarId / 100) == 3)
            {
                str = "Bro";
            }
            this._animatorStateName = string.Format("Gal_{0}_A_{1}_{2}", str, this._heartLevel.ToString(), this._action.ToString());
            GUILayout.TextField(this._animatorStateName, new GUILayoutOption[0]);
            GUI.enabled = true;
            GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.Height(150f) };
            this._faceAnimationListScrollPos = GUILayout.BeginScrollView(this._faceAnimationListScrollPos, options);
            int index = 0;
            int length = this._currentConfig.items.Length;
            while (index < length)
            {
                string name = this._currentConfig.items[index].name;
                if (GUILayout.Toggle(index == this._currentFaceAnimationItemIndex, name, new GUILayoutOption[0]) && (index != this._currentFaceAnimationItemIndex))
                {
                    this._currentFaceAnimationItemIndex = index;
                    this._curAvatar.PrepareFaceAnimation(this._currentConfig.items[this._currentFaceAnimationItemIndex].name);
                    this._playing = false;
                    this._normalizedTime = 0f;
                }
                index++;
            }
            GUILayout.EndScrollView();
            bool enabled = GUI.enabled;
            GUI.enabled = this.openned;
            if (GUILayout.Button("Add", new GUILayoutOption[0]))
            {
                List<FaceAnimationItem> list = new List<FaceAnimationItem>(this._currentConfig.items);
                FaceAnimationItem item = new FaceAnimationItem {
                    length = 1,
                    timePerFrame = 0.016f,
                    name = string.Format("anim[{0}]", list.Count),
                    leftEyeBlocks = new FaceAnimationFrameBlock[0],
                    rightEyeBlocks = new FaceAnimationFrameBlock[0],
                    mouthBlocks = new FaceAnimationFrameBlock[0]
                };
                list.Add(item);
                this._currentConfig.items = list.ToArray();
                this._curAvatar.RebuildFaceAnimation();
                this.changed = true;
            }
            if ((this._currentFaceAnimationItemIndex >= 0) && GUILayout.Button("Remove", new GUILayoutOption[0]))
            {
                List<FaceAnimationItem> list2 = new List<FaceAnimationItem>(this._currentConfig.items);
                list2.RemoveAt(this._currentFaceAnimationItemIndex);
                this._currentFaceAnimationItemIndex = (list2.Count != 0) ? Mathf.Clamp(this._currentFaceAnimationItemIndex, 0, list2.Count - 1) : -1;
                this._currentConfig.items = list2.ToArray();
                this.changed = true;
            }
            if ((this._currentFaceAnimationItemIndex >= 0) && GUILayout.Button("Insert", new GUILayoutOption[0]))
            {
                List<FaceAnimationItem> list3 = new List<FaceAnimationItem>(this._currentConfig.items);
                FaceAnimationItem item2 = new FaceAnimationItem {
                    length = 1,
                    timePerFrame = 0.016f,
                    name = string.Format("anim[{0}]", this._currentFaceAnimationItemIndex),
                    leftEyeBlocks = new FaceAnimationFrameBlock[0],
                    rightEyeBlocks = new FaceAnimationFrameBlock[0],
                    mouthBlocks = new FaceAnimationFrameBlock[0]
                };
                list3.Insert(this._currentFaceAnimationItemIndex, item2);
                this._currentConfig.items = list3.ToArray();
                this._curAvatar.RebuildFaceAnimation();
                this.changed = true;
            }
            GUILayout.EndArea();
            GUI.enabled = enabled;
            GUI.backgroundColor = backgroundColor;
            GUI.color = color;
        }

        private bool DrawBlockGraphArray(FaceAnimationFrameBlock[] blocks, TestMatInfoProvider provider, int totalLength, float width, Color color0, Color color1, bool halfColor = false)
        {
            bool flag = false;
            Color backgroundColor = GUI.backgroundColor;
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            int num = 0;
            int index = 0;
            int length = blocks.Length;
            while (index < length)
            {
                GUI.backgroundColor = ((index % 2) != 0) ? color1 : color0;
                if (halfColor)
                {
                    GUI.backgroundColor = (Color) (GUI.backgroundColor * 0.5f);
                }
                FaceAnimationFrameBlock block = blocks[index];
                int num4 = 0;
                int frameLength = block.frameLength;
                while (num4 < frameLength)
                {
                    int num6 = 0;
                    string[] matInfoNames = provider.GetMatInfoNames();
                    int num7 = 0;
                    int num8 = matInfoNames.Length;
                    while (num7 < num8)
                    {
                        if (block.frameKey == matInfoNames[num7])
                        {
                            num6 = num7;
                            break;
                        }
                        num7++;
                    }
                    GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.Width(this.frameWidth * this.frameButtonFactor) };
                    flag |= GUILayout.Button(num6.ToString(), options);
                    num++;
                    if (num >= totalLength)
                    {
                        break;
                    }
                    num4++;
                }
                if (num >= totalLength)
                {
                    break;
                }
                index++;
            }
            if (num < totalLength)
            {
                GUI.backgroundColor = Color.white;
                if (halfColor)
                {
                    GUI.backgroundColor = (Color) (GUI.backgroundColor * 0.5f);
                }
                int num9 = 0;
                int num10 = totalLength - num;
                while (num9 < num10)
                {
                    GUILayoutOption[] optionArray2 = new GUILayoutOption[] { GUILayout.Width(this.frameWidth * this.frameButtonFactor) };
                    flag |= GUILayout.Button("0", optionArray2);
                    num9++;
                }
            }
            GUILayout.EndHorizontal();
            GUI.backgroundColor = backgroundColor;
            return flag;
        }

        private void DrawCamFactor()
        {
            Color backgroundColor = GUI.backgroundColor;
            Color color = GUI.color;
            GUI.backgroundColor = Color.cyan;
            Rect position = new Rect(this.selectPanelWidth + 10f, 5f, ((Screen.width - this.selectPanelWidth) - this.blockEditPanelWidth) - 15f, this.camFactorViewHeight);
            GUI.Box(position, string.Empty);
            GUILayout.BeginArea(position);
            if (!this.openned)
            {
                Color color3 = GUI.color;
                GUI.color = Color.red;
                GUILayout.Label("View Mode", new GUILayoutOption[0]);
                GUI.color = color3;
            }
            else
            {
                GUILayout.Label(string.Format("File : {0}", this.currentFilePath), new GUILayoutOption[0]);
            }
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            int num = (int) (this.camFactor * 100f);
            GUILayout.Label(string.Format("Distance:({0})", num.ToString(), GUILayout.Width(60f)), new GUILayoutOption[0]);
            this.camFactor = GUILayout.HorizontalSlider(this.camFactor, 0f, 1f, new GUILayoutOption[0]);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUILayout.Label(string.Format("Rotation:", new object[0]), new GUILayoutOption[0]);
            this.rotation = GUILayout.HorizontalSlider(this.rotation, -100f, 100f, new GUILayoutOption[0]);
            base.transform.rotation = Quaternion.Euler(this.angleOfPitch, this.rotation, 0f);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUILayout.Label(string.Format("Heigth:", new object[0]), new GUILayoutOption[0]);
            this.heightOffset = GUILayout.HorizontalSlider(this.heightOffset, 1f, 2f, new GUILayoutOption[0]);
            base.transform.localPosition = new Vector3(0f, this.heightOffset, 0f);
            GUILayout.EndHorizontal();
            GUILayout.EndArea();
            GUI.backgroundColor = backgroundColor;
            GUI.color = color;
        }

        private void DrawClipboard()
        {
            GUILayout.Label("<Clipboard>", new GUILayoutOption[0]);
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUILayout.Label("Length", new GUILayoutOption[0]);
            GUILayout.Label("Index", new GUILayoutOption[0]);
            GUILayout.EndHorizontal();
            int num = 0;
            int count = this._clipBoard.Count;
            while (num < count)
            {
                GUILayout.BeginHorizontal(new GUILayoutOption[0]);
                GUILayout.TextField(this._clipBoard[num].frameLength.ToString(), new GUILayoutOption[0]);
                GUILayout.TextField(this._clipBoard[num].frameKey, new GUILayoutOption[0]);
                GUILayout.EndHorizontal();
                num++;
            }
        }

        private int DrawSwitchView(string[] strings, int index, float width = 90f)
        {
            int num = index;
            bool enabled = GUI.enabled;
            GUI.enabled = !this._playing;
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.Width(30f), GUILayout.Height(30f) };
            if (GUILayout.Button("<", options))
            {
                num = Mathf.Clamp(--num, 0, strings.Length - 1);
            }
            GUILayoutOption[] optionArray2 = new GUILayoutOption[] { GUILayout.Width(width), GUILayout.Height(30f) };
            GUILayout.Label(strings[index], optionArray2);
            GUILayoutOption[] optionArray3 = new GUILayoutOption[] { GUILayout.Width(30f), GUILayout.Height(30f) };
            if (GUILayout.Button(">", optionArray3))
            {
                num = Mathf.Clamp(++num, 0, strings.Length - 1);
            }
            GUILayout.EndHorizontal();
            GUI.enabled = enabled;
            return num;
        }

        private ConfigFaceAnimation GetAvatarConfigById(int id)
        {
            switch ((id / 100))
            {
                case 1:
                    return this._kianaConfig;

                case 2:
                    return this._meiConfig;

                case 3:
                    return this._bronyaConfig;
            }
            return null;
        }

        private FaceAnimationFrameBlock[] GetCurrentBlocks()
        {
            if ((this._currentFaceAnimationItemIndex < 0) || (this._currentFaceAnimationItemIndex >= this._currentConfig.items.Length))
            {
                return null;
            }
            FaceAnimationFrameBlock[] mouthBlocks = null;
            if (this._currentPart == FacePartType.LeftEye)
            {
                return this._currentConfig.items[this._currentFaceAnimationItemIndex].leftEyeBlocks;
            }
            if (this._currentPart == FacePartType.RightEye)
            {
                return this._currentConfig.items[this._currentFaceAnimationItemIndex].rightEyeBlocks;
            }
            if (this._currentPart == FacePartType.Mouth)
            {
                mouthBlocks = this._currentConfig.items[this._currentFaceAnimationItemIndex].mouthBlocks;
            }
            return mouthBlocks;
        }

        private string[] GetFaceAnimationItemNameArray()
        {
            List<string> list = new List<string>();
            int index = 0;
            int length = this._currentConfig.items.Length;
            while (index < length)
            {
                list.Add(this._currentConfig.items[index].name);
                index++;
            }
            return list.ToArray();
        }

        private int GetImageMapLength()
        {
            TestMatInfoProvider provider = null;
            if (this._currentPart == FacePartType.LeftEye)
            {
                if ((this._curAvatar.avatarId / 100) == 1)
                {
                    provider = this.kianaImageMap_L;
                }
                else if ((this._curAvatar.avatarId / 100) == 2)
                {
                    provider = this.meiImageMap_L;
                }
                else if ((this._curAvatar.avatarId / 100) == 3)
                {
                    provider = this.bronyaImageMap_L;
                }
            }
            else if (this._currentPart == FacePartType.RightEye)
            {
                if ((this._curAvatar.avatarId / 100) == 1)
                {
                    provider = this.kianaImageMap_R;
                }
                else if ((this._curAvatar.avatarId / 100) == 2)
                {
                    provider = this.meiImageMap_R;
                }
                else if ((this._curAvatar.avatarId / 100) == 3)
                {
                    provider = this.bronyaImageMap_R;
                }
            }
            else if (this._currentPart == FacePartType.Mouth)
            {
                if ((this._curAvatar.avatarId / 100) == 1)
                {
                    provider = this.kianaImageMap_M;
                }
                else if ((this._curAvatar.avatarId / 100) == 2)
                {
                    provider = this.meiImageMap_M;
                }
                else if ((this._curAvatar.avatarId / 100) == 3)
                {
                    provider = this.bronyaImageMap_M;
                }
            }
            if (provider == null)
            {
                return -1;
            }
            return provider.GetMatInfoNames().Length;
        }

        public void Init()
        {
            this.InitConfigs();
            this.InitAvatars();
            this.SwitchAvatarGameObject();
            this.heightOffset = base.transform.position.y;
        }

        private void InitAvatars()
        {
            int index = 0;
            int length = this.avatarObjects.Length;
            while (index < length)
            {
                FaceAnimationEditAvatar component = this.avatarObjects[index].GetComponent<FaceAnimationEditAvatar>();
                if (component != null)
                {
                    ConfigFaceAnimation avatarConfigById = this.GetAvatarConfigById(component.avatarId);
                    component.SetupFaceAnimation(avatarConfigById);
                }
                index++;
            }
        }

        private void InitConfigs()
        {
            this._kianaConfig = Resources.Load<ConfigFaceAnimation>("FaceAnimation/Kiana");
            this._meiConfig = Resources.Load<ConfigFaceAnimation>("FaceAnimation/Mei");
            this._bronyaConfig = Resources.Load<ConfigFaceAnimation>("FaceAnimation/Bronya");
        }

        private void InitWwise()
        {
            Singleton<WwiseAudioManager>.Create();
            string[] soundBankNames = new string[] { "BK_MainMenu" };
            Singleton<WwiseAudioManager>.Instance.PushSoundBankScale(soundBankNames);
        }

        private string MapFromIndexToName(int index)
        {
            TestMatInfoProvider provider = null;
            if (this._currentPart == FacePartType.LeftEye)
            {
                if ((this._curAvatar.avatarId / 100) == 1)
                {
                    provider = this.kianaImageMap_L;
                }
                else if ((this._curAvatar.avatarId / 100) == 2)
                {
                    provider = this.meiImageMap_L;
                }
                else if ((this._curAvatar.avatarId / 100) == 3)
                {
                    provider = this.bronyaImageMap_L;
                }
            }
            else if (this._currentPart == FacePartType.RightEye)
            {
                if ((this._curAvatar.avatarId / 100) == 1)
                {
                    provider = this.kianaImageMap_R;
                }
                else if ((this._curAvatar.avatarId / 100) == 2)
                {
                    provider = this.meiImageMap_R;
                }
                else if ((this._curAvatar.avatarId / 100) == 3)
                {
                    provider = this.bronyaImageMap_R;
                }
            }
            else if (this._currentPart == FacePartType.Mouth)
            {
                if ((this._curAvatar.avatarId / 100) == 1)
                {
                    provider = this.kianaImageMap_M;
                }
                else if ((this._curAvatar.avatarId / 100) == 2)
                {
                    provider = this.meiImageMap_M;
                }
                else if ((this._curAvatar.avatarId / 100) == 3)
                {
                    provider = this.bronyaImageMap_M;
                }
            }
            if (provider != null)
            {
                string[] matInfoNames = provider.GetMatInfoNames();
                if (((matInfoNames != null) && (index >= 0)) && (index < matInfoNames.Length))
                {
                    return matInfoNames[index];
                }
            }
            return null;
        }

        private int MapFromNameToIndex(string name)
        {
            TestMatInfoProvider provider = null;
            if (this._currentPart == FacePartType.LeftEye)
            {
                if ((this._curAvatar.avatarId / 100) == 1)
                {
                    provider = this.kianaImageMap_L;
                }
                else if ((this._curAvatar.avatarId / 100) == 2)
                {
                    provider = this.meiImageMap_L;
                }
                else if ((this._curAvatar.avatarId / 100) == 3)
                {
                    provider = this.bronyaImageMap_L;
                }
            }
            else if (this._currentPart == FacePartType.RightEye)
            {
                if ((this._curAvatar.avatarId / 100) == 1)
                {
                    provider = this.kianaImageMap_R;
                }
                else if ((this._curAvatar.avatarId / 100) == 2)
                {
                    provider = this.meiImageMap_R;
                }
                else if ((this._curAvatar.avatarId / 100) == 3)
                {
                    provider = this.bronyaImageMap_R;
                }
            }
            else if (this._currentPart == FacePartType.Mouth)
            {
                if ((this._curAvatar.avatarId / 100) == 1)
                {
                    provider = this.kianaImageMap_M;
                }
                else if ((this._curAvatar.avatarId / 100) == 2)
                {
                    provider = this.meiImageMap_M;
                }
                else if ((this._curAvatar.avatarId / 100) == 3)
                {
                    provider = this.bronyaImageMap_M;
                }
            }
            if (provider != null)
            {
                string[] matInfoNames = provider.GetMatInfoNames();
                int index = 0;
                int length = matInfoNames.Length;
                while (index < length)
                {
                    if (name == matInfoNames[index])
                    {
                        return index;
                    }
                    index++;
                }
            }
            return 0;
        }

        private void OnGUI()
        {
            this.DrawCamFactor();
            this.DrawAnimationSelectGUI();
            if ((this._currentFaceAnimationItemIndex >= 0) && (this._currentFaceAnimationItemIndex < this._currentConfig.items.Length))
            {
                GUI.enabled = this.openned;
                this.DrawAnimationPanelGUI();
                this.DrawAnimationBlocksView();
                GUI.enabled = true;
            }
        }

        private void PasteIndex()
        {
            FaceAnimationFrameBlock[] currentBlocks = this.GetCurrentBlocks();
            if (currentBlocks != null)
            {
                List<FaceAnimationFrameBlock> list = new List<FaceAnimationFrameBlock>(currentBlocks);
                for (int i = this._clipBoard.Count - 1; i >= 0; i--)
                {
                    FaceAnimationFrameBlock block = new FaceAnimationFrameBlock {
                        frameLength = this._clipBoard[i].frameLength,
                        frameKey = this._clipBoard[i].frameKey
                    };
                    list.Insert(this.selectIndex, block);
                }
                FaceAnimationItem item = this._currentConfig.items[this._currentFaceAnimationItemIndex];
                if (this._currentPart == FacePartType.LeftEye)
                {
                    item.leftEyeBlocks = list.ToArray();
                }
                if (this._currentPart == FacePartType.RightEye)
                {
                    item.rightEyeBlocks = list.ToArray();
                }
                if (this._currentPart == FacePartType.Mouth)
                {
                    item.mouthBlocks = list.ToArray();
                }
                this._curAvatar.RebuildFaceAnimation();
                this.changed = true;
            }
        }

        private void PasteTail()
        {
            FaceAnimationFrameBlock[] currentBlocks = this.GetCurrentBlocks();
            if (currentBlocks != null)
            {
                List<FaceAnimationFrameBlock> list = new List<FaceAnimationFrameBlock>(currentBlocks);
                for (int i = 0; i < this._clipBoard.Count; i++)
                {
                    FaceAnimationFrameBlock block = new FaceAnimationFrameBlock {
                        frameLength = this._clipBoard[i].frameLength,
                        frameKey = this._clipBoard[i].frameKey
                    };
                    list.Add(block);
                }
                FaceAnimationItem item = this._currentConfig.items[this._currentFaceAnimationItemIndex];
                if (this._currentPart == FacePartType.LeftEye)
                {
                    item.leftEyeBlocks = list.ToArray();
                }
                if (this._currentPart == FacePartType.RightEye)
                {
                    item.rightEyeBlocks = list.ToArray();
                }
                if (this._currentPart == FacePartType.Mouth)
                {
                    item.mouthBlocks = list.ToArray();
                }
                this._curAvatar.RebuildFaceAnimation();
                this.changed = true;
            }
        }

        public void Refresh()
        {
            this.InitAvatars();
            this.SwitchAvatarGameObject();
        }

        private void Start()
        {
            this.Init();
        }

        private void SwitchAvatarGameObject()
        {
            if (this._currentAvatarIndex < this.avatarObjects.Length)
            {
                int index = 0;
                int length = this.avatarObjects.Length;
                while (index < length)
                {
                    this.avatarObjects[index].SetActive(index == this._currentAvatarIndex);
                    index++;
                }
                this._curAvatar = this.avatarObjects[this._currentAvatarIndex].GetComponent<FaceAnimationEditAvatar>();
                this._currentConfig = this.GetAvatarConfigById(this._curAvatar.avatarId);
                if ((this._currentConfig != null) && (this._currentConfig.items.Length > 0))
                {
                    this._currentFaceAnimationItemIndex = 0;
                }
                else
                {
                    this._currentFaceAnimationItemIndex = -1;
                }
                if (this._currentFaceAnimationItemIndex >= 0)
                {
                    this._curAvatar.PrepareFaceAnimation(this._currentConfig.items[this._currentFaceAnimationItemIndex].name);
                }
                this._animator = this._curAvatar.gameObject.GetComponent<Animator>();
                this._curAvatar.RebuildFaceAnimation();
            }
        }

        private void Update()
        {
            Vector3 vector = Vector3.Lerp(this.camPosTrans0.position, this.camPosTrans1.position, this.camFactor);
            Vector3 worldPosition = Vector3.Lerp(this.camFocTrans0.position, this.camFocTrans1.position, this.camFactor);
            Camera.main.transform.position = vector;
            Camera.main.transform.LookAt(worldPosition);
            if (((this._curAvatar != null) && (this._currentConfig != null)) && (this._currentFaceAnimationItemIndex >= 0))
            {
                float num = this._currentConfig.items[this._currentFaceAnimationItemIndex].timePerFrame * this._currentConfig.items[this._currentFaceAnimationItemIndex].length;
                float time = this._normalizedTime * num;
                if (this._playing)
                {
                    time += Time.deltaTime;
                    if ((num > 0f) && (time > num))
                    {
                        if (this._useAnimator)
                        {
                            if ((this._animator == null) || this._animator.GetCurrentAnimatorStateInfo(0).IsName("StandBy"))
                            {
                                this._playing = false;
                            }
                        }
                        else
                        {
                            this._playing = false;
                        }
                    }
                    this._normalizedTime = !this._playing ? 0f : (time / num);
                }
                if (this._curAvatar != null)
                {
                    this._curAvatar.SetAnimationTime(time);
                }
                this.UpdateHotkey();
            }
        }

        private void UpdateHotkey()
        {
            if (this.openned)
            {
                if (Input.GetKey(KeyCode.Z))
                {
                    if (Input.GetKeyDown(KeyCode.C))
                    {
                        this.Copy();
                    }
                    if (Input.GetKeyDown(KeyCode.V))
                    {
                        this.PasteIndex();
                    }
                    if (Input.GetKeyDown(KeyCode.B))
                    {
                        this.PasteTail();
                    }
                    if (Input.GetKeyDown(KeyCode.R))
                    {
                        if (this._eyeCopyDirection == 0)
                        {
                            this._eyeCopyDirection = 2;
                        }
                        else if (this._eyeCopyDirection == 1)
                        {
                            this._eyeCopyDirection = 0;
                            this._currentConfig.items[this._currentFaceAnimationItemIndex].rightEyeBlocks = this.CreateInverseEyeBlocks(this._currentConfig.items[this._currentFaceAnimationItemIndex].leftEyeBlocks, 'L', 'R');
                        }
                    }
                    if (Input.GetKeyDown(KeyCode.L))
                    {
                        if (this._eyeCopyDirection == 0)
                        {
                            this._eyeCopyDirection = 1;
                        }
                        else if (this._eyeCopyDirection == 2)
                        {
                            this._eyeCopyDirection = 0;
                            this._currentConfig.items[this._currentFaceAnimationItemIndex].leftEyeBlocks = this.CreateInverseEyeBlocks(this._currentConfig.items[this._currentFaceAnimationItemIndex].rightEyeBlocks, 'R', 'L');
                        }
                    }
                }
                else
                {
                    this._eyeCopyDirection = 0;
                }
            }
        }

        public bool changed
        {
            get
            {
                return this._changed;
            }
            set
            {
                bool flag = this._changed;
                this._changed = value;
                if ((flag != this._changed) && (this.Changed != null))
                {
                    this.Changed(this._changed);
                }
            }
        }

        public string currentFilePath { get; set; }

        public bool openned { get; set; }

        public bool running { get; set; }

        public delegate void ChangedHandler(bool changed);

        public enum FacePartType
        {
            LeftEye,
            RightEye,
            Mouth
        }
    }
}

