namespace MoleMole
{
    using MoleMole.Config;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.UI;

    public class MonoLevelDropIconButton : MonoBehaviour
    {
        private const string _bg_path_jixie = "SpriteOutput/AvatarIcon/AttrJiXie1";
        private const string _bg_path_shengwu = "SpriteOutput/AvatarIcon/AttrShengWu1";
        private const string _bg_path_yineng = "SpriteOutput/AvatarIcon/AttrYiNeng1";
        private DropItemButtonClickCallBack _callBack;
        private StorageDataItemBase _dropItem;
        private bool _hasSetBGDesc;
        private bool _hideBg;
        private bool _isGrey;
        private const string _itemIconCommonBGSpritePath = "SpriteOutput/SpecialIcons/ItemCommonBG";
        private bool _originBGDescEnabled;
        public string effectAudioPattern;
        public float height;
        public bool showNewIcon;
        public float width;

        public bool CanSplit()
        {
            AvatarCardDataItem item = this._dropItem as AvatarCardDataItem;
            return ((item != null) && item.IsSplite());
        }

        public void Clear()
        {
            this._dropItem = null;
            this._callBack = null;
            Transform transform = base.transform.Find("BG/UnidentifyText");
            if (transform != null)
            {
                transform.gameObject.SetActive(false);
            }
            if (this._hasSetBGDesc)
            {
                base.transform.Find("BG/Desc").gameObject.SetActive(this._originBGDescEnabled);
            }
            this.SetItemDefaultMaterialAndColor();
        }

        [DebuggerHidden]
        private IEnumerator Coroutine_AvatarEffect(float delay, bool isRareDrop)
        {
            return new <Coroutine_AvatarEffect>c__Iterator73 { delay = delay, isRareDrop = isRareDrop, <$>delay = delay, <$>isRareDrop = isRareDrop, <>f__this = this };
        }

        private EntityNature GetAvatarAttribte()
        {
            return (EntityNature) Singleton<AvatarModule>.Instance.GetAvatarByID(AvatarMetaDataReaderExtend.GetAvatarIDsByKey(this._dropItem.ID).avatarID).Attribute;
        }

        private Sprite GetBGSprite()
        {
            switch (this.GetAvatarAttribte())
            {
                case EntityNature.Mechanic:
                    return Miscs.GetSpriteByPrefab("SpriteOutput/AvatarIcon/AttrJiXie1");

                case EntityNature.Biology:
                    return Miscs.GetSpriteByPrefab("SpriteOutput/AvatarIcon/AttrShengWu1");

                case EntityNature.Psycho:
                    return Miscs.GetSpriteByPrefab("SpriteOutput/AvatarIcon/AttrYiNeng1");
            }
            return null;
        }

        public int GetDropItemID()
        {
            return this._dropItem.ID;
        }

        public bool IsAvatarCard()
        {
            AvatarCardDataItem item = this._dropItem as AvatarCardDataItem;
            return (item != null);
        }

        public void OnClick()
        {
            if (this._callBack != null)
            {
                this._callBack(this._dropItem);
            }
        }

        private void OnDestroy()
        {
        }

        public void PlayVFX(float delay, bool isRareDrop)
        {
            base.StartCoroutine(this.Coroutine_AvatarEffect(delay, isRareDrop));
        }

        private void SetItemDefaultMaterialAndColor()
        {
            Image component = base.transform.Find("BG/Unselected").GetComponent<Image>();
            component.material = null;
            component.color = MiscData.GetColor("DropItemImageDefaultColor");
            component = base.transform.Find("BG/Image").GetComponent<Image>();
            component.material = null;
            component.color = MiscData.GetColor("DropItemImageDefaultColor");
            component = base.transform.Find("ItemIcon/ItemIcon").GetComponent<Image>();
            component.material = null;
            component.color = MiscData.GetColor("DropItemIconDefaultColor");
            component = base.transform.Find("ItemIcon/ItemIcon/Icon").GetComponent<Image>();
            component.material = null;
            component.color = MiscData.GetColor("DropItemImageDefaultColor");
            for (int i = 1; i < 6; i++)
            {
                Transform transform = base.transform.Find("Star/" + i.ToString());
                if (transform != null)
                {
                    component = transform.GetComponent<Image>();
                    component.material = null;
                    component.color = MiscData.GetColor("DropItemImageDefaultColor");
                }
            }
            for (int j = 1; j <= 6; j++)
            {
                Transform transform2 = base.transform.Find("AvatarStar/" + j.ToString());
                if (transform2 != null)
                {
                    component = transform2.GetComponent<Image>();
                    component.material = null;
                    component.color = MiscData.GetColor("DropItemImageDefaultColor");
                }
            }
            component = base.transform.Find("StigmataType/Image").GetComponent<Image>();
            component.material = null;
            component.color = MiscData.GetColor("DropItemImageDefaultColor");
            component = base.transform.Find("FragmentIcon").GetComponent<Image>();
            component.material = null;
            component.color = MiscData.GetColor("DropItemImageDefaultColor");
            Transform transform3 = base.transform.Find("BG/UnidentifyText");
            if (transform3 != null)
            {
                transform3.GetComponent<Text>().color = MiscData.GetColor("DropItemUnidentifyDefaultColor");
            }
        }

