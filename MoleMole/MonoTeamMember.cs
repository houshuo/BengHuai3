namespace MoleMole
{
    using proto;
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.EventSystems;

    public class MonoTeamMember : MonoBehaviour, IEventSystemHandler, IDragHandler, IEndDragHandler, IBeginDragHandler
    {
        private MonoSwitchTeammateAnimPlugin _animPlugin;
        private AvatarDataItem _avatarData;
        private RectTransform _baseRect;
        private Camera _camera;
        private bool _enableDrag = true;
        private int _index;
        private StageType _levelType;
        private GameObject _objDrag;
        private Sprite _oldBGSprite;
        public RefreshTeammateUI_Handler _OnRefreshTeammateUI;
        public StartSwitchAnim_Handler _OnStartSwitchAnim;
        private List<MonoTeamMember> _otherTeamMembers = new List<MonoTeamMember>();
        private const string AVATAR_NULL_BG_PATH = "SpriteOutput/AvatarTachie/BgType4";

        private GameObject CreateDragIcon()
        {
            GameObject obj2 = UnityEngine.Object.Instantiate<GameObject>(base.gameObject);
            obj2.transform.SetParent(Singleton<MainUIManager>.Instance.SceneCanvas.transform, false);
            RectTransform component = obj2.GetComponent<RectTransform>();
            component.anchorMin = new Vector2(0.5f, 0.5f);
            component.anchorMax = new Vector2(0.5f, 0.5f);
            component.pivot = new Vector2(0.5f, 0.5f);
            obj2.transform.Find("ChangeIcon").gameObject.SetActive(false);
            obj2.transform.Find("Btn").gameObject.SetActive(false);
            obj2.transform.Find("BG/LeaderTopBound").gameObject.SetActive(false);
            obj2.transform.Find("BG/HightLightFrame").gameObject.SetActive(true);
            return obj2;
        }

        private Camera GetCamera()
        {
            if (this._camera == null)
            {
                this._camera = GameObject.Find("UICamera").GetComponent<Camera>();
            }
            return this._camera;
        }

        public int GetIndex()
        {
            return this._index;
        }

        private bool InRectTransform(RectTransform rect, Vector2 localPos)
        {
            return (((localPos.x >= 0f) && (localPos.x <= rect.sizeDelta.x)) && (Mathf.Abs(localPos.y) <= (rect.sizeDelta.y / 2f)));
        }

        private MonoTeamMember IsHoverMember(PointerEventData eventData)
        {
            foreach (MonoTeamMember member in this._otherTeamMembers)
            {
                Vector2 vector;
                RectTransform component = member.GetComponent<RectTransform>();
                RectTransformUtility.ScreenPointToLocalPointInRectangle(component, eventData.position, this.GetCamera(), out vector);
                if (this.InRectTransform(component, vector))
                {
                    return member;
                }
            }
            return null;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if ((this._enableDrag && Singleton<PlayerModule>.Instance.playerData.HasTeamMember(this._levelType, this._index)) && !this._animPlugin.IsPlaying())
            {
                this._objDrag = this.CreateDragIcon();
                base.transform.Find("Content").gameObject.SetActive(false);
                this._oldBGSprite = base.transform.Find("BG/BGColor").GetComponent<Image>().sprite;
                base.transform.Find("BG/BGColor").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab("SpriteOutput/AvatarTachie/BgType4");
            }
        }

        public void OnClick()
        {
            int num = (this._avatarData != null) ? this._avatarData.avatarID : 0;
            AvatarOverviewPageContext context = new AvatarOverviewPageContext {
                type = AvatarOverviewPageContext.PageType.TeamEdit,
                selectedAvatarID = num,
                teamEditIndex = this._index,
                levelType = this._levelType,
                showAvatarRemainHP = (this._levelType != 4) ? false : true
            };
            Singleton<MainUIManager>.Instance.ShowPage(context, UIType.Page);
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (this._enableDrag && (this._objDrag != null))
            {
                Vector2 vector;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(this._baseRect, eventData.position, this.GetCamera(), out vector);
                this._objDrag.GetComponent<RectTransform>().anchoredPosition = vector;
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (this._enableDrag && (this._objDrag != null))
            {
                UnityEngine.Object.Destroy(this._objDrag);
                this._objDrag = null;
                MonoTeamMember member = this.IsHoverMember(eventData);
                if ((member != null) && Singleton<PlayerModule>.Instance.playerData.HasTeamMember(this._levelType, member.GetIndex()))
                {
                    if (this._OnStartSwitchAnim != null)
                    {
                        this._OnStartSwitchAnim(member.GetIndex(), member.GetIndex(), this._index);
                    }
                    Singleton<PlayerModule>.Instance.playerData.SwitchTeamMember(this._levelType, member.GetIndex(), this.GetIndex());
                    Singleton<NetworkManager>.Instance.NotifyUpdateAvatarTeam(this._levelType);
                    if (this._OnRefreshTeammateUI != null)
                    {
                        this._OnRefreshTeammateUI(member.GetIndex(), false);
                    }
                }
                else
                {
                    base.transform.Find("BG/BGColor").GetComponent<Image>().sprite = this._oldBGSprite;
                    base.transform.Find("Content").gameObject.SetActive(true);
                }
            }
        }

        public void RegisterCallback(RefreshTeammateUI_Handler refreshTeammateUIHandler, StartSwitchAnim_Handler startSwitchAnimHandler)
        {
            this._OnRefreshTeammateUI = refreshTeammateUIHandler;
            this._OnStartSwitchAnim = startSwitchAnimHandler;
        }

        private void SetupAvatar()
        {
            base.transform.Find("Content/StarPanel/AvatarStar").GetComponent<MonoAvatarStar>().SetupView(this._avatarData.star);
            base.transform.Find("Content/LVNum").GetComponent<Text>().text = this._avatarData.level.ToString();
            base.transform.Find("Content/Avatar").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(this._avatarData.AvatarTachie);
        }

        private void SetUpAvatarDispatched(bool isDispatched)
        {
            base.transform.Find("Content/Avatar").GetComponent<Image>().color = !isDispatched ? MiscData.GetColor("TotalWhite") : MiscData.GetColor("EndlessEnergyRunout");
            base.transform.Find("Content/LVLabel").gameObject.SetActive(!isDispatched);
            base.transform.Find("Content/LVNum").gameObject.SetActive(!isDispatched);
            base.transform.Find("Content/Hint").gameObject.SetActive(isDispatched);
        }

        public void SetupView(StageType levelType, int index, MonoSwitchTeammateAnimPlugin animPlugin, AvatarDataItem avatarData = null, RectTransform baseRect = null)
        {
            this._levelType = levelType;
            this._index = index;
            this._avatarData = avatarData;
            this._baseRect = baseRect;
            this._animPlugin = animPlugin;
            bool flag = this._index == 1;
            base.transform.Find("BG/LeaderTopBound").gameObject.SetActive(flag);
            base.transform.Find("BG/MemberTopBound").gameObject.SetActive(!flag);
            base.transform.Find("Content").gameObject.SetActive(avatarData != null);
            if (avatarData != null)
            {
                base.transform.Find("BG/BGColor").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(MiscData.Config.AvatarAttributeBGSpriteList[this._avatarData.Attribute]);
                this.SetupAvatar();
            }
            else
            {
                base.transform.Find("BG/BGColor").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab("SpriteOutput/AvatarTachie/BgType4");
            }
            this._otherTeamMembers.Clear();
            Transform transform = this._baseRect.transform.Find("TeamPanel/Team");
            for (int i = 1; i <= 3; i++)
            {
                if (i != this._index)
                {
                    MonoTeamMember component = transform.Find(i.ToString()).GetComponent<MonoTeamMember>();
                    this._otherTeamMembers.Add(component);
                }
            }
            if (levelType == 4)
            {
                base.transform.Find("HPRemain").gameObject.SetActive(avatarData != null);
                if (avatarData != null)
                {
                    base.transform.Find("HPRemain").GetComponent<MonoRemainHP>().SetAvatarHPData(Singleton<EndlessModule>.Instance.GetEndlessAvatarHPData(avatarData.avatarID), null);
                }
            }
            bool isDispatched = (avatarData != null) && Singleton<IslandModule>.Instance.IsAvatarDispatched(avatarData.avatarID);
            this.SetUpAvatarDispatched(isDispatched);
        }
    }
}

