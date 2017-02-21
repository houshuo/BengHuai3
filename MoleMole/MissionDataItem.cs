namespace MoleMole
{
    using proto;
    using System;
    using System.Collections.Generic;

    public class MissionDataItem
    {
        private static Dictionary<MissionStatus, int> _statusPriority;
        private static Dictionary<int, int> _typePriority;
        public int beginTime;
        public bool beginTimeSpecified;
        public int endTime;
        public bool endTimeSpecified;
        public int id;
        public MissionData metaData;
        public int progress;
        public MissionStatus status;

        static MissionDataItem()
        {
            Dictionary<MissionStatus, int> dictionary = new Dictionary<MissionStatus, int>();
            dictionary.Add(5, 1);
            dictionary.Add(2, 2);
            dictionary.Add(3, 3);
            dictionary.Add(1, 2);
            dictionary.Add(4, 1);
            _statusPriority = dictionary;
            Dictionary<int, int> dictionary2 = new Dictionary<int, int>();
            dictionary2.Add(2, 1);
            dictionary2.Add(1, 3);
            dictionary2.Add(3, 2);
            dictionary2.Add(4, 4);
            _typePriority = dictionary2;
        }

        public MissionDataItem(Mission mission)
        {
            this.id = (int) mission.get_mission_id();
            this.metaData = MissionDataReader.GetMissionDataByKey(this.id);
            this.status = mission.get_status();
            this.progress = (int) mission.get_progress();
            this.beginTimeSpecified = mission.get_begin_timeSpecified();
            this.beginTime = (int) mission.get_begin_time();
            this.endTimeSpecified = mission.get_end_timeSpecified();
            this.endTime = (int) mission.get_end_time();
        }

        public static int CompareToMission(MissionDataItem lobj, MissionDataItem robj)
        {
            int num = _statusPriority[lobj.status];
            int num2 = _statusPriority[robj.status];
            if (num != num2)
            {
                return (num2 - num);
            }
            num = _typePriority[lobj.metaData.type];
            num2 = _typePriority[robj.metaData.type];
            if (num != num2)
            {
                return (num2 - num);
            }
            return (lobj.id - robj.id);
        }

        public bool IsMissionEqual(Mission mission)
        {
            return ((((this.status == mission.get_status()) && (this.progress == mission.get_progress())) && (!mission.get_begin_timeSpecified() || (this.beginTime == mission.get_begin_time()))) && (!mission.get_end_timeSpecified() || (this.endTime == mission.get_end_time())));
        }

        public void UpdateFromMission(Mission mission)
        {
            this.status = mission.get_status();
            this.progress = (int) mission.get_progress();
            this.beginTimeSpecified = mission.get_begin_timeSpecified();
            this.beginTime = (int) mission.get_begin_time();
            this.endTimeSpecified = mission.get_end_timeSpecified();
            this.endTime = (int) mission.get_end_time();
        }
    }
}

