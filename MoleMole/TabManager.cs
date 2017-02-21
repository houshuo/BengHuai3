namespace MoleMole
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using UnityEngine;
    using UnityEngine.UI;

    public class TabManager
    {
        private string _showingKey;
        private Dictionary<string, Button> _tabBtnMap = new Dictionary<string, Button>();
        private Dictionary<string, GameObject> _tabContentMap = new Dictionary<string, GameObject>();

        public event OnSetActive onSetActive;

        public void Clear()
        {
            this._tabContentMap.Clear();
            this._tabBtnMap.Clear();
        }

        public List<string> GetKeys()
        {
            return new List<string>(this._tabBtnMap.Keys);
        }

        public GameObject GetShowingTabContent()
        {
            return this.GetTabContent(this._showingKey);
        }

        public string GetShowingTabKey()
        {
            return this._showingKey;
        }

        public GameObject GetTabContent(string key)
        {
            return (!this._tabContentMap.ContainsKey(key) ? null : this._tabContentMap[key]);
        }

        public void SetTab(string key, Button btn, GameObject content)
        {
            this._tabContentMap[key] = content;
            this._tabBtnMap[key] = btn;
        }

        public void ShowTab(string searchKey)
        {
            this._showingKey = searchKey;
            foreach (string str in this._tabContentMap.Keys)
            {
                if (this.onSetActive != null)
                {
                    this.onSetActive(searchKey == str, this._tabContentMap[str], this._tabBtnMap[str]);
                }
            }
        }

        public delegate void OnSetActive(bool active, GameObject content, Button btn);
    }
}

