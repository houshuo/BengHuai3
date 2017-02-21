namespace MoleMole
{
    using MoleMole.Config;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.Events;

    public class GalTouchSystem
    {
        private Animator _animator;
        private FaceAnimation _faceAnimation;
        private FaceEffect _faceEffect;
        private int _heartLevel;
        private bool _idle = true;
        private bool _init;
        private List<int> _itemIndexRecord;
        private List<TouchPatternItem> _touchPatternList;
        private const string CONFIG_FACE_ANIMATION_PATH = "FaceAnimation/";
        private const string PREFAB_FACE_EFFECT_PATH = "FaceEffect/";
        private const int REACTION_RECORD_COUNT = 20;

        public event UnityAction<bool> IdleChanged;

        public event UnityAction<int> TouchPatternTriggered;

        private bool ActReactionPattern(ReactionPattern pattern)
        {
            if (!this.idle)
            {
                return false;
            }
            if (!string.IsNullOrEmpty(pattern.bodyStateName) && (this._animator != null))
            {
                this._animator.CrossFadeInFixedTime(pattern.bodyStateName, 0.3f, 0);
            }
            if (!string.IsNullOrEmpty(pattern.faceStateName) && (this._faceAnimation != null))
            {
                this._faceAnimation.PlayFaceAnimation(pattern.faceStateName, FaceAnimationPlayMode.Normal);
            }
            if (!string.IsNullOrEmpty(pattern.faceEffectName) && (this._faceEffect != null))
            {
                this._faceEffect.ShowEffect(pattern.faceEffectName);
            }
            return true;
        }

        public bool BodyPartTouched(BodyPartType type)
        {
            if (this._touchPatternList == null)
            {
                return false;
            }
            if (!this.enable)
            {
                return false;
            }
            int index = -1;
            bool advance = false;
            ReactionPattern pattern = this.GetPatternByBodyPartTypeAndHeartLevel(type, this._heartLevel, out index, out advance);
            if (pattern == null)
            {
                return false;
            }
            bool flag2 = this.ActReactionPattern(pattern);
            if (flag2)
            {
                if (advance)
                {
                    this._itemIndexRecord.Clear();
                }
                else
                {
                    if (this._itemIndexRecord.Count >= 20)
                    {
                        this._itemIndexRecord.RemoveAt(0);
                    }
                    this._itemIndexRecord.Add(index);
                }
                this._idle = false;
                if (this.IdleChanged != null)
                {
                    this.IdleChanged(this._idle);
                }
                if (this.TouchPatternTriggered == null)
                {
                    return flag2;
                }
                int num2 = 0;
                if (type == BodyPartType.Face)
                {
                    num2 = 1;
                }
                else if (type == BodyPartType.Head)
                {
                    num2 = 2;
                }
                else if (type == BodyPartType.Chest)
                {
                    num2 = !advance ? 3 : 4;
                }
                else if (type == BodyPartType.Private)
                {
                    num2 = !advance ? 5 : 6;
                }
                else if (type == BodyPartType.Arm)
                {
                    num2 = 7;
                }
                else if (type == BodyPartType.Stomach)
                {
                    num2 = 8;
                }
                else if (type == BodyPartType.Leg)
                {
                    num2 = 9;
                }
                if (num2 != 0)
                {
                    this.TouchPatternTriggered(num2);
                }
            }
            return flag2;
        }

        private string CharacterName(int id)
        {
            switch ((id / 100))
            {
                case 1:
                    return "Kiana";

                case 2:
                    return "Mei";

                case 3:
                    return "Bronya";
            }
            return null;
        }

        private ReactionPattern GetPatternByBodyPartTypeAndHeartLevel(BodyPartType bodyPartType, int heartLevel, out int index, out bool advance)
        {
            ReactionPattern reactionPattern = null;
            index = -1;
            advance = false;
            int num = 0;
            int count = this._touchPatternList.Count;
            while (num < count)
            {
                TouchPatternItem item = this._touchPatternList[num];
                if ((item.bodyPartType == bodyPartType) && (item.heartLevel == heartLevel))
                {
                    reactionPattern = item.reactionPattern;
                    index = num;
                }
                if (reactionPattern != null)
                {
                    if (item.advanceTime > 0)
                    {
                        bool flag = true;
                        int num3 = 0;
                        int num4 = item.advanceTime - 1;
                        while (num3 < num4)
                        {
                            if ((num3 >= this._itemIndexRecord.Count) || (this._itemIndexRecord[(this._itemIndexRecord.Count - num3) - 1] != num))
                            {
                                flag = false;
                                break;
                            }
                            num3++;
                        }
                        if (flag)
                        {
                            reactionPattern = item.advanceReactionPattern;
                            advance = true;
                        }
                    }
                    return reactionPattern;
                }
                num++;
            }
            return reactionPattern;
        }

        public void Init(Animator animator, int characterId, int heartLevel, Renderer leftEyeRenderer, Renderer rightEyeRenderer, Renderer mouthRenderer, IFaceMatInfoProvider leftEyeProvider, IFaceMatInfoProvider rightEyeProvider, IFaceMatInfoProvider mouthProvider, Transform headRoot = null)
        {
            if (!this._init)
            {
                this._animator = animator;
                this._heartLevel = heartLevel;
                string characterName = this.CharacterName(characterId);
                this._touchPatternList = TouchPatternData.GetTouchPatternList(characterName);
                this._itemIndexRecord = new List<int>(20);
                this._faceAnimation = new FaceAnimation();
                ConfigFaceAnimation faceAnimation = FaceAnimationData.GetFaceAnimation(characterName);
                FacePartControl leftEye = new FacePartControl();
                leftEye.Init(leftEyeProvider, leftEyeRenderer);
                FacePartControl rightEye = new FacePartControl();
                rightEye.Init(rightEyeProvider, rightEyeRenderer);
                FacePartControl mouth = new FacePartControl();
                mouth.Init(mouthProvider, mouthRenderer);
                this._faceAnimation.Setup(faceAnimation, leftEye, rightEye, mouth);
                if (this._faceEffect != null)
                {
                    this._faceEffect.Uninit();
                    this._faceEffect = null;
                }
                if (headRoot != null)
                {
                    GameObject original = Resources.Load<GameObject>("FaceEffect/FFX_" + characterName);
                    if (original != null)
                    {
                        GameObject obj3 = UnityEngine.Object.Instantiate<GameObject>(original);
                        MonoFaceEffect component = obj3.GetComponent<MonoFaceEffect>();
                        if (component != null)
                        {
                            this._faceEffect = new FaceEffect();
                            obj3.transform.SetParent(headRoot, false);
                            this._faceEffect.Init(component);
                        }
                        else
                        {
                            UnityEngine.Object.Destroy(obj3);
                        }
                    }
                }
                this.enable = false;
                this._init = true;
            }
        }

        public void Process(float dt)
        {
            if (this._faceAnimation != null)
            {
                this._faceAnimation.Process(dt);
                if ((!this._idle && !this._faceAnimation.isPlaying) && (this._animator.GetCurrentAnimatorStateInfo(0).IsName("StandBy") && !this._animator.IsInTransition(0)))
                {
                    if (this._faceEffect != null)
                    {
                        this._faceEffect.HideAll();
                    }
                    this._idle = true;
                    if (this.IdleChanged != null)
                    {
                        this.IdleChanged(this._idle);
                    }
                }
            }
        }

        public void StopFaceAnimation()
        {
            if (this._faceAnimation != null)
            {
                this._faceAnimation.Stop();
            }
            if (this._faceEffect != null)
            {
                this._faceEffect.HideAll();
            }
        }

        public void StopVoice()
        {
            if ((this._animator != null) && (Singleton<WwiseAudioManager>.Instance != null))
            {
                Singleton<WwiseAudioManager>.Instance.StopAll(this._animator.gameObject);
            }
        }

        public bool enable { get; set; }

        public int heartLevel
        {
            get
            {
                return this._heartLevel;
            }
            set
            {
                this._heartLevel = value;
            }
        }

        public bool idle
        {
            get
            {
                if (!this._idle)
                {
                    if (this._animator == null)
                    {
                        return true;
                    }
                    this._idle = (this._animator.GetCurrentAnimatorStateInfo(0).IsName("StandBy") && !this._faceAnimation.isPlaying) && !this._animator.IsInTransition(0);
                    if (this.IdleChanged != null)
                    {
                        this.IdleChanged(this._idle);
                    }
                }
                return this._idle;
            }
        }
    }
}

