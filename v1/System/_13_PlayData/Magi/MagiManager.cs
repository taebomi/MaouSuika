using UnityEngine;

namespace SOSG.System.PlayData
{
    public class MagiManager : MonoBehaviour
    {
        [SerializeField] private MagiSO magiSO;

        private const string Key = "Magi";

        public void Initialize(ES3Settings es3Settings)
        {
            var magi = Load(es3Settings);
            magiSO.Initialize(magi);
        }

        public static Magi Load(ES3Settings es3Settings)
        {
            return ES3.Load(Key, Magi.CreateInstance(), es3Settings);
        }

        public void Save(ES3Settings es3Settings)
        {
            ES3.Save(Key, magiSO.Magi, es3Settings);
        }
    }
}