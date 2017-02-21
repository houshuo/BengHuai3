namespace MoleMole.Config
{
    using FullInspector;
    using UnityEngine;

    public class ConfigTrailData : BaseScriptableObject
    {
        public TrailData properties;

        public static ConfigTrailData CreateByTrailData(TrailData trailData)
        {
            ConfigTrailData data = ScriptableObject.CreateInstance<ConfigTrailData>();
            data.properties = new TrailData(trailData.clockwise, trailData.points, trailData.totalPolar, trailData.startPolarOffset, trailData.radiusX, trailData.radiusY, trailData.liftY, trailData.rotate, trailData.position);
            return data;
        }
    }
}

