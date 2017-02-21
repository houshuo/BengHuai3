namespace MoleMole
{
    using LuaInterface;
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public class LDWaitPointInSight : BaseLDEvent
    {
        private List<MonoSpawnPoint> _pointList = new List<MonoSpawnPoint>();

        public LDWaitPointInSight(LuaTable spawnPoints)
        {
            IEnumerator enumerator = spawnPoints.Values.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    object current = enumerator.Current;
                    int namedSpawnPointIx = Singleton<StageManager>.Instance.GetStageEnv().GetNamedSpawnPointIx(current as string);
                    this._pointList.Add(Singleton<StageManager>.Instance.GetStageEnv().spawnPoints[namedSpawnPointIx]);
                }
            }
            finally
            {
                IDisposable disposable = enumerator as IDisposable;
                if (disposable == null)
                {
                }
                disposable.Dispose();
            }
        }

        public override void Core()
        {
            foreach (MonoSpawnPoint point in this._pointList)
            {
                if (Singleton<LevelDesignManager>.Instance.IsPointInCameraFov(point.transform.position))
                {
                    base.Done();
                }
            }
        }
    }
}

