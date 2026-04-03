using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using TBM.Core.Coroutines;
using TBM.MaouSuika.Gameplay.Unit;
using UnityEditor;
using UnityEngine;
using UnityEngine.Pool;
using Object = UnityEngine.Object;

namespace TBM.MaouSuika.Gameplay.Puzzle
{
    public abstract class MergeEffectBase : MonoBehaviour
    {
        protected IObjectPool<MergeEffectBase> Pool;

        private ParticleSystem[] _particles;

        private void Awake()
        {
            _particles = GetComponentsInChildren<ParticleSystem>();
        }

        public void Initialize(IObjectPool<MergeEffectBase> pool)
        {
            Pool = pool;
        }


        public void Setup(MergeEffectColor color, float size)
        {
            SetColor(color);
            SetSize(size);
            StartCoroutine(CheckIfAliveRoutine());
        }

        private IEnumerator CheckIfAliveRoutine()
        {
            while (true)
            {
                yield return YieldCache.WaitForSeconds(0.5f);
                if (IsAnyParticleAlive()) continue;

                ReleaseToPool();
                yield break;
            }
        }

        private bool IsAnyParticleAlive()
        {
            foreach (var particle in _particles)
            {
                if (particle.IsAlive()) return true;
            }

            return false;
        }

        private void ReleaseToPool()
        {
            if (Pool == null)
            {
                Destroy(gameObject);
                return;
            }

            gameObject.SetActive(false);
            Pool.Release(this);
        }

        protected abstract void SetSize(float size);
        protected abstract void SetColor(MergeEffectColor color);

#if UNITY_EDITOR

        [Button(ButtonSizes.Large)]
        private void Dev_SelectParticles()
        {
            var particles = GetComponentsInChildren<ParticleSystem>(true);
            Selection.objects = particles.Select(particle => (Object)particle.gameObject).ToArray();
            foreach (var particle in particles)
            {
                particle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                particle.Play(true);
            }
        }

#endif
    }
}