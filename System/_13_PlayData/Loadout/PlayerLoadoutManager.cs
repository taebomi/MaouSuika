using Cysharp.Threading.Tasks;
using UnityEngine;

namespace SOSG.System.PlayData
{
    public class PlayerLoadoutManager : MonoBehaviour
    {
        [SerializeField] private PlayerLoadoutVarSO playerLoadoutVarSO;

        private const string Key = "Loadout";

        public async UniTask Initialize(ES3Settings es3Settings)
        {
            // var loadout = await Load(es3Settings);
            // playerLoadoutVarSO.SetLoadout(loadout);
        }

        private static async UniTask<PlayerLoadout> Load(ES3Settings es3Settings)
        {
            var loadoutString = ES3.Load(Key, PlayerLoadoutString.CreateDefaultInstance(), es3Settings);
            return await PlayerLoadout.ConvertFrom(loadoutString);
        }

        public void Save(ES3Settings es3Settings)
        {
            // var loadoutString = PlayerLoadoutString.ConvertFrom(playerLoadoutVarSO.data);
            // ES3.Save(Key, loadoutString, es3Settings);
        }
    }
}