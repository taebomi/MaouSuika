using System;
using MaouSuika.Core;
using UnityEngine;

namespace MaouSuika.Gameplay
{
    [CreateAssetMenu(fileName = "GameOver_VignetteSettings",
        menuName = MenuPath.Gameplay.GAMEOVER + "Vignette Settings")]
    public class GameOverVignetteSettingsSO : ScriptableObject
    {
        public VignetteSettings value;
    }
}