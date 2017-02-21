namespace MoleMole
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Threading;

    public class BackGroundWorker
    {
        private List<Action> _list = new List<Action>();
        private volatile bool _stillWorking;
        private Thread _workThread;

        public void AddBackGroundWork(Action work)
        {
            List<Action> list = this._list;
            lock (list)
            {
                this._list.Add(work);
            }
        }

        public void StartBackGroundWork(string threadName = "")
        {
            this._stillWorking = true;
            this._list.Clear();
            this._workThread = new Thread(new ThreadStart(this.WorkingThread));
            this._workThread.Name = threadName;
            this._workThread.Priority = ThreadPriority.Highest;
            this._workThread.IsBackground = true;
            this._workThread.Start();
        }

        public void StopBackGroundWork(bool clearList = true)
        {
            if (clearList)
            {
                List<Action> list = this._list;
                lock (list)
                {
                    this._list.Clear();
                }
            }
            this._stillWorking = false;
            this._workThread = null;
        }

        private void WorkingThread()
        {
            while (this._stillWorking)
            {
                if (this._list.Count == 0)
                {
                    Thread.Sleep(1);
                }
                else
                {
                    Action action;
                    List<Action> list = this._list;
                    lock (list)
                    {
                        action = this._list[0];
                    }
                    try
                    {
                        action();
                    }
                    catch (Exception)
                    {
                    }
                    List<Action> list2 = this._list;
                    lock (list2)
                    {
                        this._list.RemoveAt(0);
                    }
                    Thread.Sleep(1);
                }
            }
        }

        public int RemainCount
        {
            get
            {
                return this._list.Count;
            }
        }
    }
}

