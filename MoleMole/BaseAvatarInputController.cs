namespace MoleMole
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    public abstract class BaseAvatarInputController : BaseAvatarController
    {
        protected uint _controlType;
        [CompilerGenerated]
        private static Dictionary<string, int> <>f__switch$map2;
        [CompilerGenerated]
        private static Dictionary<string, int> <>f__switch$map3;

        public BaseAvatarInputController(uint controllerType, BaseMonoEntity avatar) : base(avatar)
        {
            this._controlType = 1;
        }

        public override void Core()
        {
        }

        private void InitInputStick()
        {
        }

        public override void SetActive(bool isActive)
        {
            base.active = isActive;
        }

        public void TryHold(string skillName)
        {
            string key = skillName;
            if (key != null)
            {
                int num;
                if (<>f__switch$map2 == null)
                {
                    Dictionary<string, int> dictionary = new Dictionary<string, int>(1);
                    dictionary.Add("ATK", 0);
                    <>f__switch$map2 = dictionary;
                }
                if (<>f__switch$map2.TryGetValue(key, out num) && (num == 0))
                {
                    base.TryHoldAttack();
                }
            }
        }

        public void TryMove(bool isMoving, float angle)
        {
            Vector3 forward;
            MonoMainCamera mainCamera = Singleton<CameraManager>.Instance.GetMainCamera();
            Vector3 xZPosition = base.avatar.XZPosition;
            if (mainCamera.followState.followAvatarAndBossState.active)
            {
                forward = mainCamera.transform.forward;
            }
            else if (mainCamera.followState.followAvatarAndCrowdState.active)
            {
                forward = mainCamera.transform.forward;
            }
            else
            {
                Vector3 vector3 = mainCamera.XZPosition;
                forward = xZPosition - vector3;
            }
            forward.y = 0f;
            forward.Normalize();
            Vector3 dir = (Vector3) (Quaternion.AngleAxis(-angle, Vector3.up) * forward);
            dir.Normalize();
            base.TryOrderMove(isMoving);
            if (isMoving)
            {
                base.TrySteer(dir, base.avatar.config.StateMachinePattern.ChangeDirLerpRatioForMove);
            }
        }

        public void TryUseSkill(string skillName)
        {
            string key = skillName;
            if (key != null)
            {
                int num;
                if (<>f__switch$map3 == null)
                {
                    Dictionary<string, int> dictionary = new Dictionary<string, int>(4);
                    dictionary.Add("ATK", 0);
                    dictionary.Add("SKL01", 1);
                    dictionary.Add("SKL02", 2);
                    dictionary.Add("SKL_WEAPON", 3);
                    <>f__switch$map3 = dictionary;
                }
                if (<>f__switch$map3.TryGetValue(key, out num))
                {
                    switch (num)
                    {
                        case 0:
                            base.TryAttack();
                            return;

                        case 1:
                            base.TryUseSkill(1);
                            return;

                        case 2:
                            base.TryUseSkill(2);
                            return;

                        case 3:
                            base.TryUseSkill(3);
                            return;
                    }
                }
            }
            throw new Exception("Invalid Type or State!");
        }
    }
}

