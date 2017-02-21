namespace MoleMole
{
    using System;

    public static class AvatarInputControllerData
    {
        public const string ATTACK_BTN_DOWN_PREFAB_PATH = "Levels/InLevelUI/FuncBtnDown";
        public const string ATTACK_BTN_UP_PREFAB_PATH = "Levels/InLevelUI/FuncBtnUp";
        public const int ATTACK_BUTTON_ID = 1;
        public const int ATTACK_BUTTON_RADIUS = 0x54;
        public const float IP5_IP6_RATIO = 1.125f;
        public const string JOYSTICK_DOWN_PREFAB_PATH = "Levels/InLevelUI/JoyStickDown";
        public const string JOYSTICK_UP_PREFAB_PATH = "Levels/InLevelUI/JoyStickUp";
        public const uint POLE_KEY_JOY_MODE = 1;
        public const int SCREEN_DEFAULT_END_X = 0x1000;
        public const int SCREEN_DEFAULT_START_X = 0;
        public const int SCREEN_DEFAULT_START_Y = 0;
        public const int SKILL_BUTTON_1_ID = 2;
        public const int SKILL_BUTTON_2_ID = 3;
        public const int STICK_BUTTON_DOWN_RADIUS = 320;
        public const int STICK_BUTTON_LEAVE_RADIUS = 450;

        public static uint GetControlTypeByCamearAndControlType(uint contType)
        {
            if (contType != 1)
            {
                throw new Exception("Invalid Type or State!");
            }
            return 1;
        }
    }
}

