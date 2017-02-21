namespace MoleMole
{
    using proto;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using UnityEngine;
    using UnityEngine.UI;

    public class MonoTechNodeUI : MonoBehaviour
    {
        [SerializeField]
        private ParticleSystem _activationVFX;
        [SerializeField]
        private Sprite _brightBG;
        private CabinTechTreeNode _data;
        [SerializeField]
        private Sprite _grayBG;
        [SerializeField]
        private Material _grayMat;
        [SerializeField]
        private Sprite _halfBG;
        private int _x;
        private int _y;

        private void ClearStatusImage()
        {
            this.ResetGrayIcon();
            base.transform.Find("Lock").gameObject.SetActive(false);
            base.transform.Find("Lock/Cabin").gameObject.SetActive(false);
            base.transform.Find("Lock/Level").gameObject.SetActive(false);
            base.transform.Find("Lock/Avatar").gameObject.SetActive(false);
            base.transform.Find("New").gameObject.SetActive(false);
        }

        private void GrayIcon()
        {
            Image component = base.transform.Find("Icon").GetComponent<Image>();
            component.material = this._grayMat;
            Color color = component.color;
            color.a = 0.5f;
            component.color = color;
        }

        public void Init(CabinTechTreeNode data, int x, int y)
        {
            this._data = data;
            if (this._data != null)
            {
                this._data.RegisterCallback(new OnTechTreeNodeActive(this.OnNodeActive));
            }
            this._x = x;
            this._y = y;
            this.SetVisible();
        }

        private bool Invalid()
        {
            return (this._data == null);
        }

        public void OnClick()
        {
            if (!this.Invalid())
            {
                Singleton<MiHoYoGameData>.Instance.LocalData.SetVisited_CabinTechTreeNode(this._data._metaData.ID);
                base.transform.Find("New").gameObject.SetActive(false);
                Singleton<MainUIManager>.Instance.ShowDialog(new TechTreeNodeDialogContext(this._data), UIType.Any);
            }
        }

        private void OnDestroy()
        {
            if (this._data != null)
            {
                this._data.UnRegisterCallback();
            }
        }

        private void OnNodeActive()
        {
            if ((this._activationVFX != null) && (this._activationVFX.gameObject != null))
            {
                Singleton<WwiseAudioManager>.Instance.Post("UI_Island_Unlock_Tech", null, null, null);
                this._activationVFX.gameObject.SetActive(true);
                Singleton<ApplicationManager>.Instance.StartCoroutine(this.StopActiveVFX());
            }
        }

        private void Refresh_00_Flash()
        {
            if ((this._data._metaData.X == 0) && (this._data._metaData.Y == 0))
            {
                bool flag = this._data._status != TechTreeNodeStatus.Active;
                base.transform.Find("Panel").gameObject.SetActive(flag);
            }
        }

        private void RefreshBorder()
        {
            this.SetBorderStyle(1);
            bool flag = this._data._status == TechTreeNodeStatus.Active;
            foreach (CabinTechTreeNode node in this._data.GetNeibours())
            {
                if (node._metaData.X > this._data._metaData.X)
                {
                    base.transform.Find("Border/LineRight/1").gameObject.SetActive(false);
                    base.transform.Find("Border/LineRight/2").gameObject.SetActive(!flag);
                    base.transform.Find("Border/LineRight/3").gameObject.SetActive(flag);
                }
                else if (node._metaData.X < this._data._metaData.X)
                {
                    base.transform.Find("Border/LineLeft/1").gameObject.SetActive(false);
                    base.transform.Find("Border/LineLeft/2").gameObject.SetActive(!flag);
                    base.transform.Find("Border/LineLeft/3").gameObject.SetActive(flag);
                }
                else if (node._metaData.Y < this._data._metaData.Y)
                {
                    base.transform.Find("Border/LineTop/1").gameObject.SetActive(false);
                    base.transform.Find("Border/LineTop/2").gameObject.SetActive(!flag);
                    base.transform.Find("Border/LineTop/3").gameObject.SetActive(flag);
                }
                else if (node._metaData.Y > this._data._metaData.Y)
                {
                    base.transform.Find("Border/LineBottom/1").gameObject.SetActive(false);
                    base.transform.Find("Border/LineBottom/2").gameObject.SetActive(!flag);
                    base.transform.Find("Border/LineBottom/3").gameObject.SetActive(flag);
                }
            }
        }

        private void RefreshNew()
        {
            bool flag = false;
            bool flag2 = Singleton<MiHoYoGameData>.Instance.LocalData.IsVisited_CabinTechTreeNode(this._data._metaData.ID);
            if ((this._data._status == TechTreeNodeStatus.Active) || (this._data._status == TechTreeNodeStatus.Unlock_Ready_Active))
            {
                flag = !flag2;
            }
            base.transform.Find("New").gameObject.SetActive(flag);
        }

        public void RefreshStatus()
        {
            if (!this.Invalid())
            {
                this.ClearStatusImage();
                this.RefreshNew();
                this.RefreshBorder();
                this.Refresh_00_Flash();
                base.transform.Find("Icon").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(this._data._metaData.Icon);
                switch (this._data._status)
                {
                    case TechTreeNodeStatus.Lock:
                    {
                        base.transform.Find("Lock").gameObject.SetActive(true);
                        base.transform.Find("BG").GetComponent<Image>().sprite = this._grayBG;
                        this.GrayIcon();
                        TechTreeNodeLockInfo info = this._data.GetLockInfo()[0];
                        if ((info._lockType != TechTreeNodeLock.AvatarLevel) && (info._lockType != TechTreeNodeLock.AvatarUnlock))
                        {
                            if (info._lockType == TechTreeNodeLock.CabinLevel)
                            {
                                Transform transform3 = base.transform.Find("Lock/Cabin");
                                transform3.gameObject.SetActive(true);
                                string cabinName = Singleton<IslandModule>.Instance.GetCabinDataByType((CabinType) this._data._metaData.Cabin).GetCabinName();
                                transform3.GetComponent<Text>().text = cabinName;
                                Transform transform4 = base.transform.Find("Lock/Level");
                                transform4.gameObject.SetActive(true);
                                transform4.GetComponent<Text>().text = string.Format("Lv.{0}", info._needLevel);
                            }
                            break;
                        }
                        Transform transform = base.transform.Find("Lock/Avatar");
                        transform.gameObject.SetActive(true);
                        transform.GetComponent<Image>().sprite = UIUtil.GetAvatarCardIcon(this._data._metaData.UnlockAvatarID);
                        Transform transform2 = base.transform.Find("Lock/Level");
                        transform2.gameObject.SetActive(true);
                        transform2.GetComponent<Text>().text = string.Format("Lv.{0}", info._needLevel);
                        break;
                    }
                    case TechTreeNodeStatus.Unlock_Ban_Active:
                        base.transform.Find("BG").GetComponent<Image>().sprite = this._grayBG;
                        this.GrayIcon();
                        break;

                    case TechTreeNodeStatus.Unlock_Ready_Active:
                        base.transform.Find("BG").GetComponent<Image>().sprite = this._halfBG;
                        break;

                    case TechTreeNodeStatus.Active:
                        base.transform.Find("BG").GetComponent<Image>().sprite = this._brightBG;
                        break;
                }
            }
        }

        private void ResetGrayIcon()
        {
            Image component = base.transform.Find("Icon").GetComponent<Image>();
            component.material = null;
            Color color = component.color;
            color.a = 1f;
            component.color = color;
        }

        private void SetActiveBorderStyle()
        {
            this.SetBorderStyle(2);
            foreach (CabinTechTreeNode node in this._data.GetNeibours())
            {
                if (node._status == TechTreeNodeStatus.Active)
                {
                    if (node._metaData.X > this._data._metaData.X)
                    {
                        base.transform.Find("Border/LineRight/2").gameObject.SetActive(false);
                        base.transform.Find("Border/LineRight/3").gameObject.SetActive(true);
                    }
                    else if (node._metaData.X < this._data._metaData.X)
                    {
                        base.transform.Find("Border/LineLeft/2").gameObject.SetActive(false);
                        base.transform.Find("Border/LineLeft/3").gameObject.SetActive(true);
                    }
                    else if (node._metaData.Y < this._data._metaData.Y)
                    {
                        base.transform.Find("Border/LineTop/2").gameObject.SetActive(false);
                        base.transform.Find("Border/LineTop/3").gameObject.SetActive(true);
                    }
                    else if (node._metaData.Y > this._data._metaData.Y)
                    {
                        base.transform.Find("Border/LineBottom/2").gameObject.SetActive(false);
                        base.transform.Find("Border/LineBottom/3").gameObject.SetActive(true);
                    }
                }
            }
        }

        private void SetBorderStyle(int style)
        {
            base.transform.Find("Border/LineBottom/1").gameObject.SetActive(style == 1);
            base.transform.Find("Border/LineBottom/2").gameObject.SetActive(style == 2);
            base.transform.Find("Border/LineBottom/3").gameObject.SetActive(style == 3);
            base.transform.Find("Border/LineRight/1").gameObject.SetActive(style == 1);
            base.transform.Find("Border/LineRight/2").gameObject.SetActive(style == 2);
            base.transform.Find("Border/LineRight/3").gameObject.SetActive(style == 3);
            base.transform.Find("Border/LineTop/1").gameObject.SetActive(style == 1);
            base.transform.Find("Border/LineTop/2").gameObject.SetActive(style == 2);
            base.transform.Find("Border/LineTop/3").gameObject.SetActive(style == 3);
            base.transform.Find("Border/LineLeft/1").gameObject.SetActive(style == 1);
            base.transform.Find("Border/LineLeft/2").gameObject.SetActive(style == 2);
            base.transform.Find("Border/LineLeft/3").gameObject.SetActive(style == 3);
        }

        private void SetVisible()
        {
            base.transform.gameObject.SetActive(!this.Invalid());
        }

        [DebuggerHidden]
        private IEnumerator StopActiveVFX()
        {
            return new <StopActiveVFX>c__Iterator78 { <>f__this = this };
        }

        [CompilerGenerated]
        private sealed class <StopActiveVFX>c__Iterator78 : IEnumerator, IDisposable, IEnumerator<object>
        {
            internal object $current;
            internal int $PC;
            internal MonoTechNodeUI <>f__this;

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
                        if ((this.<>f__this._activationVFX != null) && (this.<>f__this._activationVFX.gameObject != null))
                        {
                            this.$current = new WaitForSeconds(0.1f);
                            this.$PC = 1;
                            goto Label_010A;
                        }
                        break;

                    case 1:
                        this.<>f__this._activationVFX.playOnAwake = false;
                        this.$current = new WaitForSeconds(1.9f);
                        this.$PC = 2;
                        goto Label_010A;

                    case 2:
                        if ((this.<>f__this._activationVFX != null) && (this.<>f__this._activationVFX.gameObject != null))
                        {
                            this.<>f__this._activationVFX.playOnAwake = true;
                            this.<>f__this._activationVFX.gameObject.SetActive(false);
                            this.$PC = -1;
                            break;
                        }
                        break;
                }
                return false;
            Label_010A:
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
    }
}

