using Cysharp.Threading.Tasks;
using SOSG.Stage.SplitScreenMode;
using UnityEngine;

namespace SOSG.System.PlayData
{
    public class SplitScreenModePlayDataManager : MonoBehaviour
    {

        private const string Key = "SplitScreenMode";

        public async UniTask Initialize(ES3Settings es3Settings)
        {
        }

        private async UniTask<SplitScreenModePlayData> Load(ES3Settings es3Settings)
        {

            return null;
        }

        public void Save(ES3Settings es3Settings)
        {
            
        }
    }
}