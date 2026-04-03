using System;
using System.Collections.Generic;
using TBM.MaouSuika.Core.Input;
using TBM.MaouSuika.Core.Settings;

namespace TBM.MaouSuika.Gameplay.Puzzle
{
    public class ShooterInputModule : IDisposable
    {
        private readonly InputController _inputController;
        private readonly Dictionary<ShooterInputType, IShooterInputStrategy> _strategies;

        private IShooterInputStrategy _curStrategy;

        public ShooterInputModule(InputController inputController, PuzzleArea area)
        {
            if (inputController == null) throw new ArgumentNullException(nameof(inputController));
            if (area == null) throw new ArgumentNullException(nameof(area));

            _inputController = inputController;
            _strategies = new Dictionary<ShooterInputType, IShooterInputStrategy>()
            {
                { ShooterInputType.None, new ShooterNoneStrategy() },
                { ShooterInputType.Drag, new ShooterDragStrategy(area) },
                { ShooterInputType.Classic, new ShooterClassicStrategy() },
                { ShooterInputType.Direct, new ShooterDirectStrategy() },
                { ShooterInputType.VirtualCursor, new ShooterVirtualCursorStrategy() },
            };
            _curStrategy = _strategies[ShooterInputType.None];

            UpdateConfigs(_inputController.Profile);
            _inputController.ProfileChanged += UpdateConfigs;
        }

        public void Dispose()
        {
            if (_inputController != null) _inputController.ProfileChanged -= UpdateConfigs;
        }

        public void Setup()
        {
            SwitchStrategy(ShooterInputType.None);
        }

        public ShooterInputCommand Process(ShooterInputResult input, float deltaTime)
        {
            SwitchStrategy(input.InputType);
            return _curStrategy.Process(input, deltaTime);
        }

        private void SwitchStrategy(ShooterInputType type)
        {
            if (_curStrategy.InputType == type) return;

            _curStrategy.Exit();
            _curStrategy = _strategies[type];
            _curStrategy.Enter();
        }

        private void UpdateConfigs(InputProfile profile)
        {
            foreach (var strategy in _strategies.Values)
            {
                strategy.UpdateConfig(profile, _inputController.Scheme);
            }
        }
    }
}