using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Pool;

namespace MaouSuika.Gameplay
{
    public class SoulOrbPopEffectFinale : SoulOrbPopEffectBase
    {
        public void Play(Vector2 position)
        {
            transform.position = position;
            gameObject.SetActive(true);
            RootParticleSystem.Clear(true);
            RootParticleSystem.Play(true);
            ReturnToPoolAsync().Forget();
        }
    }
}