namespace MoleMole
{
    using System;
    using UnityEngine;

    public class GalTouchTestDrive : MonoBehaviour
    {
        private GalTouchTestAvatar _curAvatar;
        private int _currentAvatarIndex;
        public int[] avatarIds = new int[] { 0x65, 0x66, 0x67, 0x68, 0xc9, 0xca, 0xcb, 0xcc, 0x12d, 0x12e, 0x12f, 0x130 };
        public string[] avatarNames = new string[] { "Kiana_C1", "Kiana_C2", "Kiana_C3", "Kiana_C4", "Mei_C1", "Mei_C2", "Mei_C3", "Mei_C4", "Bronya_C1", "Bronya_C2", "Bronya_C3", "Bronya_C4" };
        public GameObject[] avatarObjects;
        public float camFactor;
        public Transform camFocTrans0;
        public Transform camFocTrans1;
        public Transform camPosTrans0;
        public Transform camPosTrans1;

        private void Awake()
        {
            GlobalDataManager.metaConfig = ConfigUtil.LoadConfig<ConfigMetaConfig>("Common/MetaConfig");
            TouchPatternData.ReloadFromFile();
        }

        private void InitWwise()
        {
            Singleton<WwiseAudioManager>.Create();
            string[] soundBankNames = new string[] { "BK_MainMenu" };
            Singleton<WwiseAudioManager>.Instance.PushSoundBankScale(soundBankNames);
        }

        private void OnAvatarSelectGUI()
        {
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.Width(30f), GUILayout.Height(30f) };
            if (GUILayout.Button("<", options))
            {
                this._currentAvatarIndex = Mathf.Clamp(this._currentAvatarIndex - 1, 0, this.avatarNames.Length - 1);
                this.SwitchAvatarGameObject();
            }
            GUILayoutOption[] optionArray2 = new GUILayoutOption[] { GUILayout.Width(100f), GUILayout.Height(30f) };
            GUILayout.Label(this.avatarNames[this._currentAvatarIndex], optionArray2);
            GUILayoutOption[] optionArray3 = new GUILayoutOption[] { GUILayout.Width(30f), GUILayout.Height(30f) };
            if (GUILayout.Button(">", optionArray3))
            {
                this._currentAvatarIndex = Mathf.Clamp(this._currentAvatarIndex + 1, 0, this.avatarNames.Length - 1);
                this.SwitchAvatarGameObject();
            }
            GUILayout.EndHorizontal();
        }

        private void OnAvatarSettingGUI()
        {
            if (this._curAvatar != null)
            {
                GUILayout.Label(string.Format("AvatarID : {0}", this.avatarIds[this._currentAvatarIndex].ToString()), new GUILayoutOption[0]);
                GUILayout.BeginHorizontal(new GUILayoutOption[0]);
                GUILayout.Label("HeartLevel : ", new GUILayoutOption[0]);
                GUILayout.TextField(this._curAvatar.heartLevel.ToString(), new GUILayoutOption[0]);
                if (GUILayout.Button("-", new GUILayoutOption[0]))
                {
                    this._curAvatar.heartLevel = Mathf.Clamp(this._curAvatar.heartLevel - 1, 1, 4);
                    this._curAvatar.ResetGalTouchSystem();
                }
                if (GUILayout.Button("+", new GUILayoutOption[0]))
                {
                    this._curAvatar.heartLevel = Mathf.Clamp(this._curAvatar.heartLevel + 1, 1, 4);
                    this._curAvatar.ResetGalTouchSystem();
                }
                GUILayout.EndHorizontal();
            }
        }

        private void OnGUI()
        {
            this.camFactor = GUILayout.HorizontalScrollbar(this.camFactor, 0.01f, 0f, 1f, new GUILayoutOption[0]);
            this.OnAvatarSelectGUI();
            this.OnAvatarSettingGUI();
        }

        private void Start()
        {
            this.InitWwise();
            this.SwitchAvatarGameObject();
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
                this._curAvatar = this.avatarObjects[this._currentAvatarIndex].GetComponent<GalTouchTestAvatar>();
            }
        }

        private void Update()
        {
            Vector3 vector = Vector3.Lerp(this.camPosTrans0.position, this.camPosTrans1.position, this.camFactor);
            Vector3 worldPosition = Vector3.Lerp(this.camFocTrans0.position, this.camFocTrans1.position, this.camFactor);
            Camera.main.transform.position = vector;
            Camera.main.transform.LookAt(worldPosition);
        }
    }
}

