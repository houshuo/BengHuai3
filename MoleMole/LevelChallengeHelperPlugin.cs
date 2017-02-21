namespace MoleMole
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public class LevelChallengeHelperPlugin : BaseActorPlugin
    {
        public List<BaseLevelChallenge> challengeList;
        [HideInInspector]
        public readonly LevelActor levelActor;

        public LevelChallengeHelperPlugin(LevelActor levelActor)
        {
            this.levelActor = levelActor;
            this.challengeList = new List<BaseLevelChallenge>();
            List<int> trackChallengeIds = Singleton<LevelScoreManager>.Instance.trackChallengeIds;
            if (trackChallengeIds != null)
            {
                foreach (int num in trackChallengeIds)
                {
                    this.challengeList.Add(this.GetChallengeById(num));
                }
            }
        }

        public override void Core()
        {
            for (int i = 0; i < this.challengeList.Count; i++)
            {
                BaseLevelChallenge challenge = this.challengeList[i];
                if (challenge.active)
                {
                    challenge.Core();
                }
            }
        }

        private BaseLevelChallenge GetChallengeById(int challengeId)
        {
            LevelChallengeMetaData levelChallengeMetaDataByKey = LevelChallengeMetaDataReader.GetLevelChallengeMetaDataByKey(challengeId);
            switch (levelChallengeMetaDataByKey.conditionId)
            {
                case 1:
                    return new LimitAvatarChallege(this, levelChallengeMetaDataByKey);

                case 2:
                    return new LimitAvatarKilledChallenge(this, levelChallengeMetaDataByKey);

                case 3:
                    return new MaxComboChallenge(this, levelChallengeMetaDataByKey);

                case 4:
                    return new LimitBeHitChallenge(this, levelChallengeMetaDataByKey);

                case 5:
                    return new LimitLevelTimeChallenge(this, levelChallengeMetaDataByKey);

                case 6:
                    return new LimitWitchTimeTriggeredChallenge(this, levelChallengeMetaDataByKey);

                case 7:
                    return new MaxDamageInWitchTimeChallenge(this, levelChallengeMetaDataByKey);

                case 8:
                    return new MaxUltimateSkillTriggeredChallenge(this, levelChallengeMetaDataByKey);

                case 9:
                    return new MaxDamageInCertainTimeChallenge(this, levelChallengeMetaDataByKey);

                case 10:
                    return new MaxUltimateSkillTriggeredByDistinctAvatarChallenge(this, levelChallengeMetaDataByKey);

                case 11:
                    return new LimitBeHitDownChallenge(this, levelChallengeMetaDataByKey);

                case 12:
                    return new MaxAvatarClassInTeamChallenge(this, levelChallengeMetaDataByKey);

                case 13:
                    return new MaxMonsterKilledChallenge(this, levelChallengeMetaDataByKey);

                case 14:
                    return new MaxAvatarNatureInTeamChallenge(this, levelChallengeMetaDataByKey);

                case 15:
                    return new LimitTotalDamageTakenChallenge(this, levelChallengeMetaDataByKey);

                case 0x10:
                    return new HelperAvatarAliveChallenge(this, levelChallengeMetaDataByKey);

                case 0x11:
                    return new LimitTrapTriggeredChallenge(this, levelChallengeMetaDataByKey);

                case 0x12:
                    return new MaxBoxOpenedChallenge(this, levelChallengeMetaDataByKey);

                case 0x13:
                    return new MaxDebuffAddChallenge(this, levelChallengeMetaDataByKey);

                case 20:
                    return new LimitBeAddedDebuffChallenge(this, levelChallengeMetaDataByKey);

                case 0x15:
                    return new MaxAllDamageInWitchTimeChallenge(this, levelChallengeMetaDataByKey);

                case 0x16:
                    return new MaxMonsterKilledBySwitchInAttackChallenge(this, levelChallengeMetaDataByKey);

                case 0x17:
                    return new MaxMonsterKilledInWitchTimeChallenge(this, levelChallengeMetaDataByKey);

                case 0x18:
                    return new MonsterLastKilledBySwitchInAttackChallenge(this, levelChallengeMetaDataByKey);

                case 0x19:
                    return new MaxMonsterKilledInCertainTimeChallenge(this, levelChallengeMetaDataByKey);

                case 0x1a:
                    return new MaxMonsterHitAirChallenge(this, levelChallengeMetaDataByKey);

                case 0x1b:
                case 0x1c:
                case 0x1d:
                    return new SpecialLevelTimeChallenge(this, levelChallengeMetaDataByKey);

                case 30:
                    return new MaxMonsterKilledByBranchAttackChallenge(this, levelChallengeMetaDataByKey);

                case 0x1f:
                    return new MaxQTETriggeredChallenge(this, levelChallengeMetaDataByKey);
            }
            throw new Exception("Invalid Type or State!");
        }

        public override bool ListenEvent(BaseEvent evt)
        {
            bool flag = false;
            foreach (BaseLevelChallenge challenge in this.challengeList)
            {
                if (challenge.active)
                {
                    flag |= challenge.ListenEvent(evt);
                }
            }
            return flag;
        }

        public override void OnAdded()
        {
            foreach (BaseLevelChallenge challenge in this.challengeList)
            {
                challenge.OnAdded();
            }
        }

        public override bool OnEvent(BaseEvent evt)
        {
            bool flag = false;
            foreach (BaseLevelChallenge challenge in this.challengeList)
            {
                if (challenge.active)
                {
                    flag |= challenge.OnEvent(evt);
                }
            }
            return flag;
        }

        public override bool OnPostEvent(BaseEvent evt)
        {
            bool flag = false;
            foreach (BaseLevelChallenge challenge in this.challengeList)
            {
                if (challenge.active)
                {
                    flag |= challenge.OnPostEvent(evt);
                }
            }
            return flag;
        }
    }
}

