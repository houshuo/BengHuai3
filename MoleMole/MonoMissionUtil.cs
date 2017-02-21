namespace MoleMole
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public class MonoMissionUtil : MonoBehaviour
    {
        private List<MissionDataItem> _previewList = new List<MissionDataItem>();

        public void AddPreviewMission(MissionDataItem mission)
        {
            this._previewList.Add(mission);
        }

        public void Init()
        {
            this._previewList.Clear();
        }

        private void Update()
        {
            bool flag = false;
            uint body = 0;
            for (int i = this._previewList.Count - 1; i >= 0; i--)
            {
                MissionDataItem item = this._previewList[i];
                TimeSpan span = (TimeSpan) (Miscs.GetDateTimeFromTimeStamp((uint) item.beginTime) - TimeUtil.Now);
                if (span.TotalSeconds <= item.metaData.PreviewTime)
                {
                    flag = true;
                    body = (uint) item.id;
                    this._previewList.RemoveAt(i);
                }
            }
            if (flag)
            {
                Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.MissionUpdated, body));
            }
        }
    }
}