        private void SetItemGrey()
        {
            base.transform.Find("BG/Unselected").GetComponent<Image>().color = MiscData.GetColor("DropItemImageGrey");
            base.transform.Find("BG/Image").GetComponent<Image>().color = MiscData.GetColor("DropItemImageGrey");
            base.transform.Find("ItemIcon/ItemIcon").GetComponent<Image>().color = MiscData.GetColor("DropItemIconGrey");
            Image component = base.transform.Find("ItemIcon/ItemIcon/Icon").GetComponent<Image>();
            if (component.material != component.defaultMaterial)
            {
                component.color = MiscData.GetColor("DropItemIconFullGrey");
            }
            else
            {
                component.color = MiscData.GetColor("DropItemIconGrey");
            }
            for (int i = 1; i < 6; i++)
            {
                Transform transform = base.transform.Find("Star/" + i.ToString());
                if (transform != null)
                {
                    transform.GetComponent<Image>().color = MiscData.GetColor("DropItemImageGrey");
                }
            }
            for (int j = 1; j < 6; j++)
            {
                Transform transform2 = base.transform.Find("AvatarStar/" + j.ToString());
                if (transform2 != null)
                {
                    transform2.GetComponent<Image>().color = MiscData.GetColor("DropItemImageGrey");
                }
            }
            base.transform.Find("StigmataType/Image").GetComponent<Image>().color = MiscData.GetColor("DropItemImageGrey");
            base.transform.Find("FragmentIcon").GetComponent<Image>().color = MiscData.GetColor("DropItemImageGrey");
            Transform transform3 = base.transform.Find("BG/UnidentifyText");
            if (transform3 != null)
            {
                transform3.GetComponent<Text>().color = MiscData.GetColor("DropItemUnidentifyBlack");
            }
        }

        private void SetupAvatarStar(int starNum)
        {
            for (int i = 1; i < 6; i++)
            {
                string name = string.Format("AvatarStar/{0}", i);
                base.transform.Find(name).gameObject.SetActive(i == starNum);
            }
        }

        private void SetupColor()
        {
            Color white = Color.white;
            string htmlString = MiscData.Config.ItemRarityColorList[this._dropItem.rarity];
            if (ColorUtility.TryParseHtmlString(htmlString, out white))
            {
                base.transform.Find("ItemIcon/ItemIcon").GetComponent<Image>().color = white;
            }
        }

        public void SetupFinishStageFastDropView()
        {
            base.transform.Find("BG/Unselected").GetComponent<Image>().color = Color.yellow;
        }

        public void SetupFinishStageNomalDropView()
        {
            base.transform.Find("BG/Unselected").GetComponent<Image>().color = Color.blue;
        }

        public void SetupFinishStageVeryFastDropView()
        {
            base.transform.Find("BG/Unselected").GetComponent<Image>().color = Color.red;
        }

        private void SetupFrame()
        {
            if (this._dropItem is AvatarCardDataItem)
            {
                Image component = base.transform.Find("ItemIcon/ItemIcon").GetComponent<Image>();
                component.sprite = this.GetBGSprite();
                component.color = Color.white;
            }
            else
            {
                base.transform.Find("ItemIcon/ItemIcon").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab("SpriteOutput/SpecialIcons/ItemCommonBG");
            }
        }

        private void SetupRarityView(bool showDesc)
        {
            base.transform.Find("AvatarStar").gameObject.SetActive(false);
            base.transform.Find("Star").gameObject.SetActive(false);
            if (this._dropItem is AvatarCardDataItem)
            {
                AvatarDataItem dummyAvatarDataItem = Singleton<AvatarModule>.Instance.GetDummyAvatarDataItem(AvatarMetaDataReaderExtend.GetAvatarIDsByKey(this._dropItem.ID).avatarID);
                this.SetupAvatarStar(dummyAvatarDataItem.star);
                base.transform.Find("AvatarStar").gameObject.SetActive(showDesc && (this._dropItem is AvatarCardDataItem));
            }
            else if (!(this._dropItem is AvatarFragmentDataItem))
            {
                base.transform.Find("Star").gameObject.SetActive(true);
                int rarity = this._dropItem.rarity;
                if (this._dropItem is WeaponDataItem)
                {
                    rarity = (this._dropItem as WeaponDataItem).GetMaxRarity();
                }
                else if (this._dropItem is StigmataDataItem)
                {
                    rarity = (this._dropItem as StigmataDataItem).GetMaxRarity();
                }
                base.transform.Find("Star").GetComponent<MonoItemIconStar>().SetupView(this._dropItem.rarity, rarity);
            }
        }

