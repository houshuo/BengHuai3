namespace MoleMole
{
    using System;

    public static class MainCameraData
    {
        public const float BACK_FOLLOW_SMOOTH_RATIO = 4f;
        public static readonly float CAMERA_DEFAULT_ELEVATION_DEGREE = (-CAMERA_DEFAULT_TO_GROUND_RADIAN * 57.29578f);
        public static readonly float CAMERA_DEFAULT_TO_GROUND_RADIAN = -Mathf.Atan(0.02339042f);
        public const float CAMERA_FAR_LOCATE_DISTANCE = 7f;
        public const float CAMERA_FAR_LOCATE_ElEVATION = 7f;
        public const float CAMERA_FURTHER_LOCATE_DISTANCE = 8.5f;
        public const float CAMERA_FURTHER_LOCATE_ElEVATION = 7f;
        public const float CAMERA_HIGH_LOCATE_ELEVATION = 10f;
        public const float CAMERA_HIGHER_LOCATE_ELEVATION = 15f;
        public const float CAMERA_LOCATE_DISTANCE = 6f;
        public const float CAMERA_LOCATE_ElEVATION = 3.5f;
        public const float CAMERA_LOCATE_RATIO = 0.535f;
        public const float CAMERA_LOCATE_RATIO_BOSS_STATE = 0.735f;
        public const float CAMERA_LOCATE_Y_OFFSET = 2.383333f;
        public const float CAMERA_LOOK_AT_CHAR_Y_OFFSET = 1.2f;
        public const float CAPSULE_RADIUS_REMAIN_RATIO = 0.8f;
        public const float CLOSE_FOLLOW_LERP_RATIO = 8f;
        public const float DIRECTIONAL_LIGHT_EULER_X_ROTATION = 45f;
        public const float DIRECTIONAL_LIGHT_FORWARD_LERP_RATIO = 1f;
        public const float FADING_RANGE = 1f;
        public const float FOLLOW_AVATAR_ENTERING_LERP_TIME_RATIO = 0.1f;
        public const float FOLLOW_CAMERA_MIN_NEAR_CLIP = 0.01f;
        public const float FOLLOW_CAMERA_WALL_ELEVATION_RATIO = 0.05f;
        public const float FOLLOW_CAMERA_WALL_EXTEND_POS_RATIO = 0.1f;
        public const float FOLLOW_CAMERA_WALL_LIFT_RATIO = 0.1f;
        public const float JUMP_LERP_Z_AVATAR_ROOT_HEIGHT_THRESHOLD_RATIO = 0.5f;
        public const float JUMP_LERP_Z_MULTI_RATIO = 1f;
        public const float LERP_CENTER_DELTA_MAX = 12f;
        public const float LERP_CENTER_DELTA_MIN = 5f;
        public const float LERP_RATIO_ANGLE = 5f;
        public const float LERP_RATIO_POS = 7.9f;
        public const float LERP_TIME_PROPORTION_TO_FOV = 0.1f;
        public const float LERP_TIME_PROPORTION_TO_LENGTH = 0.35f;
        public const float MAX_ANGLE_FOR_SAME_DIRECTION = 10f;
        public const float MAX_ZOOM_RADIUS = 11f;
        public const float MIN_ZOOM_RADIUS = 4f;
        public const float OUTLINE_MAX_RANGE = 3f;
        public const float OUTLINE_MIN_RANGE = 0.75f;
        public const uint POLE_VECTOR_MODE = 1;
        public const float ROTATE_TO_FACE_MAX_ANGLE_ON_UNSTABLE_ABS = 90f;
        public const float ROTATE_TO_FACE_MIN_ANGLE_ABS = 10f;
        public const float ROTATION_ANGLE_FOR_OVERLAPPING = 10f;
        public const float SHAKE_ANGLE_DIRECTED_RATIO = 0.8f;
        public const int SHAKE_DEFAULT_STEP_FRAME = 1;
        public const float SHAKE_NOT_DIRECT_SCALE_DOWN_RATIO = 0.7f;
        public const float SHAKE_ORTH_SHAKE_RATIO = 0.15f;
        public const float SMOOTH_FOLLOW_NEGATIVE_ONE_FRAME_WEIGHT = 0.3f;
        public const float SMOOTH_FOLLOW_NEGATIVE_TWO_FRAME_WEIGHT = 0.2f;
        public const float STABLE_ENTER_TIME = 3f;
        public const float STABLE_LERP_LOOK_AT_Y_MULTI_RATIO = 0.85f;
        public const float STABLE_LERP_SLOW_RATIO = 0.1f;
        public const float STABLE_LERP_Y_MULTI_RATIO = 0.85f;
        public const float STABLE_LERP_Z_MULTI_RATIO = 0.64f;
        public const float USE_HANDLE_OVERLAP_OF_AVATAR_AND_ATTACKTARGET_RATIO = 0.6f;

        public static uint GetCameraTypeByCamearAndControlType(uint contType)
        {
            if (contType != 1)
            {
                throw new Exception("Invalid Type or State!");
            }
            return 1;
        }
    }
}

