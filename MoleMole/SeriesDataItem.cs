namespace MoleMole
{
    using System;
    using System.Collections.Generic;

    public class SeriesDataItem
    {
        private SeriesMetaData _metaData;
        public List<WeekDayActivityDataItem> weekActivityList;

        public SeriesDataItem(int seriesID)
        {
            this._metaData = SeriesMetaDataReader.GetSeriesMetaDataByKey(seriesID);
            this.weekActivityList = new List<WeekDayActivityDataItem>();
        }

        public int id
        {
            get
            {
                return this._metaData.id;
            }
        }

        public string title
        {
            get
            {
                return LocalizationGeneralLogic.GetText(this._metaData.title, new object[0]);
            }
        }
    }
}

