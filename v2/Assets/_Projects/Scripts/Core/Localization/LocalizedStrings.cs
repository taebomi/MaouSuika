using UnityEngine.Localization;
using UnityEngine.Localization.Tables;

namespace MaouSuika.Core
{
    public static class LocalizedStrings
    {
        public static class SystemMessages
        {
            public const string TABLE_NAME = "System";
            
            public static readonly TableReference TABLE = TABLE_NAME;
            public static LocalizedString Get(string key) => new(TABLE, key);
            
            public static LocalizedString ArgOnly => new(TABLE, "argument_only");
        }
    }
}