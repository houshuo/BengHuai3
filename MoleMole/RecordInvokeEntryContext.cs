namespace MoleMole
{
    using FlatBuffers;
    using MoleMole.MPProtocol;
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct RecordInvokeEntryContext
    {
        public FlatBufferBuilder builder;
        public List<Offset<AbilityInvokeEntry>> outOffsetLs;
        public bool shouldRecord;
        public int offset;
        public AbilityInvokeArgument argType;
        public byte instancedAbilityID;
        public byte instanceModifierID;
        public uint targetID;
        public byte localID;
        public void Finish(bool record)
        {
            this.shouldRecord = record;
            if (this.shouldRecord)
            {
                this.WriteBuilder();
            }
        }

        public void Finish<T>(Offset<T> offset, AbilityInvokeArgument argType) where T: Table
        {
            this.offset = offset.Value;
            this.argType = argType;
            this.Finish(true);
        }

        public void Finish(AbilityInvokeArgument argType)
        {
            this.offset = -1;
            this.argType = argType;
            this.Finish(true);
        }

        private void WriteBuilder()
        {
            AbilityInvokeEntry.StartAbilityInvokeEntry(this.builder);
            AbilityInvokeEntry.AddInstancedAbilityID(this.builder, this.instancedAbilityID);
            AbilityInvokeEntry.AddInstancedModifierID(this.builder, this.instanceModifierID);
            AbilityInvokeEntry.AddTarget(this.builder, this.targetID);
            AbilityInvokeEntry.AddLocalID(this.builder, this.localID);
            if (this.argType != AbilityInvokeArgument.NONE)
            {
                AbilityInvokeEntry.AddArgumentType(this.builder, this.argType);
            }
            if (this.offset >= 0)
            {
                AbilityInvokeEntry.AddArgument(this.builder, this.offset);
            }
            Offset<AbilityInvokeEntry> item = AbilityInvokeEntry.EndAbilityInvokeEntry(this.builder);
            this.outOffsetLs.Add(item);
        }
    }
}

