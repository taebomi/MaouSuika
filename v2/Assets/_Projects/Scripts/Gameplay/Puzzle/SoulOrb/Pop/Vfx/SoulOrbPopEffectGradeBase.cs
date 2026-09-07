using UnityEngine;

namespace MaouSuika.Gameplay
{
    public abstract class SoulOrbPopEffectGradeBase : SoulOrbPopEffectBase
    {
        public void Play(Vector2 position, SoulOrbPopEffectColor color, float size)
        {
            transform.position = position;
            gameObject.SetActive(true);
            SetColor(color);
            SetSize(size);
            RootParticleSystem.Clear(true);
            RootParticleSystem.Play(true);
            ReturnToPoolAsync().Forget();
        }

        protected abstract void SetSize(float size);
        protected abstract void SetColor(SoulOrbPopEffectColor color);
    }
}