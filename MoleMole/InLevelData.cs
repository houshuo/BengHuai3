namespace MoleMole
{
    using MoleMole.Config;
    using System;
    using UnityEngine;

    public static class InLevelData
    {
        private static bool _inLevelDataInited = false;
        public static int AVATAR_HITBOX_LAYER;
        public static int AVATAR_LAYER;
        public static int BODY_TRIGGER_LAYER;
        public static int CAMERA_COLLIDER_LAYER;
        public static readonly Vector3 CREATE_INIT_FORWARD = Vector3.forward;
        public static readonly Vector3 CREATE_INIT_POS = new Vector3(0f, -100f, 0f);
        public const float CREATE_REPICK_ADD_RADIUS = 0.5f;
        public const int CREATE_REPICK_MAX = 20;
        public const float ENTITY_RAYCAST_HEIGHT = 1.1f;
        public const float ENTITY_RIGIDBODY_MASS_CAP = 200f;
        public const float FIXED_TIME_STEP = 0.02f;
        public const int FRAMEHALT_TIMESCALE_IX = 5;
        public static int HITBOX_TRIGGER_LAYER;
        public const string IN_LEVEL_UI_CANVAS_PATH = "UI/InLevelCanvas";
        public static int INACTIVE_ENTITY_LAYER;
        public static ConfigInLevelMiscData InLevelMiscData;
        public static float MIN_COLLIDER_RADIUS = 0.3f;
        public static int MONSTER_HITBOX_LAYER;
        public static int MONSTER_LAYER;
        public const float MOVE_SPEED_ANIMATOR_SCALE_DOWN_RATIO = 0.35f;
        public static readonly Vector3[] MUTIL_MODE_AVATAR_INIT_POS_LIST = new Vector3[] { Vector3.zero, new Vector3(-1f, 0f, -1f), new Vector3(1f, 0f, -1f) };
        public static readonly Vector3[] MUTIL_REMOTE_MODE_AVATAR_INIT_POS_LIST = new Vector3[] { Vector3.zero, Vector3.zero, Vector3.zero };
        public static int NORMALIZED_TIME_NAME_PARAM;
        public static int OBSTACLE_COLLIDER_LAYER;
        public const float OUT_OF_STAGE_RANGE = 100f;
        public const int PARALYZE_TIMESCALE_IX = 3;
        public const int PAUSE_TIMESCALE_IX = 6;
        public static int PERPSTAGE_LAYER;
        public const uint POLE_CONT_CAMERA_TYPE = 1;
        public static int PROJECTOR_LAYER;
        public static int PROP_HITBOX_LAYER;
        public static int PROP_LAYER;
        public static int SHADER_EMISSION;
        public static int SHADER_FALLOFF;
        public static int SHADER_TINTCOLOR;
        public static readonly Vector3[] SINGLE_MODE_AVATAR_INIT_POS_LIST = new Vector3[] { Vector3.zero, CREATE_INIT_POS, CREATE_INIT_POS };
        public static int STAGE_COLLIDER_LAYER;
        public const int STAGE_ORTH_Y_OFFSET = -100;
        public const int STOPWORLD_TIMESCALE_IX = 2;
        public const float TIME_NORMAL_RATIO = 1f;
        public const float TIME_SLOW_RATIO = 0.05f;
        public const int TIMESCALE_STACK_CAPACITY = 8;
        public const int TUTORIAL_TIMESCALE_IX = 7;
        public const int WITCHTIME_TIMESCALE_IX = 1;

        public static LayerMask GetHitboxLayerMask(ushort category)
        {
            switch (category)
            {
                case 3:
                    return (((int) 1) << AVATAR_HITBOX_LAYER);

                case 4:
                    return (((int) 1) << MONSTER_HITBOX_LAYER);

                case 7:
                    return (((int) 1) << PROP_HITBOX_LAYER);
            }
            return 0;
        }

        public static LayerMask GetLayerMask(ushort category)
        {
            switch (category)
            {
                case 3:
                    return (((int) 1) << AVATAR_LAYER);

                case 4:
                    return (((int) 1) << MONSTER_LAYER);

                case 7:
                    return (((int) 1) << PROP_LAYER);
            }
            return 0;
        }

        public static void InitInLevelData()
        {
            if (!_inLevelDataInited)
            {
                AVATAR_LAYER = LayerMask.NameToLayer("Avatar");
                AVATAR_HITBOX_LAYER = LayerMask.NameToLayer("AvatarHitbox");
                MONSTER_LAYER = LayerMask.NameToLayer("Monster");
                MONSTER_HITBOX_LAYER = LayerMask.NameToLayer("MonsterHitbox");
                STAGE_COLLIDER_LAYER = LayerMask.NameToLayer("StageCollider");
                OBSTACLE_COLLIDER_LAYER = LayerMask.NameToLayer("ObstacleCollider");
                BODY_TRIGGER_LAYER = LayerMask.NameToLayer("BodyTrigger");
                HITBOX_TRIGGER_LAYER = LayerMask.NameToLayer("HitboxTrigger");
                PERPSTAGE_LAYER = LayerMask.NameToLayer("PerpStage");
                PROJECTOR_LAYER = LayerMask.NameToLayer("Projector");
                CAMERA_COLLIDER_LAYER = LayerMask.NameToLayer("CameraCollider");
                INACTIVE_ENTITY_LAYER = LayerMask.NameToLayer("InactiveEntity");
                PROP_LAYER = LayerMask.NameToLayer("Prop");
                PROP_HITBOX_LAYER = LayerMask.NameToLayer("PropHitbox");
                NORMALIZED_TIME_NAME_PARAM = Animator.StringToHash("NormalizedTime");
                SHADER_EMISSION = Shader.PropertyToID("_Emission");
                SHADER_TINTCOLOR = Shader.PropertyToID("_TintColor");
                SHADER_FALLOFF = Shader.PropertyToID("_Falloff");
                _inLevelDataInited = true;
            }
        }

        public static bool IsOutOfStage(Vector3 vec)
        {
            return ((((vec.x < -100f) || (vec.x > 100f)) || (vec.z < -100f)) || (vec.z > 100f));
        }

        public static void ReloadFromFile()
        {
            InLevelMiscData = ConfigUtil.LoadJSONConfig<ConfigInLevelMiscData>(GlobalDataManager.metaConfig.inLevelMiscData);
        }
    }
}

