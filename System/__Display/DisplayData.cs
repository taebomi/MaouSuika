using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using UnityEngine.PlayerLoop;

// ReSharper disable InconsistentNaming

namespace SOSG.System.Display
{
    [Serializable]
    public class DisplayData
    {
        public static int ScreenWidth;
        public static int ScreenHeight;
        public static float ScreenRatio; // 세로 / 가로

        public static bool IsPillarBoxOn => ScreenRatio < MIN_SCREEN_RATIO;
        public static int ViewableScreenWidth;
        public static int PillarBoxWidth;
        public static float MinX;
        public static float ViewableX;
        public static float MaxX;


        public static Action<bool> PillarBoxEnabled;
        public static Action SceenRatioChanged;

        #region 상수

        public const float UI_HEIGHT = 1280f;
        public const float UI_CANVAS_RATIO = MAX_SCREEN_RATIO;
        public const float UI_PIXEL_PER_DOT = 3.75f;

        // 화면 별 세로 길이 비율


        public const float BOTTOM_SCREEN_MIN_Y = 0f;
        public const float BOTTOM_SCREEN_HEIGHT_Y = 0.625f;
        public const float BOTTOM_SCREEN_MAX_Y = BOTTOM_SCREEN_MIN_Y + BOTTOM_SCREEN_HEIGHT_Y;
        public const float DIALOGUE_SCREEN_MIN_Y = BOTTOM_SCREEN_MAX_Y;
        public const float DIALOGUE_SCREEN_HEIGHT_Y = 0.125f;
        public const float DIALOGUE_SCREEN_MAX_Y = DIALOGUE_SCREEN_MIN_Y + DIALOGUE_SCREEN_HEIGHT_Y;
        public const float TOP_SCREEN_MIN_Y = DIALOGUE_SCREEN_MAX_Y;
        public const float TOP_SCREEN_HEIGHT = 0.25f;

        public const float TOP_SCREEN_MAX_Y = TOP_SCREEN_MIN_Y + TOP_SCREEN_HEIGHT;

        // 모바일 관련 세팅 값
        public const int MIN_SCREEN_RATIO_X = 3;
        public const int MIN_SCREEN_RATIO_Y = 4;
        public const float MIN_SCREEN_RATIO = (float)MIN_SCREEN_RATIO_Y / MIN_SCREEN_RATIO_X; // 최소 화면비 이하일 경우 필러박스 처리
        public const float MAX_SCREEN_RATIO = 16f / 9f; // 최대 화면비 이상일 경우 카메라 사이즈 증가

        public const float LOW_RESOLUTION_MULTIPLIER = 0.5f;
        public const float MID_RESOLUTION_MULTIPLIER = 0.75f;
        public const float HIGH_RESOLUTION_MULTIPLIER = 1f;

        public const int HIGH_FPS = 60;
        public const int MID_FPS = 45;
        public const int LOW_FPS = 30;

        #endregion

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Initialize()
        {
            SceenRatioChanged = null;
            PillarBoxEnabled = null;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void InitializeBeforeSceneLoad()
        {
            InitializeData();
        }


        public static void Update()
        {
            InitializeData();
            PillarBoxEnabled?.Invoke(IsPillarBoxOn);
            SceenRatioChanged?.Invoke();
        }

        public static bool IsScreenChanged() => ScreenWidth != Screen.width || ScreenHeight != Screen.height;

        private static void InitializeData()
        {
            ScreenHeight = Screen.height;
            ScreenWidth = Screen.width;
            ScreenRatio = (float)ScreenHeight / ScreenWidth;

            if (IsPillarBoxOn)
            {
                ViewableScreenWidth = ScreenHeight * MIN_SCREEN_RATIO_X / MIN_SCREEN_RATIO_Y;
                PillarBoxWidth = (ScreenWidth - ViewableScreenWidth) / 2;
                MinX = (float)PillarBoxWidth / ScreenWidth;
                ViewableX = (float)ViewableScreenWidth / ScreenWidth;
                MaxX = 1f - MinX;
            }
            else
            {
                ViewableScreenWidth = ScreenWidth;
                PillarBoxWidth = 0;
                MinX = 0f;
                ViewableX = 1f;
                MaxX = 1f;
            }
        }
    }
}