        private void SetupStigmataAffixView(bool isIdentify)
        {
            if (!this._hideBg)
            {
                if (base.transform.Find("BG/UnidentifyText") != null)
                {
                    base.transform.Find("BG/UnidentifyText").gameObject.SetActive(!isIdentify);
                }
                base.transform.Find("BG/Desc").gameObject.SetActive(isIdentify);
            }
            Image component = base.transform.Find("ItemIcon/ItemIcon/Icon").GetComponent<Image>();
            if (isIdentify)
            {
                component.material = null;
                component.color = Color.white;
            }
            else
            {
                Material material = Miscs.LoadResource<Material>("Material/ImageMonoColor", BundleType.RESOURCE_FILE);
                component.material = material;
                component.color = MiscData.GetColor("DarkBlue");
            }
            if (base.transform.Find("QuestionMark") != null)
            {
                base.transform.Find("QuestionMark").gameObject.SetActive(!isIdentify);
            }
        }

        private void SetupStigmataTypeIcon()
        {
            base.transform.Find("StigmataType").gameObject.SetActive(this._dropItem is StigmataDataItem);
            if (this._dropItem is StigmataDataItem)
            {
                base.transform.Find("StigmataType/Image").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(MiscData.Config.StigmataTypeIconPath[this._dropItem.GetBaseType()]);
            }
        }

        public void SetupView(StorageDataItemBase itemData, DropItemButtonClickCallBack callBack = null, bool showDesc = false, bool showNewIcon = false, bool isGrey = false, bool hideBg = false)
        {
            if (!this._hasSetBGDesc)
            {
                this._originBGDescEnabled = base.transform.Find("BG/Desc").gameObject.activeSelf;
                this._hasSetBGDesc = true;
            }
            this.Clear();
            this._dropItem = itemData;
            this._callBack = callBack;
            this.showNewIcon = showNewIcon;
            this._hideBg = hideBg;
            this._isGrey = isGrey;
            this.SetupFrame();
            base.transform.Find("FragmentIcon").gameObject.SetActive(itemData is AvatarFragmentDataItem);
            Sprite spriteByPrefab = Miscs.GetSpriteByPrefab(this._dropItem.GetIconPath());
            base.transform.Find("ItemIcon/ItemIcon").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(MiscData.Config.ItemRarityBGImgPath[itemData.rarity]);
            base.transform.Find("ItemIcon/ItemIcon").GetComponent<Image>().color = Color.white;
            base.transform.Find("ItemIcon/ItemIcon/Icon").GetComponent<Image>().sprite = spriteByPrefab;
            this.SetupRarityView(showDesc);
            this.SetupStigmataTypeIcon();
            base.transform.Find("BG").gameObject.SetActive(showDesc);
            base.transform.Find("BG/Image").gameObject.SetActive(showDesc);
            string str = "\x00d7" + this._dropItem.number;
            if ((this._dropItem is WeaponDataItem) || (this._dropItem is StigmataDataItem))
            {
                str = "Lv." + this._dropItem.level;
                base.transform.Find("BG/Desc").GetComponent<Text>().text = str;
            }
            else if (this._dropItem is AvatarCardDataItem)
            {
                base.transform.Find("BG/Desc").GetComponent<Text>().text = string.Empty;
            }
            else
            {
                base.transform.Find("BG/Desc").GetComponent<Text>().text = str;
            }
            Transform transform = base.transform.Find("BG/ToFragment");
            if (transform != null)
            {
                transform.gameObject.SetActive(false);
            }
            bool flag = Singleton<StorageModule>.Instance.IsItemNew(this._dropItem.ID);
            if (this._dropItem is AvatarCardDataItem)
            {
                int avatarID = AvatarMetaDataReaderExtend.GetAvatarIDsByKey(this._dropItem.ID).avatarID;
                flag = !Singleton<AvatarModule>.Instance.GetAvatarByID(avatarID).UnLocked;
            }
            base.transform.Find("NewMark").gameObject.SetActive(showNewIcon && flag);
            if (showNewIcon && flag)
            {
                Singleton<StorageModule>.Instance.RecordNewItem(this._dropItem.ID);
            }
            if (this._dropItem is StigmataDataItem)
            {
                this.SetupStigmataAffixView((this._dropItem as StigmataDataItem).IsAffixIdentify);
            }
            if (hideBg)
            {
                base.transform.Find("BG").gameObject.SetActive(false);
            }
            if (this._isGrey)
            {
                this.SetItemGrey();
            }
        }

