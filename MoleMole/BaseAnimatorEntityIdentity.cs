namespace MoleMole
{
    using MoleMole.MPProtocol;
    using System;
    using UnityEngine;

    public abstract class BaseAnimatorEntityIdentity : BaseAbilityEntityIdentiy
    {
        protected BaseMonoAnimatorEntity _animatorEntity;
        private Vector3 _lastSendXZ;
        private float _lastSentTime;
        private float _lastSentXZAngle;
        private Vector3 _lastSetForward;
        private int[] _muteSyncTagHashes;
        private float _sendInterval;

        private void AuthorityCore()
        {
            this.AuthoritySendTransformSyncCore();
        }

        public void AuthorityOnAnimatorStateChanged(AnimatorStateInfo fromState, AnimatorStateInfo toState)
        {
            if (!Miscs.ArrayContains<int>(this._muteSyncTagHashes, toState.tagHash))
            {
                MPSendPacketContainer pc = Singleton<MPManager>.Instance.CreateSendPacket<Packet_Entity_AnimatorStateChange>();
                Packet_Entity_AnimatorStateChange.StartPacket_Entity_AnimatorStateChange(pc.builder);
                Packet_Entity_AnimatorStateChange.AddNormalizedTimeTo(pc.builder, (fromState.shortNameHash != toState.shortNameHash) ? toState.normalizedTime : 0f);
                Packet_Entity_AnimatorStateChange.AddToStateHash(pc.builder, toState.shortNameHash);
                Packet_Entity_AnimatorStateChange.AddStateSync(pc.builder, EntityStateSync.CreateEntityStateSync(pc.builder, this._animatorEntity.XZPosition.x, this._animatorEntity.XZPosition.z, MPMiscs.ForwardToXZAngles(this._animatorEntity.FaceDirection)));
                pc.Finish<Packet_Entity_AnimatorStateChange>(Packet_Entity_AnimatorStateChange.EndPacket_Entity_AnimatorStateChange(pc.builder));
                Singleton<MPManager>.Instance.SendStateUpdateToOthers(base.runtimeID, pc);
            }
        }

        public void AuthorityOnFaceDirectionSet(Vector3 forward)
        {
            this._lastSetForward = forward;
        }

        protected virtual void AuthoritySendTransformSyncCore()
        {
            if ((Time.time - this._lastSentTime) > this._sendInterval)
            {
                float xzAngle = MPMiscs.ForwardToXZAngles(this._lastSetForward);
                Vector3 xZPosition = this._animatorEntity.XZPosition;
                Vector3 vector2 = xZPosition - this._lastSendXZ;
                if (MPMiscs.NeedSyncTransform(vector2.sqrMagnitude, xzAngle - this._lastSentXZAngle))
                {
                    MPSendPacketContainer pc = Singleton<MPManager>.Instance.CreateSendPacket<Packet_Entity_TransformSync>();
                    Packet_Entity_TransformSync.StartPacket_Entity_TransformSync(pc.builder);
                    Packet_Entity_TransformSync.AddStateSync(pc.builder, EntityStateSync.CreateEntityStateSync(pc.builder, this._animatorEntity.XZPosition.x, this._animatorEntity.XZPosition.z, xzAngle));
                    pc.Finish<Packet_Entity_TransformSync>(Packet_Entity_TransformSync.EndPacket_Entity_TransformSync(pc.builder));
                    Singleton<MPManager>.Instance.SendStateUpdateToOthers(base.runtimeID, pc);
                    this._lastSendXZ = xZPosition;
                    this._lastSentXZAngle = xzAngle;
                    this._lastSentTime = Time.time;
                }
            }
        }

        public void AuthorityUserInputControllerChanged(AnimatorParameterEntry entry)
        {
            MPSendPacketContainer pc = Singleton<MPManager>.Instance.CreateSendPacket<Packet_Entity_AnimatorParameterChange>();
            Packet_Entity_AnimatorParameterChange.StartPacket_Entity_AnimatorParameterChange(pc.builder);
            Packet_Entity_AnimatorParameterChange.AddStateHash(pc.builder, entry.stateHash);
            Packet_Entity_AnimatorParameterChange.AddParameter(pc.builder, (byte) entry.type);
            switch (entry.type)
            {
                case AnimatorControllerParameterType.Float:
                    Packet_Entity_AnimatorParameterChange.AddFloatValue(pc.builder, entry.floatValue);
                    break;

                case AnimatorControllerParameterType.Int:
                    Packet_Entity_AnimatorParameterChange.AddIntValue(pc.builder, entry.intValue);
                    break;

                case AnimatorControllerParameterType.Bool:
                    Packet_Entity_AnimatorParameterChange.AddBoolValue(pc.builder, entry.boolValue);
                    break;
            }
            pc.Finish<Packet_Entity_AnimatorParameterChange>(Packet_Entity_AnimatorParameterChange.EndPacket_Entity_AnimatorParameterChange(pc.builder));
            Singleton<MPManager>.Instance.SendReliableToOthers(base.runtimeID, pc);
        }

        public override void Core()
        {
            base.Core();
            if (base.isAuthority)
            {
                this.AuthorityCore();
            }
        }

        public override void OnAuthorityStart()
        {
            base.OnAuthorityStart();
            this._animatorEntity.onAnimatorStateChanged = (Action<AnimatorStateInfo, AnimatorStateInfo>) Delegate.Combine(this._animatorEntity.onAnimatorStateChanged, new Action<AnimatorStateInfo, AnimatorStateInfo>(this.AuthorityOnAnimatorStateChanged));
            this._animatorEntity.onUserInputControllerChanged = (Action<AnimatorParameterEntry>) Delegate.Combine(this._animatorEntity.onUserInputControllerChanged, new Action<AnimatorParameterEntry>(this.AuthorityUserInputControllerChanged));
            this._animatorEntity.onSteerFaceDirectionSet = (Action<Vector3>) Delegate.Combine(this._animatorEntity.onSteerFaceDirectionSet, new Action<Vector3>(this.AuthorityOnFaceDirectionSet));
            this._lastSentTime = Time.time;
            this._lastSentXZAngle = float.MinValue;
            this._lastSendXZ = MPMiscs.UNITINIALIZED;
            this._sendInterval = this._animatorEntity.animatorConfig.MPArguments.SyncSendInterval;
            string[] muteSyncAnimatorTags = this._animatorEntity.animatorConfig.MPArguments.MuteSyncAnimatorTags;
            this._muteSyncTagHashes = new int[muteSyncAnimatorTags.Length];
            for (int i = 0; i < muteSyncAnimatorTags.Length; i++)
            {
                this._muteSyncTagHashes[i] = Animator.StringToHash(muteSyncAnimatorTags[i]);
            }
        }

        protected virtual void OnRemoteEntityAnimatorParameterChanged(Packet_Entity_AnimatorParameterChange packet)
        {
            switch (packet.Parameter)
            {
                case 1:
                    this._animatorEntity.SetLocomotionFloat(packet.StateHash, packet.FloatValue, false);
                    break;

                case 3:
                    this._animatorEntity.SetLocomotionInteger(packet.StateHash, packet.IntValue, false);
                    break;

                case 4:
                    this._animatorEntity.SetLocomotionBool(packet.StateHash, packet.BoolValue, false);
                    break;
            }
        }

        protected virtual void OnRemoteEntityAnimatorStateChanged(Packet_Entity_AnimatorStateChange packet)
        {
            this._animatorEntity.SyncAnimatorState(packet.ToStateHash, packet.NormalizedTimeTo);
            this._animatorEntity.SteerFaceDirectionTo(MPMiscs.XZAnglesToForward(packet.StateSync.XzAngle));
        }

        protected override void OnRemoteReliablePacket(MPRecvPacketContainer pc)
        {
            base.OnRemoteReliablePacket(pc);
            if (pc.packet is Packet_Entity_AnimatorParameterChange)
            {
                this.OnRemoteEntityAnimatorParameterChanged(pc.As<Packet_Entity_AnimatorParameterChange>());
            }
        }

        public override void OnRemoteStart()
        {
            base.OnRemoteStart();
            this._animatorEntity.SetUseLocalController(false);
        }

        protected override void OnRemoteStateUpdate(MPRecvPacketContainer pc)
        {
            base.OnRemoteReliablePacket(pc);
            if (pc.packet is Packet_Entity_AnimatorStateChange)
            {
                this.OnRemoteEntityAnimatorStateChanged(pc.As<Packet_Entity_AnimatorStateChange>());
            }
            else if (pc.packet is Packet_Entity_TransformSync)
            {
                this.OnRemoteTransformSync(pc.As<Packet_Entity_TransformSync>());
            }
        }

        protected virtual void OnRemoteTransformSync(Packet_Entity_TransformSync packet)
        {
            this._animatorEntity.SteerFaceDirectionTo(MPMiscs.XZAnglesToForward(packet.StateSync.XzAngle));
            Vector3 targetPosition = MPMiscs.Convert(packet.StateSync.XzPosition);
            Vector3 vector2 = targetPosition - this._animatorEntity.XZPosition;
            if (vector2.sqrMagnitude > 8f)
            {
                this._animatorEntity.SyncPosition(targetPosition);
            }
        }
    }
}

