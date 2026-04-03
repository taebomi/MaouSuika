using System;
using UnityEngine;

namespace SOSG.Stage.Suika.Shooter
{
    public class SuikaShooterAim : MonoBehaviour
    {
        private const float AimSpriteYSize = 2f;
        private const float AimSpriteMinXSize = 1.5f; // 스프라이트 슬라이스 최소 길이
        private const float AimSpriteMaxXSize = 5f; // 벽까지 길이

        [SerializeField] private SpriteRenderer sr;

        [SerializeField] private Gradient aimGradient;

        private SuikaShooter.State _state;

        public Vector2 LastDir { get; private set; }
        public float LastRatio { get; private set; }

        public void SetUp()
        {
            LastDir = Vector2.up;
            LastRatio = 0.5f;
        }

        public void SetActive(bool value)
        {
            sr.gameObject.SetActive(value);
        }

        public void SetState(SuikaShooter.State state)
        {
            _state = state;
        }

        public void UpdateDirection(Vector2 dir, float ratio)
        {
            transform.right = dir;
            LastDir = dir;
            LastRatio = ratio;

            var length = AimSpriteMinXSize + (AimSpriteMaxXSize - AimSpriteMinXSize) * ratio;
            sr.size = new Vector2(length, AimSpriteYSize);

            switch (_state)
            {
                case SuikaShooter.State.None:
                    sr.color = aimGradient.Evaluate(ratio);
                    break;
                case SuikaShooter.State.Cooldown:
                    var color = aimGradient.Evaluate(ratio);
                    color.a = SuikaShooter.CooldownAlpha;
                    sr.color = color;
                    break;
                case SuikaShooter.State.Collided:
                    sr.color = SuikaShooter.CollidedColor;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}