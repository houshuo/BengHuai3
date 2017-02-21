namespace MoleMole
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public class LevelTutorialHelperPlugin : BaseActorPlugin
    {
        [HideInInspector]
        public readonly LevelActor levelActor;
        public List<BaseLevelTutorial> tutorialList;

        public LevelTutorialHelperPlugin(LevelActor levelActor)
        {
            this.levelActor = levelActor;
            this.tutorialList = new List<BaseLevelTutorial>();
            int levelId = Singleton<LevelScoreManager>.Instance.LevelId;
            List<int> unFinishedTutorialIDList = Singleton<LevelTutorialModule>.Instance.GetUnFinishedTutorialIDList(levelId);
            if (unFinishedTutorialIDList != null)
            {
                foreach (int num2 in unFinishedTutorialIDList)
                {
                    this.tutorialList.Add(this.GetTutorialById(num2));
                }
            }
        }

        public override void Core()
        {
            foreach (BaseLevelTutorial tutorial in this.tutorialList)
            {
                if (tutorial.active)
                {
                    tutorial.Core();
                }
            }
        }

        private BaseLevelTutorial GetTutorialById(int tutorialId)
        {
            LevelTutorialMetaData levelTutorialMetaDataByKey = LevelTutorialMetaDataReader.GetLevelTutorialMetaDataByKey(tutorialId);
            switch (levelTutorialMetaDataByKey.tutorialId)
            {
                case 0x2711:
                    return new LevelTutorialPlayerTeaching(this, levelTutorialMetaDataByKey);

                case 0x2712:
                    return new LevelTutorialUltraAttack(this, levelTutorialMetaDataByKey);

                case 0x2713:
                    return new LevelTutorialBranchAttack(this, levelTutorialMetaDataByKey);

                case 0x2714:
                    return new LevelTutorialEliteAttack(this, levelTutorialMetaDataByKey);

                case 0x2715:
                    return new LevelTutorialSwapAttack(this, levelTutorialMetaDataByKey);

                case 0x2716:
                    return new LevelTutorialSwapAndRestrain(this, levelTutorialMetaDataByKey);

                case 0x2717:
                    return new LevelTutorialMonsterBlock(this, levelTutorialMetaDataByKey);

                case 0x2718:
                    return new LevelTutorialMonsterTeleport(this, levelTutorialMetaDataByKey);

                case 0x2719:
                    return new LevelTutorialMonsterShield(this, levelTutorialMetaDataByKey);

                case 0x271a:
                    return new LevelTutorialMonsterRobotDodge(this, levelTutorialMetaDataByKey);
            }
            throw new Exception("Invalid Type or State!");
        }

        public override bool ListenEvent(BaseEvent evt)
        {
            bool flag = false;
            foreach (BaseLevelTutorial tutorial in this.tutorialList)
            {
                if (tutorial.active)
                {
                    flag |= tutorial.ListenEvent(evt);
                }
            }
            return flag;
        }

        public override void OnAdded()
        {
            foreach (BaseLevelTutorial tutorial in this.tutorialList)
            {
                tutorial.OnAdded();
            }
        }

        public override bool OnEvent(BaseEvent evt)
        {
            bool flag = false;
            foreach (BaseLevelTutorial tutorial in this.tutorialList)
            {
                if (tutorial.active)
                {
                    flag |= tutorial.OnEvent(evt);
                }
            }
            return flag;
        }

        public override bool OnPostEvent(BaseEvent evt)
        {
            bool flag = false;
            foreach (BaseLevelTutorial tutorial in this.tutorialList)
            {
                if (tutorial.active)
                {
                    flag |= tutorial.OnPostEvent(evt);
                }
            }
            return flag;
        }
    }
}

