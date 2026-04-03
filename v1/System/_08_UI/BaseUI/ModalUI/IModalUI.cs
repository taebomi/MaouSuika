using UnityEngine;

namespace SOSG.System.UI
{
    public interface IModalUI
    {
        // ReSharper disable once InconsistentNaming
        public Canvas Canvas { get; }

        public void OnOverlayClicked();
    }
}