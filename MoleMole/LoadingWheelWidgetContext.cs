namespace MoleMole
{
    using MoleMole.Config;
    using System;
    using System.Runtime.InteropServices;

    public class LoadingWheelWidgetContext : BaseWidgetContext
    {
        private bool _isSimpleWaitCmd;
        private CanvasTimer _timer;
        private ushort _waitCmdID;
        public bool ignoreMaxWaitTime;
        private float MAX_WAIT_TIME;
        public Action timeOutCallBack;

        public LoadingWheelWidgetContext()
        {
            this.MAX_WAIT_TIME = 5f;
            this.SetConfig();
            this._isSimpleWaitCmd = false;
        }

        public LoadingWheelWidgetContext(ushort waitCmd, Action timeOutCallBack = null)
        {
            this.MAX_WAIT_TIME = 5f;
            this.SetConfig();
            this._isSimpleWaitCmd = true;
            this._waitCmdID = waitCmd;
            this.timeOutCallBack = timeOutCallBack;
        }

        public override void Destroy()
        {
            if (this._timer != null)
            {
                this._timer.Destroy();
            }
            base.Destroy();
        }

        public void Finish()
        {
            this.Destroy();
        }

        public override bool OnPacket(NetPacketV1 pkt)
        {
            if (this._isSimpleWaitCmd && (pkt.getCmdId() == this._waitCmdID))
            {
                this.Finish();
            }
            return false;
        }

        private void OnTimeOut()
        {
            if (this.timeOutCallBack != null)
            {
                this.timeOutCallBack();
            }
            this.Destroy();
        }

        private void SetConfig()
        {
            ContextPattern pattern = new ContextPattern {
                contextName = "LoadingWheelDialogContext",
                viewPrefabPath = "UI/Menus/Dialog/LoadingWheelDialog",
                cacheType = ViewCacheType.DontCache
            };
            base.config = pattern;
            base.uiType = UIType.MostFront;
        }

        public void SetMaxWaitTime(float maxWaitTime)
        {
            if (maxWaitTime > 0f)
            {
                this.MAX_WAIT_TIME = maxWaitTime;
            }
        }

        protected override bool SetupView()
        {
            if (this._timer != null)
            {
                this._timer.Destroy();
            }
            if (!this.ignoreMaxWaitTime)
            {
                this._timer = Singleton<MainUIManager>.Instance.SceneCanvas.CreateTimer(this.MAX_WAIT_TIME, 0f);
                this._timer.timeUpCallback = new Action(this.OnTimeOut);
            }
            return false;
        }
    }
}

