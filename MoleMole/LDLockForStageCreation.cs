namespace MoleMole
{
    using System;
    using UnityEngine;

    public class LDLockForStageCreation : BaseLDEvent
    {
        private int _frameIx;
        private bool _isBeginStage;
        private bool _needUpdate;
        private bool _stageReady;
        private const int WAIT_FRAME_CNT = 3;

        public LDLockForStageCreation(bool locked, bool isBeginStage)
        {
            if (locked)
            {
                Time.timeScale = 0f;
                Time.fixedDeltaTime = 0f;
                Singleton<EventManager>.Instance.SetPauseDispatching(true);
                Singleton<MonsterManager>.Instance.UnOccupyAllPreloadedMonsters();
                base.Done();
            }
            else
            {
                Singleton<MonsterManager>.Instance.DestroyUnOccupiedPreloadMonsters();
                Singleton<EffectManager>.Instance.ReloadEffectPool();
            }
            this._frameIx = 0;
            this._needUpdate = !locked;
            this._isBeginStage = isBeginStage;
        }

        public override void Core()
        {
            if (this._needUpdate)
            {
                if (this._frameIx == 0)
                {
                    if (!this._isBeginStage)
                    {
                        GC.Collect();
                        GC.WaitForPendingFinalizers();
                    }
                }
                else if (this._frameIx == 1)
                {
                    if (!this._isBeginStage)
                    {
                        Resources.UnloadUnusedAssets();
                    }
                    Singleton<EventManager>.Instance.SetPauseDispatching(false);
                }
                else if ((this._frameIx >= 3) && this._stageReady)
                {
                    Time.timeScale = 1f;
                    Time.fixedDeltaTime = 0.02f * Time.timeScale;
                    base.Done();
                }
                this._frameIx++;
            }
        }

        public override void OnEvent(BaseEvent evt)
        {
            if (evt is EvtStageReady)
            {
                this._stageReady = true;
            }
        }
    }
}

