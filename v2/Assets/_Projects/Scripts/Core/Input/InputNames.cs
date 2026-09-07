namespace MaouSuika.Core
{
    public static class InputNames
    {
        public static class ControlSchemes
        {
            public const string KEYBOARD_MOUSE = "KeyboardMouse";
            public const string GAMEPAD = "Gamepad";
        }

        public static class ActionMaps
        {
            public const string COMMON = "Common";
            public const string PUZZLE = "Puzzle";
            public const string SKILL = "Skill";
            public const string UI = "UI";
        }

        public static class CommonActions
        {
            public const string PAUSE = "Pause";
        }

        public static class PuzzleActions
        {
            public const string FIRE = "Fire";
            public const string AIM = "Aim";
            public const string USE_SKILL = "UseSkill";
        }

        public static class SkillActions
        {
            public const string POINT = "Point";
            public const string NAVIGATE = "Navigate";
            public const string CONFIRM = "Confirm";
            public const string CANCEL = "Cancel";
        }
    }
}