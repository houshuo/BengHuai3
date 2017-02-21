namespace MoleMole
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    public abstract class BaseAvatarAIController : BaseAvatarController, IAIController
    {
        [CompilerGenerated]
        private static Dictionary<string, int> <>f__switch$map1;
        private const float AI_CHANGE_DIR_LERP_RATIO = 40f;

        public BaseAvatarAIController(BaseMonoEntity avatar) : base(avatar)
        {
        }

        void IAIController.TryClearAttackTarget()
        {
            base.TryClearAttackTarget();
        }

        void IAIController.TrySetAttackTarget(BaseMonoEntity attackTarget)
        {
            base.TrySetAttackTarget(attackTarget);
        }

        public void TryMove(float speed)
        {
            base.TryOrderMove(true);
        }

        public void TryMoveHorizontal(float speed)
        {
        }

        public void TrySteer(Vector3 dir)
        {
            base.TrySteer(dir, 40f);
        }

        public void TrySteer(Vector3 dir, float lerpRatio)
        {
            base.TrySteer(dir, 40f);
        }

        public void TrySteerInstant(Vector3 dir)
        {
        }

        public void TryStop()
        {
            base.TryOrderMove(false);
        }

        public bool TryUseSkill(string skillName)
        {
            string key = skillName;
            if (key != null)
            {
                int num;
                if (<>f__switch$map1 == null)
                {
                    Dictionary<string, int> dictionary = new Dictionary<string, int>(3);
                    dictionary.Add("ATK", 0);
                    dictionary.Add("SKL01", 1);
                    dictionary.Add("SKL02", 2);
                    <>f__switch$map1 = dictionary;
                }
                if (<>f__switch$map1.TryGetValue(key, out num))
                {
                    switch (num)
                    {
                        case 0:
                            base.TryAttack();
                            goto Label_009A;

                        case 1:
                            base.TryUseSkill(1);
                            goto Label_009A;

                        case 2:
                            base.TryUseSkill(2);
                            goto Label_009A;
                    }
                }
            }
            throw new Exception("Invalid Type or State!");
        Label_009A:
            return true;
        }
    }
}

