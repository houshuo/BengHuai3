namespace MoleMole
{
    using BehaviorDesigner.Runtime.Tasks;
    using System;
    using System.Collections.Generic;

    [TaskCategory("Avatar")]
    public class CheckTargetAttackStateAndRange : BaseAvatarAction
    {
        public const float DEFAULT_MONSTER_ATTACK_RANGE = 3f;
        public float TimeBeforeAttack = 0.48f;

        public override TaskStatus OnUpdate()
        {
            if (!Singleton<LevelManager>.Instance.levelActor.witchTimeLevelBuff.isActive)
            {
                List<BaseMonoMonster> allMonsters = Singleton<MonsterManager>.Instance.GetAllMonsters();
                for (int i = 0; i < allMonsters.Count; i++)
                {
                    BaseMonoMonster monster = allMonsters[i];
                    if (monster.isGoingToAttack(this.TimeBeforeAttack) && (Miscs.DistancForVec3IgnoreY(base._avatar.XZPosition, monster.XZPosition) < monster.config.AIArguments.AttackRange))
                    {
                        return TaskStatus.Success;
                    }
                }
            }
            return TaskStatus.Failure;
        }
    }
}