        public void StopRareEffect()
        {
            base.transform.Find("Particle1").gameObject.SetActive(false);
            base.transform.Find("Particle2").gameObject.SetActive(false);
            foreach (ParticleSystem system in base.transform.Find("Particle1").GetComponentsInChildren<ParticleSystem>())
            {
                system.Stop();
            }
            foreach (ParticleSystem system2 in base.transform.Find("Particle2").GetComponentsInChildren<ParticleSystem>())
            {
                system2.Stop();
            }
        }

        [CompilerGenerated]
        private sealed class <Coroutine_AvatarEffect>c__Iterator73 : IEnumerator, IDisposable, IEnumerator<object>
        {
            internal object $current;
            internal int $PC;
            internal float <$>delay;
            internal bool <$>isRareDrop;
            internal ParticleSystem[] <$s_1914>__4;
            internal int <$s_1915>__5;
            internal ParticleSystem[] <$s_1916>__7;
            internal int <$s_1917>__8;
            internal MonoLevelDropIconButton <>f__this;
            internal AvatarCardDataItem <avatarCard>__0;
            internal AvatarFragmentDataItem <fragment>__3;
            internal int <fragmentId>__1;
            internal AvatarFragmentMetaData <metaData>__2;
            internal ParticleSystem <ps>__6;
            internal ParticleSystem <ps>__9;
            internal float delay;
            internal bool isRareDrop;

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
                        goto Label_0324;

                    case 1:
                        if (!this.<>f__this.CanSplit())
                        {
                            break;
                        }
                        this.<>f__this.transform.Find("BecomeFragmentEffect").gameObject.GetComponent<ParticleSystem>().Play();
                        if (!string.IsNullOrEmpty(this.<>f__this.effectAudioPattern))
                        {
                            Singleton<WwiseAudioManager>.Instance.Post(this.<>f__this.effectAudioPattern, null, null, null);
                        }
                        this.$current = new WaitForSeconds(0.2f);
                        this.$PC = 2;
                        goto Label_0324;

                    case 2:
                        this.<avatarCard>__0 = this.<>f__this._dropItem as AvatarCardDataItem;
                        this.<fragmentId>__1 = AvatarMetaDataReaderExtend.GetAvatarIDsByKey(this.<>f__this._dropItem.ID).avatarFragmentID;
                        this.<metaData>__2 = AvatarFragmentMetaDataReader.GetAvatarFragmentMetaDataByKey(this.<fragmentId>__1);
                        this.<fragment>__3 = new AvatarFragmentDataItem(this.<metaData>__2);
                        this.<fragment>__3.number = this.<avatarCard>__0.GetSpliteFragmentNum();
                        this.<>f__this.SetupView(this.<fragment>__3, this.<>f__this._callBack, true, true, false, false);
                        this.<>f__this.transform.Find("AvatarStar").gameObject.SetActive(false);
                        this.<>f__this.transform.Find("BG/ToFragment").gameObject.SetActive(true);
                        break;

                    case 3:
                        this.<>f__this.transform.Find("Particle2").gameObject.SetActive(false);
                        this.$PC = -1;
                        goto Label_0322;

                    default:
                        goto Label_0322;
                }
                if (this.isRareDrop)
                {
                    this.<>f__this.transform.Find("Particle1").gameObject.SetActive(true);
                    this.<>f__this.transform.Find("Particle2").gameObject.SetActive(true);
                    this.<$s_1914>__4 = this.<>f__this.transform.Find("Particle1").GetComponentsInChildren<ParticleSystem>();
                    this.<$s_1915>__5 = 0;
                    while (this.<$s_1915>__5 < this.<$s_1914>__4.Length)
                    {
                        this.<ps>__6 = this.<$s_1914>__4[this.<$s_1915>__5];
                        this.<ps>__6.Play();
                        this.<$s_1915>__5++;
                    }
                    this.<$s_1916>__7 = this.<>f__this.transform.Find("Particle2").GetComponentsInChildren<ParticleSystem>();
                    this.<$s_1917>__8 = 0;
                    while (this.<$s_1917>__8 < this.<$s_1916>__7.Length)
                    {
                        this.<ps>__9 = this.<$s_1916>__7[this.<$s_1917>__8];
                        this.<ps>__9.Play();
                        this.<$s_1917>__8++;
                    }
                    if (!string.IsNullOrEmpty(this.<>f__this.effectAudioPattern))
                    {
                        Singleton<WwiseAudioManager>.Instance.Post(this.<>f__this.effectAudioPattern, null, null, null);
                    }
                }
                this.$current = new WaitForSeconds(1.5f);
                this.$PC = 3;
                goto Label_0324;
            Label_0322:
                return false;
            Label_0324:
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

