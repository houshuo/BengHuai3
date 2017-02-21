namespace MoleMole
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public class MonoSwitchTeammateAnimPlugin : MonoBehaviour
    {
        private float _animDuration = 0.25f;
        private float _animEclapsed;
        private Vector2 _animFrom;
        private int _animFromIndex;
        private Vector2 _animTo;
        private int _animToIndex;
        private bool _bSwitchAnim;
        private List<MonoTeamMember> _memberList;
        public RefreshTeammateUI_Handler _OnRefreshTeammateUI;
        private GameObject _switchAnimObj;

        private GameObject CreateAnimIcon(int dataIndex)
        {
            GameObject original = null;
            original = this.GetTeamMember(dataIndex).gameObject;
            GameObject obj3 = UnityEngine.Object.Instantiate<GameObject>(original);
            obj3.transform.SetParent(original.transform.parent, false);
            obj3.transform.Find("ChangeIcon").gameObject.SetActive(false);
            obj3.transform.Find("Btn").gameObject.SetActive(false);
            obj3.transform.Find("BG/LeaderTopBound").gameObject.SetActive(false);
            obj3.GetComponent<RectTransform>().anchoredPosition = original.GetComponent<RectTransform>().anchoredPosition;
            return obj3;
        }

        private MonoTeamMember GetTeamMember(int index)
        {
            foreach (MonoTeamMember member in this._memberList)
            {
                if (member.GetIndex() == index)
                {
                    return member;
                }
            }
            return null;
        }

        public bool IsPlaying()
        {
            return this._bSwitchAnim;
        }

        private void OnSwitchAnimEnd()
        {
            this._switchAnimObj.GetComponent<RectTransform>().anchoredPosition = this._animTo;
            this._bSwitchAnim = false;
            UnityEngine.Object.Destroy(this._switchAnimObj);
            this._switchAnimObj = null;
            bool bSelfSkill = (this._animFromIndex == 1) || (this._animToIndex == 1);
            if (this._OnRefreshTeammateUI != null)
            {
                this._OnRefreshTeammateUI(this._animToIndex, bSelfSkill);
            }
        }

        public void RegisterCallback(RefreshTeammateUI_Handler refreshTeammateUIHandler)
        {
            this._OnRefreshTeammateUI = refreshTeammateUIHandler;
        }

        private void Start()
        {
            this._memberList = new List<MonoTeamMember>();
            Transform transform = base.transform.Find("TeamPanel/Team");
            for (int i = 1; i <= 3; i++)
            {
                MonoTeamMember component = transform.Find(i.ToString()).gameObject.GetComponent<MonoTeamMember>();
                this._memberList.Add(component);
            }
        }

        public void StartSwitchAnim(int dataIndex, int fromIndex, int toIndex)
        {
            this._bSwitchAnim = true;
            this._switchAnimObj = this.CreateAnimIcon(dataIndex);
            this._animFrom = this.GetTeamMember(fromIndex).GetComponent<RectTransform>().anchoredPosition;
            this._animTo = this.GetTeamMember(toIndex).GetComponent<RectTransform>().anchoredPosition;
            this._animEclapsed = 0f;
            this._animFromIndex = fromIndex;
            this._animToIndex = toIndex;
        }

        private void Update()
        {
            if (this._bSwitchAnim && (this._switchAnimObj != null))
            {
                this._animEclapsed += Time.deltaTime;
                float t = this._animEclapsed / this._animDuration;
                if (t < 1f)
                {
                    this._switchAnimObj.GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(this._animFrom, this._animTo, t);
                }
                else
                {
                    this.OnSwitchAnimEnd();
                }
            }
        }
    }
}

