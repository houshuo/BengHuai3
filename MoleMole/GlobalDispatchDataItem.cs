namespace MoleMole
{
    using SimpleJSON;
    using System;
    using System.Collections.Generic;

    public class GlobalDispatchDataItem
    {
        public readonly List<RegionDataItem> regionList = new List<RegionDataItem>();

        public GlobalDispatchDataItem(JSONNode json)
        {
            IEnumerator<JSONNode> enumerator = json["region_list"].Childs.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    JSONNode current = enumerator.Current;
                    RegionDataItem item = new RegionDataItem {
                        dispatchUrl = (string) current["dispatch_url"],
                        name = (string) current["name"]
                    };
                    this.regionList.Add(item);
                }
            }
            finally
            {
                if (enumerator == null)
                {
                }
                enumerator.Dispose();
            }
        }

        public class RegionDataItem
        {
            public string dispatchUrl;
            public string name;
        }
    }
}

