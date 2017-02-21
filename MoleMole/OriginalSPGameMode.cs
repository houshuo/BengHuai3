namespace MoleMole
{
    using MoleMole.Config;
    using System;
    using UnityEngine;

    public class OriginalSPGameMode : IGameMode
    {
        public virtual void DestroyRuntimeID(uint runtimeID)
        {
        }

        public virtual LayerMask GetAbilityHitboxTargettingMask(uint ownerID, MixinTargetting targetting)
        {
            ushort num;
            ushort enemyCategory;
            LayerMask hitboxLayerMask;
            if ((ownerID == 0x21800001) || (Singleton<RuntimeIDManager>.Instance.ParseCategory(ownerID) == 7))
            {
                num = 4;
                enemyCategory = 3;
            }
            else
            {
                num = Singleton<RuntimeIDManager>.Instance.ParseCategory(ownerID);
                enemyCategory = this.GetEnemyCategory(num);
            }
            if ((ownerID == 0x21800001) || (Singleton<RuntimeIDManager>.Instance.ParseCategory(ownerID) == 7))
            {
                num = 4;
                enemyCategory = 3;
            }
            switch (targetting)
            {
                case MixinTargetting.None:
                    return 0;

                case MixinTargetting.Allied:
                    return InLevelData.GetHitboxLayerMask(num);

                case MixinTargetting.Enemy:
                    hitboxLayerMask = InLevelData.GetHitboxLayerMask(enemyCategory);
                    if (num == 3)
                    {
                        hitboxLayerMask |= ((int) 1) << InLevelData.PROP_HITBOX_LAYER;
                    }
                    return hitboxLayerMask;

                case MixinTargetting.All:
                    hitboxLayerMask = InLevelData.GetHitboxLayerMask(num) | InLevelData.GetHitboxLayerMask(enemyCategory);
                    if (num == 3)
                    {
                        hitboxLayerMask |= ((int) 1) << InLevelData.PROP_HITBOX_LAYER;
                    }
                    return hitboxLayerMask;
            }
            return 0;
        }

        public virtual LayerMask GetAbilityTargettingMask(uint ownerID, MixinTargetting targetting)
        {
            ushort num;
            ushort enemyCategory;
            LayerMask layerMask;
            if ((ownerID == 0x21800001) || (Singleton<RuntimeIDManager>.Instance.ParseCategory(ownerID) == 7))
            {
                num = 4;
                enemyCategory = 3;
            }
            else
            {
                num = Singleton<RuntimeIDManager>.Instance.ParseCategory(ownerID);
                enemyCategory = this.GetEnemyCategory(num);
            }
            switch (targetting)
            {
                case MixinTargetting.None:
                    return 0;

                case MixinTargetting.Allied:
                    return InLevelData.GetLayerMask(num);

                case MixinTargetting.Enemy:
                    layerMask = InLevelData.GetLayerMask(enemyCategory);
                    if (num == 3)
                    {
                        layerMask |= ((int) 1) << InLevelData.PROP_LAYER;
                    }
                    return layerMask;

                case MixinTargetting.All:
                    layerMask = InLevelData.GetLayerMask(num) | InLevelData.GetLayerMask(enemyCategory);
                    if (num == 3)
                    {
                        layerMask |= ((int) 1) << InLevelData.PROP_LAYER;
                    }
                    return layerMask;
            }
            return 0;
        }

        public virtual T[] GetAlliedActorsOf<T>(BaseActor actor) where T: BaseActor
        {
            return Singleton<EventManager>.Instance.GetActorByCategory<T>(Singleton<RuntimeIDManager>.Instance.ParseCategory(actor.runtimeID));
        }

        public virtual LayerMask GetAttackPatternDefaultLayerMask(uint runtimeID)
        {
            switch (Singleton<RuntimeIDManager>.Instance.ParseCategory(runtimeID))
            {
                case 3:
                    return ((((int) 1) << InLevelData.MONSTER_HITBOX_LAYER) | (((int) 1) << InLevelData.PROP_HITBOX_LAYER));

                case 4:
                    return (((int) 1) << InLevelData.AVATAR_HITBOX_LAYER);
            }
            throw new Exception("Invalid Type or State!");
        }

        public virtual T[] GetEnemyActorsOf<T>(BaseActor actor) where T: BaseActor
        {
            return Singleton<EventManager>.Instance.GetActorByCategory<T>(this.GetEnemyCategory(Singleton<RuntimeIDManager>.Instance.ParseCategory(actor.runtimeID)));
        }

        private ushort GetEnemyCategory(ushort category)
        {
            ushort num = 0;
            if (category == 3)
            {
                return 4;
            }
            if (category == 4)
            {
                return 3;
            }
            if (category == 1)
            {
                num = 1;
            }
            return num;
        }

        public virtual void HandleLocalPlayerAvatarDie(BaseMonoAvatar diedAvatar)
        {
            BaseMonoAvatar firstAliveAvatar = Singleton<AvatarManager>.Instance.GetFirstAliveAvatar();
            if (firstAliveAvatar != null)
            {
                diedAvatar.gameObject.SetActive(false);
                Singleton<LevelManager>.Instance.levelActor.SwapLocalAvatar(diedAvatar.GetRuntimeID(), firstAliveAvatar.GetRuntimeID());
                if (Singleton<LevelManager>.Instance.levelActor.levelMode == LevelActor.Mode.Single)
                {
                    Singleton<LevelManager>.Instance.levelActor.abilityPlugin.AddOrGetAbilityAndTriggerOnTarget(AbilityData.GetAbilityConfig("Level_AvatarReviveInvincible"), firstAliveAvatar.GetRuntimeID(), 2f);
                }
            }
            else
            {
                Singleton<WwiseAudioManager>.Instance.Post("BGM_PauseMenu_On", null, null, null);
                if (Singleton<LevelScoreManager>.Instance.LevelType == 4)
                {
                    Singleton<LevelManager>.Instance.SetPause(false);
                    Singleton<EventManager>.Instance.FireEvent(new EvtLevelState(EvtLevelState.State.EndLose, EvtLevelState.LevelEndReason.EndLoseAllDead, 0), MPEventDispatchMode.Normal);
                    Singleton<CameraManager>.Instance.GetMainCamera().SetFailPostFX(true);
                }
                else if (Singleton<LevelDesignManager>.Instance.state == LevelDesignManager.LDState.Running)
                {
                    Singleton<LevelManager>.Instance.SetPause(true);
                    Singleton<MainUIManager>.Instance.ShowDialog(new InLevelReviveDialogContext(Singleton<AvatarManager>.Instance.GetTeamLeader().GetRuntimeID(), diedAvatar.XZPosition, true), UIType.Any);
                    Singleton<CameraManager>.Instance.GetMainCamera().SetFailPostFX(true);
                }
            }
        }

        public virtual bool IsEnemy(uint fromID, uint toID)
        {
            return true;
        }

        public virtual void RegisterRuntimeID(uint runtimeID)
        {
        }

        public virtual bool ShouldAttackPatternSendBeingHit(uint beHitEntityID)
        {
            return false;
        }
    }
}

