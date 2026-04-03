using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TBM.MaouSuika.Gameplay.Player;
using UnityEngine;

namespace TBM.MaouSuika.Gameplay.Puzzle
{
    public class PuzzleContext
    {
        [ShowInInspector, ReadOnly] public PlayerContext PlayerContext { get; private set; }
        [ShowInInspector, ReadOnly] public PuzzleArea Area { get; private set; }
        [ShowInInspector, ReadOnly] public SuikaTierDataTable TierDataTable { get; private set; }

        private PuzzleContext()
        {
        }

        public class Builder
        {
            private PlayerContext _playerContext;
            private SuikaTierConfigSO _tierConfig;
            private PuzzleArea _area;

            public Builder()
            {
            }

            public Builder SetTierConfig(SuikaTierConfigSO config)
            {
                _tierConfig = config;
                return this;
            }

            public Builder SetPlayerContext(PlayerContext context)
            {
                _playerContext = context;
                return this;
            }

            public Builder SetArea(PuzzleArea area)
            {
                _area = area;
                return this;
            }

            public PuzzleContext Build()
            {
                if (_playerContext == null) throw new InvalidOperationException($"{nameof(_playerContext)} is null.");
                if (_tierConfig == null) throw new InvalidOperationException($"{nameof(_tierConfig)} is null.");
                if (_area == null) throw new InvalidOperationException($"{nameof(_area)} is null.");

                var context = new PuzzleContext
                {
                    PlayerContext = _playerContext,
                    Area = _area,
                    TierDataTable =
                        new SuikaTierDataTable(SuikaTierDataBuilder.Build(_tierConfig, _playerContext.MonsterLoadout))
                };

                return context;
            }
        }
    }
}