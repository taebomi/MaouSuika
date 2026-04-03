using UnityEngine;

namespace TBM.MaouSuika.Gameplay.Puzzle
{
    public class ShooterAimModule : MonoBehaviour
    {
        [SerializeField] private AimStyleSO aimStyle;
        [SerializeField] private AimVisualizer visualizer;

        private bool IsAiming { get; set; }
        private AimData _lastValidAim;
        private AimData _lastFiredAim;

        public AimData AutoFireAim => IsAiming ? _lastValidAim : _lastFiredAim;

        public void Setup()
        {
            IsAiming = false;
            _lastValidAim = AimData.Default;
            _lastFiredAim = AimData.Default;
            visualizer.HideAim();
        }

        public void HandleCommand(ShooterInputCommand command, ShooterState state)
        {
            switch (command.Type)
            {
                case ShooterInputCommand.CommandType.None:
                    Logger.Warning($"Invalid command: {command.Type}");
                    break;
                case ShooterInputCommand.CommandType.Idle:
                    CancelAim();
                    break;
                case ShooterInputCommand.CommandType.Aim:
                    ProcessAim(command.AimData, state);
                    break;
                case ShooterInputCommand.CommandType.Fire:
                    ProcessAim(command.AimData, state);
                    RecordFiredAim(command.AimData);
                    break;
                default:
                    throw new System.NotImplementedException();
            }
        }

        private void CancelAim()
        {
            IsAiming = false;
            visualizer.HideAim();
        }

        private void ProcessAim(AimData aimData, ShooterState state)
        {
            IsAiming = true;
            _lastValidAim = aimData;

            var visualData = aimStyle.Evaluate(_lastValidAim, state);
            visualizer.ShowAim(_lastValidAim.Direction, visualData);
        }

        private void RecordFiredAim(AimData aimData)
        {
            _lastFiredAim = aimData;
        }
    }
}