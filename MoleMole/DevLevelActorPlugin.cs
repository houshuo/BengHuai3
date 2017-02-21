namespace MoleMole
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    public class DevLevelActorPlugin : BaseActorPlugin
    {
        public MonoDevLevel _devLevel;

        public DevLevelActorPlugin(MonoDevLevel devLevel)
        {
            this._devLevel = devLevel;
        }

        public override bool OnEvent(BaseEvent evt)
        {
            if (evt is EvtStageReady)
            {
                this.OnStageReady((EvtStageReady) evt);
            }
            return false;
        }

        private void OnStageReady(EvtStageReady evt)
        {
        }

        [DebuggerHidden]
        private IEnumerator WaitAndDo()
        {
            return new <WaitAndDo>c__Iterator2F();
        }

        [DebuggerHidden]
        private IEnumerator WaitAndSpawnGoodsAndProps()
        {
            return new <WaitAndSpawnGoodsAndProps>c__Iterator30();
        }

        [CompilerGenerated]
        private sealed class <WaitAndDo>c__Iterator2F : IEnumerator, IDisposable, IEnumerator<object>
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
                        this.$current = new WaitForSeconds(3f);
                        this.$PC = 1;
                        goto Label_013D;

                    case 1:
                        HPProfile.Begin();
                        Singleton<PropObjectManager>.Instance.CreatePropObject(0x21800001, "InvisibleProp", 0f, 0f, Vector3.zero, Vector3.forward, false);
                        HPProfile.End("1st invincible prop");
                        this.$current = null;
                        this.$PC = 2;
                        goto Label_013D;

                    case 2:
                        HPProfile.Begin();
                        Singleton<PropObjectManager>.Instance.CreatePropObject(0x21800001, "InvisibleProp", 0f, 0f, Vector3.zero, Vector3.forward, false);
                        HPProfile.End("2nd invincible prop");
                        HPProfile.Begin();
                        Singleton<EffectManager>.Instance.TriggerEntityEffectPattern("Monster_No_Break_Hit", Vector3.zero, Vector3.forward, Vector3.one, Singleton<LevelManager>.Instance.levelEntity);
                        HPProfile.End("1st effect");
                        HPProfile.Begin();
                        Singleton<EffectManager>.Instance.TriggerEntityEffectPattern("Monster_No_Break_Hit", Vector3.zero, Vector3.forward, Vector3.one, Singleton<LevelManager>.Instance.levelEntity);
                        HPProfile.End("2st effect");
                        this.$PC = -1;
                        break;
                }
                return false;
            Label_013D:
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

        [CompilerGenerated]
        private sealed class <WaitAndSpawnGoodsAndProps>c__Iterator30 : IEnumerator, IDisposable, IEnumerator<object>
        {
            internal object $current;
            internal int $PC;
            internal uint <id>__0;

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
                        this.$current = new WaitForSeconds(2f);
                        this.$PC = 1;
                        return true;

                    case 1:
                        LDDropDataItem.GetLDDropDataItemByName("HPMedic").CreateDropGoods(new Vector3(-6f, 0f, 6f), Vector3.forward, true);
                        LDDropDataItem.GetLDDropDataItemByName("SPMedic").CreateDropGoods(new Vector3(-5f, 0f, 6f), Vector3.forward, true);
                        LDDropDataItem.GetLDDropDataItemByName("Coin").CreateDropGoods(new Vector3(-4f, 0f, 6f), Vector3.forward, true);
                        LDDropDataItem.GetLDDropDataItemByName("Boost").CreateDropGoods(new Vector3(-3f, 0f, 6f), Vector3.forward, true);
                        LDDropDataItem.GetLDDropDataItemByName("Crit").CreateDropGoods(new Vector3(-2f, 0f, 6f), Vector3.forward, true);
                        LDDropDataItem.GetLDDropDataItemByName("Shielded").CreateDropGoods(new Vector3(-1f, 0f, 6f), Vector3.forward, true);
                        Singleton<DynamicObjectManager>.Instance.CreateStageExitField(0x21800001, new Vector3(1f, 0f, 6f), Vector3.forward);
                        Singleton<PropObjectManager>.Instance.CreatePropObject(0x21800001, "GeneralBox", 100000f, 0f, new Vector3(3f, 0f, 6f), Vector3.forward, false);
                        Singleton<PropObjectManager>.Instance.CreatePropObject(0x21800001, "AdvancedBox", 10f, 0f, new Vector3(5f, 0f, 6f), Vector3.forward, false);
                        Singleton<PropObjectManager>.Instance.CreatePropObject(0x21800001, "JokeBox", 10f, 0f, new Vector3(-6f, 0f, 4f), Vector3.forward, false);
                        Singleton<PropObjectManager>.Instance.CreatePropObject(0x21800001, "Barrel", 10f, 100f, new Vector3(-4f, 0f, 4f), Vector3.forward, false);
                        Singleton<DynamicObjectManager>.Instance.CreateBarrierField(0x21800001, "Barrier_01", new Vector3(0f, 0f, -6f), Vector3.right, 3f);
                        Singleton<PropObjectManager>.Instance.CreatePropObject(0x21800001, "Switch", 10f, 0f, new Vector3(-2f, 0f, 4f), Vector3.forward, false);
                        this.<id>__0 = Singleton<PropObjectManager>.Instance.CreatePropObject(0x21800001, "Trap_Fire", 10f, 0f, new Vector3(4f, 0f, 4f), Vector3.forward, false);
                        Singleton<LevelDesignManager>.Instance.EnableFireProp(this.<id>__0, 5f, 3f);
                        Singleton<PropObjectManager>.Instance.CreatePropObject(0x21800001, "Trap_Palsy_Bomb", 10f, 100f, new Vector3(7f, 0f, 4f), Vector3.forward, false);
                        Singleton<PropObjectManager>.Instance.CreatePropObject(0x21800001, "Trap_Palsy", 10f, 100f, new Vector3(0f, 0f, 2f), Vector3.forward, false);
                        Singleton<DynamicObjectManager>.Instance.CreateEquipItem(0x21800001, 0x4e21, new Vector3(5f, 0f, -7f), Vector3.forward, true, 1);
                        Singleton<DynamicObjectManager>.Instance.CreateStigmataItem(0x21800001, 0x7531, new Vector3(6f, 0f, -7f), Vector3.forward, true, 1);
                        Singleton<DynamicObjectManager>.Instance.CreateMaterialItem(0x21800001, 0x3e9, new Vector3(7f, 0f, -7f), Vector3.forward, true, 1);
                        Singleton<DynamicObjectManager>.Instance.CreateAvatarFragmentItem(0x21800001, 0x2775, new Vector3(8f, 0f, -7f), Vector3.forward, true, 1);
                        this.$PC = -1;
                        break;
                }
                return false;
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

