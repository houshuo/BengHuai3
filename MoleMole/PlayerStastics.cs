namespace MoleMole
{
    using System;

    public class PlayerStastics
    {
        public SafeInt32 screenRotateTimes;
        public SafeFloat stageTime;

        public PlayerStastics()
        {
            this.screenRotateTimes = 0;
            this.stageTime = 0f;
            this.screenRotateTimes = 0;
            this.stageTime = 0f;
        }

        public PlayerStastics(float levelTime, int screenRotateTimes)
        {
            this.screenRotateTimes = 0;
            this.stageTime = 0f;
            this.stageTime = levelTime;
            this.screenRotateTimes = screenRotateTimes;
        }

        public void ResetPlayerStasticsData()
        {
            this.screenRotateTimes = 0;
            this.stageTime = 0f;
        }
    }
}

