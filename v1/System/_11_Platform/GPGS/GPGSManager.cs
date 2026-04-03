using System;
using Cysharp.Threading.Tasks;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using TaeBoMi;
using UnityEngine;

namespace SOSG.GPGS
{
    public class GPGSManager : MonoBehaviour
    {
        [SerializeField] private GPGSManagerSO gpgsManagerSO;

        private Action<bool> _authenticateCallback;

        public void SetUp()
        {
            InitializeCallbacks(true);
            Authenticate(null);
        }


        public void TearDown()
        {
            InitializeCallbacks(false);
        }

        private void InitializeCallbacks(bool value)
        {
            if (value)
            {
                gpgsManagerSO.OnAuthenticateRequested = Authenticate;
                gpgsManagerSO.OnLeaderboardShowRequested = ShowLeaderboard;
                gpgsManagerSO.OnScoreReportRequested = ReportScore;
            }
            else
            {
                gpgsManagerSO.OnAuthenticateRequested = null;
                gpgsManagerSO.OnLeaderboardShowRequested = null;
                gpgsManagerSO.OnScoreReportRequested = null;
            }
        }

        private void Authenticate(Action<bool> callback)
        {
            _authenticateCallback = callback;
            // PlayGamesPlatform.Instance.Authenticate(OnAuthenticated);
        }

        private void OnAuthenticated(SignInStatus status)
        {
            TBMUtility.Log($"# GPGS Manager - Authenticated Result : {status}");
            if (status is SignInStatus.Success)
            {
                _authenticateCallback?.Invoke(true);
            }
            else
            {
                _authenticateCallback?.Invoke(false);
            }
        }

        private void ShowLeaderboard(string id, LeaderboardTimeSpan timeSpan, Action<bool> resultCallback)
        {
            TBMUtility.Log($"# GPGS Manager - Show Leaderboard");
            // PlayGamesPlatform.Instance.ShowLeaderboardUI(id, LeaderboardTimeSpan.AllTime,
            //     uiStatus =>
            //     {
            //         TBMUtility.Log($"# GPGS Manager - Show Leaderboard Result : {uiStatus}");
            //         resultCallback?.Invoke(uiStatus is UIStatus.Valid);
            //     });
        }

        private void ReportScore(long score, string id)
        {
            TBMUtility.Log($"# GPGS Manager - Report Score");
            // PlayGamesPlatform.Instance.ReportScore(score, id, OnScoreReported);
        }

        private void OnScoreReported(bool success)
        {
            TBMUtility.Log($"# GPGS Manager - Report Score Result : {success}");
        }
    }
}