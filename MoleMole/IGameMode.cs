namespace MoleMole
{
    using MoleMole.Config;
    using System;
    using UnityEngine;

    public interface IGameMode
    {
        void DestroyRuntimeID(uint runtimeID);
        LayerMask GetAbilityHitboxTargettingMask(uint ownerID, MixinTargetting targetting);
        LayerMask GetAbilityTargettingMask(uint ownerID, MixinTargetting targetting);
        T[] GetAlliedActorsOf<T>(BaseActor actor) where T: BaseActor;
        LayerMask GetAttackPatternDefaultLayerMask(uint runtimeID);
        T[] GetEnemyActorsOf<T>(BaseActor actor) where T: BaseActor;
        void HandleLocalPlayerAvatarDie(BaseMonoAvatar diedAvatar);
        bool IsEnemy(uint fromID, uint toID);
        void RegisterRuntimeID(uint runtimeID);
        bool ShouldAttackPatternSendBeingHit(uint beHitEntityID);
    }
}

