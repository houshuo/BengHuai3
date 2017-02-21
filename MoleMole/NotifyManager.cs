namespace MoleMole
{
    using System;
    using System.Collections.Generic;

    public class NotifyManager
    {
        private List<BaseContext> _contextList = new List<BaseContext>();
        private List<BaseModule> _moduleList = new List<BaseModule>();

        private NotifyManager()
        {
        }

        public void ClearAllContext()
        {
            this._contextList.Clear();
        }

        public bool FireNotify(Notify cmd)
        {
            bool flag = false;
            if (cmd.type == NotifyTypes.NetwrokPacket)
            {
                NetPacketV1 body = cmd.body as NetPacketV1;
                flag |= this.HandlePacketForModules(body);
            }
            BaseContext[] contextArray = new BaseContext[this._contextList.Count];
            this._contextList.CopyTo(contextArray);
            foreach (BaseContext context in contextArray)
            {
                if (context != null)
                {
                    flag |= context.HandleNotify(cmd);
                }
            }
            return false;
        }

        private bool HandlePacketForModules(NetPacketV1 pkt)
        {
            bool flag = false;
            for (int i = 0; i < this._moduleList.Count; i++)
            {
                flag |= this._moduleList[i].OnPacket(pkt);
            }
            return flag;
        }

        public void RegisterContext(BaseContext context)
        {
            if (!context.config.ignoreNotify && !this._contextList.Contains(context))
            {
                this._contextList.Add(context);
            }
        }

        public void RegisterModule(BaseModule listener)
        {
            if (!this._moduleList.Contains(listener))
            {
                this._moduleList.Add(listener);
            }
        }

        public void RemoveContext(BaseContext context)
        {
            if (!context.config.ignoreNotify && this._contextList.Contains(context))
            {
                this._contextList.Remove(context);
            }
        }
    }
}

