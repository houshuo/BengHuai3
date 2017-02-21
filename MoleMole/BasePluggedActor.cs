namespace MoleMole
{
    using FullInspector;
    using System;
    using System.Collections.Generic;

    public abstract class BasePluggedActor : BaseActor
    {
        [ShowInInspector]
        private List<BaseActorPlugin> _pluginList = new List<BaseActorPlugin>();
        private Dictionary<System.Type, BaseActorPlugin> _pluginMap = new Dictionary<System.Type, BaseActorPlugin>();
        public Func<BaseEvent, bool> rejectBaseEventHandlingPredicate;

        protected BasePluggedActor()
        {
        }

        public void AddPlugin(BaseActorPlugin plugin)
        {
            System.Type type = plugin.GetType();
            this.AddPlugin(plugin, type);
        }

        private void AddPlugin(BaseActorPlugin plugin, System.Type type)
        {
            if (!this._pluginMap.ContainsKey(type))
            {
                this._pluginMap.Add(type, plugin);
                this._pluginList.Add(plugin);
                plugin.OnAdded();
            }
        }

        public void AddPluginAs<T>(BaseActorPlugin plugin) where T: BaseActorPlugin
        {
            this.AddPlugin(plugin, typeof(T));
        }

        public override void Core()
        {
            base.Core();
            for (int i = 0; i < this._pluginList.Count; i++)
            {
                this._pluginList[i].Core();
            }
        }

        public List<BaseActorPlugin> GetAllPlugins()
        {
            return this._pluginList;
        }

        public T GetPlugin<T>() where T: BaseActorPlugin
        {
            BaseActorPlugin plugin;
            this._pluginMap.TryGetValue(typeof(T), out plugin);
            return (T) plugin;
        }

        public T2 GetPluginAs<T1, T2>() where T1: BaseActorPlugin where T2: T1
        {
            return (T2) this.GetPlugin<T1>();
        }

        public bool HasPlugin<T>() where T: BaseActorPlugin
        {
            return this._pluginMap.ContainsKey(typeof(T));
        }

        public override bool ListenEvent(BaseEvent evt)
        {
            bool flag = false;
            for (int i = 0; i < this._pluginList.Count; i++)
            {
                flag |= this._pluginList[i].ListenEvent(evt);
            }
            return flag;
        }

        public sealed override bool OnEvent(BaseEvent evt)
        {
            bool flag = (this.rejectBaseEventHandlingPredicate == null) ? false : this.rejectBaseEventHandlingPredicate(evt);
            bool flag2 = false;
            flag2 |= this.OnPluginPreEvent(evt);
            if (!flag)
            {
                flag2 |= this.OnEventWithPlugins(evt);
            }
            flag2 |= this.OnPluginPostEvent(evt);
            if ((flag || !flag2) || evt.requireResolve)
            {
            }
            if (!flag)
            {
                flag2 |= this.OnEventResolves(evt);
            }
            if ((flag || !flag2) || evt.requireResolve)
            {
            }
            return (flag2 | this.OnPluginResolvedEvent(evt));
        }

        public virtual bool OnEventResolves(BaseEvent evt)
        {
            return false;
        }

        public virtual bool OnEventWithPlugins(BaseEvent evt)
        {
            return false;
        }

        private bool OnPluginPostEvent(BaseEvent evt)
        {
            bool flag = false;
            for (int i = 0; i < this._pluginList.Count; i++)
            {
                flag |= this._pluginList[i].OnPostEvent(evt);
            }
            return flag;
        }

        private bool OnPluginPreEvent(BaseEvent evt)
        {
            bool flag = false;
            for (int i = 0; i < this._pluginList.Count; i++)
            {
                flag |= this._pluginList[i].OnEvent(evt);
            }
            return flag;
        }

        private bool OnPluginResolvedEvent(BaseEvent evt)
        {
            bool flag = false;
            for (int i = 0; i < this._pluginList.Count; i++)
            {
                flag |= this._pluginList[i].OnResolvedEvent(evt);
            }
            return flag;
        }

        public override void OnRemoval()
        {
            base.OnRemoval();
            for (int i = 0; i < this._pluginList.Count; i++)
            {
                this._pluginList[i].OnRemoved();
            }
            this._pluginList.Clear();
            this._pluginMap.Clear();
        }

        public void RemovePlugin<T>()
        {
            this.RemovePlugin(typeof(T));
        }

        public void RemovePlugin(BaseActorPlugin plugin)
        {
            this.RemovePlugin(plugin.GetType());
        }

        public void RemovePlugin(System.Type type)
        {
            if (this._pluginMap.ContainsKey(type))
            {
                BaseActorPlugin item = this._pluginMap[type];
                this._pluginMap.Remove(type);
                this._pluginList.Remove(item);
                item.OnRemoved();
            }
        }
    }
}

