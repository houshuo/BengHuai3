namespace MoleMole
{
    using System;
    using UnityEngine;

    [ExecuteInEditMode]
    public class MonoItemAttributeDiff : MonoBehaviour
    {
        private AvatarDataItem _avatarData;
        private StorageDataItemBase _itemAfter;
        private StorageDataItemBase _itemBefore;
        private Action<Transform, float, float> _setupAttr;
        public bool showKeepIcon;

        private void SetupStatus()
        {
            float hPAddWithAffix = 0f;
            float hPAdd = 0f;
            if (this._itemBefore != null)
            {
                if (this._itemBefore is StigmataDataItem)
                {
                    hPAddWithAffix = (this._itemBefore as StigmataDataItem).GetHPAddWithAffix(this._avatarData);
                }
                else
                {
                    hPAddWithAffix = this._itemBefore.GetHPAdd();
                }
            }
            if (this._itemAfter != null)
            {
                if (this._itemAfter is StigmataDataItem)
                {
                    hPAdd = (this._itemAfter as StigmataDataItem).GetHPAddWithAffix(this._avatarData);
                }
                else
                {
                    hPAdd = this._itemAfter.GetHPAdd();
                }
            }
            if (this._setupAttr != null)
            {
                this._setupAttr(base.transform.Find("HP"), hPAddWithAffix, hPAdd);
            }
            float sPAddWithAffix = 0f;
            float sPAdd = 0f;
            if (this._itemBefore != null)
            {
                if (this._itemBefore is StigmataDataItem)
                {
                    sPAddWithAffix = (this._itemBefore as StigmataDataItem).GetSPAddWithAffix(this._avatarData);
                }
                else
                {
                    sPAddWithAffix = this._itemBefore.GetSPAdd();
                }
            }
            if (this._itemAfter != null)
            {
                if (this._itemAfter is StigmataDataItem)
                {
                    sPAdd = (this._itemAfter as StigmataDataItem).GetSPAddWithAffix(this._avatarData);
                }
                else
                {
                    sPAdd = this._itemAfter.GetSPAdd();
                }
            }
            if (this._setupAttr != null)
            {
                this._setupAttr(base.transform.Find("SP"), sPAddWithAffix, sPAdd);
            }
            float attackAddWithAffix = 0f;
            float attackAdd = 0f;
            if (this._itemBefore != null)
            {
                if (this._itemBefore is StigmataDataItem)
                {
                    attackAddWithAffix = (this._itemBefore as StigmataDataItem).GetAttackAddWithAffix(this._avatarData);
                }
                else
                {
                    attackAddWithAffix = this._itemBefore.GetAttackAdd();
                }
            }
            if (this._itemAfter != null)
            {
                if (this._itemAfter is StigmataDataItem)
                {
                    attackAdd = (this._itemAfter as StigmataDataItem).GetAttackAddWithAffix(this._avatarData);
                }
                else
                {
                    attackAdd = this._itemAfter.GetAttackAdd();
                }
            }
            if (this._setupAttr != null)
            {
                this._setupAttr(base.transform.Find("ATK"), attackAddWithAffix, attackAdd);
            }
            float defenceAddWithAffix = 0f;
            float defenceAdd = 0f;
            if (this._itemBefore != null)
            {
                if (this._itemBefore is StigmataDataItem)
                {
                    defenceAddWithAffix = (this._itemBefore as StigmataDataItem).GetDefenceAddWithAffix(this._avatarData);
                }
                else
                {
                    defenceAddWithAffix = this._itemBefore.GetDefenceAdd();
                }
            }
            if (this._itemAfter != null)
            {
                if (this._itemAfter is StigmataDataItem)
                {
                    defenceAdd = (this._itemAfter as StigmataDataItem).GetDefenceAddWithAffix(this._avatarData);
                }
                else
                {
                    defenceAdd = this._itemAfter.GetDefenceAdd();
                }
            }
            if (this._setupAttr != null)
            {
                this._setupAttr(base.transform.Find("DEF"), defenceAddWithAffix, defenceAdd);
            }
            float criticalAddWithAffix = 0f;
            float criticalAdd = 0f;
            if (this._itemBefore != null)
            {
                if (this._itemBefore is StigmataDataItem)
                {
                    criticalAddWithAffix = (this._itemBefore as StigmataDataItem).GetCriticalAddWithAffix(this._avatarData);
                }
                else
                {
                    criticalAddWithAffix = this._itemBefore.GetCriticalAdd();
                }
            }
            if (this._itemAfter != null)
            {
                if (this._itemAfter is StigmataDataItem)
                {
                    criticalAdd = (this._itemAfter as StigmataDataItem).GetCriticalAddWithAffix(this._avatarData);
                }
                else
                {
                    criticalAdd = this._itemAfter.GetCriticalAdd();
                }
            }
            if (this._setupAttr != null)
            {
                this._setupAttr(base.transform.Find("CRT"), criticalAddWithAffix, criticalAdd);
            }
        }

        public void SetupView(AvatarDataItem avatarData, StorageDataItemBase itemBefore, StorageDataItemBase itemAfter, Action<Transform, float, float> setupAttr)
        {
            this._avatarData = avatarData;
            this._itemBefore = itemBefore;
            this._itemAfter = itemAfter;
            this._setupAttr = setupAttr;
            this.SetupStatus();
        }
    }
}

