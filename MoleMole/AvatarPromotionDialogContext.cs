namespace MoleMole
{
    using MoleMole.Config;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using UnityEngine;
    using UnityEngine.Events;

    public class AvatarPromotionDialogContext : BaseDialogContext
    {
        private Animator _animator;
        private bool _ignoreButton = true;
        private ParticleSystem _starVFX;
        private AvatarDataItem avatarData;

        public AvatarPromotionDialogContext(AvatarDataItem avatarData)
        {
            ContextPattern pattern = new ContextPattern {
                contextName = "AvatarPromotionDialogContext",
                viewPrefabPath = "UI/Menus/Dialog/AvatarPromotionDialog"
            };
            base.config = pattern;
            this.avatarData = avatarData;
        }

        protected override void BindViewCallbacks()
        {
            base.BindViewCallback(base.view.transform.Find("Dialog/Content/SingleButton/Btn").GetComponent<Button>(), new UnityAction(this.Resume));
        }

        public void Close()
        {
            this.Destroy();
        }

        [DebuggerHidden]
        private IEnumerator DelayDestroy(float delay)
        {
            return new <DelayDestroy>c__Iterator56 { delay = delay, <$>delay = delay, <>f__this = this };
        }

        private bool OnAnimCallBack(string param)
        {
            if (param == "PromotionDialogPause")
            {
                this._animator.speed = 0f;
                this._ignoreButton = false;
            }
            else if (param == "StarVFX")
            {
                base.view.transform.Find("Dialog/Content/Star/Image").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(MiscData.Config.AvatarStarIcons[this.avatarData.star]);
                this._starVFX.Play();
            }
            return false;
        }

        public override bool OnNotify(Notify ntf)
        {
            return ((ntf.type == NotifyTypes.AnimCallBack) && this.OnAnimCallBack((string) ntf.body));
        }

        private void Resume()
        {
            if (!this._ignoreButton)
            {
                this._animator.speed = 1f;
                Singleton<ApplicationManager>.Instance.StartCoroutine(this.DelayDestroy(1f));
                this._ignoreButton = true;
            }
        }

        private void SetupStar()
        {
            this._starVFX = base.view.transform.Find("Dialog/Content/Star/StarShining").GetComponent<ParticleSystem>();
            base.view.transform.Find("Dialog/Content/Star/Image").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(MiscData.Config.AvatarStarIcons[this.avatarData.star - 1]);
        }

        protected override bool SetupView()
        {
            this._ignoreButton = true;
            this._animator = base.view.GetComponent<Animator>();
            this._animator.SetTrigger("Play");
            this._animator.speed = 1f;
            AvatarStarMetaData avatarStarMetaDataByKey = AvatarStarMetaDataReader.GetAvatarStarMetaDataByKey(this.avatarData.avatarID, this.avatarData.star - 1);
            AvatarStarMetaData data2 = AvatarStarMetaDataReader.GetAvatarStarMetaDataByKey(this.avatarData.avatarID, this.avatarData.star);
            base.view.transform.Find("Dialog/Content/HP/RatioBeforeNumText").GetComponent<Text>().text = string.Format("{0:N2}", avatarStarMetaDataByKey.hpAdd);
            base.view.transform.Find("Dialog/Content/HP/RatioAfterNumText").GetComponent<Text>().text = string.Format("{0:N2}", data2.hpAdd);
            base.view.transform.Find("Dialog/Content/HP/AddNumText").GetComponent<Text>().text = string.Format("+{0:N2}", (data2.hpBase - avatarStarMetaDataByKey.hpBase) + ((data2.hpAdd - avatarStarMetaDataByKey.hpAdd) * this.avatarData.level));
            base.view.transform.Find("Dialog/Content/SP/RatioBeforeNumText").GetComponent<Text>().text = string.Format("{0:N2}", avatarStarMetaDataByKey.spAdd);
            base.view.transform.Find("Dialog/Content/SP/RatioAfterNumText").GetComponent<Text>().text = string.Format("{0:N2}", data2.spAdd);
            base.view.transform.Find("Dialog/Content/SP/AddNumText").GetComponent<Text>().text = string.Format("+{0:N2}", (data2.spBase - avatarStarMetaDataByKey.spBase) + ((data2.spAdd - avatarStarMetaDataByKey.spAdd) * this.avatarData.level));
            base.view.transform.Find("Dialog/Content/ATK/RatioBeforeNumText").GetComponent<Text>().text = string.Format("{0:N2}", avatarStarMetaDataByKey.atkAdd);
            base.view.transform.Find("Dialog/Content/ATK/RatioAfterNumText").GetComponent<Text>().text = string.Format("{0:N2}", data2.atkAdd);
            base.view.transform.Find("Dialog/Content/ATK/AddNumText").GetComponent<Text>().text = string.Format("+{0:N2}", (data2.atkBase - avatarStarMetaDataByKey.atkBase) + ((data2.atkAdd - avatarStarMetaDataByKey.atkAdd) * this.avatarData.level));
            base.view.transform.Find("Dialog/Content/DEF/RatioBeforeNumText").GetComponent<Text>().text = string.Format("{0:N2}", avatarStarMetaDataByKey.dfsAdd);
            base.view.transform.Find("Dialog/Content/DEF/RatioAfterNumText").GetComponent<Text>().text = string.Format("{0:N2}", data2.dfsAdd);
            base.view.transform.Find("Dialog/Content/DEF/AddNumText").GetComponent<Text>().text = string.Format("+{0:N2}", (data2.dfsBase - avatarStarMetaDataByKey.dfsBase) + ((data2.dfsAdd - avatarStarMetaDataByKey.dfsAdd) * this.avatarData.level));
            base.view.transform.Find("Dialog/Content/CRT/RatioBeforeNumText").GetComponent<Text>().text = string.Format("{0:N2}", avatarStarMetaDataByKey.crtAdd);
            base.view.transform.Find("Dialog/Content/CRT/RatioAfterNumText").GetComponent<Text>().text = string.Format("{0:N2}", data2.crtAdd);
            base.view.transform.Find("Dialog/Content/CRT/AddNumText").GetComponent<Text>().text = string.Format("+{0:N2}", (data2.crtBase - avatarStarMetaDataByKey.crtBase) + ((data2.crtAdd - avatarStarMetaDataByKey.crtAdd) * this.avatarData.level));
            int cost = this.avatarData.GetCost(this.avatarData.star - 1);
            int maxCost = this.avatarData.MaxCost;
            base.view.transform.Find("Dialog/Content/COST/RatioBeforeNumText").GetComponent<Text>().text = cost.ToString();
            base.view.transform.Find("Dialog/Content/COST/RatioAfterNumText").GetComponent<Text>().text = maxCost.ToString();
            base.view.transform.Find("Dialog/Content/COST/AddNumText").GetComponent<Text>().text = string.Format("+{0}", maxCost - cost);
            this.SetupStar();
            return false;
        }

        [CompilerGenerated]
        private sealed class <DelayDestroy>c__Iterator56 : IEnumerator, IDisposable, IEnumerator<object>
        {
            internal object $current;
            internal int $PC;
            internal float <$>delay;
            internal AvatarPromotionDialogContext <>f__this;
            internal float delay;

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
                        this.$current = new WaitForSeconds(this.delay);
                        this.$PC = 1;
                        return true;

                    case 1:
                        this.<>f__this.Close();
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

