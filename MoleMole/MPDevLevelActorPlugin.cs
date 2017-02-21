namespace MoleMole
{
    using MoleMole.MPProtocol;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    public class MPDevLevelActorPlugin : BaseActorPlugin
    {
        public MonoMPDevLevel _mpDevLevel;

        public MPDevLevelActorPlugin(MonoMPDevLevel devLevel)
        {
            this._mpDevLevel = devLevel;
        }

        [DebuggerHidden]
        private IEnumerator MasterWaitAndDo()
        {
            return new <MasterWaitAndDo>c__Iterator49();
        }

        public override bool OnEvent(BaseEvent evt)
        {
            return ((evt is EvtStageReady) && this.OnStageReady((EvtStageReady) evt));
        }

        private bool OnStageReady(EvtStageReady evt)
        {
            if (Singleton<MPManager>.Instance.isMaster && (Singleton<MPLevelManager>.Instance.mpMode == MPMode.Normal))
            {
                this._mpDevLevel.StartCoroutine(this.MasterWaitAndDo());
            }
            return true;
        }

        [CompilerGenerated]
        private sealed class <MasterWaitAndDo>c__Iterator49 : IEnumerator, IDisposable, IEnumerator<object>
        {
            internal object $current;
            internal int $PC;

            [DebuggerHidden]
            public void Dispose()
            {
                this.$PC = -1;
            }

            public bool MoveNext()
            {
                uint num = (uint) this.$PC;
                this.$PC = -1;
                switch (num)
                {
                    case 0:
                        this.$current = new WaitForSeconds(5f);
                        this.$PC = 1;
                        goto Label_00FA;

                    case 1:
                        break;

                    case 2:
                        Singleton<MonsterManager>.Instance.CreateMonster("DeadArcher", "Default", 10, true, Vector3.zero, Singleton<RuntimeIDManager>.Instance.GetNextRuntimeID(4), false, 0, true, false, 0);
                        goto Label_00DC;

                    case 3:
                        goto Label_00DC;

                    default:
                        goto Label_00F8;
                }
            Label_0045:
                Singleton<MonsterManager>.Instance.CreateMonster("DeadWalker", "Default", 10, true, Vector3.zero, Singleton<RuntimeIDManager>.Instance.GetNextRuntimeID(4), false, 0, true, false, 0);
                this.$current = new WaitForSeconds(2f);
                this.$PC = 2;
                goto Label_00FA;
            Label_00DC:
                while (Singleton<MonsterManager>.Instance.LivingMonsterCount() > 0)
                {
                    this.$current = new WaitForSeconds(2f);
                    this.$PC = 3;
                    goto Label_00FA;
                }
                goto Label_0045;
                this.$PC = -1;
            Label_00F8:
                return false;
            Label_00FA:
                return true;
            }

            [DebuggerHidden]
            public void Reset()
            {
                throw new NotSupportedException();
            }

            object IEnumerator<object>.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }

            object IEnumerator.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }
        }
    }
}

