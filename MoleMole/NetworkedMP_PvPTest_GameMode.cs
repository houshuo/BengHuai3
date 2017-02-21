namespace MoleMole
{
    using MoleMole.Config;
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public class NetworkedMP_PvPTest_GameMode : NetworkedMP_Default_GameMode
    {
        private List<uint>[] _peerGroups = new List<uint>[7];

        public NetworkedMP_PvPTest_GameMode()
        {
            for (int i = 0; i < this._peerGroups.Length; i++)
            {
                this._peerGroups[i] = new List<uint>();
            }
        }

        public override void DestroyRuntimeID(uint runtimeID)
        {
            if (Singleton<RuntimeIDManager>.Instance.IsSyncedRuntimeID(runtimeID))
            {
                uint index = Singleton<RuntimeIDManager>.Instance.ParsePeerID(runtimeID);
                this._peerGroups[index].Remove(runtimeID);
            }
        }

        public override LayerMask GetAbilityHitboxTargettingMask(uint ownerID, MixinTargetting targetting)
        {
            switch (Singleton<RuntimeIDManager>.Instance.ParseCategory(ownerID))
            {
                case 3:
                    return (((((int) 1) << InLevelData.AVATAR_HITBOX_LAYER) | (((int) 1) << InLevelData.MONSTER_HITBOX_LAYER)) | (((int) 1) << InLevelData.PROP_HITBOX_LAYER));

                case 4:
                    return ((((int) 1) << InLevelData.AVATAR_HITBOX_LAYER) | (((int) 1) << InLevelData.MONSTER_HITBOX_LAYER));

                case 7:
                    return ((((int) 1) << InLevelData.AVATAR_HITBOX_LAYER) | (((int) 1) << InLevelData.MONSTER_HITBOX_LAYER));
            }
            return 0;
        }

        public override LayerMask GetAbilityTargettingMask(uint ownerID, MixinTargetting targetting)
        {
            switch (Singleton<RuntimeIDManager>.Instance.ParseCategory(ownerID))
            {
                case 3:
                    return (((((int) 1) << InLevelData.AVATAR_LAYER) | (((int) 1) << InLevelData.MONSTER_LAYER)) | (((int) 1) << InLevelData.PROP_LAYER));

                case 4:
                    return ((((int) 1) << InLevelData.AVATAR_LAYER) | (((int) 1) << InLevelData.MONSTER_LAYER));

                case 7:
                    return ((((int) 1) << InLevelData.AVATAR_LAYER) | (((int) 1) << InLevelData.MONSTER_LAYER));
            }
            return 0;
        }

        public override T[] GetAlliedActorsOf<T>(BaseActor actor) where T: BaseActor
        {
            uint index = Singleton<RuntimeIDManager>.Instance.ParsePeerID(actor.runtimeID);
            List<T> list = new List<T>();
            List<uint> list2 = this._peerGroups[index];
            for (int i = 0; i < list2.Count; i++)
            {
                T item = Singleton<EventManager>.Instance.GetActor<T>(list2[i]);
                if (item != null)
                {
                    list.Add(item);
                }
            }
            return list.ToArray();
        }

        public override LayerMask GetAttackPatternDefaultLayerMask(uint runtimeID)
        {
            return (((((int) 1) << InLevelData.MONSTER_HITBOX_LAYER) | (((int) 1) << InLevelData.AVATAR_HITBOX_LAYER)) | (((int) 1) << InLevelData.PROP_HITBOX_LAYER));
        }

        public override T[] GetEnemyActorsOf<T>(BaseActor actor) where T: BaseActor
        {
            uint num = Singleton<RuntimeIDManager>.Instance.ParsePeerID(actor.runtimeID);
            List<T> list = new List<T>();
            for (int i = 1; i < this._peerGroups.Length; i++)
            {
                if (i != num)
                {
                    List<uint> list2 = this._peerGroups[i];
                    for (int j = 0; j < list2.Count; j++)
                    {
                        for (int k = 0; k < list2.Count; k++)
                        {
                            T item = Singleton<EventManager>.Instance.GetActor<T>(list2[k]);
                            if (item != null)
                            {
                                list.Add(item);
                            }
                        }
                    }
                }
            }
            return list.ToArray();
        }

        public override bool IsEnemy(uint fromID, uint toID)
        {
            if (!Singleton<RuntimeIDManager>.Instance.IsSyncedRuntimeID(fromID))
            {
                BaseActor actor = Singleton<EventManager>.Instance.GetActor(fromID);
                if (actor == null)
                {
                    return false;
                }
                fromID = actor.ownerID;
            }
            if (!Singleton<RuntimeIDManager>.Instance.IsSyncedRuntimeID(toID))
            {
                BaseActor actor2 = Singleton<EventManager>.Instance.GetActor(toID);
                if (actor2 == null)
                {
                    return false;
                }
                toID = actor2.ownerID;
            }
            return (Singleton<RuntimeIDManager>.Instance.ParsePeerID(fromID) != Singleton<RuntimeIDManager>.Instance.ParsePeerID(toID));
        }

        public override void RegisterRuntimeID(uint runtimeID)
        {
            if (Singleton<RuntimeIDManager>.Instance.IsSyncedRuntimeID(runtimeID))
            {
                uint index = Singleton<RuntimeIDManager>.Instance.ParsePeerID(runtimeID);
                this._peerGroups[index].Add(runtimeID);
            }
        }
    }
}

