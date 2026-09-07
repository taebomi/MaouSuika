using System.Threading;

namespace TBM.Core
{
    public static class CancellationTokenSourceExtensions
    {
        public static void CancelAndDispose(this CancellationTokenSource cts)
        {
            if (cts == null) return;

            try
            {
                cts.Cancel();
            }
            finally
            {
                cts.Dispose();
            }
        }
    }
}