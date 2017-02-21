namespace MoleMole
{
    using MoleMole.Config;
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public static class GraphicsSettingUtil
    {
        private static bool _hasSettingScreenResolution;
        public static Resolution _originScreenResolution;
        public static bool _UsingNativeResolution = true;
        public static int _UsingResolutionX;
        public static int _UsingResolutionY;
        public static Action<bool> onPostFXChanged;

        public static void ApplyResolution(Dictionary<ResolutionQualityGrade, int> resolutionPercentage, ResolutionQualityGrade resolutionQuality, int recommendResX, int recommendResY)
        {
            ResolutionQualityGrade grade = resolutionQuality;
            int currentScreenResolutionWidth = 0;
            int currentScreenResolutionHeight = 0;
            SetScreenResolution(grade, ref currentScreenResolutionWidth, ref currentScreenResolutionHeight);
            int num3 = resolutionPercentage[grade];
            int resX = Mathf.RoundToInt(((float) (currentScreenResolutionWidth * num3)) / 100f);
            int resY = Mathf.RoundToInt(((float) (currentScreenResolutionHeight * num3)) / 100f);
            switch (grade)
            {
                case ResolutionQualityGrade.Middle:
                case ResolutionQualityGrade.Low:
                    GetResolutionKeepScale(recommendResX, recommendResY, currentScreenResolutionWidth, currentScreenResolutionHeight, ref resX, ref resY);
                    if (grade == ResolutionQualityGrade.Low)
                    {
                        resX = Mathf.RoundToInt(((float) (resX * num3)) / 100f);
                        resY = Mathf.RoundToInt(((float) (resY * num3)) / 100f);
                    }
                    break;
            }
            _UsingNativeResolution = (resX == currentScreenResolutionWidth) && (resY == currentScreenResolutionHeight);
            _UsingResolutionX = resX;
            _UsingResolutionY = resY;
            PostFXWithResScale scale = UnityEngine.Object.FindObjectOfType<PostFXWithResScale>();
            if ((scale != null) && scale.enabled)
            {
                scale.CameraResWidth = resX;
                scale.CameraResHeight = resY;
            }
            else
            {
                Screen.SetResolution(resX, resY, Screen.fullScreen);
            }
            GraphicsUtils.RebindAllRenderTexturesToCamera();
        }

        public static void EnableAvatarsDynamicBone(bool enabled)
        {
            GlobalVars.AVATAR_USE_DYNAMIC_BONE = enabled;
            if (Singleton<AvatarManager>.Instance != null)
            {
                foreach (BaseMonoAvatar avatar in Singleton<AvatarManager>.Instance.GetAllAvatars())
                {
                    foreach (DynamicBone bone in avatar.gameObject.GetComponentsInChildren<DynamicBone>(true))
                    {
                        bone.enabled = enabled;
                    }
                }
            }
        }

        public static void EnableColorGrading(bool enabled)
        {
            PostFX tfx = UnityEngine.Object.FindObjectOfType<PostFX>();
            if (tfx != null)
            {
                tfx.UseColorGrading = enabled;
            }
        }

        public static void EnableDistortion(bool enabled)
        {
            PostFX tfx = UnityEngine.Object.FindObjectOfType<PostFX>();
            if (tfx != null)
            {
                tfx.UseDistortion = enabled;
            }
        }

        public static void EnableDynamicBone(bool enabled)
        {
            EnableAvatarsDynamicBone(enabled);
            EnableMonstersDynamicBone(enabled);
        }

        public static void EnableFXAA(bool enabled)
        {
            PostFX tfx = UnityEngine.Object.FindObjectOfType<PostFX>();
            if (tfx != null)
            {
                tfx.FXAA = enabled;
            }
        }

        public static void EnableHDR(bool enabled)
        {
            PostFX tfx = UnityEngine.Object.FindObjectOfType<PostFX>();
            if (tfx != null)
            {
                tfx.HDRBuffer = enabled;
            }
        }

        public static void EnableMonstersDynamicBone(bool enabled)
        {
            GlobalVars.MONSTER_USE_DYNAMIC_BONE = enabled;
            if (Singleton<MonsterManager>.Instance != null)
            {
                foreach (BaseMonoMonster monster in Singleton<MonsterManager>.Instance.GetAllMonsters())
                {
                    foreach (DynamicBone bone in monster.gameObject.GetComponentsInChildren<DynamicBone>(true))
                    {
                        bone.enabled = enabled;
                    }
                }
            }
        }

        public static void EnablePostFX(bool enabled, bool forceWhenDisable = false)
        {
            PostFX tfx = UnityEngine.Object.FindObjectOfType<PostFX>();
            if (tfx != null)
            {
                if (!enabled && forceWhenDisable)
                {
                    tfx.enabled = false;
                }
                else
                {
                    tfx.enabled = true;
                    tfx.OnlyResScale = !enabled;
                }
                tfx.FastMode = !enabled;
                tfx.originalEnabled = tfx.enabled;
                if (onPostFXChanged != null)
                {
                    onPostFXChanged(tfx.enabled);
                }
            }
        }

        public static void EnableReflection(bool enabled)
        {
            GlobalVars.USE_REFLECTION = enabled;
            foreach (ReflectionBase base2 in UnityEngine.Object.FindObjectsOfType<ReflectionBase>())
            {
                base2.SetFastMode(!enabled);
            }
        }

        public static void EnableStaticCloudMode(bool enabled)
        {
            GlobalVars.STATIC_CLOUD_MODE = enabled;
        }

        public static void EnableUIAvatarsDynamicBone(bool enabled)
        {
            GlobalVars.UI_AVATAR_USE_DYNAMIC_BONE = enabled;
            BaseMonoCanvas sceneCanvas = Singleton<MainUIManager>.Instance.SceneCanvas;
            if ((sceneCanvas != null) && (((sceneCanvas is MonoMainCanvas) || (sceneCanvas is MonoTestUI)) || (sceneCanvas is MonoGameEntry)))
            {
                Avatar3dModelContext context;
                if (sceneCanvas is MonoMainCanvas)
                {
                    context = ((MonoMainCanvas) sceneCanvas).avatar3dModelContext;
                }
                else if (sceneCanvas is MonoTestUI)
                {
                    context = ((MonoTestUI) sceneCanvas).avatar3dModelContext;
                }
                else
                {
                    context = ((MonoGameEntry) sceneCanvas).avatar3dModelContext;
                }
                if (context != null)
                {
                    foreach (Transform transform in context.GetAllAvatars())
                    {
                        foreach (DynamicBone bone in transform.gameObject.GetComponentsInChildren<DynamicBone>(true))
                        {
                            bone.enabled = enabled;
                        }
                    }
                }
            }
        }

        private static void GetResolutionKeepScale(int recommendResX, int recommendResY, int currentScreenResolutionWidth, int currentScreenResolutionHeight, ref int resX, ref int resY)
        {
            if ((currentScreenResolutionWidth <= recommendResX) || (currentScreenResolutionHeight <= recommendResY))
            {
                resX = currentScreenResolutionWidth;
                resY = currentScreenResolutionHeight;
            }
            else
            {
                float num = ((float) recommendResX) / ((float) currentScreenResolutionWidth);
                float num2 = ((float) recommendResY) / ((float) currentScreenResolutionHeight);
                float num3 = ((float) currentScreenResolutionWidth) / ((float) currentScreenResolutionHeight);
                if (num2 <= num)
                {
                    resX = Mathf.RoundToInt(recommendResY * num3);
                    resY = recommendResY;
                }
                else
                {
                    resX = recommendResX;
                    resY = Mathf.RoundToInt(((float) recommendResX) / num3);
                }
            }
        }

        public static void SetPostEffectBufferSizeByQuality(Dictionary<PostEffectQualityGrade, int> postFxGradeBufferSize, PostEffectQualityGrade quality)
        {
            PostFX tfx = UnityEngine.Object.FindObjectOfType<PostFX>();
            if (tfx != null)
            {
                int num = 0;
                postFxGradeBufferSize.TryGetValue(quality, out num);
                tfx.internalBufferSize = (PostFXBase.InternalBufferSizeEnum) num;
            }
        }

        public static void SetPostFXContrast(float contrastDelta)
        {
            PostFX tfx = UnityEngine.Object.FindObjectOfType<PostFX>();
            if (tfx != null)
            {
                if (Singleton<LevelManager>.Instance != null)
                {
                    tfx.constrast = 2f + contrastDelta;
                }
                else
                {
                    tfx.constrast = 2.1f + contrastDelta;
                }
            }
        }

        private static void SetScreenResolution(ResolutionQualityGrade resolutionQuality, ref int currentScreenResolutionWidth, ref int currentScreenResolutionHeight)
        {
            if (!_hasSettingScreenResolution)
            {
                _originScreenResolution = Screen.currentResolution;
                _hasSettingScreenResolution = true;
            }
            currentScreenResolutionWidth = _originScreenResolution.width;
            currentScreenResolutionHeight = _originScreenResolution.height;
            if (resolutionQuality != ResolutionQualityGrade.High)
            {
                int resX = 0;
                int resY = 0;
                GetResolutionKeepScale(0x500, 720, _originScreenResolution.width, _originScreenResolution.height, ref resX, ref resY);
                Screen.SetResolution(resX, resY, Screen.fullScreen);
                currentScreenResolutionWidth = resX;
                currentScreenResolutionHeight = resY;
            }
            else
            {
                Screen.SetResolution(_originScreenResolution.width, _originScreenResolution.height, Screen.fullScreen);
                currentScreenResolutionWidth = _originScreenResolution.width;
                currentScreenResolutionHeight = _originScreenResolution.height;
            }
        }

        public static void SetTargetFrameRate(int targetFrameRate)
        {
            Application.targetFrameRate = targetFrameRate;
        }
    }
}

