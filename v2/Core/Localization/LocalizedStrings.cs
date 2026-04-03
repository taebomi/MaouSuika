using UnityEngine.Localization;
using UnityEngine.Localization.Tables;

namespace TBM.MaouSuika.Core.Localization
{
    public static class LocalizedStrings
    {
        public static class System
        {
            public const string TABLE_NAME = "System";
            
            public static TableReference TableReference = TABLE_NAME;
            public static LocalizedString Get(string key) => new(TableReference, key);
            
            public static LocalizedString ArguOnly => new(TableReference, "argu_only");
        }
    }
}