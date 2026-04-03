using UnityEngine;

namespace SOSG.System.PlayData
{
    public class PlayStatisticsManager : MonoBehaviour
    {
        [SerializeField] private GamePlayStatisticsSO gamePlayStatisticsSO;

        private const string Key = "PlayStatistics";

        public void Initialize(ES3Settings es3Settings)
        {
            var gamePlayStatistics = Load(es3Settings);
            gamePlayStatisticsSO.Initialize(gamePlayStatistics);
        }

        private static GamePlayStatistics Load(ES3Settings es3Settings)
        {
            return ES3.Load(Key, new GamePlayStatistics(), es3Settings);
        }

        public void Save(ES3Settings es3Settings)
        {
            ES3.Save(Key, gamePlayStatisticsSO.data, es3Settings);
        }
    }
}