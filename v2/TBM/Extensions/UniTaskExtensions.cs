using System.Threading;

namespace TBM.Extensions
{
    public static class UniTaskExtensions
    {
        public static void CancelAndDispose(this CancellationTokenSource cts)
        {
            cts.Cancel();
            cts.Dispose();
        }
    }
}