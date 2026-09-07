using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using MaouSuika.Core;
using R3;
using UnityEngine;

namespace MaouSuika.Gameplay
{
    public class SoulOrbRoot : MonoBehaviour
    {
        private const double POP_INTERVAL = 0.1;

        [SerializeField] private SoulOrbSpawner spawner;
        [SerializeField] private SoulOrbMerger merger;
        [SerializeField] private SoulOrbPopEffectPlayer popEffectPlayer;

        private MonsterLoadout _monsterLoadout;

        private readonly Subject<SoulOrb> _gameOverOrbPopped = new();

        public ISoulOrbSpawner Spawner => spawner;
        public IReadOnlyList<SoulOrb> ActiveOrbs => spawner.ActiveOrbs;

        public Observable<SoulOrbMergeResult> MergeFinished => merger.MergeFinished;
        public Observable<SoulOrb> GameOverOrbPopped => _gameOverOrbPopped;


        public void Initialize(
            MonsterLoadout loadout,
            Action<CameraShakeRequest> requestCameraShake,
            Action<HapticRequest> requestHaptics)
        {
            _monsterLoadout = loadout;

            merger.Initialize(spawner);
            spawner.Initialize(loadout, merger.TryMerge);
            popEffectPlayer.Initialize(requestCameraShake, requestHaptics);
        }

        public void Setup()
        {
            spawner.Setup();
            merger.Setup();
        }

        public void TickPlaying(float deltaTime)
        {
            TickOrbs(deltaTime);
        }

        public void TickGameOver(float deltaTime)
        {
            TickOrbs(deltaTime);
        }

        public void Stop()
        {
            merger.Stop();
        }

        private void OnDestroy()
        {
            _gameOverOrbPopped.Dispose();
        }

        public void PlayMergeEffect(SoulOrbMergeResult result, int combo)
        {
            if (result.ResultTier is not { } bornTier)
            {
                popEffectPlayer.PlayFinale(result.Position);
                return;
            }

            popEffectPlayer.PlayMerge(new SoulOrbPopEffectInfo(
                position: result.Position,
                size: result.Size,
                tier: bornTier,
                color: _monsterLoadout[result.SourceTier].soulOrbPopEffectColor,
                combo: combo
            ));
        }

        public bool TryPop(SoulOrb orb, int effectCombo)
        {
            if (orb == null || !orb.CanBeTargeted) return false;

            popEffectPlayer.Play(new SoulOrbPopEffectInfo(orb, effectCombo));

            spawner.Despawn(orb);
            return true;
        }

        public void PlayPopEffect(in SoulOrbSnapshot snapshot, int effectCombo)
        {
            popEffectPlayer.Play(new SoulOrbPopEffectInfo(snapshot, effectCombo));
        }

        public async UniTask PlayGameOverSequenceAsync(CancellationToken token)
        {
            var soulOrbs = new List<SoulOrb>(spawner.ActiveOrbs);
            foreach (var orb in soulOrbs)
            {
                orb.SetPhase(SoulOrbPhase.Inert);
            }

            soulOrbs.Sort((a, b) => a.RbPosition.y.CompareTo(b.RbPosition.y));

            foreach (var orb in soulOrbs)
            {
                _gameOverOrbPopped.OnNext(orb);
                popEffectPlayer.Play(new SoulOrbPopEffectInfo(orb, 0));
                spawner.Despawn(orb);
                await UniTask.Delay(TimeSpan.FromSeconds(POP_INTERVAL), cancellationToken: token);
            }
        }


        private void TickOrbs(float deltaTime)
        {
            foreach (var orb in spawner.ActiveOrbs)
            {
                orb.TickVisuals(deltaTime);
            }
        }
    }
}