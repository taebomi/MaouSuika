using System.IO;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace TBM
{
    public class Logger
    {
        // todo 파일 로그 구현 필요
        
        public static void Info(string msg, [CallerFilePath] string filePath = "",
            [CallerMemberName] string memberName = "", [CallerLineNumber] int lineNumber = 0)
        {
#if UNITY_EDITOR
            var info = $"{GetUnityDebugHeader(filePath, memberName, lineNumber)} {msg}";
            Debug.Log(info);
#endif
        }

        // ReSharper disable Unity.PerformanceAnalysis
        public static void Warning(string msg, [CallerFilePath] string filePath = "",
            [CallerMemberName] string memberName = "", [CallerLineNumber] int lineNumber = 0)
        {
#if UNITY_EDITOR
            var warning = $"{GetUnityDebugHeader(filePath, memberName, lineNumber)} {msg}";
            Debug.LogWarning(warning);
#endif
        }

        // ReSharper disable Unity.PerformanceAnalysis
        public static void Error(string msg, [CallerFilePath] string filePath = "",
            [CallerMemberName] string memberName = "", [CallerLineNumber] int lineNumber = 0)
        {
#if UNITY_EDITOR
            var error = $"{GetUnityDebugHeader(filePath, memberName, lineNumber)} {msg}";
            Debug.LogError(error);
#endif
        }

#if UNITY_EDITOR
        private static string GetUnityDebugHeader(string filePath, string memberName, int lineNumber)
        {
            const string frontText = "<color=yellow>#</color>";
            var fileName = Path.GetFileNameWithoutExtension(filePath);
            var header = $"{frontText} <color=cyan>[{fileName}/{memberName}:{lineNumber}]</color>";
            return header;
        }
#endif
    }
}