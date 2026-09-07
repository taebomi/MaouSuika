using System;
using MaouSuika.Core;
using UnityEngine;

namespace MaouSuika.Gameplay
{
    [Serializable]
    public class InputProfile
    {
        public ShooterControlSchemeSettings keyboardMouse;
        public ShooterControlSchemeSettings gamepad;

        public static InputProfile CreateDefault()
        {
            return new InputProfile
            {
                keyboardMouse = CreateKeyboardMouseDefault(),
                gamepad = CreateGamepadDefault(),
            };
        }

        public void ResetToDefault()
        {
            var defaults = CreateDefault();
            keyboardMouse.CopyFrom(defaults.keyboardMouse);
            gamepad.CopyFrom(defaults.gamepad);
        }

        public ShooterControlSchemeSettings GetSettings(string controlScheme)
        {
            return controlScheme switch
            {
                InputNames.ControlSchemes.KEYBOARD_MOUSE => keyboardMouse,
                InputNames.ControlSchemes.GAMEPAD => gamepad,
                _ => throw new NotSupportedException($"{controlScheme} is not supported."),
            };
        }


        public void Normalize()
        {
            var defaults = CreateDefault();

            keyboardMouse ??= defaults.keyboardMouse;
            gamepad ??= defaults.gamepad;

            keyboardMouse.Normalize(defaults.keyboardMouse);
            gamepad.Normalize(defaults.gamepad);
        }

        public InputProfile Copy()
        {
            var copy = CreateDefault();
            copy.CopyFrom(this);
            return copy;
        }

        public void CopyFrom(InputProfile source)
        {
            keyboardMouse.CopyFrom(source.keyboardMouse);
            gamepad.CopyFrom(source.gamepad);
        }

        private static ShooterControlSchemeSettings CreateKeyboardMouseDefault()
        {
            return new ShooterControlSchemeSettings()
            {
                controlMode = ShooterControlMode.Drag,
                direct = new ShooterDirectInputSettings(false),
                drag = new ShooterDragInputSettings(dragRange: 3f, isSlingshot: false)
            };
        }

        private static ShooterControlSchemeSettings CreateGamepadDefault()
        {
            return new ShooterControlSchemeSettings()
            {
                controlMode = ShooterControlMode.Direct,
                direct = new ShooterDirectInputSettings(false),
                drag = new ShooterDragInputSettings(dragRange: 3f, isSlingshot: false)
            };
        }
    }
}