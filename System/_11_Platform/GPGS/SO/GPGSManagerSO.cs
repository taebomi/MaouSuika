using System;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine;

namespace SOSG.GPGS
{
    [CreateAssetMenu(menuName = "TaeBoMi/GPGS/GPGS Manager", fileName = "GPGSManagerSO")]
    public class GPGSManagerSO : ScriptableObject
    {
        public bool IsAuthenticated => true;

        // public bool IsAuthenticated => PlayGamesPlatform.Instance.IsAuthenticated();
        public Action<Action<bool>> OnAuthenticateRequested; // Action<bool> : resultCallback 성공 여부
        public Action<string, LeaderboardTimeSpan, Action<bool>> OnLeaderboardShowRequested;
        public Action<long, string> OnScoreReportRequested;

        /// <param name="resultCallback">성공 여부 반환</param>
        public void Authenticate(Action<bool> resultCallback)
        {
            if (OnAuthenticateRequested is null)
            {
                resultCallback?.Invoke(false);
            }

            OnAuthenticateRequested?.Invoke(resultCallback);
        }

        public void ShowLeaderboard(string id, LeaderboardTimeSpan timeSpan, Action<bool> resultCallback)
        {
            if (OnLeaderboardShowRequested is null)
            {
                resultCallback?.Invoke(false);
            }

            OnLeaderboardShowRequested?.Invoke(id, timeSpan, resultCallback);
        }

        public void ReportScore(long score, string id)
        {
            OnScoreReportRequested?.Invoke(score, id);
        }
    }
}