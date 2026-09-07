using System;
using MaouSuika.Core;
using UnityEngine;

namespace MaouSuika.Gameplay
{
    [CreateAssetMenu(fileName = "MonsterSpriteAnimationPreset",
        menuName = MenuPath.Gameplay.Unit.MONSTER + "Sprite Animation Preset")]
    public class MonsterSpriteAnimationPresetSO : ScriptableObject
    {
        [Serializable]
        public class StatePreset
        {
            [Min(1)]
            [SerializeField] private int fps;
            [SerializeField] private bool loop;

            public int Fps => fps;
            public bool Loop => loop;

            public StatePreset(int fps, bool loop)
            {
                this.fps = fps;
                this.loop = loop;
            }
        }

        [SerializeField] private StatePreset idle = new(5, true);
        [SerializeField] private StatePreset move = new(5, true);
        [SerializeField] private StatePreset attack = new(10, false);
        [SerializeField] private StatePreset hit = new(10, true);
        [SerializeField] private StatePreset die = new(10, false);

        public StatePreset Get(MonsterAnimationState state)
        {
            return state switch
            {
                MonsterAnimationState.Idle => idle,
                MonsterAnimationState.Move => move,
                MonsterAnimationState.Attack => attack,
                MonsterAnimationState.Hit => hit,
                MonsterAnimationState.Die => die,
                _ => throw new ArgumentOutOfRangeException(nameof(state), state, null),
            };
        }
    }
}
