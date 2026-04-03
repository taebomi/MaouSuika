using FMODUnity;
using TBM.MaouSuika.Core.Audio;
using UnityEngine;

namespace TBM.MaouSuika.Gameplay.Audio
{
    public class GameplayBgmController : MonoBehaviour
    {
        [SerializeField] private EventReference bgm;


        public void PlayBgm()
        {
            AudioManager.Instance.PlayBgm(bgm);
        }
    }
}