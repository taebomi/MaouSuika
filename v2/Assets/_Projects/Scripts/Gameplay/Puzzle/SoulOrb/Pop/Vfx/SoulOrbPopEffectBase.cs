using System;
using Cysharp.Threading.Tasks;
using TBM.Pool;
using UnityEngine;
using UnityEngine.Pool;

namespace MaouSuika.Gameplay
{
    public abstract class SoulOrbPopEffectBase : MonoBehaviour, ISelfReleasingPoolable
    {
        protected ParticleSystem RootParticleSystem;
        private Action _returnToPool;

        private void Awake()
        {
            RootParticleSystem = GetComponent<ParticleSystem>();
        }

        public void Initialize(Action returnToPool)
        {
            _returnToPool = returnToPool;
        }

        protected async UniTaskVoid ReturnToPoolAsync()
        {
            await UniTask.WaitUntil(() => !RootParticleSystem.IsAlive(true),
                cancellationToken: destroyCancellationToken);
            if (_returnToPool == null)
            {
                Destroy(gameObject);
                return;
            }

            _returnToPool();
        }
    }
}