using System;
using TBM.MaouSuika.Core.Input;
using TBM.MaouSuika.Gameplay.Puzzle;

namespace TBM.MaouSuika.Core.Settings
{
    [Serializable]
    public class InputProfile
    {
        public KeyboardMouseSettings keyboardMouse;
        public GamepadSettings gamepad;
        public TouchscreenSettings touchscreen;

        public InputProfile()
        {
            keyboardMouse = new KeyboardMouseSettings();
            gamepad = new GamepadSettings();
            touchscreen = new TouchscreenSettings();
        }

        public InputProfile(InputProfile profile)
        {
            keyboardMouse = new KeyboardMouseSettings(profile.keyboardMouse);
            gamepad = new GamepadSettings(profile.gamepad);
            touchscreen = new TouchscreenSettings(profile.touchscreen);
        }

        public DragAimConfig GetDragConfig(string scheme)
        {
            return scheme switch
            {
                InputConstants.ControlScheme.TOUCHSCREEN => touchscreen.dragAimConfig,
                _ => keyboardMouse.dragAimConfig,
            };
        }

        public ClassicAimConfig GetMoveConfig(string scheme)
        {
            return scheme switch
            {
                InputConstants.ControlScheme.GAMEPAD => gamepad.classicAimConfig,
                _ => keyboardMouse.classicAimConfig,
            };
        }

        public DirectAimConfig GetDirectAimConfig(string scheme)
        {
            return gamepad.directAimConfig;
        }

        public float GetDeadZone(string scheme)
        {
            return scheme == InputConstants.ControlScheme.GAMEPAD ? gamepad.deadZone : 0f;
        }

        public VirtualCursorAimConfig GetVirtualCursorAimConfig(string scheme)
        {
            return scheme switch
            {
                InputConstants.ControlScheme.GAMEPAD => gamepad.virtualCursorAimConfig,
                _ => keyboardMouse.virtualCursorAimConfig,
            };
        }

        public ShooterInputType GetShooterInputType(string scheme)
        {
            return scheme switch
            {
                InputConstants.ControlScheme.KEYBOARD_MOUSE => GetKeyboardMouseInputType(),
                InputConstants.ControlScheme.MOUSE => ShooterInputType.Drag,
                InputConstants.ControlScheme.KEYBOARD => ShooterInputType.Classic,
                InputConstants.ControlScheme.GAMEPAD => GetGamepadInputType(),
                InputConstants.ControlScheme.TOUCHSCREEN => ShooterInputType.Drag,
                _ => GetKeyboardMouseInputType(),
            };
        }

        private ShooterInputType GetKeyboardMouseInputType()
        {
            return keyboardMouse.aimMode switch
            {
                KeyboardMouseSettings.AimMode.MouseDrag => ShooterInputType.Drag,
                KeyboardMouseSettings.AimMode.KeyboardMove => ShooterInputType.Classic,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private ShooterInputType GetGamepadInputType()
        {
            return gamepad.aimMode switch
            {
                GamepadSettings.AimMode.Direct => ShooterInputType.Direct,
                GamepadSettings.AimMode.Move => ShooterInputType.Classic,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        [Serializable, ES3Serializable]
        public class KeyboardMouseSettings
        {
            public enum AimMode
            {
                MouseDrag,
                KeyboardMove,
            }

            public AimMode aimMode;
            public ClassicAimConfig classicAimConfig;
            public DragAimConfig dragAimConfig;
            public VirtualCursorAimConfig virtualCursorAimConfig;

            public KeyboardMouseSettings()
            {
                aimMode = AimMode.MouseDrag;
                classicAimConfig = ClassicAimConfig.Default;
                dragAimConfig = DragAimConfig.Default;
                virtualCursorAimConfig = VirtualCursorAimConfig.Default;
            }

            public KeyboardMouseSettings(KeyboardMouseSettings other)
            {
                aimMode = other.aimMode;
                classicAimConfig = other.classicAimConfig;
                dragAimConfig = other.dragAimConfig;
                virtualCursorAimConfig = other.virtualCursorAimConfig;
            }
        }

        [Serializable, ES3Serializable]
        public class GamepadSettings
        {
            public enum AimMode
            {
                Direct,
                Move,
            }

            public AimMode aimMode;
            public float deadZone;
            public ClassicAimConfig classicAimConfig;
            public DirectAimConfig directAimConfig;
            public VirtualCursorAimConfig virtualCursorAimConfig;

            public GamepadSettings()
            {
                aimMode = AimMode.Direct;
                deadZone = 0.05f;
                classicAimConfig = ClassicAimConfig.Default;
                directAimConfig = DirectAimConfig.Default;
                virtualCursorAimConfig = VirtualCursorAimConfig.Default;
            }

            public GamepadSettings(GamepadSettings settings)
            {
                aimMode = settings.aimMode;
                deadZone = settings.deadZone;
                classicAimConfig = settings.classicAimConfig;
                directAimConfig = settings.directAimConfig;
                virtualCursorAimConfig = settings.virtualCursorAimConfig;
            }
        }

        [Serializable, ES3Serializable]
        public class TouchscreenSettings
        {
            public DragAimConfig dragAimConfig;

            public TouchscreenSettings()
            {
                dragAimConfig = DragAimConfig.Default;
            }

            public TouchscreenSettings(TouchscreenSettings settings)
            {
                dragAimConfig = settings.dragAimConfig;
            }
        }
    }
}