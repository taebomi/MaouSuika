using Cysharp.Threading.Tasks;
using UnityEngine.Rendering;

namespace TBM.MaouSuika.Core.UI
{
    public interface IUIService
    {
        // UI Base
        void ShowUI(UIBase ui);
        void HideUI(UIBase ui);
        
        // Loading Overlay
        void ShowLoadingOverlay();
        void HideLoadingOverlay();
        
        // MessageBox
        UniTask<DebugUI.MessageBox> ShowMessageBoxAsync(MessageBoxRequest request);
        void HideMessageBox(DebugUI.MessageBox messageBox);
    }
}